using Traders.RefEmu.Cpu;
using Traders.RefEmu.Hardware;

namespace Traders.RefEmu.Machine;

/// <summary>Configuration of the emulated PC. All values are inputs of a reference run and must be recorded with it.</summary>
public sealed class MachineOptions
{
    /// <summary>Host directory that appears as the current directory of drive C:.</summary>
    public required string HostDirectory { get; init; }

    /// <summary>Emulated CPU speed. Defines how emulated time advances (deterministically) with executed instructions.</summary>
    public long InstructionsPerSecond { get; init; } = 3_000_000;

    /// <summary>Wall-clock time at the start of the run (DOS date/time and BIOS ticks derive from it).</summary>
    public DateTime StartTime { get; init; } = new(1991, 6, 1, 12, 0, 0);

    /// <summary>Install an int 64h stub that records the calls of the game to the FXDRIVER sound driver.</summary>
    public bool RecordSoundDriver { get; init; } = true;

    public ushort PspSegment { get; init; } = 0x0FF0;
    public ushort MemoryTop { get; init; } = 0xA000;
}

/// <summary>Why <see cref="PcMachine.Run"/> returned.</summary>
public enum StopReason { InstructionLimit, Condition, ProgramExit, ReturnedToHost }

/// <summary>
/// A deterministic IBM PC/AT with VGA, DOS 5 and BIOS services implemented on the host side.
/// Host services are reached through small stubs in the BIOS area (opcode F1 = host trap), so
/// programs that hook and chain interrupt vectors behave as on real hardware.
/// </summary>
public sealed partial class PcMachine : IIoBus
{
    public const ushort StubSegment = 0xF000;
    private const ushort StopOffset = 0x0000;
    private const ushort VectorStubBase = 0x0100;
    private const byte TrapService = 0x00, TrapStop = 0x01;

    public MachineOptions Options { get; }
    public Memory Mem { get; } = new();
    public Cpu286 Cpu { get; }
    public Pic Pic { get; } = new();
    public Pit Pit { get; } = new();
    public Keyboard Keyboard { get; } = new();
    public VgaAdapter Vga { get; } = new();
    public DosFileSystem Files { get; }
    public EventLog Log { get; } = new();

    public bool Exited { get; private set; }
    public int ExitCode { get; private set; }

    private long _timeOffset;      // PIT clocks added by idle fast-forwarding
    private long _nextTimerIrq;
    private bool _stopHit;
    private readonly List<(long Time, byte Scancode)> _scheduledKeys = [];
    private int _nextScheduledKey;

    public PcMachine(MachineOptions options)
    {
        Options = options;
        Files = new DosFileSystem(options.HostDirectory);
        Cpu = new Cpu286(Mem, this) { HostTrap = OnHostTrap };
        Mem.Map(Vga, 0xA0000, 0xB0000);
        Vga.Now = () => Now;
        Vga.UnknownPort = (p, v) => LogPort($"VGA port {p:X3} not modelled", $" <- {v:X2}");
        Pit.ReloadChanged += (ch, reload) =>
        {
            if (ch == 0) _nextTimerIrq = Now + reload;
            LogEvent(EventKind.Port, $"PIT channel {ch} reload {reload}");
        };
        _nextTimerIrq = Pit.Channel0Period;
        InitBios();
        InitDos();
    }

    /// <summary>Emulated time in PIT clocks since power-on.</summary>
    public long Now => Cpu.InstructionCount * Pit.ClockHz / Options.InstructionsPerSecond + _timeOffset;

    public double Seconds => Now / (double)Pit.ClockHz;

    public void LogEvent(EventKind kind, string text) => Log.Add(new EmuEvent(Cpu.InstructionCount, Now, kind, text));

    // ------------------------------------------------------------------ input script

    /// <summary>Schedules a key press (make + break) at an emulated time in seconds.</summary>
    public void ScheduleKey(double seconds, byte makeScancode)
    {
        long t = (long)(seconds * Pit.ClockHz);
        _scheduledKeys.Add((t, makeScancode));
        _scheduledKeys.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    // ------------------------------------------------------------------ I/O bus

    public byte In8(ushort port)
    {
        if (VgaAdapter.Handles(port)) return Vga.In(port);
        switch (port)
        {
            case 0x20:
            case 0x21: return Pic.Read(port);
            case >= 0x40 and <= 0x43: return Pit.Read(port, Now);
            case 0x60:
            case 0x61:
            case 0x64: return Keyboard.Read(port, Now);
            case 0x201: return 0xFF; // no joystick
            default:
                LogPort($"in {port:X3} (unmapped, reads FF)");
                return 0xFF;
        }
    }

    public void Out8(ushort port, byte value)
    {
        if (VgaAdapter.Handles(port)) { Vga.Out(port, value); return; }
        switch (port)
        {
            case 0x20:
            case 0x21: Pic.Write(port, value); return;
            case >= 0x40 and <= 0x43: Pit.Write(port, value, Now); return;
            case 0x61: Keyboard.Write(port, value); return;
            case 0x60:
            case 0x64: return;
            default:
                LogPort($"out {port:X3} (unmapped)", $" <- {value:X2}");
                return;
        }
    }

    private readonly Dictionary<string, int> _portLogCount = [];

    /// <summary>Logs unusual port accesses, the first few per port and direction only.</summary>
    private void LogPort(string key, string detail = "")
    {
        _portLogCount.TryGetValue(key, out int n);
        _portLogCount[key] = n + 1;
        if (n < 3) LogEvent(EventKind.Port, key + detail);
        else if (n == 3) LogEvent(EventKind.Port, key + " ... (further accesses not logged)");
    }

    // ------------------------------------------------------------------ execution

    /// <summary>Runs until the program exits, <paramref name="stopWhen"/> holds, control returns to the host, or the limit is reached.</summary>
    public StopReason Run(long maxInstructions, Func<PcMachine, bool>? stopWhen = null)
    {
        long limit = Cpu.InstructionCount + maxInstructions;
        int idle = 0;
        while (true)
        {
            if (Exited) return StopReason.ProgramExit;
            if (_stopHit) { _stopHit = false; return StopReason.ReturnedToHost; }
            if (stopWhen is not null && stopWhen(this)) return StopReason.Condition;
            if (Cpu.InstructionCount >= limit) return StopReason.InstructionLimit;

            if (Cpu.Halted)
            {
                if (!Cpu.GetFlag(Cpu286.IF)) throw new EmulationException($"CPU halted with interrupts disabled at {Cpu.Cs:X4}:{Cpu.IP:X4}");
                if (++idle > 1_000_000) throw new EmulationException($"CPU idles without interrupts at {Cpu.Cs:X4}:{Cpu.IP:X4}");
                FastForwardToNextEvent();
            }
            else
            {
                idle = 0;
                Cpu.Step();
            }
            ServiceDevices();
        }
    }

    private void FastForwardToNextEvent()
    {
        long next = _nextTimerIrq;
        if (_nextScheduledKey < _scheduledKeys.Count) next = Math.Min(next, _scheduledKeys[_nextScheduledKey].Time);
        long now = Now;
        if (next > now) _timeOffset += next - now;
        else _timeOffset += 1;
    }

    private void ServiceDevices()
    {
        long now = Now;
        if (now >= _nextTimerIrq)
        {
            Pic.Raise(0);
            long period = Pit.Channel0Period;
            _nextTimerIrq += period;
            if (_nextTimerIrq <= now) _nextTimerIrq = now + period; // missed ticks are dropped like on real hardware
        }
        while (_nextScheduledKey < _scheduledKeys.Count && _scheduledKeys[_nextScheduledKey].Time <= now)
        {
            byte sc = _scheduledKeys[_nextScheduledKey++].Scancode;
            Keyboard.Press(sc);
            LogEvent(EventKind.Keyboard, $"key {sc:X2} scheduled");
        }
        if (Keyboard.Poll(now)) Pic.Raise(1);

        if (Cpu.GetFlag(Cpu286.IF) && !Cpu.InterruptShadow)
        {
            int irq = Pic.Pending();
            if (irq >= 0) Cpu.HardwareInterrupt(Pic.Acknowledge(irq));
        }
    }

    /// <summary>Runs until emulated time has advanced by <paramref name="seconds"/>.</summary>
    public StopReason RunFor(double seconds, Func<PcMachine, bool>? stopWhen = null)
    {
        long target = Now + (long)(seconds * Pit.ClockHz);
        return Run(long.MaxValue / 4, m => m.Now >= target || (stopWhen?.Invoke(m) ?? false));
    }

    /// <summary>
    /// Calls a far C function (Turbo C calling convention: arguments pushed right to left, caller pops)
    /// with the current DS/SS:SP and returns DX:AX.
    /// </summary>
    public (ushort Ax, ushort Dx) CallFar(ushort seg, ushort off, ushort[] args, long maxInstructions = 50_000_000)
    {
        for (int i = args.Length - 1; i >= 0; i--) Cpu.Push(args[i]);
        Cpu.Push(StubSegment);
        Cpu.Push(StopOffset);
        Cpu.Cs = seg;
        Cpu.IP = off;
        var reason = Run(maxInstructions);
        if (reason != StopReason.ReturnedToHost)
            throw new EmulationException($"Call {seg:X4}:{off:X4} did not return ({reason}) at {Cpu.Cs:X4}:{Cpu.IP:X4}");
        Cpu.Sp += (ushort)(2 * args.Length);
        return (Cpu.Ax, Cpu.Dx);
    }

    // ------------------------------------------------------------------ host traps

    private void OnHostTrap(Cpu286 cpu, byte kind)
    {
        if (kind == TrapStop)
        {
            _stopHit = true;
            return;
        }
        if (kind != TrapService) throw new EmulationException($"Unknown host trap {kind:X2}");
        byte vector = Mem.Read8(cpu.Cs, cpu.IP);
        ushort trapIp = (ushort)(cpu.IP - 2);
        cpu.IP++;
        ServiceInterrupt(vector, trapIp);
    }

    /// <summary>Re-executes the current host stub later (used by blocking BIOS/DOS input calls).</summary>
    private void RetryLater(ushort trapIp)
    {
        Cpu.IP = trapIp;
        Cpu.SetFlag(Cpu286.IF, true);
        Cpu.Halted = true;
    }

    /// <summary>Copies IF from the caller's flags on the stack (BIOS/DOS run with the caller's interrupt state).</summary>
    private void TakeCallerInterruptFlag()
    {
        ushort stacked = Mem.Read16(Cpu.Ss, (ushort)(Cpu.Sp + 4));
        Cpu.SetFlag(Cpu286.IF, (stacked & Cpu286.IF) != 0);
    }

    private void SetCarry(bool on) => Cpu.SetFlag(Cpu286.CF, on);

    private void ServiceInterrupt(byte vector, ushort trapIp)
    {
        switch (vector)
        {
            case 0x08: TimerTick(); return;
            case 0x09: KeyboardIrq(); return;
            case 0x1C: return;
        }
        if (vector >= 0x0A && vector <= 0x0F || vector >= 0x70 && vector <= 0x77)
        {
            Pic.Eoi();
            return;
        }

        TakeCallerInterruptFlag();
        switch (vector)
        {
            case 0x00: throw new EmulationException($"Divide error without program handler (vector 0) at {Cpu.Cs:X4}:{Cpu.IP:X4}");
            case 0x10: Int10(); return;
            case 0x11: Cpu.Ax = Mem.Read16(0x40, 0x10); return;
            case 0x12: Cpu.Ax = Mem.Read16(0x40, 0x13); return;
            case 0x15: Cpu.Ax = (ushort)((Cpu.Ax & 0x00FF) | 0x8600); SetCarry(true); return;
            case 0x16: Int16(trapIp); return;
            case 0x1A: Int1A(); return;
            case 0x20: Exit(0); return;
            case 0x21: Int21(trapIp); return;
            case 0x23: Exit(0); return;
            case 0x24: Cpu.Ax = (ushort)((Cpu.Ax & 0xFF00) | 3); return; // fail
            case 0x28: return;
            case 0x2F: Cpu.Ax &= 0xFF00; return;
            case 0x33: Cpu.Ax = 0; return; // no mouse driver
            case 0x64: SoundDriver(); return;
            default:
                LogEvent(EventKind.Bios, $"int {vector:X2} AX={Cpu.Ax:X4} ignored");
                return;
        }
    }

    private void Exit(int code)
    {
        Exited = true;
        ExitCode = code;
        LogEvent(EventKind.Dos, $"exit {code}");
    }

    private void SoundDriver()
    {
        var c = Cpu;
        LogEvent(EventKind.Sound, $"int 64 AX={c.Ax:X4} BX={c.Bx:X4} CX={c.Cx:X4} DX={c.Dx:X4} SI={c.Si:X4} DI={c.Di:X4} DS={c.Ds:X4} ES={c.Es:X4}");
        if (c.Ax == 0x12) c.Bx &= 0xFF00; // status query: nothing playing
    }

    // ------------------------------------------------------------------ setup

    private void InitBios()
    {
        Mem.A20 = false;
        // stop stub
        Mem.Write8(StubSegment, StopOffset, 0xF1);
        Mem.Write8(StubSegment, StopOffset + 1, TrapStop);
        for (int v = 0; v < 256; v++)
        {
            ushort off = (ushort)(VectorStubBase + v * 8);
            byte[] code = v switch
            {
                0x08 => [0xF1, TrapService, 0x08, 0xCD, 0x1C, 0xCF],                          // tick, int 1Ch, iret
                0x09 or 0x1C or (>= 0x0A and <= 0x0F) or (>= 0x70 and <= 0x77) => [0xF1, TrapService, (byte)v, 0xCF],
                _ => [0xF1, TrapService, (byte)v, 0xCA, 0x02, 0x00],                          // retf 2 keeps result flags
            };
            Mem.WriteBlock(Memory.Linear(StubSegment, off), code);
            bool installed = v is not (>= 0x60 and <= 0x67) || (v == 0x64 && Options.RecordSoundDriver);
            if (installed)
            {
                Mem.Write16(0, (ushort)(v * 4), off);
                Mem.Write16(0, (ushort)(v * 4 + 2), StubSegment);
            }
        }

        // BIOS data area
        Mem.Write16(0x40, 0x10, 0x0021);               // equipment: floppy, 80x25 colour
        Mem.Write16(0x40, 0x13, 640);                  // KB of conventional memory
        Mem.Write16(0x40, 0x1A, 0x1E);                 // keyboard buffer head
        Mem.Write16(0x40, 0x1C, 0x1E);                 // keyboard buffer tail
        Mem.Write16(0x40, 0x80, 0x1E);
        Mem.Write16(0x40, 0x82, 0x3E);
        Mem.Write16(0x40, 0x63, 0x3D4);                // CRTC base
        Mem.Write8(0x40, 0x84, 24);
        Mem.Write16(0x40, 0x85, 16);
        Mem.Write8(0x40, 0x87, 0x60);
        Mem.Write8(0x40, 0x88, 0x09);
        Mem.Write8(0x40, 0x89, 0x11);
        var midnight = Options.StartTime.TimeOfDay;
        uint ticks = (uint)(midnight.TotalSeconds * Pit.ClockHz / 65536.0);
        Mem.Write16(0x40, 0x6C, (ushort)ticks);
        Mem.Write16(0x40, 0x6E, (ushort)(ticks >> 16));
        Vga.SetMode(0x03, true);
        SetVideoBda(0x03);
    }

    private void TimerTick()
    {
        uint t = (uint)(Mem.Read16(0x40, 0x6C) | (Mem.Read16(0x40, 0x6E) << 16));
        t++;
        if (t >= 0x1800B0) { t = 0; Mem.Write8(0x40, 0x70, 1); }
        Mem.Write16(0x40, 0x6C, (ushort)t);
        Mem.Write16(0x40, 0x6E, (ushort)(t >> 16));
        Pic.Eoi();
    }
}
