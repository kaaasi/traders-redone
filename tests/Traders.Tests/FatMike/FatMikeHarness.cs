using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.FatMike;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Field;
using Traders.Tests.Input;
using Traders.Tests.Phases;
using C = Traders.Core.Field.FieldCursor;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.FatMike;

/// <summary>
/// The original's side of the differential tests of Fat Mike 1F63:000F (finding docs/routines/1F63_000F_fatmike.md): the
/// program in the RefEmu (LoadOnly or a snapshot of a real game), its drawing routines, the output layers, sound, controls,
/// pause and the wait loop 21FD:0004 replaced by a far return and recorded with their arguments. The shared field routines
/// of segment 0609 (10D2, 11E7, 1BE5, 13C5), 21AF:000D/0412/0472 and the typewriter 0A69:0288/0243/01AD run in the original;
/// the typed character is recorded from DS:0140 at the replaced text call 0A96:0525. The replaced list calls keep the list
/// count DS:15BE as the original's list does (0A96:001B sets 0, every request adds one), because 1F63:060E reads it.
/// </summary>
internal sealed class FatMikeHarness
{
    public const ushort Seg = 0x1F63;

    /// <summary>Calls replaced by retf (name = event kind or host method); <c>List</c> = adds to the command list.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name, bool List)[] Stubs =
    [
        (0x0A96, 0x0089, 6, nameof(FatMikeEventKind.CopyAtlas), true),
        (0x0A96, 0x00E2, 6, nameof(FatMikeEventKind.CopyPage), true),
        (0x0A96, 0x0267, 8, nameof(FatMikeEventKind.Tile), true),
        (0x0A69, 0x002C, 4, nameof(FatMikeEventKind.VerticalLine), true),
        (0x0A69, 0x004D, 8, nameof(FatMikeEventKind.ShadowText), true),
        (0x0A69, 0x011B, 9, nameof(FatMikeEventKind.ShadowNumber), true),
        (0x1F63, 0x104B, 2, nameof(FatMikeEventKind.Indicator), true),
        (0x0609, 0x1879, 3, nameof(FatMikeEventKind.Emblem), true),
        (0x21FD, 0x05DB, 1, nameof(FatMikeEventKind.Font), false),
        (0x0A96, 0x0525, 0, nameof(FatMikeEventKind.TypeCharacter), true),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound), false),
        (0x0609, 0x1ED9, 0, nameof(FieldEventKind.DrawBackground), true),
        (0x0609, 0x1256, 4, nameof(FieldEventKind.DrawFrame), true),
        (0x0609, 0x1C83, 2, nameof(FieldEventKind.DrawField), true),
        (0x0609, 0x1CB1, 2, nameof(FieldEventKind.DrawFieldAndNeighbours), true),
        (0x0A96, 0x0030, 6, nameof(FieldEventKind.DrawCommand), true),
        (0x186C, 0x0002, 1, nameof(FieldEventKind.LoadImage), false),
        (0x21AF, 0x04CD, 0, nameof(FieldEventKind.ResetObjectRestore), false),
        (0x21AF, 0x022B, 5, nameof(FieldEventKind.ObjectBuffers), false),
        (0x21AF, 0x00CA, 6, nameof(FieldEventKind.ObjectImage), false),
        (0x21FD, 0x037D, 1, nameof(FieldEventKind.FadeOut), false),
        (0x21FD, 0x026C, 1, nameof(FieldEventKind.BasePalette), false),
        (0x21FD, 0x02C9, 2, nameof(FieldEventKind.PaletteColour), false),
        (0x21FD, 0x0329, 0, nameof(FieldEventKind.FadeIn), false),
        (0x0A96, 0x001B, 0, nameof(FieldEventKind.ClearList), false),
        (0x0A96, 0x06A8, 0, nameof(FieldEventKind.RunList), false),
        (0x0A96, 0x0C3F, 0, nameof(FieldEventKind.ShowPage), false),
        (0x21FD, 0x060D, 0, nameof(FieldEventKind.ListOnBothPages), false),
        (0x21FD, 0x051F, 0, nameof(FieldEventKind.FlipPages), false),
        (0x23CD, 0x071C, 0, nameof(FieldEventKind.RestoreObjects), false),
        (0x23CD, 0x06E9, 0, nameof(FieldEventKind.DrawObjects), false),
        (0x21FD, 0x0185, 2, nameof(FieldEventKind.Delay), false),
        (0x0E9E, 0x037A, 0, "ReadControls", false),
        (0x21FD, 0x0245, 0, "PauseCheck", false),
        (0x21FD, 0x0004, 1, "WaitLoop", false),
        // contest 1F63:0B03 and its second field round 0609:0080 (the field's drawing as in the planting tests)
        (0x0A96, 0x05FB, 7, nameof(FatMikeEventKind.Number), true),
        (0x2295, 0x006D, 2, nameof(FatMikeEventKind.DacColour), false),
        (0x0609, 0x0C73, 0, nameof(FieldEventKind.DrawSelection), true),
        (0x0609, 0x1854, 2, nameof(FieldEventKind.DrawRank), true),
        (0x0609, 0x1A03, 0, nameof(FieldEventKind.DrawPanel), true),
        (0x0609, 0x1A97, 0, nameof(FieldEventKind.DrawPurchaseUsed), true),
        (0x0609, 0x1AC0, 0, nameof(FieldEventKind.DrawPlayer), true),
        (0x0609, 0x0204, 0, nameof(FieldEventKind.DrawStatus), false),
        (0x21FD, 0x0665, 0, nameof(FieldEventKind.LastFrame), false),
    ];

    /// <summary>The second field round 0609:0080, replaced and recorded as <c>SecondRound()</c> when the harness is built so.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name, bool List) SecondRoundStub =
        (0x0609, 0x0080, 0, "SecondRound", false);

    /// <summary>Calls that run in the original and are only recorded.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Traced =
    [
        (0x21AF, 0x000D, 0, nameof(FieldEventKind.InitObjects)),
        (0x21AF, 0x0412, 2, nameof(FieldEventKind.ShowObject)),
        (0x21AF, 0x0472, 0, nameof(FatMikeEventKind.HideObjects)),
        (0x0609, 0x13C5, 4, nameof(FieldEventKind.DrawBorder)),
        (0x0A69, 0x0288, 8, nameof(FatMikeEventKind.TypeLines)),
    ];

    /// <summary>DS:15BE, the count of the 0A96 command list (the Vga list's in the port).</summary>
    public const int ListCount = 0x15BE;

    /// <summary>
    /// Bytes the original writes that belong to the output layers: object pointers and restore records (21AF), the character
    /// buffer DS:0140 of the typewriter and the list count DS:15BE.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] OutputOwned =
    [
        .. PlantingDifferentialTests.OutputOwned,
        (0x0140, 1, "0A69:01AD character being typed (Vga text)"),
        (ListCount, 2, "0A96 command list count (Vga list)"),
    ];

    /// <summary>Segment outside the program for texts written by the tests.</summary>
    public const ushort Scratch = 0x9000;

    public OriginalProgram Program { get; }
    public byte[] Initial { get; }
    public List<string> Calls { get; } = [];
    public List<short> RandomArgs { get; } = [];

    /// <summary>Actions DS:3308 written by the replaced 0E9E:037A at control read n (same script for the port).</summary>
    public Func<int, sbyte[]>? Script { get; set; }

    public int ControlReads { get; set; }

    private bool _muted;

    /// <summary>True when 0609:0080 is replaced (the port's recorder then records the call instead of running the round).</summary>
    public bool StubSecondRound { get; }

    /// <summary>Passes of the second round's field loop after which both sides end it (DS:3CE6 = 0; endless humans).</summary>
    public int PassLimit { get; set; } = 400;

    public int Passes { get; set; }

    public FatMikeHarness(OriginalProgram? program = null, bool stubSecondRound = true)
    {
        Program = program ?? OriginalProgram.LoadOnly(TestGameData.DosDir);
        Initial = Program.DataSegment();
        StubSecondRound = stubSecondRound;
        var m = Program.Machine;
        foreach (var (seg, off, args, name, list) in stubSecondRound ? [.. Stubs, SecondRoundStub] : Stubs)
        {
            ushort s = Program.Segment(seg);
            m.AddCodeHook(s, off, _ =>
            {
                if (_muted) return;
                if (name == nameof(FieldEventKind.ClearList)) Program.WriteWord(ListCount, 0);
                if (list) Program.WriteWord(ListCount, (ushort)(Program.ReadWord(ListCount) + 1));
                Calls.Add(Record(name, args));
                if (name == "ReadControls" && Script is { } script)
                {
                    var a = script(ControlReads++);
                    for (int i = 0; i < 4; i++) Program.WriteByte((ushort)(C.Action + i), unchecked((byte)a[i]));
                }
            });
            m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
        }
        foreach (var (seg, off, args, name) in Traced)
            m.AddCodeHook(Program.Segment(seg), off, _ =>
            {
                if (_muted) return;
                // TypeLines without its segment argument (DS of the RefEmu, DGROUP in the port)
                var values = Enumerable.Range(0, args).Where(i => name != nameof(FatMikeEventKind.TypeLines) || i != 4)
                    .Select(i => (short)Program.StackArgument(i));
                Calls.Add($"{name}({string.Join(", ", values)})");
            });
        // 0A69:0243 called by the contest itself (not by 0288): one typed line
        m.AddCodeHook(Program.Segment(0x0A69), 0x0243, _ =>
        {
            if (_muted || m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2)) != Program.Segment(Seg)) return;
            var values = Enumerable.Range(0, 8).Select(i => (short)Program.StackArgument(i)).ToList();
            if ((ushort)values[4] == Program.Dgroup) values[4] = unchecked((short)L.Segment);
            Calls.Add($"{nameof(FatMikeEventKind.TypeLine)}({string.Join(", ", values)})");
        });
        // the second round's field loop test 0609:046A: counts the passes, ends the loop after PassLimit of them
        m.AddCodeHook(Program.Segment(0x0609), 0x046A, _ =>
        {
            if (Passes++ >= PassLimit) Program.WriteByte(Planting.Players, 0);
        });
        // The drawing calls of the border 13C5 are not recorded (the scene draws the border from DrawBorder).
        m.AddCodeHook(Program.Segment(0x0609), 0x13C8, _ => _muted = true);
        m.AddCodeHook(Program.Segment(0x0609), 0x153A, _ => _muted = false);
        m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
    }

    private string Record(string name, int args)
    {
        if (name == nameof(FatMikeEventKind.TypeCharacter))
        {
            if (Program.StackArgument(2) == 0x0140 && Program.StackArgument(3) == Program.Dgroup)
                return $"{name}({Program.ReadByte(0x0140)})";
            // 0A96:0525 with another text: the contest's text DS:3B70
            name = nameof(FatMikeEventKind.Text);
            args = 6;
        }
        var values = Enumerable.Range(0, args).Select(i => (short)Program.StackArgument(i)).ToList();
        if (name == nameof(FatMikeEventKind.Text) && (ushort)values[3] == Program.Dgroup) values[3] = unchecked((short)L.Segment);
        if (name == nameof(FatMikeEventKind.ShadowText) && (ushort)values[3] == Program.Dgroup) values[3] = unchecked((short)L.Segment);
        if (name == nameof(FatMikeEventKind.Emblem)) values.Add((sbyte)Program.ReadByte(L.ActivePlayer));
        return $"{name}({string.Join(", ", values)})";
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

    /// <summary>The text pointers Fat Mike types (notice, field notices, lines EC…/EF…) with short texts in <see cref="Scratch"/>.</summary>
    public void WriteTexts(Random r)
    {
        string[] texts = ["FAT MIKE IS HERE", "", " PAY UP ", "X", "OR ELSE", " "];
        ushort at = 0x8000;
        int[] pointers =
        [
            FatMikePhase.NoticeLines, FatMikePhase.NoticeLines + 4, FatMikePhase.WarningLines, FatMikePhase.WarningLines + 4,
            .. Enumerable.Range(0, 3).Select(i => FatMikePhase.PunishLines + 4 * i),
            .. Enumerable.Range(0, 3).Select(i => FatMikePhase.TextTable + 4 * (0xEC + i)),
            .. Enumerable.Range(0, 3).Select(i => FatMikePhase.TextTable + 4 * (0xEF + i)),
            ContestPhase.HeadingText, ContestPhase.FieldText, Planting.SecondRoundLines, Planting.SecondRoundLines + 4,
        ];
        foreach (int ptr in pointers)
        {
            if (r.Next(5) == 0)
            {
                Program.WriteDword((ushort)ptr, 0);
                continue;
            }
            var text = texts[r.Next(texts.Length)];
            Program.WriteWord((ushort)ptr, at);
            Program.WriteWord((ushort)(ptr + 2), Scratch);
            foreach (var c in text) Program.Machine.Mem.Write8(Scratch, at++, (byte)c);
            Program.Machine.Mem.Write8(Scratch, at++, 0);
        }
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

/// <summary>The port's side: records the same calls as <see cref="FatMikeHarness"/> and answers controls from the same script.</summary>
internal sealed class FatMikeRecorder(FatMikeHarness h) : IContestHost, IPlantingHost, IFieldTexts
{
    public List<string> Calls { get; } = [];
    public int ControlReads { get; private set; }

    public void Emit(FieldEvent e) =>
        Calls.Add((e.Kind == FieldEventKind.DrawBorder ? e with { Args = e.Args[..4] } : e).ToString());

    public void Emit(FatMikeEvent e) => Calls.Add(e.Kind switch
    {
        FatMikeEventKind.TypeCharacter => $"{e.Kind}({e.Args[0]})",
        FatMikeEventKind.TypeLines => (e with { Args = [.. e.Args[..4], .. e.Args[5..]] }).ToString(),
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

    public int Passes { get; private set; }

    /// <summary>
    /// 0609:0080: recorded when the harness replaces it; else the core's round with this recorder as the field's host (the
    /// computer and the helpers of lane anbau-ki as in the game), its field loop ended after the harness's pass limit.
    /// </summary>
    public IEnumerable<FieldWait> SecondRound(Dgroup d, OriginalRandom random)
    {
        if (h.StubSecondRound)
        {
            Calls.Add("SecondRound()");
            return [];
        }
        return Round(d, random);
    }

    private IEnumerable<FieldWait> Round(Dgroup d, OriginalRandom random)
    {
        ContestPhase.PrepareSecondRound(d, this);
        foreach (var w in Planting.SetupFrames(d, random, this)) yield return w;
        while (Planting.Running(d) && Passes < h.PassLimit)
        {
            foreach (var w in Planting.PassFrames(d, random, this)) yield return w;
            Passes++;
        }
        if (Planting.Running(d)) d.Set(Planting.Players, (byte)0);
        Emit(new FieldEvent(FieldEventKind.LastFrame));
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    // the field's host of the second round: lane anbau-ki's computer and helpers, as PlantingHost
    public void ComputerTurn(Dgroup d, short x, short y) => PlantingAi.ComputerTurn(d, x, y);
    public bool CanPlaceHq(Dgroup d) => PlantingAiHelpers.CanPlant(d);
    public bool HasOwnNeighbour(Dgroup d, short x, short y) => PlantingAiHelpers.HasOwnNeighbour(d, x, y);
    public void ComputerPlacedHq(Dgroup d) => PlantingAi.ChooseWantKind(d);
    public void ComputerBoughtField(Dgroup d) => PlantingAi.CommitCandidate(d);
}
