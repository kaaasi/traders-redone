using Traders.Core;
using Traders.Core.Extra;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Field;
using Traders.Tests.Input;
using Traders.Tests.Phases;
using C = Traders.Core.Field.FieldCursor;
using L = Traders.Core.State.DgroupLayout;
using X = Traders.Core.Extra.ExtraPhase;

namespace Traders.Tests.Extra;

/// <summary>
/// The original's side of the differential tests of the extra phase 1DEF (finding docs/routines/1DEF_000C_zusatzphase.md):
/// the program in the RefEmu (LoadOnly or a snapshot of a real game), its drawing routines, the output layers, sound,
/// controls, pause and the wait loop 21FD:0004 replaced by a far return and recorded with their arguments. The shared field
/// routines of segment 0609 (1F2D, 11E7, 13C5, 1FCC, 1FE0, 215E, 21BA), 21AF:0412 and the typewriter 0A69:0288/0243/01AD
/// run in the original; the typed character is recorded from DS:0140 at the replaced text call 0A96:0525.
/// </summary>
internal sealed class ExtraHarness
{
    public const ushort Seg = 0x1DEF;

    /// <summary>Calls replaced by retf (name = event kind or host method).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x0609, 0x1912, 3, nameof(ExtraEventKind.DrawGoods)),
        (0x0A96, 0x057A, 7, nameof(ExtraEventKind.Number)),
        (0x0A96, 0x013B, 8, nameof(ExtraEventKind.MaskedBlit)),
        (0x0A96, 0x0525, 0, nameof(ExtraEventKind.TypeCharacter)),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound)),
        (0x0609, 0x1A03, 0, nameof(FieldEventKind.DrawPanel)),
        (0x0609, 0x1ED9, 0, nameof(FieldEventKind.DrawBackground)),
        (0x0609, 0x1256, 4, nameof(FieldEventKind.DrawFrame)),
        (0x0A96, 0x0030, 6, nameof(FieldEventKind.DrawCommand)),
        (0x186C, 0x0002, 1, nameof(FieldEventKind.LoadImage)),
        (0x21AF, 0x04CD, 0, nameof(FieldEventKind.ResetObjectRestore)),
        (0x21AF, 0x022B, 5, nameof(FieldEventKind.ObjectBuffers)),
        (0x21AF, 0x00CA, 6, nameof(FieldEventKind.ObjectImage)),
        (0x21FD, 0x037D, 1, nameof(FieldEventKind.FadeOut)),
        (0x21FD, 0x026C, 1, nameof(FieldEventKind.BasePalette)),
        (0x21FD, 0x02C9, 2, nameof(FieldEventKind.PaletteColour)),
        (0x21FD, 0x0329, 0, nameof(FieldEventKind.FadeIn)),
        (0x0A96, 0x001B, 0, nameof(FieldEventKind.ClearList)),
        (0x0A96, 0x06A8, 0, nameof(FieldEventKind.RunList)),
        (0x0A96, 0x0C3F, 0, nameof(FieldEventKind.ShowPage)),
        (0x21FD, 0x060D, 0, nameof(FieldEventKind.ListOnBothPages)),
        (0x21FD, 0x051F, 0, nameof(FieldEventKind.FlipPages)),
        (0x23CD, 0x071C, 0, nameof(FieldEventKind.RestoreObjects)),
        (0x23CD, 0x06E9, 0, nameof(FieldEventKind.DrawObjects)),
        (0x21FD, 0x0185, 2, nameof(FieldEventKind.Delay)),
        (0x21FD, 0x0665, 0, nameof(FieldEventKind.LastFrame)),
        (0x0E9E, 0x037A, 0, "ReadControls"),
        (0x21FD, 0x0245, 0, "PauseCheck"),
        (0x21FD, 0x0004, 1, "WaitLoop"),
    ];

    /// <summary>Calls that run in the original and are only recorded.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Traced =
    [
        (0x21AF, 0x0412, 2, nameof(FieldEventKind.ShowObject)),
        (0x0609, 0x13C5, 4, nameof(FieldEventKind.DrawBorder)),
        (0x0A69, 0x0288, 8, nameof(ExtraEventKind.TypeLines)),
    ];

    /// <summary>
    /// Bytes the original writes that belong to the output layers: the object pointers and restore records of 21AF:0412 and
    /// the character buffer DS:0140 of the typewriter 0A69:01AD.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] OutputOwned =
    [
        .. PlantingDifferentialTests.OutputOwned,
        (0x0140, 1, "0A69:01AD character being typed (Vga text)"),
    ];

    /// <summary>Segment outside the program for the test's arrays (routines called alone) and texts.</summary>
    public const ushort Scratch = 0x9000;

    public const ushort GroundAt = 0x0100, GoodsAt = 0x0200, ClaimsAt = 0x0300, TextsAt = 0x8000;

    public OriginalProgram Program { get; }
    public byte[] Initial { get; }
    public List<string> Calls { get; } = [];
    public List<short> RandomArgs { get; } = [];

    /// <summary>Actions DS:3308 written by the replaced 0E9E:037A at control read n (same script for the port).</summary>
    public Func<int, sbyte[]>? Script { get; set; }

    public int ControlReads { get; set; }

    /// <summary>SS:BP of the last entry into 1DEF:000C (after its <c>mov bp, sp</c>).</summary>
    public ExtraFrame? EnteredFrame { get; private set; }

    private bool _muted;

    public ExtraHarness(OriginalProgram? program = null)
    {
        Program = program ?? OriginalProgram.LoadOnly(TestGameData.DosDir);
        Initial = Program.DataSegment();
        var m = Program.Machine;
        foreach (var (seg, off, args, name) in Stubs)
        {
            ushort s = Program.Segment(seg);
            m.AddCodeHook(s, off, _ =>
            {
                if (_muted) return;
                if (name == nameof(ExtraEventKind.TypeCharacter))
                {
                    Calls.Add($"{name}({Program.ReadByte(0x0140)})");
                    return;
                }
                Calls.Add($"{name}({string.Join(", ", Enumerable.Range(0, args).Select(i => (short)Program.StackArgument(i)))})");
                if (name == "ReadControls" && Script is { } script)
                {
                    var a = script(ControlReads++);
                    for (int i = 0; i < 4; i++) Program.WriteByte((ushort)(C.Action + i), unchecked((byte)a[i]));
                }
            });
            m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
        }
        m.Mem.Write8(Program.Segment(0x0A96), 0x0267, 0xCB); // drawing inside 13C5, not recorded
        foreach (var (seg, off, args, name) in Traced)
            m.AddCodeHook(Program.Segment(seg), off, _ =>
            {
                if (_muted) return;
                // TypeLines without its segment argument (DS of the RefEmu, DGROUP in the port)
                var values = Enumerable.Range(0, args).Where(i => name != nameof(ExtraEventKind.TypeLines) || i != 4)
                    .Select(i => (short)Program.StackArgument(i));
                Calls.Add($"{name}({string.Join(", ", values)})");
            });
        m.AddCodeHook(Program.Segment(0x0609), 0x13C8, _ => _muted = true);
        m.AddCodeHook(Program.Segment(0x0609), 0x153A, _ => _muted = false);
        m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
        m.AddCodeHook(Program.Segment(Seg), 0x000F, _ => EnteredFrame = new ExtraFrame(m.Cpu.Bp, m.Cpu.Ss));
    }

    public byte[] FarString(ushort offset, ushort segment)
    {
        var bytes = new List<byte>();
        for (ushort i = 0; ; i++)
        {
            byte b = Program.Machine.Mem.Read8(segment, (ushort)(offset + i));
            if (b == 0) return [.. bytes];
            bytes.Add(b);
        }
    }

    /// <summary>The three far pointers of DS point to the test's arrays in <see cref="Scratch"/>.</summary>
    public void PointToScratch()
    {
        Program.WriteWord(X.GroundPointer, GroundAt);
        Program.WriteWord(X.GroundPointer + 2, Scratch);
        Program.WriteWord(X.GoodsPointer, GoodsAt);
        Program.WriteWord(X.GoodsPointer + 2, Scratch);
        Program.WriteWord(X.ClaimsPointer, ClaimsAt);
        Program.WriteWord(X.ClaimsPointer + 2, Scratch);
    }

    private (ushort Seg, ushort Off) Pointer(int at) => (Program.ReadWord((ushort)(at + 2)), Program.ReadWord((ushort)at));

    /// <summary>Writes the board into the arrays the far pointers of DS point to.</summary>
    public void WriteBoard(ExtraBoard board)
    {
        var mem = Program.Machine.Mem;
        var (gs, go) = Pointer(X.GroundPointer);
        var (ws, wo) = Pointer(X.GoodsPointer);
        var (cs, co) = Pointer(X.ClaimsPointer);
        for (int i = 0; i < ExtraBoard.Cells; i++)
        {
            mem.Write16(gs, (ushort)(go + 2 * i), unchecked((ushort)board.Ground[i]));
            mem.Write16(ws, (ushort)(wo + 2 * i), unchecked((ushort)board.Goods[i]));
            mem.Write8(cs, (ushort)(co + i), board.Claims[i]);
        }
    }

    /// <summary>Reads the arrays the far pointers of DS point to.</summary>
    public ExtraBoard ReadBoard()
    {
        var mem = Program.Machine.Mem;
        var (gs, go) = Pointer(X.GroundPointer);
        var (ws, wo) = Pointer(X.GoodsPointer);
        var (cs, co) = Pointer(X.ClaimsPointer);
        var board = new ExtraBoard();
        for (int i = 0; i < ExtraBoard.Cells; i++)
        {
            board.Ground[i] = (short)mem.Read16(gs, (ushort)(go + 2 * i));
            board.Goods[i] = (short)mem.Read16(ws, (ushort)(wo + 2 * i));
            board.Claims[i] = mem.Read8(cs, (ushort)(co + i));
        }
        return board;
    }

    /// <summary>Two notice lines as far pointers at DS:3B5C (some null) with texts in <see cref="Scratch"/>.</summary>
    public void WriteNotice(Random r)
    {
        string[] texts = ["A SHIP FULL OF GOODS", "", "  FOR ALL ", "X", "GRAB WHAT YOU CAN BEFORE THE OTHERS", " "];
        ushort at = TextsAt;
        for (int line = 0; line < 2; line++)
        {
            ushort ptr = (ushort)(X.NoticeLines + 4 * line);
            if (r.Next(6) == 0)
            {
                Program.WriteDword(ptr, 0);
                continue;
            }
            var text = texts[r.Next(texts.Length)];
            Program.WriteWord(ptr, at);
            Program.WriteWord((ushort)(ptr + 2), Scratch);
            foreach (var c in text) Program.Machine.Mem.Write8(Scratch, at++, (byte)c);
            Program.Machine.Mem.Write8(Scratch, at++, 0);
        }
    }

    public static void AssertSameBoard(string label, ExtraBoard original, ExtraBoard port)
    {
        for (int i = 0; i < ExtraBoard.Cells; i++)
        {
            Assert.True(original.Ground[i] == port.Ground[i], $"{label}: ground[{i}] original {original.Ground[i]}, port {port.Ground[i]}");
            Assert.True(original.Goods[i] == port.Goods[i], $"{label}: goods[{i}] original {original.Goods[i]}, port {port.Goods[i]}");
            Assert.True(original.Claims[i] == port.Claims[i], $"{label}: claims[{i}] original {original.Claims[i]}, port {port.Claims[i]}");
        }
    }

    public static ExtraBoard Copy(ExtraBoard b)
    {
        var c = new ExtraBoard();
        b.Ground.CopyTo(c.Ground, 0);
        b.Goods.CopyTo(c.Goods, 0);
        b.Claims.CopyTo(c.Claims, 0);
        return c;
    }

    /// <summary>The data segment of the original against the port's (outside the output's and the interrupts' bytes).</summary>
    public static void Compare(string label, byte[] original, Dgroup port)
    {
        var o = (byte[])original.Clone();
        foreach (var (off, len, _) in OutputOwned) Array.Copy(port.Bytes, off, o, off, len);
        foreach (var (off, len) in RefEmuWeekOracle.InterruptOwned) Array.Copy(port.Bytes, off, o, off, len);
        ControlHarness.AssertSameData(label, o, port.Bytes);
    }
}

/// <summary>The port's side: records the same calls as <see cref="ExtraHarness"/> and answers controls from the same script.</summary>
internal sealed class ExtraRecorder(ExtraHarness h) : IExtraHost
{
    public List<string> Calls { get; } = [];
    public int ControlReads { get; private set; }

    public void Emit(FieldEvent e) =>
        Calls.Add((e.Kind == FieldEventKind.DrawBorder ? e with { Args = e.Args[..4] } : e).ToString());

    public void Emit(ExtraEvent e) => Calls.Add(e.Kind switch
    {
        ExtraEventKind.TypeCharacter => $"{e.Kind}({e.Args[0]})",
        ExtraEventKind.TypeLines => (e with { Args = [.. e.Args[..4], .. e.Args[5..]] }).ToString(),
        _ => e.ToString(),
    });

    public void ReadControls(Dgroup d)
    {
        Calls.Add("ReadControls()");
        if (h.Script is not { } script) return;
        var a = script(ControlReads++);
        for (int i = 0; i < 4; i++) d.Set(C.Action + i, a[i]);
    }

    public IEnumerable<FieldWait> PauseCheck(Dgroup d)
    {
        Calls.Add("PauseCheck()");
        return [];
    }

    public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
    {
        Calls.Add($"WaitLoop({mode})");
        return [];
    }

    public byte[] FarString(ushort offset, ushort segment) =>
        h.FarString(offset, segment == L.Segment ? h.Program.Dgroup : segment);
}
