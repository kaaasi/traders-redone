using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using C = Traders.Core.Field.FieldCursor;
using F = Traders.Core.Field.FieldGrid;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Field;

/// <summary>
/// Differential tests of the cursor logic 0609:047B and what it calls (finding docs/routines/0609_047B_cursor.md). The
/// original runs in the RefEmu (LoadOnly, direct calls). Its drawing routines (0609:0C73, 0609:1854, 0A96:0030), the sound
/// call 21A7:0054 and the computer player 135D:000E are replaced by a far return and only recorded with their arguments; the
/// port emits the same calls as <see cref="FieldEvent"/>s. Compared: the whole data segment, the random state, the number and
/// arguments of random() calls and the sequence of recorded calls.
/// </summary>
public class FieldCursorDifferentialTests
{
    private const ushort Seg = 0x0609;

    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x0609, 0x0C73, 0, nameof(FieldEventKind.DrawSelection)),
        (0x0609, 0x1854, 2, nameof(FieldEventKind.DrawRank)),
        (0x0A96, 0x0030, 6, nameof(FieldEventKind.DrawCommand)),
        (0x21A7, 0x0054, 2, nameof(FieldEventKind.Sound)),
        (0x135D, 0x000E, 2, "ComputerTurn"),
    ];

    private sealed class Harness
    {
        public OriginalProgram P { get; } = OriginalProgram.LoadOnly(TestGameData.DosDir);
        public byte[] Initial { get; }
        public List<string> Calls { get; } = [];
        public List<short> RandomArgs { get; } = [];

        public Harness()
        {
            Initial = P.DataSegment();
            var m = P.Machine;
            foreach (var (seg, off, args, name) in Stubs)
            {
                ushort s = P.Segment(seg);
                m.AddCodeHook(s, off, _ =>
                    Calls.Add($"{name}({string.Join(", ", Enumerable.Range(0, args).Select(i => (short)P.StackArgument(i)))})"));
                m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
            }
            m.AddCodeHook(P.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)P.StackArgument(0)));
        }
    }

    private sealed class Recorder : IFieldOutput
    {
        public List<string> Calls { get; } = [];
        public void ComputerTurn(Dgroup d, short x, short y) => Calls.Add($"ComputerTurn({x}, {y})");
        public void Emit(FieldEvent e) => Calls.Add(e.ToString());
    }

    private static readonly sbyte[] Kinds = [-2, 0, 1, 2, 3, 4, 4, -2];

    /// <summary>Random field state around the cursor routines; <paramref name="h"/> gets the same bytes as the returned DS.</summary>
    private static void Arrange(Harness h, Random r)
    {
        var p = h.P;
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), h.Initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void W(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));
        W(F.Columns, r.Next(10) == 0 ? r.Next(-1, 11) : r.Next(4, 10));
        W(F.Rows, r.Next(10) == 0 ? r.Next(-1, 8) : r.Next(3, 7));
        B(F.CurrentPlayer, r.Next(4));
        for (int i = 0; i < 4; i++)
        {
            B(C.CursorX + i, r.Next(12) == 0 ? r.Next(-3, 12) : r.Next(0, 9));
            B(C.CursorY + i, r.Next(12) == 0 ? r.Next(-3, 9) : r.Next(0, 6));
            B(L.IsComputer + i, r.Next(5) == 0 ? 1 : r.Next(4) == 0 ? r.Next(256) : 0);
            B(C.Mode + i, r.Next(3) == 0 ? 4 : r.Next(8));
            B(C.Action + i, r.Next(8) == 0 ? new[] { -1, 4, 5, 0x0A, 0x80, 0x7F }[r.Next(6)] : r.Next(4));
            W(C.MoveTimer + 2 * i, r.Next(2) == 0 ? 0 : r.Next(-3, 20));
            W(C.Direction + 2 * i, r.Next(65536));
            W(C.MoveFrame + 2 * i, r.Next(65536));
            B(C.Selection + i, r.Next(-2, 6));
            W(C.TimeLeft + 2 * i, r.Next(3) == 0 ? 0 : r.Next(5) == 0 ? r.Next(-40, 70) : r.Next(1, 48));
            B(C.Blink + i, r.Next(2) == 0 ? 0 : r.Next(1, 21));
            p.WriteDword((ushort)(C.MoveDelay + 4 * i), (uint)r.Next());
            W(C.SpriteFrame + 0x1A * i, r.Next(65536));
            B(C.Sprite + 0x1A * i, r.Next(256));
        }
        for (int c = 0; c < 9 * 6; c++)
        {
            B(F.Owner + c, r.Next(-1, 4));
            B(F.Kind + c, Kinds[r.Next(Kinds.Length)]);
            B(C.Terrain + c, r.Next(3) == 0 ? 3 : r.Next(5));
        }
        B(C.TimeLimit, r.Next(3) == 0 ? 0 : r.Next(1, 3));
        B(C.NoTime, r.Next(4) == 0 ? 1 : r.Next(3) == 0 ? 2 : 0);
        B(C.Finished, r.Next(256));
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
    }

    private static void AssertSame(Harness h, string label, ushort off, Action<Dgroup, OriginalRandom, Recorder> port,
        params ushort[] args)
    {
        var d = new Dgroup(h.P.DataSegment());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var rec = new Recorder();
        port(d, random, rec);
        d.Set(L.RandomSeed, random.Seed);
        h.Calls.Clear();
        h.RandomArgs.Clear();
        h.P.Call(Seg, off, args);
        ControlHarness.AssertSameData(label, h.P.DataSegment(), d.Bytes);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
        Assert.All(h.RandomArgs, a => Assert.Equal(200, a));
    }

    // [DIFFERENTIAL] 0609:047B
    // [DIFFERENTIAL] 0609:0B8E
    // [DIFFERENTIAL] 0609:0BEA
    // [DIFFERENTIAL] 0609:0F0D
    // [DIFFERENTIAL] 0609:0F70
    // [DIFFERENTIAL] 0609:0EA1
    // [DIFFERENTIAL] 0609:0EDD
    // [DIFFERENTIAL] 0609:21BA
    // [DIFFERENTIAL] 21AF:045E
    // [DIFFERENTIAL] 0609:1B72
    // [DIFFERENTIAL] 0609:215E
    [GameDataFact]
    public void Cursor_step_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x047B);
        var seen = new HashSet<string>();
        for (int n = 0; n < 3000; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"047B case {n}", 0x047B, (d, rnd, rec) => C.Step(d, rnd, rec));
            foreach (var c in h.Calls) seen.Add(c[..c.IndexOf('(')]);
        }
        // Every replaced call was reached at least once (the paths behind them are covered).
        foreach (var s in Stubs) Assert.Contains(s.Name, seen);
    }

    [GameDataFact]
    public void Selection_step_matches_the_original_for_all_kinds_and_steps()
    {
        var h = new Harness();
        var r = new Random(0x0BEA);
        for (int n = 0; n < 1500; n++)
        {
            Arrange(h, r);
            short kind = (short)r.Next(-4, 8), step = (short)new[] { -1, 1, 1, -1, 2, -2, 0 }[r.Next(7)];
            short x = (short)r.Next(-2, 11), y = (short)r.Next(-2, 8);
            AssertSame(h, $"0BEA({kind},{step},{x},{y}) case {n}", 0x0BEA,
                (d, _, rec) => C.StepSelection(d, kind, step, x, y, rec), (ushort)kind, (ushort)step, (ushort)x, (ushort)y);
        }
    }

    [GameDataFact]
    public void Neighbour_test_matches_the_original()
    {
        var h = new Harness();
        var r = new Random(0x0F0D);
        for (int n = 0; n < 600; n++)
        {
            Arrange(h, r);
            short x = (short)r.Next(-2, 11), y = (short)r.Next(-2, 8);
            var d = new Dgroup(h.P.DataSegment());
            byte al = (byte)h.P.Call(Seg, 0x0F0D, (ushort)x, (ushort)y).Ax;
            Assert.True(al == (C.HasForeignNeighbour(d, x, y) ? 1 : 0), $"0F0D({x},{y}) case {n}");
            al = (byte)h.P.Call(Seg, 0x0F70, (ushort)x, (ushort)y).Ax;
            Assert.True(al == (C.IsForeign(d, x, y) ? 1 : 0), $"0F70({x},{y}) case {n}");
            ControlHarness.AssertSameData($"case {n}", h.P.DataSegment(), d.Bytes);
        }
    }

    [GameDataFact]
    public void Time_bar_and_time_use_match_the_original()
    {
        var h = new Harness();
        var r = new Random(0x1B72);
        for (int n = 0; n < 600; n++)
        {
            Arrange(h, r);
            sbyte step = (sbyte)new[] { -1, 1, 0, 2, -128 }[r.Next(5)];
            // The high byte of the argument is garbage in the original (only AL is set); 1B72 uses the low byte only.
            ushort arg = (ushort)((r.Next(256) << 8) | (byte)step);
            AssertSame(h, $"1B72({step}) case {n}", 0x1B72, (d, _, rec) => C.TimeBar(d, step, rec), arg);
            Arrange(h, r);
            bool result = true;
            AssertSame(h, $"0EA1 case {n}", 0x0EA1, (d, _, rec) => result = C.UseTime(d, rec));
            Assert.Equal(result ? 1 : 0, (byte)h.P.Machine.Cpu.Ax);
        }
    }

    [GameDataFact]
    public void Idle_starts_a_blink_when_random_draws_zero()
    {
        var h = new Harness();
        var r = new Random(0x215E);
        int started = 0;
        for (uint seed = 1; started < 5; seed++)
        {
            // Input selection only: a seed whose next random(200) is 0 (the expected state comes from the original run).
            if (new OriginalRandom(new RecordedClock([]), seed).Next(200) != 0) continue;
            Arrange(h, r);
            h.P.WriteDword(OriginalProgram.RandomStateOffset, seed);
            int player = h.P.ReadByte(F.CurrentPlayer);
            h.P.WriteByte((ushort)(C.Blink + player), 0);
            AssertSame(h, $"215E seed {seed}", 0x215E, (d, rnd, _) => C.Idle(d, rnd));
            Assert.Equal(0x13, h.P.ReadByte((ushort)(C.Blink + player)));
            started++;
        }
    }
}
