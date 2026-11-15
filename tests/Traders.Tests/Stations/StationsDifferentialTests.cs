using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Tests.Stations;

/// <summary>
/// Differential tests of the pixel-free routines of the stations phase 1B03:0009 (finding docs/routines/1B03_0009_stationen.md,
/// lane stationen part 1) through <see cref="StationsHarness"/>: random input states, the original routine in the RefEmu, the
/// port on the same bytes; data segment with DS:10D2, number of random() calls, AX and the output calls must match.
/// </summary>
public sealed class StationsDifferentialTests
{
    private static StationsHarness? _harness;
    private static readonly object Lock = new();

    private static StationsHarness H()
    {
        lock (Lock) return _harness ??= new StationsHarness();
    }

    private static (IEnumerable<FieldWait>?, Func<int>?) Act(Action a) => (null, () => { a(); return 0; });

    /// <summary>A data segment with every value the phase reads set from <paramref name="r"/> within the game's ranges.</summary>
    private static Dgroup State(Inputs r, bool humans = true) => Fill(H().Initial(), r, humans);

    /// <summary>Sets every value the phase reads in <paramref name="d"/> from <paramref name="r"/>; returns <paramref name="d"/>.</summary>
    internal static Dgroup Fill(Dgroup d, Inputs r, bool humans = true)
    {
        d.Set(L.RandomSeed, r.Next());
        d.Set(L.ActivePlayer, (sbyte)r.Range(-3, 7));
        d.Set(L.ComputerIq, (short)r.Range(0, 3));
        d.Set(L.PiratePresence, (short)r.Range(0, 3));
        d.Set(L.GemTrading, (byte)r.Range(0, 1));
        d.Set(L.OilConsumption, (byte)r.Range(0, 1));
        d.Set(L.GemPriceFactor, r.Range(40, 160));
        var stations = Enumerable.Range(0, 12).OrderBy(_ => r.Next()).ToArray();
        for (int i = 0; i < 12; i++)
        {
            int v = r.Range(0, 5);
            d.Set(L.StationValue + 2 * i, (short)(v == 0 ? 10 : v == 1 || v == 2 ? -1 : v - 3 + r.Range(0, 1)));
            d.Set(L.StationKind + 2 * i, (short)r.Range(0, 2));
            d.Set(L.StationVisited + i, (byte)r.Range(0, 1));
            d.Set(S.Taken + i, (byte)(r.Chance(70) ? 0 : r.Range(1, 2)));
        }
        for (int p = 0; p < 4; p++)
        {
            bool computer = !humans || r.Chance(50);
            d.Set(L.IsComputer + p, (byte)(computer ? 1 : 0));
            d.Set(ControlReader.ControlType + p, (byte)(p % 2 == 0 ? 4 : 3));
            d.Set(L.Station + 2 * p, (short)(r.Chance(40) ? -1 : stations[p]));
            d.Set(S.Cursor + 2 * p, (short)r.Range(0, 11));
            d.Set(L.FieldsByKind + 4 * p + 3, (sbyte)(r.Chance(20) ? 0 : r.Range(1, 12)));
            d.Set(L.Rank + p, (byte)r.Range(0, 3));
            d.Set(L.GemCredit + 2 * p, (short)r.Range(0, 2100));
            d.Set(L.Oil + 2 * p, (short)r.Range(0, 45));
            d.Set(L.Cash + 4 * p, r.Chance(30) ? r.Range(-2000, 1500) : r.Range(0, 2_000_000));
            d.Set(S.Lost + p, (sbyte)r.Range(0, 6));
            d.Set(S.Occupied + p, (byte)(r.Chance(20) ? 1 : 0));
        }
        d.Set(S.PiratePosition, (sbyte)(r.Chance(40) ? -1 : r.Range(0, 2)));
        d.Set(S.PirateState, (sbyte)(r.Chance(40) ? 0 : r.Range(1, 18)));
        d.Set(S.Attacked, (sbyte)(r.Chance(60) ? -1 : r.Range(0, 5)));
        d.Set(S.PiratesReady, (byte)r.Range(0, 1));
        d.Set(S.Flash, (sbyte)r.Range(-1, 8));
        d.Set(S.ShipMoved, (byte)r.Range(0, 1));
        d.Set(S.LightFrame, (byte)r.Range(0, 3));
        d.Set(S.LightCounter, (short)r.Range(-2, 12));
        d.Set(S.RocketIndex, (sbyte)r.Range(0, 5));
        d.Set(S.RocketY, (short)r.Range(150, 199));
        d.Set(L.FieldColumns, (short)r.Range(4, 9));
        d.Set(L.FieldRows, (short)r.Range(3, 6));
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                int c = L.FieldIndex(x, y);
                d.Set(L.FieldOwner + c, (sbyte)r.Range(-1, 3));
                d.Set(L.FieldKind + c, (sbyte)(r.Chance(40) ? 3 : r.Range(-2, 4)));
            }
        return d;
    }

    // [DIFFERENTIAL] 1B03:0079
    [GameDataFact]
    public void Station_values_0079()
    {
        var r = new Inputs(0x0079);
        for (int i = 0; i < 150; i++)
            H().AssertSame(State(r), 0x0079, [], ph => Act(ph.DrawStationValues), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:148B
    [GameDataFact]
    public void Object_setup_148B() =>
        H().AssertSame(State(new Inputs(0x148B)), 0x148B, [], ph => Act(ph.SetUpObjects), "objects");

    // [DIFFERENTIAL] 1B03:0101
    // [DIFFERENTIAL] 1B03:13EE
    [GameDataFact]
    public void New_stations_0101()
    {
        var r = new Inputs(0x0101);
        int renewed = 0;
        for (int i = 0; i < 80; i++)
        {
            var d = State(r);
            var o = H().RunOriginal(d, 0x0101, []);
            if (o.Calls.Count > 0) renewed++;
            H().AssertSame(d, 0x0101, [], ph => (ph.NewStations(), null), $"state {i}");
        }
        Assert.InRange(renewed, 3, 40);
    }

    // [DIFFERENTIAL] 1B03:0153
    [GameDataFact]
    public void Station_marks_0153()
    {
        var r = new Inputs(0x0153);
        for (int i = 0; i < 30; i++)
            H().AssertSame(State(r), 0x0153, [], ph => (ph.DrawStationMarks(), null), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:01C3
    // [DIFFERENTIAL] 1B03:1257
    // [DIFFERENTIAL] 1B03:128D
    // [DIFFERENTIAL] 1B03:1345
    [GameDataFact]
    public void Pirates_01C3()
    {
        var r = new Inputs(0x01C3);
        for (int i = 0; i < 120; i++)
            H().AssertSame(State(r), 0x01C3, [], ph => (ph.PlacePirates(), null), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:0201
    // [DIFFERENTIAL] 1B03:0548
    // [DIFFERENTIAL] 1B03:141D
    // [DIFFERENTIAL] 1B03:1617
    [GameDataFact]
    public void Cursors_0201()
    {
        var r = new Inputs(0x0201);
        for (int i = 0; i < 60; i++)
            H().AssertSame(State(r), 0x0201, [], ph => (ph.PlaceCursors(), null), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:0413
    [GameDataFact]
    public void Cursor_step_0413()
    {
        var r = new Inputs(0x0413);
        for (int i = 0; i < 120; i++)
        {
            var d = State(r);
            d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
            short step = (short)(r.Chance(50) ? 1 : -1);
            if (r.Chance(15)) d.Set(S.Cursor + 2 * d.S8(L.ActivePlayer), (short)(step == 1 ? 11 : 0));
            H().AssertSame(d, 0x0413, [(ushort)step], ph => Act(() => ph.MoveCursor(step)), $"state {i}, step {step}");
        }
    }

    // [DIFFERENTIAL] 1B03:04D4
    [GameDataFact]
    public void Take_station_04D4()
    {
        var r = new Inputs(0x04D4);
        for (int i = 0; i < 120; i++)
        {
            var d = State(r);
            d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
            H().AssertSame(d, 0x04D4, [], ph => Act(ph.TakeStation), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0632
    [GameDataFact]
    public void Best_station_0632()
    {
        var r = new Inputs(0x0632);
        for (int i = 0; i < 300; i++)
        {
            var d = State(r);
            d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
            H().AssertSameResult(d, 0x0632, [], ph => ph.BestStation(), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:05E7
    [GameDataFact]
    public void Computer_controls_05E7()
    {
        var r = new Inputs(0x05E7);
        for (int i = 0; i < 100; i++)
            H().AssertSame(State(r), 0x05E7, [], ph => Act(ph.SetComputerControls), $"state {i}");
    }

    /// <summary>
    /// Keys per pass for the two human key sets (cursor keys for players 0 and 2, letter keys for 1 and 3): random presses at
    /// first, from pass 40 on fire every fourth pass and a step right in between, so every human confirms in the end.
    /// </summary>
    internal static Func<int, IReadOnlyCollection<byte>> ChoiceKeys(int seed) => pass =>
    {
        var keys = new List<byte>();
        uint h = (uint)(seed * 7919 + pass * 104729);
        h ^= h >> 13;
        h *= 0x5BD1E995;
        h ^= h >> 15;
        foreach (var (right, left, fire, shift) in new (byte, byte, byte, int)[] { (0x4D, 0x4B, 0x52, 0), (0x2D, 0x2C, 0x39, 8) })
        {
            if (pass >= 40)
            {
                if (pass % 4 == 0) keys.Add(fire);
                else if (pass % 4 == 2 && ((h >> shift) & 1) == 0) keys.Add(right);
                continue;
            }
            switch ((h >> shift) % 6)
            {
                case 0: keys.Add(right); break;
                case 1: keys.Add(left); break;
                case 2: keys.Add(fire); break;
            }
        }
        return keys;
    };

    // [DIFFERENTIAL] 1B03:02C0
    // [DIFFERENTIAL] 1B03:12D7
    // [DIFFERENTIAL] 1B03:131B
    [GameDataFact]
    public void Choice_loop_02C0()
    {
        var r = new Inputs(0x02C0);
        for (int i = 0; i < 24; i++)
        {
            var d = State(r, humans: i % 3 != 0);
            for (int s = 0; s < 12; s++) d.Set(S.Taken + s, (byte)0);
            for (int p = 0; p < 4; p++)
            {
                short st = d.S16(L.Station + 2 * p);
                if (st != -1) d.Set(S.Taken + st, (byte)1);
                d.Set(ControlReader.MoveDelay + 4 * p, 0);
                d.Set(ControlReader.FireDelay + 4 * p, 0);
            }
            var keys = ChoiceKeys(i);
            H().AssertSame(d, 0x02C0, [], ph => (ph.Choose(), null), $"state {i}", keys);
        }
    }

    // [DIFFERENTIAL] 1B03:00D4
    // [DIFFERENTIAL] 1B03:15D7
    [GameDataFact]
    public void Selection_00D4()
    {
        var r = new Inputs(0x00D4);
        for (int i = 0; i < 12; i++)
        {
            var d = State(r, humans: i % 2 == 0);
            for (int p = 0; p < 4; p++)
            {
                d.Set(ControlReader.MoveDelay + 4 * p, 0);
                d.Set(ControlReader.FireDelay + 4 * p, 0);
            }
            H().AssertSame(d, 0x00D4, [], ph => (ph.Selection(), null), $"state {i}", ChoiceKeys(100 + i));
        }
    }

    // [DIFFERENTIAL] 1B03:0740
    [GameDataFact]
    public void Flight_reset_0740() =>
        H().AssertSame(State(new Inputs(0x0740)), 0x0740, [], ph => Act(ph.ResetFlight), "reset");

    // [DIFFERENTIAL] 1B03:0763
    // [DIFFERENTIAL] 1B03:10EA
    [GameDataFact]
    public void Occupied_messages_0763()
    {
        var r = new Inputs(0x0763);
        for (int i = 0; i < 60; i++)
            H().AssertSame(State(r), 0x0763, [], ph => (ph.OccupiedMessages(), null), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:10EA
    [GameDataTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(11)]
    public void Message_10EA(short count)
    {
        var d = State(new Inputs((uint)(0x10EA + count)));
        d.Set(L.ActivePlayer, (sbyte)(count & 3));
        H().AssertSame(d, 0x10EA, [(ushort)S.CashBonusText, H().Dgroup, (ushort)count],
            ph => (ph.Message(S.CashBonusText, count), null), $"count {count}");
    }

    // [DIFFERENTIAL] 1B03:07C4
    // [DIFFERENTIAL] 1B03:0D75
    // [DIFFERENTIAL] 1B03:1454
    [GameDataFact]
    public void Rocket_lines_07C4()
    {
        var r = new Inputs(0x07C4);
        for (int i = 0; i < 40; i++)
        {
            var d = State(r);
            d.Set(S.RocketTotal, (sbyte)r.Range(-3, 10));
            H().AssertSame(d, 0x07C4, [], ph => (ph.PrepareRockets(), null), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0849
    [GameDataFact]
    public void Launch_schedule_0849()
    {
        var r = new Inputs(0x0849);
        for (int i = 0; i < 150; i++)
        {
            var d = State(r);
            int total = 0;
            for (int p = 0; p < 4; p++)
            {
                sbyte n = (sbyte)(r.Chance(20) ? 0 : r.Range(1, 13));
                d.Set(L.FieldsByKind + 4 * p + 3, n);
                d.Set(S.Launched + p, (byte)0);
                total += n;
            }
            d.Set(S.RocketTotal, (sbyte)total);
            d.Set(S.Flying, (byte)0);
            H().AssertSame(d, 0x0849, [], ph => Act(ph.ScheduleLaunches), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0CCF
    [GameDataFact]
    public void Rocket_position_0CCF()
    {
        var r = new Inputs(0x0CCF);
        for (int i = 0; i < 400; i++)
        {
            var d = State(r);
            int p = r.Range(0, 3);
            d.Set(L.ActivePlayer, (sbyte)p);
            d.Set(L.Station + 2 * p, (short)r.Range(0, 11));
            short x = d.S16(S.StationX + 2 * d.S16(L.Station + 2 * p));
            short target = (short)(r.Chance(10) ? x + 8 : r.Range(10, 308));
            short progress = (short)(r.Chance(10) ? (r.Chance(50) ? 0 : 0x59) : r.Range(0, 0x59));
            H().AssertSame(d, 0x0CCF, [(ushort)target, (ushort)progress], ph => Act(() => ph.RocketPosition(target, progress)),
                $"state {i}, target {target}, progress {progress}");
        }
    }

    // [DIFFERENTIAL] 1B03:0A62
    // [DIFFERENTIAL] 1B03:139D
    // [DIFFERENTIAL] 1B03:13C9
    [GameDataFact]
    public void Pirate_step_0A62()
    {
        var r = new Inputs(0x0A62);
        for (int i = 0; i < 400; i++)
            H().AssertSame(State(r), 0x0A62, [], ph => Act(ph.PirateStep), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:0B7A
    [GameDataFact]
    public void Attack_decision_0B7A()
    {
        var r = new Inputs(0x0B7A);
        for (int i = 0; i < 400; i++)
        {
            var d = State(r);
            d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
            d.Set(S.RocketY, (short)(r.Chance(20) ? 0xBE + r.Range(0, 1) : r.Range(0, 199)));
            H().AssertSame(d, 0x0B7A, [], ph => Act(ph.AttackDecision), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0C89
    // [DIFFERENTIAL] 1B03:0CAC
    [GameDataFact]
    public void Flash_0CAC()
    {
        var r = new Inputs(0x0CAC);
        for (int i = 0; i < 12; i++)
        {
            var d = State(r);
            H().AssertSame(d, 0x0C89, [], ph => (ph.FlashStep(), null), $"step {i}");
            H().AssertSame(d, 0x0CAC, [], ph => (ph.FlashOut(), null), $"out {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0DC0
    [GameDataFact]
    public void All_lost_0DC0()
    {
        var r = new Inputs(0x0DC0);
        for (int i = 0; i < 60; i++)
        {
            var d = State(r);
            for (int p = 0; p < 4; p++)
                if (r.Chance(40)) d.Set(S.Lost + p, d.S8(L.FieldsByKind + 4 * p + 3));
            H().AssertSame(d, 0x0DC0, [], ph => (ph.AllLostMessages(), null), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:12D7
    [GameDataFact]
    public void Lights_12D7()
    {
        var r = new Inputs(0x12D7);
        for (int i = 0; i < 120; i++)
        {
            short period = (short)r.Range(-1, 12);
            H().AssertSame(State(r), 0x12D7, [(ushort)period], ph => Act(() => ph.AnimateLights(period)), $"state {i}");
        }
    }

    // [DIFFERENTIAL] 1B03:0E11
    [GameDataFact]
    public void Lost_fields_0E11()
    {
        var r = new Inputs(0x0E11);
        for (int i = 0; i < 120; i++)
            H().AssertSame(State(r), 0x0E11, [], ph => Act(ph.RemoveLostFields), $"state {i}");
    }

    // [DIFFERENTIAL] 1B03:0EA9
    [GameDataFact]
    public void Bonuses_0EA9()
    {
        var r = new Inputs(0x0EA9);
        int bonuses = 0;
        for (int i = 0; i < 1200; i++)
        {
            var d = State(r);
            if (r.Chance(30)) d.Set(L.Cash + 4 * r.Range(0, 3), r.Chance(50) ? 1000 : 999);
            for (int p = 0; p < 4; p++)
            {
                // Near the caps 40 (oil) and 2000 (gem credit): the sum hits the cap exactly or just above it.
                if (r.Chance(50)) d.Set(L.Oil + 2 * p, (short)r.Range(17, 40));
                if (r.Chance(50)) d.Set(L.GemCredit + 2 * p, (short)r.Range(1700, 2000));
            }
            var o = H().RunOriginal(d, 0x0EA9, []);
            bonuses += o.Calls.Count(c => c.StartsWith(nameof(StationCall.NumberText)));
            H().AssertSame(d, 0x0EA9, [], ph => (ph.Bonuses(), null), $"state {i}");
        }
        Assert.True(bonuses > 50, $"only {bonuses} bonuses");
    }
}
