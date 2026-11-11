using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.RefEmu;

/// <summary>
/// Convenience layer for differential tests against the original T.EXE: addresses are given as in
/// the analysis (load-image segments, DGROUP offsets) and translated to the running machine.
/// </summary>
public sealed class OriginalProgram
{
    /// <summary>DGROUP paragraph in the load image (data segment of T.EXE).</summary>
    public const ushort ImageDgroup = 0x28DC;

    /// <summary>Emulated time at which the main menu is fully shown after <see cref="BootToMainMenu"/>'s inputs.</summary>
    public const double MainMenuTime = 17.5;

    /// <summary>Hash of the main menu frame (<see cref="FrameDump.Hash"/>) reached by <see cref="BootToMainMenu"/>.</summary>
    public const string MainMenuFrameHash = "DBAB2787F9AA5222";

    /// <summary>
    /// Main menu frame hash when FXDRIVER runs first. With the resident driver the menu is entered later and random()
    /// has another state there; the menu calls random() while it is built, so the frame differs (docs/analyse/REFEMU.md).
    /// </summary>
    public const string MainMenuFrameHashWithFxDriver = "A1237183394811CE";

    public PcMachine Machine { get; }

    private OriginalProgram(PcMachine machine) => Machine = machine;

    public ushort Segment(ushort imageSegment) => (ushort)(Machine.LoadSegment + imageSegment);

    public ushort Dgroup => Segment(ImageDgroup);

    /// <summary>
    /// Loads T.EXE without running it: DS = DGROUP, private stack at 9000:FFF0. The data segment holds
    /// the initialised values from the executable. Suitable for calling self-contained routines.
    /// </summary>
    public static OriginalProgram LoadOnly(string dosDirectory, MachineOptions? options = null)
    {
        var m = new PcMachine(options ?? new MachineOptions { HostDirectory = dosDirectory });
        m.LoadProgram("T.EXE");
        m.Cpu.Ds = (ushort)(m.LoadSegment + ImageDgroup);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        return new OriginalProgram(m);
    }

    /// <summary>
    /// Boots T.EXE with the standard reference inputs (any four digits at the "INSERT COIN!" code
    /// request – the original accepts every answer) and runs until the main menu is displayed.
    /// </summary>
    public static OriginalProgram BootToMainMenu(string dosDirectory, MachineOptions? options = null, bool runFxDriver = false)
    {
        var p = Start(dosDirectory, options, runFxDriver);
        p.Machine.RunFor(MainMenuTime);
        return p;
    }

    /// <summary>
    /// Loads T.EXE (optionally after running FXDRIVER.EXE as resident sound driver, like TRADERS.BAT)
    /// and schedules the standard answer to the code request; does not run T.EXE yet.
    /// </summary>
    public static OriginalProgram Start(string dosDirectory, MachineOptions? options = null, bool runFxDriver = false)
    {
        options ??= new MachineOptions { HostDirectory = dosDirectory, RecordSoundDriver = !runFxDriver };
        var m = new PcMachine(options);
        if (runFxDriver) m.RunResidentProgram("FXDRIVER.EXE");
        m.LoadProgram("T.EXE");
        double t0 = m.Seconds;
        for (int i = 0; i < 4; i++) m.ScheduleKey(t0 + 14.0 + 0.4 * i, 0x02);
        var p = new OriginalProgram(m);
        if (runFxDriver) p.HookSoundDriverCalls();
        return p;
    }

    /// <summary>Logs every int 64h call into the resident FXDRIVER (event kind Sound), without changing the run.</summary>
    private void HookSoundDriverCalls()
    {
        var m = Machine;
        ushort off = m.Mem.Read16(0, 0x64 * 4), seg = m.Mem.Read16(0, 0x64 * 4 + 2);
        if (seg == 0 && off == 0) throw new InvalidOperationException("FXDRIVER did not install int 64h.");
        m.AddCodeHook(seg, off, mm =>
        {
            var c = mm.Cpu;
            mm.LogEvent(EventKind.Sound, $"int 64 AX={c.Ax:X4} BX={c.Bx:X4} CX={c.Cx:X4} DX={c.Dx:X4} SI={c.Si:X4} DI={c.Di:X4} DS={c.Ds:X4} ES={c.Es:X4}");
        });
    }

    /// <summary>Presses a key (set-1 make code) <paramref name="delay"/> seconds from now.</summary>
    public void Press(byte makeScancode, double delay = 0.2, double hold = 0.1) =>
        Machine.ScheduleKey(Machine.Seconds + delay, makeScancode, hold);

    /// <summary>Traces all far calls into the graphics driver (segments 0A96 and 0A69).</summary>
    public CallTracer TraceGraphicsDriver(int argWords = 8) => TraceSegments([0x0A96, 0x0A69], argWords);

    /// <summary>Traces all far calls into the given image segments.</summary>
    public CallTracer TraceSegments(ushort[] imageSegments, int argWords = 6)
    {
        var exe = Machine.Files.Load("T.EXE") ?? throw new FileNotFoundException("T.EXE");
        var targets = MzLoader.FarCallTargets(exe).Where(t => imageSegments.Contains(t.Seg));
        return new CallTracer(Machine, targets, argWords);
    }

    // ------------------------------------------------------------------ week run as oracle
    // Addresses from docs/analyse/BESTAND.md §1.4 (sequence confirmed by probe P1): the week loop calls the weekly
    // prices first and passes phase markers 0..6 to a small routine with the marker number as its only argument.

    /// <summary>Image address of the weekly price routine, entered once at the start of every week.</summary>
    public const ushort WeekStartSegment = 0x1777, WeekStartOffset = 0x02DC;

    /// <summary>Image address of the phase marker routine (one word argument: marker number 0..6).</summary>
    public const ushort PhaseMarkerSegment = 0x21FD, PhaseMarkerOffset = 0x0977;

    /// <summary>Image address of the new-game routine, entered after RETURN in the main menu.</summary>
    public const ushort NewGameSegment = 0x0E31, NewGameOffset = 0x0003;

    /// <summary>DS offset of the week counter (0 in the first week).</summary>
    public const ushort WeekCounterOffset = 0x4B9A;

    /// <summary>DS offset of the 32-bit state of random().</summary>
    public const ushort RandomStateOffset = 0x10D2;

    /// <summary>
    /// <see cref="Start"/>, runs to the main menu and presses RETURN after <paramref name="menuDelay"/> seconds
    /// (default game: four computer players). The game then runs on its own; nothing is executed after the key is scheduled.
    /// The random state at the new game depends on <paramref name="menuDelay"/> (probe P5).
    /// </summary>
    public static OriginalProgram StartGame(string dosDirectory, MachineOptions? options = null, bool runFxDriver = false,
        double menuDelay = 0.5)
    {
        var p = Start(dosDirectory, options, runFxDriver);
        p.Machine.RunFor(MainMenuTime);
        p.Press(Scancodes.Enter, delay: menuDelay);
        return p;
    }

    /// <summary>Week counter of the running game.</summary>
    public int Week => ReadWord(WeekCounterOffset);

    /// <summary>Current state of random().</summary>
    public uint RandomState => ReadDword(RandomStateOffset);

    /// <summary>True when the CPU is at the entry of the given image routine (before its first instruction).</summary>
    public bool IsAt(ushort imageSegment, ushort offset) =>
        Machine.Cpu.Cs == Segment(imageSegment) && Machine.Cpu.IP == offset;

    /// <summary>
    /// Runs until the entry of the weekly prices for <paramref name="week"/> (week counter already advanced) and stops before
    /// its first instruction. Returns false if it is not reached within <paramref name="maxSeconds"/> emulated seconds.
    /// </summary>
    public bool RunToWeekStart(int week, double maxSeconds) =>
        RunUntil(() => IsAt(WeekStartSegment, WeekStartOffset) && Week == week, maxSeconds);

    /// <summary>
    /// Runs until phase marker <paramref name="marker"/> of week <paramref name="week"/> is entered and stops before the
    /// marker routine runs (the data segment then shows the state between the phases). False if not reached in time.
    /// </summary>
    public bool RunToMarker(int week, int marker, double maxSeconds) =>
        RunUntil(() => IsAt(PhaseMarkerSegment, PhaseMarkerOffset) && Week == week && StackArgument(0) == marker, maxSeconds);

    /// <summary>
    /// Word argument <paramref name="index"/> of a far routine at its entry (SS:SP points to the return address).
    /// </summary>
    public ushort StackArgument(int index) => Machine.Mem.Read16(Machine.Cpu.Ss, (ushort)(Machine.Cpu.Sp + 4 + 2 * index));

    private bool RunUntil(Func<bool> condition, double maxSeconds)
    {
        // A stop at the current position would end the run at once; step past it first.
        if (condition()) Machine.Run(1);
        // RunFor also reports the time limit as Condition, so test the condition itself afterwards.
        Machine.RunFor(maxSeconds, _ => condition());
        return !Machine.Exited && condition();
    }

    // ------------------------------------------------------------------ machine snapshot (docs/analyse/REFEMU.md)

    /// <summary>
    /// Captures the whole machine at the current stop (for example after <see cref="RunToMarker"/>). The snapshot holds
    /// memory derived from T.EXE: keep it in memory or in a temporary folder, never in the repository.
    /// </summary>
    public MachineSnapshot Snapshot() => MachineSnapshot.Capture(Machine);

    /// <summary>
    /// Continues a run from <paramref name="snapshot"/> in a new machine. If FXDRIVER is resident in the snapshot
    /// (int 64h points outside the host stubs) its calls are logged again as in <see cref="Start"/>.
    /// </summary>
    public static OriginalProgram Resume(string dosDirectory, MachineSnapshot snapshot, SnapshotParts parts = SnapshotParts.All)
    {
        var p = new OriginalProgram(snapshot.Restore(dosDirectory, parts));
        ushort seg = p.Machine.Mem.Read16(0, 0x64 * 4 + 2);
        if (seg != 0 && seg != PcMachine.StubSegment) p.HookSoundDriverCalls();
        return p;
    }

    /// <summary>Calls a far routine given by its image address; returns DX:AX.</summary>
    public (ushort Ax, ushort Dx) Call(ushort imageSegment, ushort offset, params ushort[] args) =>
        Machine.CallFar(Segment(imageSegment), offset, args);

    public byte ReadByte(ushort dsOffset) => Machine.Mem.Read8(Dgroup, dsOffset);
    public ushort ReadWord(ushort dsOffset) => Machine.Mem.Read16(Dgroup, dsOffset);
    public uint ReadDword(ushort dsOffset) => (uint)(ReadWord(dsOffset) | (ReadWord((ushort)(dsOffset + 2)) << 16));
    public void WriteByte(ushort dsOffset, byte v) => Machine.Mem.Write8(Dgroup, dsOffset, v);
    public void WriteWord(ushort dsOffset, ushort v) => Machine.Mem.Write16(Dgroup, dsOffset, v);

    public void WriteDword(ushort dsOffset, uint v)
    {
        WriteWord(dsOffset, (ushort)v);
        WriteWord((ushort)(dsOffset + 2), (ushort)(v >> 16));
    }

    /// <summary>Copy of the whole 64 KB data segment (for state comparisons).</summary>
    public byte[] DataSegment() => Machine.Mem.ReadBlock(Dgroup, 0, 0x10000);

    public FrameDump Frame() => FrameDump.Capture(Machine.Vga);
}
