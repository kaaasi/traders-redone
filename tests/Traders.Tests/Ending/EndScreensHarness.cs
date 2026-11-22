using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Field;
using Traders.Tests.Input;
using Traders.Tests.Phases;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Ending;

/// <summary>
/// The original's side of the differential tests of the end screens 20B3:0053–0088 (finding docs/routines/20B3_008E_ende.md):
/// the program in the RefEmu with its drawing routines, the output layers, pages, palettes, objects' output parts, sound,
/// music, the report 0825:000A and the key wait 27C6:00D2 replaced by a far return and recorded with their arguments. The
/// routines of segment 20B3 itself, the page helpers 21FD:0620/0634, the field routines 0609:1F2D/11E7/13C5/1FCC/1FE0/21BA, 21AF:0412/045E, 0825:06CB/07C4
/// and random() run in the original. The pixel reader 23CD:06DE answers from <see cref="Pixels"/> (a script shared with the
/// port's recorder, so the reveal's draws depend on the read values as in the game); its reads are recorded with the value.
/// The key wait sets DS:115D = 1 as a make in the keyboard handler would.
/// </summary>
internal sealed class EndScreensHarness
{
    public const ushort Seg = 0x20B3, Entry = 0x0005;

    /// <summary>Calls replaced by retf (name = event kind or host method).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x186C, 0x0002, 1, nameof(FieldEventKind.LoadImage)),
        (0x21FD, 0x037D, 1, nameof(FieldEventKind.FadeOut)),
        (0x21FD, 0x026C, 1, nameof(FieldEventKind.BasePalette)),
        (0x21FD, 0x02C9, 2, nameof(FieldEventKind.PaletteColour)),
        (0x21FD, 0x0329, 0, nameof(FieldEventKind.FadeIn)),
        (0x0A96, 0x001B, 0, nameof(FieldEventKind.ClearList)),
        (0x0A96, 0x06A8, 0, nameof(FieldEventKind.RunList)),
        (0x0A96, 0x0C3F, 0, nameof(FieldEventKind.ShowPage)),
        (0x0A96, 0x0030, 6, nameof(FieldEventKind.DrawCommand)),
        (0x0A96, 0x0267, 8, nameof(EndingEventKind.Tile)),
        (0x0A96, 0x013B, 8, nameof(EndingEventKind.MaskedBlit)),
        (0x0A96, 0x04D7, 5, nameof(EndingEventKind.FillRect)),
        (0x0A96, 0x0525, 6, nameof(EndingEventKind.Text)),
        (0x0A96, 0x057A, 7, nameof(EndingEventKind.Number)),
        (0x0A96, 0x05FB, 7, nameof(EndingEventKind.Money)),
        (0x0A96, 0x03FB, 3, nameof(EndingEventKind.Pixel)),
        (0x0A96, 0x0433, 5, nameof(EndingEventKind.Line)),
        (0x0A69, 0x002C, 4, nameof(EndingEventKind.VerticalLine)),
        (0x0609, 0x1ED9, 0, nameof(FieldEventKind.DrawBackground)),
        (0x0609, 0x1256, 4, nameof(FieldEventKind.DrawFrame)),
        (0x0609, 0x1879, 3, nameof(EndingEventKind.Emblem)),
        (0x0609, 0x1A03, 0, nameof(FieldEventKind.DrawPanel)),
        (0x21FD, 0x060D, 0, nameof(FieldEventKind.ListOnBothPages)),
        (0x21FD, 0x051F, 0, nameof(FieldEventKind.FlipPages)),
        (0x21FD, 0x0665, 0, nameof(FieldEventKind.LastFrame)),
        (0x21FD, 0x0185, 2, nameof(FieldEventKind.Delay)),
        (0x21FD, 0x0590, 0, nameof(EndingEventKind.DrawOnShownPage)),
        (0x21FD, 0x03EB, 2, nameof(EndingEventKind.PaletteStep)),
        (0x21FD, 0x04DA, 1, nameof(EndingEventKind.BlendColor)),
        (0x23CD, 0x071C, 0, nameof(FieldEventKind.RestoreObjects)),
        (0x23CD, 0x06E9, 0, nameof(FieldEventKind.DrawObjects)),
        (0x0B67, 0x01D4, 0, nameof(EndingEventKind.DrawShownPage)),
        (0x0B67, 0x0206, 0, nameof(EndingEventKind.DrawHiddenPage)),
        (0x21AF, 0x022B, 5, nameof(FieldEventKind.ObjectBuffers)),
        (0x21AF, 0x00CA, 6, nameof(FieldEventKind.ObjectImage)),
        (0x21AF, 0x04CD, 0, nameof(FieldEventKind.ResetObjectRestore)),
        (0x218B, 0x0094, 2, nameof(EndingEventKind.Music)),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound)),
        (0x0825, 0x000A, 1, "Report"),
        (0x27C6, 0x00D2, 0, nameof(EndingEventKind.WaitKey)),
        (0x23CD, 0x06DE, 2, nameof(EndingEventKind.ReadPixel)),
    ];

    /// <summary>Calls that run in the original and are only recorded.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Traced =
    [
        (0x21AF, 0x0412, 2, nameof(FieldEventKind.ShowObject)),
        (0x21AF, 0x045E, 1, nameof(EndingEventKind.HideObject)),
        (0x0609, 0x13C5, 4, nameof(FieldEventKind.DrawBorder)),
    ];

    public OriginalProgram Program { get; }
    public List<string> Calls { get; } = [];
    public List<short> RandomArgs { get; } = [];

    /// <summary>Value of pixel read n (shared with the port's recorder).</summary>
    public Func<int, byte> Pixels { get; }

    public int PixelReads { get; private set; }

    /// <summary>Pixel reads after which the script answers only colours other than 2 and 6 (abort counter of the reveal).</summary>
    public const int ReadLimit = 200_000;

    private bool _muted;

    public EndScreensHarness(OriginalProgram program, Func<int, byte> pixels)
    {
        Program = program;
        Pixels = pixels;
        var m = Program.Machine;
        foreach (var (seg, off, args, name) in Stubs)
        {
            ushort s = Program.Segment(seg);
            m.AddCodeHook(s, off, _ =>
            {
                if (_muted) return;
                if (name == nameof(EndingEventKind.ReadPixel))
                {
                    byte v = Pixels(PixelReads++);
                    m.Cpu.Ax = v;
                    Calls.Add($"{name}({(short)Program.StackArgument(0)}, {(short)Program.StackArgument(1)}, {v})");
                    return;
                }
                if (name == nameof(EndingEventKind.WaitKey)) Program.WriteByte(EndScreens.MakeCount, 1);
                Calls.Add(Record(name, args));
            });
            m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
        }
        foreach (var (seg, off, args, name) in Traced)
            m.AddCodeHook(Program.Segment(seg), off, _ =>
            {
                if (_muted) return;
                Calls.Add($"{name}({string.Join(", ", Enumerable.Range(0, args).Select(i => (short)Program.StackArgument(i)))})");
            });
        // the drawing calls of the border 13C5 are not recorded (the scene draws it from DrawBorder)
        m.AddCodeHook(Program.Segment(0x0609), 0x13C8, _ => _muted = true);
        m.AddCodeHook(Program.Segment(0x0609), 0x153A, _ => _muted = false);
        m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
    }

    private string Record(string name, int args)
    {
        var values = Enumerable.Range(0, args).Select(i => (short)Program.StackArgument(i)).ToList();
        if (name == nameof(EndingEventKind.Text) && (ushort)values[3] == Program.Dgroup) values[3] = unchecked((short)L.Segment);
        if (name == nameof(EndingEventKind.Emblem)) values.Add((sbyte)Program.ReadByte(L.ActivePlayer));
        return $"{name}({string.Join(", ", values)})";
    }

    /// <summary>The whole of 20B3:0005 in the RefEmu; returns AL.</summary>
    public byte Run() => (byte)Program.CallWithBudget(2_000_000_000, Seg, Entry).Ax;

    /// <summary>The data segment of the original against the port's (outside the output's and the interrupts' bytes).</summary>
    public static void Compare(string label, byte[] original, Dgroup port)
    {
        var o = (byte[])original.Clone();
        foreach (var (off, len, _) in PlantingDifferentialTests.OutputOwned) Array.Copy(port.Bytes, off, o, off, len);
        foreach (var (off, len) in RefEmuWeekOracle.InterruptOwned) Array.Copy(port.Bytes, off, o, off, len);
        ControlHarness.AssertSameData(label, o, port.Bytes);
    }
}

/// <summary>The port's side: records the same calls as <see cref="EndScreensHarness"/> and answers pixels from the same script.</summary>
internal sealed class EndScreensRecorder(Func<int, byte> pixels) : IEndingHost
{
    public List<string> Calls { get; } = [];
    public int PixelReads { get; private set; }

    public void ComputerTurn(Dgroup d, short x, short y) =>
        throw new InvalidOperationException("The end screens have no computer turn.");

    public void Emit(FieldEvent e) =>
        Calls.Add((e.Kind == FieldEventKind.DrawBorder ? e with { Args = e.Args[..4] } : e).ToString());

    public void Emit(EndingEvent e)
    {
        Calls.Add(e.ToString());
        if (e.Kind == EndingEventKind.WaitKey) (State ?? throw new InvalidOperationException("State not set.")).Set(EndScreens.MakeCount, (byte)1);
    }

    public byte ReadPixel(RulePage page, short x, short y) => pixels(PixelReads++);

    public IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random)
    {
        Calls.Add("Report(0)");
        return [];
    }

    /// <summary>The port's data segment: the key wait counts a make at once in it (as the harness's stub does).</summary>
    public Dgroup? State { get; set; }
}
