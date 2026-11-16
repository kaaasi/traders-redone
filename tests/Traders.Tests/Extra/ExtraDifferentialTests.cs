using Traders.Core;
using Traders.Core.Extra;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using C = Traders.Core.Field.FieldCursor;
using F = Traders.Core.Field.FieldGrid;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Field.Planting;
using X = Traders.Core.Extra.ExtraPhase;

namespace Traders.Tests.Extra;

/// <summary>
/// Differential tests of the extra phase 1DEF:000C and its routines (finding docs/routines/1DEF_000C_zusatzphase.md) against
/// the original in the RefEmu (<see cref="ExtraHarness"/>): the same state on both sides, then the routine – compared are the
/// whole data segment (outside <see cref="ExtraHarness.OutputOwned"/>), the three stack arrays, the random state, the number
/// and arguments of random() calls and the sequence of recorded calls.
/// </summary>
public class ExtraDifferentialTests
{
    private const ushort Seg = ExtraHarness.Seg;

    /// <summary>Random state of the phase; the RefEmu's DS (and with <paramref name="board"/> its scratch arrays) gets it.</summary>
    private static ExtraBoard Arrange(ExtraHarness h, Random r, bool keepGame = false, bool board = true)
    {
        var p = h.Program;
        if (!keepGame) p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), h.Initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void W(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));
        B(F.CurrentPlayer, r.Next(4));
        for (int i = 0; i < 4; i++)
        {
            B(C.CursorX + i, r.Next(20) == 0 ? r.Next(-1, 7) : r.Next(0, 6));
            B(C.CursorY + i, r.Next(20) == 0 ? r.Next(-1, 8) : r.Next(0, 7));
            if (!keepGame || r.Next(3) == 0) B(L.IsComputer + i, r.Next(3) == 0 ? 1 : r.Next(8) == 0 ? r.Next(256) : 0);
            B(C.Action + i, r.Next(5) == 0 ? -1 : r.Next(6) == 0 ? 0x0A : r.Next(8) == 0 ? new[] { 4, 5, -2, 0x7F, -128 }[r.Next(5)] : r.Next(4));
            W(C.MoveTimer + 2 * i, r.Next(2) == 0 ? 0 : r.Next(8) == 0 ? r.Next(-3, 65536) : r.Next(1, 0x12));
            W(C.Direction + 2 * i, r.Next(4));
            W(C.MoveFrame + 2 * i, r.Next(4) == 0 ? r.Next(-2, 4) : 0);
            B(C.Blink + i, r.Next(3) == 0 ? r.Next(0, 20) : 0);
            W(X.ColourOfPlayer + 2 * i, r.Next(65536));
            W(P.ObjectFrame + P.ObjectSize * i, r.Next(0, 9));
            W(P.ObjectX + P.ObjectSize * i, r.Next(0, 320));
            W(P.ObjectY + P.ObjectSize * i, r.Next(0, 200));
            B(P.Objects + P.ObjectSize * i, r.Next(256));
            for (int k = 0; k < 4; k++)
            {
                W(L.Stock + 8 * i + 2 * k, r.Next(6) == 0 ? r.Next(32700, 32768) : r.Next(8) == 0 ? r.Next(-40, 65536) : r.Next(0, 1000));
                W(X.Gains + 8 * i + 2 * k, r.Next(4) == 0 ? 0 : r.Next(8) == 0 ? r.Next(65536) : r.Next(0, 200));
            }
        }
        B(L.LimitLow, r.Next(8) == 0 ? r.Next(256) : r.Next(3, 31));
        W(X.Collected, r.Next(8) == 0 ? r.Next(-5, 65536) : r.Next(0, 25));
        W(L.LoopWare, r.Next(65536));
        W(L.ProductionIdle, r.Next(4) == 0 ? r.Next(65536) : r.Next(0, 40));
        W(L.ProductionActive, r.Next(4) == 0 ? r.Next(65536) : r.Next(0, 40));
        if (!keepGame)
        {
            B(L.Weather, r.Next(4) == 0 ? r.Next(-2, 8) : r.Next(0, 6));
            W(P.PictureNumber, r.Next(10));
            W(P.CardMode, r.Next(6) == 0 ? new[] { 2, 0x63, 0 }[r.Next(3)] : 1);
        }
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
        if (!keepGame || r.Next(2) == 0) h.WriteNotice(r);
        var b = new ExtraBoard();
        if (!board) return b;
        int density = r.Next(1, 8);
        for (int i = 0; i < ExtraBoard.Cells; i++)
        {
            b.Ground[i] = (short)(r.Next(30) == 0 ? r.Next(-2, 7) : r.Next(0, 4));
            b.Goods[i] = (short)(r.Next(density) == 0 ? r.Next(0, 4) : -1);
            b.Claims[i] = r.Next(4) == 0 ? (byte)r.Next(4) : r.Next(20) == 0 ? (byte)r.Next(256) : (byte)0xFF;
        }
        h.PointToScratch();
        h.WriteBoard(b);
        return b;
    }

    /// <summary>Runs the port on a copy of the RefEmu's state, then the original; compares everything.</summary>
    private static (ushort Ax, ushort Dx) AssertSame(ExtraHarness h, string label, ushort off,
        Action<Dgroup, OriginalRandom, ExtraRecorder, ExtraBoard> port, ExtraBoard? board, params ushort[] args)
    {
        var d = new Dgroup(h.Program.DataSegment());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var rec = new ExtraRecorder(h);
        var b = board is null ? new ExtraBoard() : ExtraHarness.Copy(board);
        port(d, random, rec, b);
        d.Set(L.RandomSeed, random.Seed);
        h.Calls.Clear();
        h.RandomArgs.Clear();
        h.ControlReads = 0;
        var result = h.Program.Call(Seg, off, args);
        ExtraHarness.Compare(label, h.Program.DataSegment(), d);
        if (board is not null) ExtraHarness.AssertSameBoard(label, h.ReadBoard(), b);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
        return result;
    }

    private static void Drain(IEnumerable<FieldWait> frames)
    {
        foreach (var _ in frames) { }
    }

    /// <summary>A random state whose next random(20) is 0 (<paramref name="zero"/>) or not.</summary>
    internal static uint SeedFor(Random r, bool zero)
    {
        while (true)
        {
            uint seed = (uint)r.Next() * 2654435761u;
            var probe = new OriginalRandom(new RecordedClock([]), seed);
            if ((probe.Next(0x14) == 0) == zero) return seed;
        }
    }

    // [DIFFERENTIAL] 1DEF:00DB
    [GameDataFact]
    public void Decision_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x00DB);
        int[] seen = new int[3]; // by chance, by production, not
        for (int n = 0; n < 3000; n++)
        {
            Arrange(h, r, board: false);
            if (r.Next(3) == 0) h.Program.WriteDword(OriginalProgram.RandomStateOffset, SeedFor(r, true));
            if (r.Next(3) == 0) // borders of 2·active < idle
            {
                int active = r.Next(-5, 30), idle = 2 * active + r.Next(-1, 2);
                h.Program.WriteWord(L.ProductionActive, unchecked((ushort)active));
                h.Program.WriteWord(L.ProductionIdle, unchecked((ushort)idle));
            }
            bool port = false;
            var (ax, _) = AssertSame(h, $"00DB case {n}", 0x00DB, (d, rnd, _, _) => port = X.Triggered(d, rnd), null);
            Assert.Equal(port, (ax & 0xFF) != 0);
            short a = (short)h.Program.ReadWord(L.ProductionActive), i = (short)h.Program.ReadWord(L.ProductionIdle);
            seen[(ax & 0xFF) == 0 ? 2 : (short)(a << 1) < i ? 1 : 0]++;
        }
        Assert.True(seen.All(s => s > 200), $"seen {string.Join(", ", seen)}");
    }

    // [DIFFERENTIAL] 1DEF:0173
    // [DIFFERENTIAL] 1DEF:01AF
    // [DIFFERENTIAL] 1DEF:020E
    [GameDataFact]
    public void Board_set_up_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x0173);
        for (int n = 0; n < 400; n++)
        {
            var b = Arrange(h, r);
            AssertSame(h, $"0173 case {n}", 0x0173, (_, rnd, _, pb) => X.Ground(rnd, pb), b);
            b = Arrange(h, r);
            AssertSame(h, $"01AF case {n}", 0x01AF, (_, rnd, _, pb) => X.Scatter(rnd, pb), b);
            Assert.Equal(X.Pieces, h.ReadBoard().Goods.Count(g => g != -1));
            b = Arrange(h, r);
            AssertSame(h, $"020E case {n}", 0x020E, (_, _, _, pb) => X.ClearClaims(pb), b);
        }
    }

    // [DIFFERENTIAL] 1DEF:0308
    [GameDataFact]
    public void Cell_drawing_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x0308);
        for (int n = 0; n < 1500; n++)
        {
            var b = Arrange(h, r);
            short cell = (short)r.Next(0, ExtraBoard.Cells);
            AssertSame(h, $"0308 case {n}", 0x0308, (d, _, rec, pb) => X.DrawCell(d, rec, pb, cell), b, (ushort)cell);
        }
    }

    // [DIFFERENTIAL] 1DEF:0227
    // [DIFFERENTIAL] 1DEF:0105
    // [DIFFERENTIAL] 1DEF:03CD
    [GameDataFact]
    public void Board_notice_and_players_match_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x0227);
        int typed = 0;
        for (int n = 0; n < 300; n++)
        {
            var b = Arrange(h, r);
            AssertSame(h, $"0227 case {n}", 0x0227, (d, rnd, rec, pb) => Drain(X.BoardFrames(d, rnd, rec, pb)), b);
            Assert.Contains(h.Calls, c => c.StartsWith(nameof(FieldEventKind.DrawBorder), StringComparison.Ordinal));
            b = Arrange(h, r);
            AssertSame(h, $"0105 case {n}", 0x0105, (d, _, rec, _) => Drain(X.NoticeFrames(d, rec)), b);
            typed += h.Calls.Count(c => c.StartsWith(nameof(ExtraEventKind.TypeCharacter), StringComparison.Ordinal));
            b = Arrange(h, r);
            AssertSame(h, $"03CD case {n}", 0x03CD, (d, rnd, rec, _) => X.Players(d, rnd, rec), b);
        }
        Assert.True(typed > 500, $"typed {typed}");
    }

    // [DIFFERENTIAL] 1DEF:0651
    // [DIFFERENTIAL] 1DEF:06F8
    [GameDataFact]
    public void Picking_up_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x0651);
        int picked = 0, empty = 0;
        for (int n = 0; n < 3000; n++)
        {
            var b = Arrange(h, r);
            int pl = (sbyte)h.Program.ReadByte(F.CurrentPlayer);
            int x = (sbyte)h.Program.ReadByte((ushort)(C.CursorX + pl)), y = (sbyte)h.Program.ReadByte((ushort)(C.CursorY + pl));
            int cell = y * 6 + x;
            if (cell < 0 || cell >= ExtraBoard.Cells)
            {
                h.Program.WriteByte((ushort)(C.CursorX + pl), 0);
                h.Program.WriteByte((ushort)(C.CursorY + pl), 0);
                cell = 0;
            }
            if (r.Next(2) == 0)
            {
                b.Goods[cell] = (short)r.Next(0, 4);
                h.WriteBoard(b);
            }
            if (b.Goods[cell] == -1) empty++;
            else picked++;
            AssertSame(h, $"0651 case {n}", 0x0651, (d, _, rec, pb) => X.Collect(d, rec, pb), b);
            b = Arrange(h, r);
            h.Program.WriteWord(L.LoopWare, (ushort)r.Next(0, 4));
            AssertSame(h, $"06F8 case {n}", 0x06F8, (d, _, rec, _) => X.ShowGain(d, rec), b);
        }
        Assert.True(picked > 1000 && empty > 500, $"picked {picked}, empty {empty}");
    }

    // [DIFFERENTIAL] 1DEF:07AD
    [GameDataFact]
    public void Computer_direction_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x07AD);
        var actions = new int[4];
        int none = 0;
        for (int n = 0; n < 4000; n++)
        {
            var b = Arrange(h, r);
            if (r.Next(10) == 0) // nothing left for this player: the target stays cell 0
            {
                for (int i = 0; i < ExtraBoard.Cells; i++) if (r.Next(2) == 0) b.Goods[i] = -1; else b.Claims[i] = (byte)((h.Program.ReadByte(F.CurrentPlayer) + 1) % 4);
                h.WriteBoard(b);
                none++;
            }
            AssertSame(h, $"07AD case {n}", 0x07AD, (d, _, _, pb) => X.Aim(d, pb), b);
            int pl = (sbyte)h.Program.ReadByte(F.CurrentPlayer);
            actions[h.Program.ReadByte((ushort)(C.Action + pl)) & 3]++;
        }
        Assert.True(actions.All(a => a > 300) && none > 200, $"actions {string.Join(", ", actions)}, none {none}");
    }

    // [DIFFERENTIAL] 1DEF:0751
    [GameDataFact]
    public void Crediting_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x0751);
        int above = 0, wrapped = 0;
        for (int n = 0; n < 1000; n++)
        {
            Arrange(h, r, board: false);
            var before = h.Program.DataSegment();
            AssertSame(h, $"0751 case {n}", 0x0751, (d, _, _, _) => X.Credit(d), null);
            for (int i = 0; i < 16; i++)
            {
                int stock = BitConverter.ToInt16(before, L.Stock + 2 * i), gain = BitConverter.ToInt16(before, X.Gains + 2 * i);
                short after = (short)h.Program.ReadWord((ushort)(L.Stock + 2 * i));
                if (after > 999) above++;
                if (after != stock + gain) wrapped++;
            }
        }
        // no upper bound: stocks above 999 stay, sums beyond 7FFFh wrap (16-bit)
        Assert.True(above > 1000 && wrapped > 100, $"above 999 {above}, wrapped {wrapped}");
    }

    /// <summary>Actions per control read for the humans; the same for both sides.</summary>
    internal static Func<int, sbyte[]> Script(int seed, bool idle) => read =>
    {
        var sr = new Random(seed + read);
        sbyte[] choices = idle ? [-1, -1, 0, 1, 2, 3, 0x0A, 4] : [0, 1, 2, 3, 0, 1, 2, 3, -1];
        return [.. Enumerable.Range(0, 4).Select(_ => choices[sr.Next(choices.Length)])];
    };

    /// <summary>
    /// The walk 1DEF:04A9 pass by pass: the original is stopped at its loop test (1DEF:0636) before every pass, the data segment
    /// and the arrays there are compared with the port after the same number of passes. Humans follow a scripted control; at
    /// least one computer collects so every run ends.
    /// </summary>
    // [DIFFERENTIAL] 1DEF:04A9
    // [DIFFERENTIAL] 0609:1FE0
    // [DIFFERENTIAL] 0609:215E
    [GameDataFact]
    public void Walk_matches_the_original_pass_by_pass()
    {
        var h = new ExtraHarness();
        var r = new Random(0x04A9);
        var snapshots = new List<(byte[] Ds, ExtraBoard Board)>();
        h.Program.Machine.AddCodeHook(h.Program.Segment(Seg), 0x0636, _ => snapshots.Add((h.Program.DataSegment(), h.ReadBoard())));
        long passes = 0;
        for (int n = 0; n < 60; n++)
        {
            var b = Arrange(h, r);
            // a valid start: 0173/01AF/020E/03CD have run (Collect counts from 0, no move running, cursors on the board)
            var rs = new OriginalRandom(new RecordedClock([]), (uint)r.Next());
            X.Ground(rs, b);
            X.Scatter(rs, b);
            X.ClearClaims(b);
            h.WriteBoard(b);
            h.Program.WriteWord(X.Collected, (ushort)(r.Next(4) == 0 ? r.Next(0, 25) : 0));
            int computers = 0;
            for (int i = 0; i < 4; i++)
            {
                h.Program.WriteByte((ushort)(C.CursorX + i), (byte)r.Next(0, 6));
                h.Program.WriteByte((ushort)(C.CursorY + i), (byte)r.Next(0, 7));
                h.Program.WriteWord((ushort)(C.MoveTimer + 2 * i), 0);
                h.Program.WriteWord((ushort)(C.MoveFrame + 2 * i), 0);
                bool computer = r.Next(2) == 0;
                h.Program.WriteByte((ushort)(L.IsComputer + i), (byte)(computer ? 1 : 0));
                if (computer) computers++;
            }
            if (computers == 0) h.Program.WriteByte((ushort)(L.IsComputer + r.Next(4)), 1);
            h.Script = Script(r.Next(), r.Next(2) == 0);

            var d = new Dgroup(h.Program.DataSegment());
            var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
            var rec = new ExtraRecorder(h);
            var pb = ExtraHarness.Copy(b);
            var states = new List<(byte[] Ds, ExtraBoard Board)> { ((byte[])d.Bytes.Clone(), ExtraHarness.Copy(pb)) };
            while (X.Running(d))
            {
                X.Pass(d, random, rec, pb);
                d.Set(L.RandomSeed, random.Seed);
                states.Add(((byte[])d.Bytes.Clone(), ExtraHarness.Copy(pb)));
                Assert.True(states.Count < 20_000, $"04A9 case {n}: no end, collected {d.S16(X.Collected)}");
            }
            rec.Emit(new FieldEvent(FieldEventKind.LastFrame));

            snapshots.Clear();
            h.Calls.Clear();
            h.RandomArgs.Clear();
            h.ControlReads = 0;
            h.Program.Call(Seg, 0x04A9);
            Assert.Equal(states.Count, snapshots.Count);
            for (int i = 0; i < states.Count; i++)
            {
                var o = snapshots[i].Ds;
                ExtraHarness.Compare($"04A9 case {n} pass {i}", o, new Dgroup(states[i].Ds));
                ExtraHarness.AssertSameBoard($"04A9 case {n} pass {i}", snapshots[i].Board, states[i].Board);
            }
            ExtraHarness.Compare($"04A9 case {n} end", h.Program.DataSegment(), d);
            Assert.True(h.Calls.SequenceEqual(rec.Calls),
                $"04A9 case {n}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
            Assert.True(h.RandomArgs.Count == random.Calls, $"04A9 case {n}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
            passes += states.Count - 1;
            h.Script = null;
        }
        Assert.True(passes > 5000, $"passes {passes}");
    }

    /// <summary>
    /// The whole phase from LoadOnly states: entered (by chance or by production) or not; the port gets the frame the original
    /// had (SS:BP at 1DEF:000F) for the far pointers. The wait loop 21FD:0004 is replaced on both sides.
    /// </summary>
    // [DIFFERENTIAL] 1DEF:000C
    [GameDataFact]
    public void Phase_matches_the_original()
    {
        var h = new ExtraHarness();
        var r = new Random(0x000C);
        int entered = 0, skipped = 0;
        for (int n = 0; n < 60; n++)
        {
            Arrange(h, r, board: false);
            bool enter = r.Next(3) != 0;
            h.Program.WriteDword(OriginalProgram.RandomStateOffset, SeedFor(r, enter && r.Next(2) == 0));
            h.Program.WriteWord(L.ProductionActive, (ushort)r.Next(0, 20));
            h.Program.WriteWord(L.ProductionIdle, (ushort)(enter ? 50 : 0));
            for (int i = 0; i < 4; i++) h.Program.WriteByte((ushort)(L.IsComputer + i), (byte)(r.Next(3) == 0 ? 0 : 1));
            if (Enumerable.Range(0, 4).All(i => h.Program.ReadByte((ushort)(L.IsComputer + i)) == 0)) h.Program.WriteByte(L.IsComputer, 1);
            h.Script = Script(r.Next(), r.Next(2) == 0);
            RunWhole(h, $"000C case {n}", out bool fell);
            if (fell) entered++;
            else skipped++;
            h.Script = null;
        }
        Assert.True(entered > 20 && skipped > 10, $"entered {entered}, skipped {skipped}");
    }

    /// <summary>Runs 1DEF:000C on both sides (the original first, for its frame) and compares everything.</summary>
    internal static void RunWhole(ExtraHarness h, string label, out bool fell)
    {
        var d = new Dgroup(h.Program.DataSegment());
        h.Calls.Clear();
        h.RandomArgs.Clear();
        h.ControlReads = 0;
        h.Program.Call(Seg, 0x000C);
        var frame = h.EnteredFrame ?? throw new InvalidOperationException("1DEF:000C not entered");
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var rec = new ExtraRecorder(h);
        var board = new ExtraBoard();
        Drain(X.Run(d, random, rec, board, frame));
        d.Set(L.RandomSeed, random.Seed);
        ExtraHarness.Compare(label, h.Program.DataSegment(), d);
        fell = h.Calls.Count > 0;
        if (fell) ExtraHarness.AssertSameBoard(label, h.ReadBoard(), board);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
    }

    /// <summary>
    /// The phase in real games: the original (four computers) is run to the entry of 1DEF:000C in weeks 0 to 3; there the state
    /// is taken as it is (stocks, 2F26, production counts, the text table of the program) and, in further runs, players are made
    /// human and the decision forced either way through the random state and the production counts. The first run of each week
    /// also check the frame of the game at the entry (<see cref="ExtraStep.GameFrame"/>).
    /// </summary>
    // [DIFFERENTIAL] 1DEF:000C
    [GameDataFact]
    public void Phase_in_real_games_matches_the_original()
    {
        var game = OriginalProgram.StartGame(TestGameData.DosDir);
        var r = new Random(0x24);
        int entered = 0, skipped = 0;
        for (int week = 0; week < 4; week++)
        {
            Assert.True(game.RunToRoutine(Seg, 0x000C, 900), $"1DEF:000C not reached (week {game.Week})");
            Assert.Equal(week, game.Week);
            var cpu = game.Machine.Cpu; // at the entry: BP will be SP − 2 after push bp
            Assert.Equal(ExtraStep.GameFrame, new ExtraFrame((ushort)(cpu.Sp - 2), (ushort)(cpu.Ss - game.Dgroup + L.Segment)));
            var snapshot = game.Snapshot();
            for (int run = 0; run < 6; run++)
            {
                var h = new ExtraHarness(OriginalProgram.Resume(TestGameData.DosDir, snapshot));
                if (run > 0)
                {
                    for (int i = 0; i < 4; i++)
                        if (r.Next(2) == 0) h.Program.WriteByte((ushort)(L.IsComputer + i), 0);
                    if (run >= 3) Arrange(h, r, keepGame: true, board: false);
                    if (Enumerable.Range(0, 4).All(i => h.Program.ReadByte((ushort)(L.IsComputer + i)) == 0))
                        h.Program.WriteByte((ushort)(L.IsComputer + r.Next(4)), 1);
                    bool enter = run != 5;
                    h.Program.WriteDword(OriginalProgram.RandomStateOffset, SeedFor(r, enter && r.Next(2) == 0));
                    h.Program.WriteWord(L.ProductionActive, (ushort)r.Next(0, 10));
                    h.Program.WriteWord(L.ProductionIdle, (ushort)(enter ? 30 : 0));
                }
                h.Script = Script(r.Next(), r.Next(2) == 0);
                RunWhole(h, $"week {week} run {run}", out bool fell);
                if (fell)
                {
                    entered++;
                    Assert.Contains(h.Calls, c => c.StartsWith(nameof(ExtraEventKind.TypeCharacter), StringComparison.Ordinal));
                    Assert.Contains(h.Calls, c => c.StartsWith(nameof(ExtraEventKind.Number), StringComparison.Ordinal));
                }
                else skipped++;
            }
            game.Machine.RunFor(0.05);
        }
        Assert.True(entered >= 16 && skipped >= 4, $"entered {entered}, skipped {skipped}");
    }
}
