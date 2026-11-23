using Traders.Assets;
using Traders.Audio.Music;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Audio;

/// <summary>
/// The resident FXDRIVER in the RefEmu next to the port <see cref="FxMusicDriver"/>, both driven with the same int 64h
/// calls and timer calls (CS:1262, one call = four timer ticks). Every step returns the OPL writes of both sides; the driver's
/// data segment can be compared byte for byte. The buffers of function 10h lie where the game puts them (one block of
/// 24 × 0C04h bytes, normalised far pointers).
/// </summary>
internal sealed class MusicDriverHarness
{
    private const ushort StubSegment = 0x8000, CallStub = 0x0000, TickStub = 0x0010, NameOffset = 0x0100;
    private const ushort BufferSegment = 0x5000;

    /// <summary>Offsets of the driver's data segment that differ by construction: saved int 8 vector (016A–016D),
    /// and the register block (REGPACK) of the DOS call that restores it (017E–0191).</summary>
    public static readonly (int From, int To)[] NotCompared = [(0x016A, 0x016E), (0x017E, 0x0192)];

    public PcMachine Machine { get; }
    public FxMusicDriver Port { get; }
    public ushort Load { get; }
    public ushort DataSegment { get; }
    public long Ticks { get; private set; }

    /// <summary>The driver's handler runs on every fourth timer tick (PIT divisor 4000h); the harness numbers the calls like that,
    /// starting at a phase of its own.</summary>
    public const int TimerTicksPerCall = 4;
    public const long FirstTimerTick = 2;

    private readonly OplTrace _trace;
    private readonly List<(byte, byte)> _portWrites = [];
    private int _traceSeen;

    public MusicDriverHarness()
    {
        Machine = new PcMachine(new MachineOptions { HostDirectory = TestGameData.DosDir, RecordSoundDriver = false });
        Machine.RunResidentProgram("FXDRIVER.EXE");
        // Only Tick() may run the driver's timer handler: IRQ 0 masked, so function 01h's int 8 hook never fires.
        Machine.Pic.Write(0x21, 0x01);
        Machine.Cpu.Ss = 0x9000;
        Machine.Cpu.Sp = 0xFFF0;
        ushort code = Machine.Mem.Read16(0, 0x64 * 4 + 2);
        Load = (ushort)(code - FxDriverImage.CodeParagraph);
        var image = FxDriverImage.Load(TestGameData.Dos("FXDRIVER.EXE"));
        DataSegment = (ushort)(Load + image.DataParagraph);
        var m = Machine.Mem;
        byte[] stub = [0xCD, 0x64, 0xCB]; // int 64h; retf
        for (int i = 0; i < stub.Length; i++) m.Write8(StubSegment, (ushort)(CallStub + i), stub[i]);
        byte[] tick = [0x9C, 0x9A, 0x62, 0x12, (byte)code, (byte)(code >> 8), 0xCB]; // pushf; call far code:1262; retf
        for (int i = 0; i < tick.Length; i++) m.Write8(StubSegment, (ushort)(TickStub + i), tick[i]);
        _trace = new OplTrace(Machine);
        Port = new FxMusicDriver(image, name => File.Exists(TestGameData.Dos(name)) ? File.ReadAllBytes(TestGameData.Dos(name)) : null);
        Port.Written += (r, v) => _portWrites.Add((r, v));
        Port.Install();
        TakeWrites();
    }

    /// <summary>OPL writes since the last step: (original, port).</summary>
    public (List<(byte, byte)> Original, List<(byte, byte)> Port) TakeWrites()
    {
        var o = _trace.Writes.Skip(_traceSeen).Select(w => (w.Register, w.Value)).ToList();
        _traceSeen = _trace.Writes.Count;
        var p = _portWrites.ToList();
        _portWrites.Clear();
        return (o, p);
    }

    /// <summary>int 64h on both sides; function 00h with a module name.</summary>
    public ((ushort, ushort, ushort, ushort) Original, (ushort, ushort, ushort, ushort) Port) Call(ushort ax, ushort bx = 0,
        ushort cx = 0, ushort dx = 0, string? name = null)
    {
        if (ax == 0x00 && name is not null)
        {
            for (int i = 0; i < name.Length; i++) Machine.Mem.Write8(StubSegment, (ushort)(NameOffset + i), (byte)name[i]);
            Machine.Mem.Write8(StubSegment, (ushort)(NameOffset + name.Length), 0);
            (bx, cx) = (StubSegment, NameOffset);
        }
        var cpu = Machine.Cpu;
        (cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx) = (ax, bx, cx, dx);
        Machine.CallFar(StubSegment, CallStub, []);
        var original = (cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx);
        if (ax == 0x01) KeepTimerManual();
        var port = Port.Call(ax, bx, cx, dx, name);
        return (original, port);
    }

    /// <summary>Function 01h hooked the driver into int 8; the vector goes back to the old handler so that only
    /// <see cref="Tick"/> runs the driver's timer handler (one call = one tick, R9PL-2).</summary>
    private void KeepTimerManual()
    {
        var m = Machine.Mem;
        m.Write16(0, 0x20, m.Read16(DataSegment, 0x016A));
        m.Write16(0, 0x22, m.Read16(DataSegment, 0x016C));
    }

    /// <summary>One call of the timer handler on both sides (the port only while its timer is hooked).</summary>
    public void Tick()
    {
        Machine.CallFar(StubSegment, TickStub, []);
        Ticks++;
        if (Port.TimerActive) Port.Tick(FirstTimerTick + TimerTicksPerCall * Ticks);
    }

    /// <summary>The game's start sequence (TON §3): 06h (BX = 0), 05h, 24 × 10h with the buffers of one block.</summary>
    public void Boot()
    {
        Call(0x06);
        Call(0x05);
        for (int i = 0; i < FxMusicDriver.BufferCount; i++)
        {
            long linear = BufferSegment * 16L + FxMusicDriver.BufferSize * (long)i;
            Call(0x10, (ushort)i, (ushort)(linear >> 4), (ushort)(linear & 15));
        }
    }

    /// <summary>The music call of 218B:0094 for piece <paramref name="name"/> with the flag (bit 0 slow fade-in, bit 1 repeat).</summary>
    public void Play(string name, int flag)
    {
        Call(0x12);
        Call(0x00, name: name);
        Call(0x18, (ushort)((flag & 2) != 0 ? 1 : 0));
        Call(0x1B, (ushort)((flag & 1) != 0 ? 3 : 0x7F));
        Call(0x1A);
        Call(0x01);
    }

    /// <summary>Offsets of the driver's data segment (0…15FFh) where the port differs from the original.</summary>
    public List<int> DataDifferences()
    {
        var result = new List<int>();
        for (int i = 0; i < 0x1600; i++)
        {
            if (NotCompared.Any(r => i >= r.From && i < r.To)) continue;
            if (Machine.Mem.Read8(DataSegment, (ushort)i) != Port.DataByte(i)) result.Add(i);
        }
        return result;
    }

    /// <summary>Byte offsets where the port's pattern buffers differ from the emulated buffers.</summary>
    public List<string> BufferDifferences()
    {
        var result = new List<string>();
        for (int b = 0; b < FxMusicDriver.BufferCount; b++)
        {
            long linear = BufferSegment * 16L + FxMusicDriver.BufferSize * (long)b;
            var buf = Port.Buffer(b);
            for (int i = 0; i < FxMusicDriver.BufferSize; i++)
            {
                long a = linear + i;
                if (Machine.Mem.Read8((ushort)(a >> 4), (ushort)(a & 15)) != buf[i]) result.Add($"{b}:{i:X3}");
            }
        }
        return result;
    }

    public static string Hex(IEnumerable<int> offsets) => string.Join(" ", offsets.Select(o => o.ToString("X4")));

    public static string Hex(IEnumerable<(byte R, byte V)> writes) => string.Join(" ", writes.Select(w => $"{w.R:X2}={w.V:X2}"));
}
