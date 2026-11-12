using Traders.Assets.Loading;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Phases;

namespace Traders.Tests.Startup;

/// <summary>
/// [DIFFERENTIAL] 0DFD:000D – program step 2 of the core (<see cref="ProgramStart"/>) against the original in lockstep
/// (<see cref="RefEmuProgramOracle.PortHandler"/>): same data segment at the call site, the core runs the step, the RefEmu runs
/// 0DFD:000D to its return with some key input at the code request, and the whole data segment is compared outside the
/// bytes of the output layers and the host (<see cref="OutputOwned"/>, finding 0DFD_000D_programmstart.md). The clock value
/// is the input the host gives: what Borland time() returned in the original at the code request. Expected values come only
/// from the RefEmu.
/// </summary>
public class ProgramStartDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>
    /// Bytes 0DFD:000D writes that the core leaves to the output layers (Traders.Vga), the host (heap, C runtime, files),
    /// the input (int 9) and the music (R10). The text table DS:37CC (192D:18B6) with its buffer pointer DS:4884 and the
    /// plaque top DS:5334 (192D:16AF) are the core's and are compared.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] OutputOwned =
    [
        (0x0142, 2, "23CD:05B6 via 21FD:05DB: text advance and shadow height (Vga)"),
        (0x0238, 1, "0B67:0238 shown page (Vga pages)"),
        (0x082E, 4, "21FD:051F page indices (Vga pages)"),
        (0x10A5, 2, "218B:0094(0, 0) at 0DFD:02FE: music playing (R10)"),
        (0x10A9, 2, "218B:0094(0, 0) at 0DFD:02FE: current piece (R10)"),
        (0x10D8, 2, "21FD:04DA blend colour (Vga palettes)"),
        (0x1158, 1, "last make code of the int 9 handler 27C6:006F (input, F1 = B)"),
        (0x115D, 1, "make counter of the int 9 handler 27C6:0073 (input)"),
        (0x128A, 2, "C runtime heap top (fopen of the music file, 218B:00D3)"),
        (0x1300, 8, "C runtime file table (open, 218B:00D3)"),
        (0x15BA, 0x9FA, "0A96 drawing list (Vga command list)"),
        (0x2BF0, 0x1E, "0DFD:0158 font records with heap pointers (Vga fonts)"),
        (0x330C, 0xD0, "21AF:000D object records with pointers (Vga objects)"),
        (0x34A4, 2, "21FD:05DB line height (Vga)"),
        (0x3CA8, 2, "21FD:05DB text advance (Vga)"),
        (0x3CAA, 4, "0DFD:0074 heap buffer (texts)"),
        (0x3DCB, 0x300, "21FD:026C/02C9 base palette (Vga palettes)"),
        (0x40DE, 8, "0DFD:0074 page numbers (Vga pages)"),
        (0x44DE, 4, "0DFD:0132 mask buffer pointer copy"),
        (0x4516, 4, "21FD:051F current draw page (Vga pages)"),
        (0x45BC, 4, "0DFD:0132 atlas pointer copy"),
        (0x45D6, 4, "0DFD:0074 heap buffer (atlas)"),
        (0x4BC6, 4, "0DFD:0074 heap buffer (masks)"),
        (0x4C00, 0x113, "186C cache table with heap pointers (host)"),
        (0x5017, 0x300, "186C:0002 loaded palette (Vga palettes)"),
        (0x5318, 0x18, "186C:021E heap block"),
        (0x54E8, 0x300, "21FD:03EB working palette (Vga palettes)"),
        (0x5894, 8, "C runtime tzset: time zone names (time())"),
    ];

    /// <summary>Lengths of the picture files as the host reads them (Traders.Assets).</summary>
    private static ImageFileLengths Files(int number)
    {
        var (file, unpacked) = ImageFiles.Lengths(number, name => File.ReadAllBytes(TestGameData.Dos(name)));
        return new ImageFileLengths(file, unpacked);
    }

    private static MachineOptions Options(DateTime start) => new() { HostDirectory = Dir, RecordSoundDriver = true, StartTime = start };

    /// <summary>Return address of the call of Borland time() in random(0) 21FD:01BA; DX:AX holds the clock value there.</summary>
    private const ushort TimeReturn = 0x01CE;

    /// <summary>Return address of the plaque call 192D:16AF in the code request (21FD:0771).</summary>
    private const ushort PlaqueCall = 0x0771, PlaqueReturn = 0x0776;

    /// <summary>
    /// Inputs of the core taken from a separate run of the original with the same options: the value Borland time()
    /// returned at the code request (the host's clock input), the far address of the text buffer DS:4884 (the host's heap)
    /// and the arguments of the random() calls inside the plaque 192D:16AF (for the counter-check without new seed).
    /// </summary>
    private static (int Clock, List<short> PlaqueDraws, uint TextBuffer) OriginalInputs(DateTime start, Action<OriginalProgram> keys)
    {
        var p = OriginalProgram.Start(Dir, Options(start));
        keys(p);
        ushort seg = p.Segment(0x21FD);
        int? clock = null;
        bool inPlaque = false;
        var draws = new List<short>();
        p.Machine.AddCodeHook(seg, TimeReturn, m => clock ??= (int)(m.Cpu.Ax | (uint)m.Cpu.Dx << 16));
        p.Machine.AddCodeHook(seg, PlaqueCall, _ => inPlaque = true);
        p.Machine.AddCodeHook(seg, PlaqueReturn, _ => inPlaque = false);
        p.Machine.AddCodeHook(seg, 0x01BA, _ => { if (inPlaque) draws.Add((short)p.StackArgument(0)); });
        p.Machine.Run(2_000_000_000, m => m.Cpu.Cs == seg && m.Cpu.IP == PlaqueReturn);
        Assert.True(clock.HasValue && p.Machine.Cpu.Cs == seg && p.Machine.Cpu.IP == PlaqueReturn && draws.Count > 0,
            "code request of the original not reached");
        return (clock!.Value, draws, p.ReadDword((ushort)MenuLayout.TextBuffer));
    }

    /// <summary>The text file as the host gives it, at the buffer address the original's heap chose.</summary>
    internal static TextFile Texts(uint buffer) =>
        new(File.ReadAllBytes(TestGameData.Dos("ENGLISCH.TXT")), (ushort)buffer, (ushort)(buffer >> 16));

    /// <summary>The plaque of lane menue, checking the arguments of the code request.</summary>
    private sealed class CheckedPlaque : ITextPlaque
    {
        private readonly MenuTextPlaque _plaque = new();

        public int Shown { get; private set; }

        public void Show(Dgroup d, OriginalRandom random, short height, short step, int texts, short extra)
        {
            Assert.Equal((CodeQuery.PlaqueHeight, CodeQuery.PlaqueStep, CodeQuery.Texts, CodeQuery.PlaqueExtra), (height, step, texts, extra));
            _plaque.Show(d, random, height, step, texts, extra);
            Shown++;
        }
    }

    private sealed record Run(Dgroup Core, OriginalProgram Program, RefEmuProgramOracle Oracle, ProgramStart Start, ProgramMachine Machine);

    /// <summary>Lockstep from the first call of main to the call site of program step 3; step 1 and 2 are the core's.</summary>
    private static Run Lockstep(DateTime start, Action<OriginalProgram> keys)
    {
        var (clock, _, buffer) = OriginalInputs(start, keys);
        var p = OriginalProgram.Start(Dir, Options(start));
        keys(p);
        var oracle = new RefEmuProgramOracle(p);
        oracle.RunToCallSite(ProgramLoop.Step(1));
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var programStart = new ProgramStart(Files, new CheckedPlaque(), Texts(buffer));
        var defaults = ProgramSteps.Defaults(ProgramInitialisation.AdLib);
        var handlers = new Dictionary<int, IProgramStepHandler>
        {
            [1] = oracle.PortHandler(core, defaults[1], RefEmuProgramOracle.HostOwnedByInitialisation),
            [2] = oracle.PortHandler(core, programStart, OutputOwned),
            [3] = oracle.StopAtCallSite(core),
        };
        var machine = new ProgramMachine(core, new OriginalRandom(new RecordedClock([clock])), handlers,
            new Dictionary<int, IWeekStepHandler>());
        for (int f = 0; f < 5 && machine.Status != ProgramStatus.Waiting; f++) machine.Advance(new PhaseTick(machine.LastFrame + 1));
        return new Run(core, p, oracle, programStart, machine);
    }

    private static readonly DateTime ReferenceStart = new(1991, 6, 1, 12, 0, 0);

    private static void NoExtraKeys(OriginalProgram p) { }

    /// <summary>
    /// Keys typed at the code request: whenever the original starts waiting for a key (21FD:07A5, also after a rejected
    /// key), the next one is pressed 0.05 s later; the four standard digits "1" of <see cref="OriginalProgram.Start"/> at
    /// 14 s come after the request has returned.
    /// </summary>
    internal static Action<OriginalProgram> KeysAtRequest(params byte[] scancodes) => p =>
    {
        int next = 0;
        p.Machine.AddCodeHook(p.Segment(0x21FD), 0x07A5, m =>
        {
            if (next < scancodes.Length) m.ScheduleKey(m.Seconds + 0.05, scancodes[next++]);
        });
    };

    public static TheoryData<string> Inputs => new() { "1111", "keypad 7 8 9 4", "wrong digits 9 9 9 9", "letters and minus first" };

    private static Action<OriginalProgram> InputFor(string name) => name switch
    {
        "1111" => NoExtraKeys,
        "keypad 7 8 9 4" => KeysAtRequest(0x47, 0x48, 0x49, 0x4B),
        "wrong digits 9 9 9 9" => KeysAtRequest(0x0A, 0x0A, 0x0A, 0x0A),
        "letters and minus first" => KeysAtRequest(0x1E, 0x4A, 0x0B, 0x0B, 0x4F),
        _ => throw new ArgumentOutOfRangeException(nameof(name)),
    };

    /// <summary>
    /// Program start with the reference clock and different answers at the code request: the data segment after 0DFD:000D
    /// is the original's outside the output-owned bytes (picture table DS:09C8/09CE, seed DS:10D2, decoded texts and code
    /// DS:10FF..1124, text table DS:37CC with DS:4884 and plaque top DS:5334 included), whatever is typed; the seed equals
    /// the RefEmu's and the text buffer the original's heap buffer.
    /// </summary>
    // [DIFFERENTIAL] 21FD:0687 – code request (state part) inside the program start, any input
    // [DIFFERENTIAL] 0DFD:0239 – the six preloaded pictures (their table entries; the order is checked in StartupTests)
    // [DIFFERENTIAL] 186C:0002 – picture table after seven loads; [DIFFERENTIAL] 186C:0997 – lengths of the first load
    // [DIFFERENTIAL] 192D:16AF – plaque of the code request (DS:5334 and its random draws) through MenuTextPlaque
    [GameDataTheory]
    [MemberData(nameof(Inputs))]
    public void Program_start_leaves_the_data_segment_of_the_original_for_any_input(string input)
    {
        var r = Lockstep(ReferenceStart, InputFor(input));

        Assert.Empty(r.Oracle.Differences);
        Assert.Equal(ProgramStatus.Waiting, r.Machine.Status);
        Assert.Equal(3, r.Machine.NextStep);
        Assert.Equal(r.Program.RandomState, r.Core.U32(DgroupLayout.RandomSeed));
        Assert.NotNull(r.Start.Code);
        // the code the original shows: the text at [DS:10E8] in the RefEmu
        int text = r.Program.ReadWord(CodeQuery.CodeText);
        string shown = new(Enumerable.Range(0, 10).Select(i => (char)r.Program.ReadByte((ushort)(text + i))).ToArray());
        Assert.Equal("CODE: " + r.Start.Code!.Code.PadRight(4), shown);
        // the text buffer after 192D:18B6 equals the original's heap buffer over the whole file
        ushort off = r.Core.U16(MenuLayout.TextBuffer), seg = r.Core.U16(MenuLayout.TextBuffer + 2);
        int length = File.ReadAllBytes(TestGameData.Dos("ENGLISCH.TXT")).Length;
        for (int i = 0; i < length; i++)
            Assert.True(r.Program.Machine.Mem.Read8(seg, (ushort)(off + i)) == r.Start.TextBuffer![i], $"text buffer byte {i:X4}");
    }

    /// <summary>
    /// Another clock value gives another seed and another code, again equal to the original's; the two clocks cover both
    /// forms of the column in the text (one digit and blank, two digits).
    /// </summary>
    [GameDataFact]
    public void Program_start_with_another_clock_gives_the_seed_and_code_of_the_original()
    {
        var a = Lockstep(new DateTime(1992, 1, 15, 8, 30, 1), NoExtraKeys);
        var b = Lockstep(ReferenceStart, NoExtraKeys);

        Assert.Empty(a.Oracle.Differences);
        Assert.Equal(a.Program.RandomState, a.Core.U32(DgroupLayout.RandomSeed));
        Assert.Empty(b.Oracle.Differences);
        Assert.NotEqual(b.Core.U32(DgroupLayout.RandomSeed), a.Core.U32(DgroupLayout.RandomSeed));
        Assert.InRange(a.Start.Code!.Column, 0, 9);
        Assert.InRange(b.Start.Code!.Column, 10, 31);
    }

    /// <summary>
    /// Counter-check: the code request without the new seed (core skips random(0)) leaves another seed than the original –
    /// the comparison after the step fails.
    /// </summary>
    [GameDataFact]
    public void Program_start_without_reseeding_differs_from_the_original()
    {
        var p = OriginalProgram.Start(Dir, Options(ReferenceStart));
        var oracle = new RefEmuProgramOracle(p);
        oracle.RunToCallSite(ProgramLoop.Step(1));
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var defaults = ProgramSteps.Defaults(ProgramInitialisation.AdLib);
        var handlers = new Dictionary<int, IProgramStepHandler>
        {
            [1] = oracle.PortHandler(core, defaults[1], RefEmuProgramOracle.HostOwnedByInitialisation),
            [2] = oracle.PortHandler(core, new WithoutReseed(OriginalInputs(ReferenceStart, NoExtraKeys).PlaqueDraws), OutputOwned),
        };
        var machine = new ProgramMachine(core, new OriginalRandom(new RecordedClock([])), handlers, new Dictionary<int, IWeekStepHandler>());

        var e = Assert.Throws<InvalidOperationException>(() => machine.Advance(new PhaseTick(0)));
        Assert.Contains("DS:10D", e.Message);
    }

    /// <summary>Program start with the code request but without random(0) (the draw then continues from seed 1).</summary>
    private sealed class WithoutReseed(IReadOnlyList<short> plaqueDraws) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext context)
        {
            var d = context.State;
            foreach (int word in ImageTable.PreloadOrder) ImageTable.RecordLoad(d, d.S16(word), Files);
            ImageTable.RecordLoad(d, d.S16(ImageTable.CodeRequestNumber), Files);
            CodeQuery.DecodeTexts(d);
            context.Random.Next(0x3FF);
            foreach (short n in plaqueDraws) context.Random.Next(n);
            return StepOutcome.Finished(0);
        }

        public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException();
    }
}
