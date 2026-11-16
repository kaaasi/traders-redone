using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Field;
using Traders.Tests.Input;
using Traders.Tests.Phases;
using C = Traders.Core.Field.FieldCursor;
using F = Traders.Core.Field.FieldGrid;
using H = Traders.Core.Harvest.Harvest;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Field.Planting;

namespace Traders.Tests.Harvest;

/// <summary>
/// Differential tests of the harvest 22A3:02D3 (finding docs/routines/22A3_02D3_ernte.md). The original runs in the RefEmu
/// (LoadOnly or a snapshot of a real game at the entry of 22A3:02D3, direct calls). Its drawing routines, the output layers,
/// sound, controls and the pause check are replaced by a far return and recorded with their arguments; the port emits the
/// same calls as events (or host calls). The shared field routines of segment 0609 (1F2D, 1BE5, 13C5, 11E7, 1FCC, 21BA),
/// 21AF:0412 and the typewriter 0A69:0288/0243/01AD run in the original; the typed character is recorded from DS:0140 at the
/// replaced text call 0A96:0525. Compared: the whole data segment (outside <see cref="OutputOwned"/>), the random state, the
/// number of random() calls and the sequence of recorded calls.
/// </summary>
public class HarvestDifferentialTests
{
    private const ushort Seg = 0x22A3;

    /// <summary>Calls replaced by retf (name = event kind or host method).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x0609, 0x1854, 2, nameof(FieldEventKind.DrawRank)),
        (0x0609, 0x1AC0, 0, nameof(FieldEventKind.DrawPlayer)),
        (0x0609, 0x18E5, 0, nameof(HarvestEventKind.DrawEmptyPanel)),
        (0x0609, 0x1912, 3, nameof(HarvestEventKind.DrawGoods)),
        (0x0609, 0x1F04, 4, nameof(HarvestEventKind.TileArea)),
        (0x0A96, 0x057A, 7, nameof(HarvestEventKind.Number)),
        (0x0A96, 0x0433, 5, nameof(HarvestEventKind.Line)),
        (0x0A96, 0x04D7, 5, nameof(HarvestEventKind.FillRect)),
        (0x0A96, 0x0525, 0, nameof(HarvestEventKind.TypeCharacter)),
        (0x21FD, 0x05DB, 1, nameof(HarvestEventKind.SetFont)),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound)),
        (0x0609, 0x1C83, 2, nameof(FieldEventKind.DrawField)),
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
    ];

    /// <summary>Calls that run in the original and are only recorded.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Traced =
    [
        (0x21AF, 0x0412, 2, nameof(FieldEventKind.ShowObject)),
        (0x0609, 0x13C5, 4, nameof(FieldEventKind.DrawBorder)),
        (0x0A69, 0x0288, 8, nameof(HarvestEventKind.TypeLines)),
    ];

    /// <summary>
    /// Bytes the original writes that belong to the output layers: the object pointers and restore records of 21AF:0412 and
    /// the character buffer DS:0140 of the typewriter 0A69:01AD (drawn as text by the scene).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] OutputOwned =
    [
        .. PlantingDifferentialTests.OutputOwned,
        (0x0140, 1, "0A69:01AD character being typed (Vga text)"),
    ];

    /// <summary>Far segment of the test texts of the weather notice (outside the program).</summary>
    private const ushort TextSegment = 0x9000;

    internal sealed class Harness
    {
        public OriginalProgram Program { get; }
        public byte[] Initial { get; }
        public List<string> Calls { get; } = [];
        public List<short> RandomArgs { get; } = [];

        /// <summary>Actions DS:3308 written by the replaced 0E9E:037A at control read n (same script for the port).</summary>
        public Func<int, sbyte[]>? Script { get; set; }

        public int ControlReads { get; set; }
        private bool _muted;

        public Harness(OriginalProgram? program = null)
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
                    if (name == nameof(HarvestEventKind.TypeCharacter))
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
                    var values = Enumerable.Range(0, args).Where(i => name != nameof(HarvestEventKind.TypeLines) || i != 4)
                        .Select(i => (short)Program.StackArgument(i));
                    Calls.Add($"{name}({string.Join(", ", values)})");
                });
            m.AddCodeHook(Program.Segment(0x0609), 0x13C8, _ => _muted = true);
            m.AddCodeHook(Program.Segment(0x0609), 0x153A, _ => _muted = false);
            m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
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
    }

    internal sealed class Recorder(Harness h) : IHarvestHost
    {
        public List<string> Calls { get; } = [];
        public int ControlReads { get; private set; }

        public void Emit(FieldEvent e) =>
            Calls.Add((e.Kind == FieldEventKind.DrawBorder ? e with { Args = e.Args[..4] } : e).ToString());

        public void Emit(HarvestEvent e) => Calls.Add(e.Kind switch
        {
            HarvestEventKind.TypeCharacter => $"{e.Kind}({e.Args[0]})",
            HarvestEventKind.TypeLines => (e with { Args = [.. e.Args[..4], .. e.Args[5..]] }).ToString(),
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

        public byte[] FarString(ushort offset, ushort segment) =>
            h.FarString(offset, segment == L.Segment ? h.Program.Dgroup : segment);
    }

    private static readonly sbyte[] Kinds = [-2, 0, 1, 2, 3, 4, 0, 1, 2];

    private static readonly string[] Notices =
        ["THE SUN IS SHINING", "", "  RAIN ", "X", "A LONG LINE OF THE WEATHER NOTICE", " "];

    /// <summary>Random harvest state; the RefEmu's DS gets it, the port copies it.</summary>
    private static void Arrange(Harness h, Random r, bool keepGame = false)
    {
        var p = h.Program;
        if (!keepGame) p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), h.Initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void W(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));
        int columns = keepGame ? p.ReadWord(F.Columns) : r.Next(10) == 0 ? r.Next(-1, 10) : r.Next(4, 10);
        int rows = keepGame ? p.ReadWord(F.Rows) : r.Next(10) == 0 ? r.Next(-1, 7) : r.Next(3, 7);
        W(F.Columns, columns);
        W(F.Rows, rows);
        B(F.CurrentPlayer, r.Next(4));
        if (!keepGame)
        {
            W(0x451E, r.Next(0, 80));
            W(0x4528, r.Next(50, 80));
        }
        for (int i = 0; i < 4; i++)
        {
            B(C.CursorX + i, r.Next(12) == 0 ? r.Next(-3, 12) : r.Next(-1, Math.Max(0, columns)));
            B(C.CursorY + i, r.Next(12) == 0 ? r.Next(-3, 9) : r.Next(0, Math.Max(1, rows)));
            B(L.IsComputer + i, r.Next(3) == 0 ? 1 : r.Next(8) == 0 ? r.Next(256) : 0);
            B(C.Mode + i, r.Next(6) == 0 ? r.Next(256) : r.Next(6, 9));
            B(C.Action + i, r.Next(3) == 0 ? 0x0A : r.Next(3) == 0 ? 0 : r.Next(8) == 0 ? new[] { -1, 4, 0x8A, 0x7F }[r.Next(4)] : r.Next(4));
            B(H.Automatic + i, r.Next(4) == 0 ? 1 : r.Next(8) == 0 ? r.Next(256) : 0);
            W(H.Counter + 2 * i, r.Next(4) == 0 ? 0 : r.Next(8) == 0 ? r.Next(-3, 65536) : r.Next(0, 31));
            W(H.ColourOfPlayer + 2 * i, r.Next(65536));
            p.WriteDword((ushort)(C.MoveDelay + 4 * i), (uint)r.Next());
            W(P.ObjectFrame + P.ObjectSize * i, r.Next(0, 9));
            W(P.ObjectX + P.ObjectSize * i, r.Next(0, 320));
            W(P.ObjectY + P.ObjectSize * i, r.Next(0, 200));
            B(P.Objects + P.ObjectSize * i, r.Next(256));
            for (int k = 0; k < 4; k++)
                W(L.Stock + 8 * i + 2 * k, r.Next(4) == 0 ? r.Next(960, 1000) : r.Next(8) == 0 ? r.Next(-40, 1100) : r.Next(0, 1000));
        }
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                if (!keepGame || r.Next(3) == 0)
                {
                    B(F.Owner + x * 6 + y, r.Next(-1, 4));
                    B(F.Kind + x * 6 + y, Kinds[r.Next(Kinds.Length)]);
                }
                if (!keepGame || r.Next(2) == 0)
                    B(H.Yield + x * 0x1A + y, r.Next(4) == 0 ? 0 : r.Next(10) == 0 ? r.Next(256) : r.Next(4, 61));
            }
        B(L.Weather, !keepGame && r.Next(4) == 0 ? r.Next(-2, 8) : r.Next(0, 6));
        B(P.Players, r.Next(5));
        if (!keepGame) W(P.PictureNumber, r.Next(10));
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
        // weather notice: three far pointers per weather, some null, texts outside the program
        if (keepGame && r.Next(2) == 0) return;
        ushort at = 0;
        for (int w = -2; w < 8; w++)
            for (int line = 0; line < 3; line++)
            {
                ushort ptr = (ushort)(H.WeatherLines + 12 * w + 4 * line);
                if (r.Next(6) == 0)
                {
                    p.WriteDword(ptr, 0);
                    continue;
                }
                var text = Notices[r.Next(Notices.Length)];
                p.WriteWord(ptr, at);
                p.WriteWord((ushort)(ptr + 2), TextSegment);
                foreach (var c in text) p.Machine.Mem.Write8(TextSegment, at++, (byte)c);
                p.Machine.Mem.Write8(TextSegment, at++, 0);
            }
    }

    private static void Compare(string label, byte[] original, Dgroup port)
    {
        var o = (byte[])original.Clone();
        foreach (var (off, len, _) in OutputOwned) Array.Copy(port.Bytes, off, o, off, len);
        foreach (var (off, len) in RefEmuWeekOracle.InterruptOwned) Array.Copy(port.Bytes, off, o, off, len);
        ControlHarness.AssertSameData(label, o, port.Bytes);
    }

    private static void AssertSame(Harness h, string label, ushort off, Action<Dgroup, OriginalRandom, Recorder> port)
    {
        var d = new Dgroup(h.Program.DataSegment());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var rec = new Recorder(h);
        h.ControlReads = 0;
        port(d, random, rec);
        d.Set(L.RandomSeed, random.Seed);
        h.Calls.Clear();
        h.RandomArgs.Clear();
        h.ControlReads = 0;
        h.Program.Call(Seg, off);
        Compare(label, h.Program.DataSegment(), d);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
    }

    private static void Drain(IEnumerable<FieldWait> frames)
    {
        foreach (var _ in frames) { }
    }

    /// <summary>Actions per control read: fire, right, other directions, none; the same for both sides.</summary>
    private static Func<int, sbyte[]> Script(int seed, int fireAfter) => read =>
    {
        var sr = new Random(seed + read);
        if (read >= fireAfter) return [0x0A, 0x0A, 0x0A, 0x0A];
        return [.. Enumerable.Range(0, 4).Select(_ => (sbyte)new[] { 0x0A, 0, 0, 1, 2, 3, -1, -1 }[sr.Next(8)])];
    };

    // [DIFFERENTIAL] 22A3:04F7
    // [DIFFERENTIAL] 22A3:0806
    // [DIFFERENTIAL] 0609:21BA
    [GameDataFact]
    public void Next_field_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x04F7);
        int done = 0, collecting = 0;
        for (int n = 0; n < 3000; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"04F7 case {n}", 0x04F7, (d, _, rec) => H.NextField(d, rec));
            int pl = (sbyte)h.Program.ReadByte(F.CurrentPlayer);
            if (h.Program.ReadByte((ushort)(C.Mode + pl)) == H.Done) done++;
            else collecting++;
        }
        Assert.True(done > 100 && collecting > 100, $"done {done}, collecting {collecting}");
    }

    // [DIFFERENTIAL] 22A3:062E
    // [DIFFERENTIAL] 22A3:07C2
    // [DIFFERENTIAL] 22A3:082F
    // [DIFFERENTIAL] 22A3:0874
    [GameDataFact]
    public void Collection_step_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x062E);
        int added = 0, capped = 0, growing = 0;
        for (int n = 0; n < 4000; n++)
        {
            Arrange(h, r);
            int pl = r.Next(4);
            h.Program.WriteByte(F.CurrentPlayer, (byte)pl);
            int x = r.Next(0, 9), y = r.Next(0, 6);
            if (r.Next(8) != 0)
            {
                h.Program.WriteByte((ushort)(C.CursorX + pl), (byte)x);
                h.Program.WriteByte((ushort)(C.CursorY + pl), (byte)y);
            }
            x = (sbyte)h.Program.ReadByte((ushort)(C.CursorX + pl));
            y = (sbyte)h.Program.ReadByte((ushort)(C.CursorY + pl));
            int yieldAt = x * 0x1A + y;
            int amount = h.Program.ReadByte((ushort)(H.Yield + yieldAt));
            if (r.Next(2) == 0) h.Program.WriteWord((ushort)(H.Counter + 2 * pl), (ushort)(amount / 2 + r.Next(-1, 2)));
            int counter = (short)h.Program.ReadWord((ushort)(H.Counter + 2 * pl)) + 1;
            int kind = (sbyte)h.Program.ReadByte((ushort)(F.Kind + x * 6 + y));
            int stockBefore = (short)h.Program.ReadWord((ushort)(L.Stock + 8 * pl + 2 * kind));
            AssertSame(h, $"062E case {n}", 0x062E, (d, _, rec) => H.Collect(d, rec));
            if ((short)counter <= amount / 2) growing++;
            else if (stockBefore + amount > H.StockLimit) capped++;
            else added++;
        }
        Assert.True(added > 100 && capped > 100 && growing > 100, $"added {added}, capped {capped}, growing {growing}");
    }

    // [DIFFERENTIAL] 22A3:04B1
    // [DIFFERENTIAL] 22A3:04DA
    [GameDataFact]
    public void Waiting_step_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x04B1);
        for (int n = 0; n < 2000; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"04B1 case {n}", 0x04B1, (d, _, rec) => H.Advance(d, rec));
            Arrange(h, r);
            AssertSame(h, $"04DA case {n}", 0x04DA, (d, _, _) => H.CheckAutomatic(d));
        }
    }

    // [DIFFERENTIAL] 22A3:0340
    // [DIFFERENTIAL] 22A3:08C1
    // [DIFFERENTIAL] 0A69:0288
    // [DIFFERENTIAL] 0A69:0243
    // [DIFFERENTIAL] 0A69:01AD
    [GameDataFact]
    public void Panels_and_notice_match_the_original()
    {
        var h = new Harness();
        var r = new Random(0x0340);
        int typed = 0;
        for (int n = 0; n < 600; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0340 case {n}", 0x0340, (d, _, rec) => Drain(H.PanelsFrames(d, rec)));
            Arrange(h, r);
            AssertSame(h, $"08C1 case {n}", 0x08C1, (d, _, rec) => Drain(H.NoticeFrames(d, rec)));
            typed += h.Calls.Count(c => c.StartsWith(nameof(HarvestEventKind.TypeCharacter), StringComparison.Ordinal));
        }
        Assert.True(typed > 1000, $"typed {typed}");
    }

    /// <summary>
    /// The collection loop 22A3:0426 pass by pass: the original is stopped at its loop test (22A3:04A3) after every pass, the
    /// data segment there is compared with the port after the same number of passes. The replaced 0E9E:037A writes scripted
    /// actions; after a scripted number of reads all players press fire (automatic), so every run ends.
    /// </summary>
    // [DIFFERENTIAL] 22A3:0426
    [GameDataFact]
    public void Collection_loop_matches_the_original_pass_by_pass()
    {
        var h = new Harness();
        var r = new Random(0x0426);
        var snapshots = new List<byte[]>();
        h.Program.Machine.AddCodeHook(h.Program.Segment(Seg), 0x04A3, _ => snapshots.Add(h.Program.DataSegment()));
        for (int n = 0; n < 150; n++)
        {
            Arrange(h, r);
            // valid harvest start: the panels have run (modes 6 or 8 with the cursor on a field, or 7), players counted
            int players = 0;
            for (int i = 0; i < 4; i++)
            {
                byte mode = (byte)new[] { 6, 7, 7, 8 }[r.Next(4)];
                if (mode == 8)
                {
                    int x = (sbyte)h.Program.ReadByte((ushort)(C.CursorX + i));
                    if (x < 0) h.Program.WriteByte((ushort)(C.CursorX + i), 0);
                }
                h.Program.WriteWord((ushort)(H.Counter + 2 * i), 0); // as left by 0340 and 062E
                h.Program.WriteByte((ushort)(C.Mode + i), mode);
                if (mode != 6) players++;
            }
            h.Program.WriteByte(P.Players, (byte)players);
            h.Script = Script(r.Next(), r.Next(0, 60));

            var d = new Dgroup(h.Program.DataSegment());
            var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
            var rec = new Recorder(h);
            var states = new List<byte[]> { (byte[])d.Bytes.Clone() };
            while (d.U8(P.Players) != 0)
            {
                H.Pass(d, rec);
                states.Add((byte[])d.Bytes.Clone());
                Assert.True(states.Count < 5000, $"0426 case {n}: no end, modes {string.Join(",", Enumerable.Range(0, 4).Select(i => d.U8(C.Mode + i)))} auto {string.Join(",", Enumerable.Range(0, 4).Select(i => d.U8(H.Automatic + i)))} x {string.Join(",", Enumerable.Range(0, 4).Select(i => d.S8(C.CursorX + i)))} y {string.Join(",", Enumerable.Range(0, 4).Select(i => d.S8(C.CursorY + i)))} cols {d.S16(F.Columns)} rows {d.S16(F.Rows)} players {d.U8(P.Players)} cnt {string.Join(",", Enumerable.Range(0, 4).Select(i => d.S16(H.Counter + 2 * i)))}");
            }
            rec.Emit(new FieldEvent(FieldEventKind.LastFrame));

            snapshots.Clear();
            h.Calls.Clear();
            h.RandomArgs.Clear();
            h.ControlReads = 0;
            h.Program.Call(Seg, 0x0426);
            Assert.Equal(states.Count, snapshots.Count);
            for (int i = 0; i < states.Count; i++) Compare($"0426 case {n} pass {i}", snapshots[i], new Dgroup(states[i]));
            Assert.True(h.Calls.SequenceEqual(rec.Calls),
                $"0426 case {n}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
            Assert.Empty(h.RandomArgs);
            h.Script = null;
        }
    }

    // [DIFFERENTIAL] 22A3:02D3
    [GameDataFact]
    public void Harvest_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x02D3);
        for (int n = 0; n < 120; n++)
        {
            Arrange(h, r);
            h.Script = Script(r.Next(), r.Next(0, 80));
            AssertSame(h, $"02D3 case {n}", 0x02D3, (d, rnd, rec) => Drain(H.Run(d, rnd, rec)));
            Assert.Contains(h.Calls, c => c.StartsWith("DrawBorder", StringComparison.Ordinal));
            h.Script = null;
        }
    }

    /// <summary>
    /// The harvest in real games: the original (four computers) is run to the entry of 22A3:02D3 in weeks 0 to 3; there the
    /// state is taken as it is (fields, yields of 22A3:0003, stocks, the text table of the program) and, in further runs,
    /// players are made human, stocks set near the limit and fields varied. Both sides then run 22A3:02D3 to its end.
    /// </summary>
    // [DIFFERENTIAL] 22A3:02D3
    [GameDataFact]
    public void Harvest_in_real_games_matches_the_original()
    {
        var game = OriginalProgram.StartGame(TestGameData.DosDir);
        var r = new Random(0x16);
        for (int week = 0; week < 4; week++)
        {
            Assert.True(game.RunToRoutine(Seg, 0x02D3, 900), $"22A3:02D3 not reached (week {game.Week})");
            Assert.Equal(week, game.Week);
            var snapshot = game.Snapshot();
            for (int run = 0; run < 6; run++)
            {
                var h = new Harness(OriginalProgram.Resume(TestGameData.DosDir, snapshot));
                if (run > 0)
                {
                    for (int i = 0; i < 4; i++)
                        if (r.Next(2) == 0) h.Program.WriteByte((ushort)(L.IsComputer + i), 0);
                    if (run >= 3) Arrange(h, r, keepGame: true);
                }
                h.Script = Script(r.Next(), run == 0 ? 0 : r.Next(0, 120));
                AssertSame(h, $"week {week} run {run}", 0x02D3, (d, rnd, rec) => Drain(H.Run(d, rnd, rec)));
                Assert.Contains(h.Calls, c => c.StartsWith(nameof(HarvestEventKind.TypeCharacter), StringComparison.Ordinal));
                Assert.Contains(h.Calls, c => c.StartsWith(nameof(HarvestEventKind.Line), StringComparison.Ordinal));
            }
        }
    }
}
