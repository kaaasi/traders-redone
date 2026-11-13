using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using C = Traders.Core.Field.FieldCursor;
using F = Traders.Core.Field.FieldGrid;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Field.Planting;

namespace Traders.Tests.Field;

/// <summary>
/// Differential tests of the planting phase 0609:0004 (finding docs/routines/0609_0004_anbau.md). The original runs in the
/// RefEmu (LoadOnly, direct calls). Its drawing routines, the output layers, sound, controls, pause, the wait loop of lane
/// bilanz and the computer hooks of lane anbau-ki are replaced by a far return and recorded with their arguments; the port
/// emits the same calls as <see cref="FieldEvent"/>s (or host calls). 21AF:000D, 21AF:0412 and the border 0609:13C5 run in the
/// original and are only recorded (their table, restore and drawing writes are the output's and excluded, see
/// <see cref="OutputOwned"/>). The helpers 0609:074D and 0609:100F (lane anbau-ki) run in the original; the port asks the
/// same routines in the RefEmu on its own data segment. Compared: the whole data segment, the random state, the number and
/// arguments of random() calls and the sequence of recorded calls.
/// </summary>
public class PlantingDifferentialTests
{
    private const ushort Seg = 0x0609;

    /// <summary>Calls replaced by retf (name = <see cref="FieldEventKind"/> or host method).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x0609, 0x0C73, 0, nameof(FieldEventKind.DrawSelection)),
        (0x0609, 0x1854, 2, nameof(FieldEventKind.DrawRank)),
        (0x0A96, 0x0030, 6, nameof(FieldEventKind.DrawCommand)),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound)),
        (0x135D, 0x000E, 2, "ComputerTurn"),
        (0x135D, 0x0AB3, 0, "ComputerPlacedHq"),
        (0x135D, 0x0935, 0, "ComputerBoughtField"),
        (0x0609, 0x1C83, 2, nameof(FieldEventKind.DrawField)),
        (0x0609, 0x1CB1, 2, nameof(FieldEventKind.DrawFieldAndNeighbours)),
        (0x0609, 0x1A03, 0, nameof(FieldEventKind.DrawPanel)),
        (0x0609, 0x1A97, 0, nameof(FieldEventKind.DrawPurchaseUsed)),
        (0x0609, 0x1AC0, 0, nameof(FieldEventKind.DrawPlayer)),
        (0x0609, 0x0204, 0, nameof(FieldEventKind.DrawStatus)),
        (0x0609, 0x1ED9, 0, nameof(FieldEventKind.DrawBackground)),
        (0x0609, 0x1256, 4, nameof(FieldEventKind.DrawFrame)),
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
        (0x21AF, 0x000D, 0, nameof(FieldEventKind.InitObjects)),
        (0x21AF, 0x0412, 2, nameof(FieldEventKind.ShowObject)),
        (0x0609, 0x13C5, 4, nameof(FieldEventKind.DrawBorder)),
    ];

    /// <summary>
    /// Bytes the original writes through 21AF:000D and 21AF:0412 that belong to the output layers (Traders.Vga keeps them):
    /// the pointer fields +04…+13h of the eight object records and the restore records DS:33E4 (8 bytes per object).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] OutputOwned =
    [
        .. Enumerable.Range(0, 8).Select(i => (P.Objects + i * P.ObjectSize + 4, 0x10, "21AF object table pointers")),
        (0x33E4, 0x40, "21AF restore records"),
    ];

    private sealed class Harness
    {
        public OriginalProgram Program { get; } = OriginalProgram.LoadOnly(TestGameData.DosDir);
        public byte[] Initial { get; }
        public List<string> Calls { get; } = [];
        public List<short> RandomArgs { get; } = [];

        /// <summary>Actions DS:3308 written by the replaced 0E9E:037A in pass n (same script for the port).</summary>
        public Func<int, sbyte[]>? Script { get; set; }

        public int ControlReads { get; set; }
        private bool _muted;

        public Harness(params (ushort Seg, ushort Off)[] alsoStub)
        {
            Initial = Program.DataSegment();
            var m = Program.Machine;
            foreach (var (seg, off, args, name) in Stubs.Concat(alsoStub.Select(a => (a.Seg, a.Off, 0, "FieldLoop"))))
            {
                ushort s = Program.Segment(seg);
                m.AddCodeHook(s, off, _ =>
                {
                    if (_muted) return;
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
                    Calls.Add($"{name}({string.Join(", ", Enumerable.Range(0, args).Select(i => (short)Program.StackArgument(i)))})");
                });
            // The drawing calls of the border 13C5 are not recorded (the scene draws the border from DrawBorder).
            m.AddCodeHook(Program.Segment(0x0609), 0x13C8, _ => _muted = true);
            m.AddCodeHook(Program.Segment(0x0609), 0x153A, _ => _muted = false);
            m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
        }

        /// <summary>0609:074D or 0609:100F of lane anbau-ki on the port's data segment (the original routine in the RefEmu).</summary>
        public byte Helper(Dgroup d, ushort off, params ushort[] args)
        {
            var saved = Program.DataSegment();
            var calls = Calls.Count;
            Program.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(Program.Dgroup, 0), d.Bytes.AsSpan(0, L.Size).ToArray());
            byte al = (byte)Program.Call(Seg, off, args).Ax;
            ControlHarness.AssertSameData($"helper {off:X4} changed DS", Program.DataSegment(), d.Bytes);
            Program.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(Program.Dgroup, 0), saved);
            Assert.Equal(calls, Calls.Count);
            return al;
        }
    }

    private sealed class Recorder(Harness h) : IPlantingHost
    {
        public List<string> Calls { get; } = [];
        public int ControlReads { get; private set; }
        public void ComputerTurn(Dgroup d, short x, short y) => Calls.Add($"ComputerTurn({x}, {y})");
        public void Emit(FieldEvent e) => Calls.Add(e.ToString());
        public bool CanPlaceHq(Dgroup d) => h.Helper(d, 0x074D) != 0;
        public bool HasOwnNeighbour(Dgroup d, short x, short y) => h.Helper(d, 0x100F, (ushort)x, (ushort)y) == 1;
        public void ComputerPlacedHq(Dgroup d) => Calls.Add("ComputerPlacedHq()");
        public void ComputerBoughtField(Dgroup d) => Calls.Add("ComputerBoughtField()");

        public void ReadControls(Dgroup d)
        {
            Calls.Add("ReadControls()");
            if (h.Script is not { } script) return;
            var a = script(ControlReads++);
            for (int i = 0; i < 4; i++) d.Set(C.Action + i, a[i]);
        }

        public void PauseCheck(Dgroup d) => Calls.Add("PauseCheck()");
        public void WaitLoop(Dgroup d, OriginalRandom random, short mode) => Calls.Add($"WaitLoop({mode})");
    }

    private static readonly sbyte[] Kinds = [-2, 0, 1, 2, 3, 4, 4, -2];

    /// <summary>Random planting state; the RefEmu's DS gets it, the port copies it.</summary>
    private static void Arrange(Harness h, Random r)
    {
        var p = h.Program;
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), h.Initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void W(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));
        int columns = r.Next(10) == 0 ? r.Next(-1, 11) : r.Next(4, 10), rows = r.Next(10) == 0 ? r.Next(-1, 8) : r.Next(3, 7);
        W(F.Columns, columns);
        W(F.Rows, rows);
        B(F.CurrentPlayer, r.Next(4));
        W(MenuOriginX, r.Next(0, 80));
        W(MenuOriginY, r.Next(50, 80));
        for (int i = 0; i < 4; i++)
        {
            B(C.CursorX + i, r.Next(12) == 0 ? r.Next(-3, 12) : r.Next(0, 9));
            B(C.CursorY + i, r.Next(12) == 0 ? r.Next(-3, 9) : r.Next(0, 6));
            B(L.IsComputer + i, r.Next(4) == 0 ? 1 : r.Next(6) == 0 ? r.Next(256) : 0);
            B(P.Mode + i, r.Next(6) == 0 ? r.Next(256) : r.Next(2, 7));
            B(C.Action + i, r.Next(3) == 0 ? 0x0A : r.Next(8) == 0 ? new[] { -1, 4, 0x8A, 0x7F }[r.Next(4)] : r.Next(4));
            W(C.MoveTimer + 2 * i, r.Next(2) == 0 ? 0 : r.Next(5) == 0 ? r.Next(-3, 2) : r.Next(1, 18));
            W(C.Direction + 2 * i, r.Next(4));
            W(C.MoveFrame + 2 * i, r.Next(3) == 0 ? r.Next(-2, 3) : r.Next(0, 12));
            B(C.Selection + i, r.Next(-2, 6));
            W(C.TimeLeft + 2 * i, r.Next(3) == 0 ? 0 : r.Next(1, 48));
            B(C.Blink + i, r.Next(2) == 0 ? 0 : r.Next(1, 21));
            p.WriteDword((ushort)(C.MoveDelay + 4 * i), (uint)r.Next());
            W(P.ObjectFrame + P.ObjectSize * i, r.Next(5) == 0 ? r.Next(65536) : r.Next(0, 9));
            W(P.ObjectX + P.ObjectSize * i, r.Next(0, 320));
            W(P.ObjectY + P.ObjectSize * i, r.Next(0, 200));
            W(P.ObjectLayer + P.ObjectSize * i, r.Next(65536));
            B(P.Objects + P.ObjectSize * i, r.Next(256));
            bool hq = r.Next(3) != 0;
            B(L.HqX + i, hq ? r.Next(0, Math.Max(1, columns)) : -1);
            B(L.HqY + i, hq ? r.Next(0, Math.Max(1, rows)) : r.Next(-1, 6));
            B(P.Purchase + i, r.Next(3) == 0 ? r.Next(256) : r.Next(2));
            bool sold = r.Next(2) == 0;
            B(P.SoldX + i, sold ? r.Next(r.Next(8) == 0 ? -3 : 0, 9) : -1);
            B(P.SoldY + i, sold ? r.Next(0, 6) : r.Next(-1, 6));
            W(L.Rockets + 2 * i, r.Next(3) == 0 ? 0 : r.Next(-2, 11));
            for (int k = 0; k < 4; k++) W(L.Stock + 8 * i + 2 * k, r.Next(3) == 0 ? r.Next(0, 12) : r.Next(-5, 1000));
            p.WriteDword((ushort)(L.Cash + 4 * i), (uint)(r.Next(5) == 0 ? r.Next(int.MinValue, int.MaxValue) : r.Next(-5000, 200000)));
            B(L.FatMikeFlag + i, r.Next(3) == 0 ? r.Next(256) : 0);
        }
        for (int c = 0; c < 9 * 6; c++)
        {
            B(F.Owner + c, r.Next(-1, 4));
            B(F.Kind + c, Kinds[r.Next(Kinds.Length)]);
            B(C.Terrain + c, r.Next(3) == 0 ? 3 : r.Next(5));
        }
        B(P.PlantCost, r.Next(5) == 0 ? r.Next(-128, 128) : r.Next(6, 31));
        W(L.LandPrice, r.Next(5) == 0 ? r.Next(65536) : r.Next(100, 551));
        W(L.ComputerIq, r.Next(6) == 0 ? r.Next(65536) : r.Next(4));
        B(L.Weather, r.Next(3) == 0 ? new[] { 3, 5 }[r.Next(2)] : r.Next(-2, 8));
        B(F.FreeFields, r.Next(4) == 0 ? r.Next(-128, 1) : r.Next(0, 54));
        W(P.PictureNumber, r.Next(10));
        B(C.TimeLimit, r.Next(3) == 0 ? 0 : r.Next(1, 3));
        B(P.NoTime, r.Next(3) == 0 ? 1 : r.Next(4) == 0 ? 2 : 0);
        B(P.Players, r.Next(256));
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
    }

    private const int MenuOriginX = 0x451E, MenuOriginY = 0x4528;

    private static void Compare(Harness h, string label, byte[] original, Dgroup port, bool outputOwned = false)
    {
        var o = (byte[])original.Clone();
        if (outputOwned)
            foreach (var (off, len, _) in OutputOwned)
                Array.Copy(port.Bytes, off, o, off, len);
        ControlHarness.AssertSameData(label, o, port.Bytes);
    }

    private static void AssertSame(Harness h, string label, ushort off, Action<Dgroup, OriginalRandom, Recorder> port,
        bool outputOwned = false)
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
        Compare(h, label, h.Program.DataSegment(), d, outputOwned);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
    }

    // [DIFFERENTIAL] 0609:064D
    // [DIFFERENTIAL] 0609:069F
    // [DIFFERENTIAL] 0609:07D6
    // [DIFFERENTIAL] 0609:096A
    // [DIFFERENTIAL] 0609:0AEF
    [GameDataFact]
    public void Field_actions_match_the_original()
    {
        var h = new Harness();
        var r = new Random(0x064D);
        var seen = new HashSet<string>();
        for (int n = 0; n < 6000; n++)
        {
            Arrange(h, r);
            // Bias towards the interesting cases: cursor on the own HQ, own field, free field next to an own one.
            int pl = h.Program.ReadByte(F.CurrentPlayer);
            int x = (sbyte)h.Program.ReadByte((ushort)(C.CursorX + pl)), y = (sbyte)h.Program.ReadByte((ushort)(C.CursorY + pl));
            switch (r.Next(5))
            {
                case 0:
                    h.Program.WriteByte((ushort)(L.HqX + pl), (byte)x);
                    h.Program.WriteByte((ushort)(L.HqY + pl), (byte)y);
                    break;
                case 1 when x is >= 0 and < 9 && y is >= 0 and < 6:
                    h.Program.WriteByte((ushort)(F.Owner + x * 6 + y), (byte)pl);
                    if (r.Next(2) == 0)
                    {
                        h.Program.WriteByte((ushort)(P.SoldX + pl), (byte)x);
                        h.Program.WriteByte((ushort)(P.SoldY + pl), (byte)y);
                    }
                    break;
                case 2 when x is >= 1 and < 9 && y is >= 0 and < 6:
                    h.Program.WriteByte((ushort)(F.Owner + x * 6 + y), 0xFF);
                    h.Program.WriteByte((ushort)(F.Owner + (x - 1) * 6 + y), (byte)pl);
                    h.Program.WriteByte((ushort)(P.Purchase + pl), 1);
                    break;
            }
            AssertSame(h, $"064D case {n}", 0x064D, (d, _, rec) => P.Action(d, rec));
            foreach (var c in h.Calls) seen.Add(c[..c.IndexOf('(')]);
        }
        foreach (var name in new[] { "DrawField", "DrawFieldAndNeighbours", "DrawPanel", "DrawPurchaseUsed", "DrawSelection",
                     "DrawRank", "Sound", "ComputerPlacedHq", "ComputerBoughtField" })
            Assert.Contains(name, seen);
    }

    // [DIFFERENTIAL] 0609:1FE0
    [GameDataFact]
    public void Cursor_move_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x1FE0);
        for (int n = 0; n < 2000; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"1FE0 case {n}", 0x1FE0, (d, _, rec) => P.Move(d, rec));
        }
    }

    // [DIFFERENTIAL] 0609:0160
    // [DIFFERENTIAL] 0609:032A
    // [DIFFERENTIAL] 0609:1FCC
    // [DIFFERENTIAL] 21AF:0412
    // [DIFFERENTIAL] 0609:0FA9
    // [DIFFERENTIAL] 0609:11E7
    // [DIFFERENTIAL] 0609:1F2D
    // [DIFFERENTIAL] 0609:1EA3
    // [DIFFERENTIAL] 0609:1BE5
    // [DIFFERENTIAL] 0609:13C5
    [GameDataFact]
    public void Single_set_up_routines_match_the_original()
    {
        var h = new Harness();
        var r = new Random(0x0160);
        for (int n = 0; n < 1500; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0160 case {n}", 0x0160, (d, _, _) => P.StartPositions(d));
            Arrange(h, r);
            AssertSame(h, $"032A case {n}", 0x032A, (d, _, rec) => P.PlaceCursors(d, rec), outputOwned: true);
            Arrange(h, r);
            AssertSame(h, $"0FA9 case {n}", 0x0FA9, (d, _, _) => P.Settle(d));
            Arrange(h, r);
            AssertSame(h, $"11E7 case {n}", 0x11E7, (d, _, rec) => P.WeatherColours(d, rec));
            Arrange(h, r);
            AssertSame(h, $"1F2D case {n}", 0x1F2D, (d, _, rec) => P.CursorObjects(d, rec));
            Arrange(h, r);
            AssertSame(h, $"1EA3 case {n}", 0x1EA3, (d, _, rec) => P.Panels(d, rec));
            if (n % 5 != 0) continue;
            Arrange(h, r);
            AssertSame(h, $"1BE5 case {n}", 0x1BE5, (d, rnd, rec) => P.Board(d, rnd, rec));
        }
    }

    // [DIFFERENTIAL] 0609:0004
    // [DIFFERENTIAL] 0609:00D6
    // [DIFFERENTIAL] 21AF:000D
    [GameDataFact]
    public void Planting_set_up_and_settlement_match_the_original()
    {
        var h = new Harness((0x0609, 0x03E0));
        var r = new Random(0x0004);
        var seen = new HashSet<string>();
        for (int n = 0; n < 400; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0004 case {n}", 0x0004, (d, rnd, rec) =>
            {
                P.Prepare(d, rec);
                P.Setup(d, rnd, rec);
                rec.Calls.Add("FieldLoop()");
                P.Settle(d);
            }, outputOwned: true);
            Assert.Contains("FieldLoop()", h.Calls);
            // 00D6 alone, also with DS:1582 = 1 (second round of 0609:0080: no wait loop, cursors shown at once)
            Arrange(h, r);
            AssertSame(h, $"00D6 case {n}", 0x00D6, (d, rnd, rec) =>
            {
                P.Setup(d, rnd, rec);
                rec.Calls.Add("FieldLoop()");
            }, outputOwned: true);
            seen.UnionWith(h.Calls.Select(c => c[..c.IndexOf('(')]));
        }
        Assert.Contains("WaitLoop", seen);
        Assert.Contains("ShowObject", seen);
    }

    /// <summary>
    /// The field loop 0609:03E0 pass by pass: the original is stopped at its loop test (0609:046A) after every pass, the data
    /// segment there is compared with the port after the same number of <see cref="Planting.Pass"/>es. The replaced 0E9E:037A
    /// writes scripted actions (fire, directions, none) in both. After at most 40 passes the test ends the original's loop.
    /// </summary>
    // [DIFFERENTIAL] 0609:03E0
    [GameDataFact]
    public void Field_loop_matches_the_original_pass_by_pass()
    {
        var h = new Harness();
        var r = new Random(0x03E0);
        var snapshots = new List<byte[]>();
        int limit = 40;
        h.Program.Machine.AddCodeHook(h.Program.Segment(Seg), 0x046A, _ =>
        {
            snapshots.Add(h.Program.DataSegment());
            if (snapshots.Count > limit) h.Program.WriteByte(P.Players, 0);
        });
        for (int n = 0; n < 60; n++)
        {
            Arrange(h, r);
            for (int i = 0; i < 4; i++) // valid modes and some players still on the field
                h.Program.WriteByte((ushort)(P.Mode + i), (byte)r.Next(2, 7));
            h.Program.WriteByte(P.Players, (byte)r.Next(1, 5));
            var seed = r.Next();
            sbyte[] Script(int pass)
            {
                var sr = new Random(seed + pass);
                return [.. Enumerable.Range(0, 4).Select(_ => (sbyte)new[] { 0x0A, 0x0A, 0, 1, 2, 3, -1, -1 }[sr.Next(8)])];
            }
            h.Script = Script;
            limit = r.Next(1, 41);

            var d = new Dgroup(h.Program.DataSegment());
            var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
            var rec = new Recorder(h);
            var states = new List<byte[]>();
            states.Add((byte[])d.Bytes.Clone());
            int passes = 0;
            while (P.Running(d) && passes < limit)
            {
                P.Pass(d, random, rec);
                d.Set(L.RandomSeed, random.Seed);
                states.Add((byte[])d.Bytes.Clone());
                passes++;
            }
            if (P.Running(d)) d.Set(P.Players, (byte)0); // the test ends the loop like in the original
            rec.Emit(new FieldEvent(FieldEventKind.LastFrame));

            snapshots.Clear();
            h.Calls.Clear();
            h.RandomArgs.Clear();
            h.ControlReads = 0;
            h.Program.Call(Seg, 0x03E0);
            Assert.Equal(states.Count, snapshots.Count);
            for (int i = 0; i < states.Count; i++)
                ControlHarness.AssertSameData($"03E0 case {n} pass {i}", snapshots[i], states[i]);
            Assert.True(h.Calls.SequenceEqual(rec.Calls),
                $"03E0 case {n}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
            Assert.Equal(h.RandomArgs.Count, (int)random.Calls);
            h.Script = null;
        }
    }
}
