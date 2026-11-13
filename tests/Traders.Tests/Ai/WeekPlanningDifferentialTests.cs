using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Phases;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;
using W = Traders.Core.Ai.WeekPlanningLayout;

namespace Traders.Tests.Ai;

/// <summary>
/// Differential tests of the week planning 11A3:0000 (docs/routines/11A3_0000_wochenplanung.md). A real game (default
/// settings, four computer players) runs four weeks in the RefEmu in lockstep with the phase machine; calls 6 and 21 are
/// the port (<see cref="WeekPlanningStep"/>) and are compared at their return. The data segments at their call sites are
/// kept and serve as inputs for single calls with both pass arguments, with mixed human/computer seats, all levels, a set
/// or free target player and randomly varied stocks, fields and settings. Expected values come only from the original.
/// </summary>
public class WeekPlanningDifferentialTests
{
    private const int Weeks = 4;

    private static readonly Lazy<GameRun> Run = new(RunGame);

    /// <summary>Data segment at a call site of 11A3:0000 in the real game.</summary>
    private sealed record CallState(int Week, byte Pass, byte[] Bytes)
    {
        public Dgroup Copy() => new((byte[])Bytes.Clone());

        public override string ToString() => $"week {Week} pass {Pass}";
    }

    private sealed record GameRun(List<CallState> States, List<string> Differences, List<(int Week, int Step, long Draws)> Ported);

    /// <summary>
    /// [DIFFERENTIAL] 11A3:0000 – weeks 0..3 of a real game: calls 6 and 21 are the port, every other call the RefEmu (rule
    /// steps the ported rules); at the return of every ported call the whole data segment and DS:10D2 equal the RefEmu.
    /// </summary>
    [GameDataFact]
    public void Four_weeks_with_ported_week_planning_match_the_original()
    {
        var run = Run.Value;
        Assert.Empty(run.Differences);
        for (int week = 0; week < Weeks; week++)
            Assert.Equal([WeekPlanningStep.BeforePlantingCall, WeekPlanningStep.BeforeTradeCall],
                run.Ported.Where(x => x.Week == week && x.Step is 6 or 21).Select(x => x.Step));
        Assert.Equal(2 * Weeks, run.States.Count);
    }

    /// <summary>
    /// [DIFFERENTIAL] 11A3:0000 – the call states of the real game with both pass arguments, every level (DS:08EA, DS:2E8E),
    /// mixed human/computer seats and every target player DS:4580; then random variations of stocks, cash, prices, rockets,
    /// fields (owner, kind, quality), stations and the flags 4BBE/4BC2. Every listed path of the original
    /// (<see cref="WeekPlanningHarness.Branches"/>) is passed, random(5) is drawn in some runs and not in others.
    /// Counter-checks (local): pass ignored (always 0) → red; random(5) compared with == 1 instead of == 0 → red.
    /// </summary>
    // Every subroutine is checked through 11A3:0000 (one tag per address until tools/abdeckung.py reads collective tags):
    // [DIFFERENTIAL] 11A3:007D
    // [DIFFERENTIAL] 11A3:0180
    // [DIFFERENTIAL] 11A3:043F
    // [DIFFERENTIAL] 11A3:0599
    // [DIFFERENTIAL] 11A3:061A
    // [DIFFERENTIAL] 11A3:067D
    // [DIFFERENTIAL] 11A3:078A
    // [DIFFERENTIAL] 11A3:0AA5
    // [DIFFERENTIAL] 11A3:0B3B
    // [DIFFERENTIAL] 11A3:0D35
    // [DIFFERENTIAL] 11A3:0E01
    // [DIFFERENTIAL] 11A3:0EB8
    // [DIFFERENTIAL] 11A3:0F2E
    // [DIFFERENTIAL] 11A3:104B
    [GameDataFact]
    public void Single_calls_match_the_original_on_varied_game_states()
    {
        var h = new WeekPlanningHarness();
        int runs = 0;
        foreach (var s in Run.Value.States)
        {
            for (byte pass = 0; pass <= 1; pass++)
            {
                h.AssertSame(s.Copy(), pass, $"{s} as pass {pass}");
                runs++;
                for (short level = 0; level <= 3; level++)
                {
                    var d = s.Copy();
                    d.Set(L.Level, level);
                    d.Set(L.ComputerIq, level);
                    h.AssertSame(d, pass, $"{s} pass {pass} level {level}");
                    runs++;
                }
                foreach (int seats in new[] { 0b0000, 0b0001, 0b0101, 0b1010, 0b1110, 0b0111 })
                {
                    var d = s.Copy();
                    for (int p = 0; p < L.PlayerCount; p++) d.Set(L.IsComputer + p, (byte)((seats >> p) & 1));
                    h.AssertSame(d, pass, $"{s} pass {pass} seats {seats:B4}");
                    runs++;
                }
                for (sbyte target = -1; target <= 3; target++)
                {
                    var d = s.Copy();
                    d.Set(W.TargetPlayer, target);
                    h.AssertSame(d, pass, $"{s} pass {pass} target {target}");
                    runs++;
                }
            }
        }

        var r = new Inputs(0x11A3);
        for (int i = 0; i < 600; i++)
        {
            var s = Run.Value.States[i % Run.Value.States.Count];
            var d = s.Copy();
            Vary(d, r);
            byte pass = (byte)(i % 2);
            h.AssertSame(d, pass, $"variation {i} of {s} as pass {pass}");
            runs++;
        }

        var missing = WeekPlanningHarness.Branches.Where(b => !h.Hits.Contains(b.Offset)).Select(b => $"{b.Offset:X4} {b.Path}").ToList();
        Assert.True(missing.Count == 0, "paths not exercised: " + string.Join(", ", missing));
        Assert.InRange(h.RunsWithDraw, 1, runs - 1);
    }

    /// <summary>The step handler binds call 6 to pass 0 and call 21 to pass 1 and finishes at once.</summary>
    [Fact]
    public void Handler_maps_the_calls_to_the_passes()
    {
        Assert.Equal(0, WeekPlanningStep.ForCall(6).Pass);
        Assert.Equal(1, WeekPlanningStep.ForCall(21).Pass);
        Assert.Throws<ArgumentOutOfRangeException>(() => WeekPlanningStep.ForCall(8));
        foreach (var (call, pass) in new[] { (6, 0), (21, 1) })
        {
            var step = WeekLoop.Step(call);
            Assert.Equal(((ushort)0x11A3, (ushort)0x0000, (int?)pass), (step.TargetSegment, step.TargetOffset, step.Argument));
        }
    }

    /// <summary>
    /// Random variation of the inputs 11A3 reads, within values the game can hold (owners -1..3, kinds -2 and 0..4, quality
    /// 0..3, prices 1..999, field counts per kind 0..20, FieldTotal 8..54): the original then never divides by zero.
    /// </summary>
    private static void Vary(Dgroup d, Inputs r)
    {
        for (int p = 0; p < L.PlayerCount; p++)
        {
            if (r.Chance(30)) d.Set(L.IsComputer + p, (byte)(r.Chance(70) ? 1 : 0));
            for (int w = 0; w < L.WareCount; w++)
                if (r.Chance(50)) d.Set(L.Stock + 8 * p + 2 * w, (short)r.Range(0, 999));
            if (r.Chance(30)) d.Set(L.Stock + 8 * p + 6, d.S16(L.Stock + 6)); // ties in ware 3
            if (r.Chance(50)) d.Set(L.Cash + 4 * p, r.Range(-2000, 400000));
            if (r.Chance(40)) d.Set(L.Rockets + 2 * p, (short)r.Range(0, 10));
            if (r.Chance(40)) d.Set(L.FatMikeFlag + p, (byte)r.Range(0, 1));
            if (r.Chance(40)) d.Set(0x4BBE + p, (byte)r.Range(0, 1));
            if (r.Chance(40)) d.Set(L.Rank + p, (byte)r.Range(0, 3));
            if (r.Chance(40)) d.Set(L.Station + 2 * p, (short)r.Range(-1, L.StationCount - 1));
            if (r.Chance(40)) d.Set(W.MainKind + 2 * p, (short)r.Range(0, 2));
            for (int k = 0; k < 4; k++)
                if (r.Chance(40)) d.Set(L.FieldsByKind + 4 * p + k, (sbyte)r.Range(0, 20));
        }
        for (int s = 0; s < L.StationCount; s++)
        {
            if (r.Chance(30)) d.Set(L.StationVisited + s, (byte)r.Range(0, 1));
            if (r.Chance(30)) d.Set(L.StationKind + 2 * s, (short)r.Range(0, 2));
        }
        for (int w = 0; w < L.WareCount; w++)
            if (r.Chance(30)) d.Set(L.WarePrice + 2 * w, (short)r.Range(1, 999));
        if (r.Chance(50)) d.Set(W.TargetPlayer, (sbyte)r.Range(-1, 3));
        if (r.Chance(40)) d.Set(L.FieldTotal, (sbyte)r.Range(8, 54));
        for (int x = 0; x < L.FieldColumnsMax; x++)
            for (int y = 0; y < L.FieldRowsMax; y++)
            {
                int cell = L.FieldIndex(x, y);
                if (r.Chance(25)) d.Set(L.FieldOwner + cell, (sbyte)r.Range(-1, 3));
                if (r.Chance(25)) d.Set(L.FieldKind + cell, (sbyte)(r.Chance(30) ? -2 : r.Range(0, 4)));
                if (r.Chance(25)) d.Set(L.FieldQuality + cell, (sbyte)r.Range(0, 3));
            }
        // the second and later players' leads over the first in ware 3 (paths of 078A for the target player)
        if (r.Chance(30))
        {
            short top = (short)r.Range(300, 999);
            d.Set(L.Stock + 6, top);
            for (int p = 1; p < L.PlayerCount; p++) d.Set(L.Stock + 8 * p + 6, (short)(top - r.Range(1, 400)));
        }
    }

    private static GameRun RunGame()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        var core = new Dgroup(p.DataSegment().AsSpan(0, L.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);

        WeeklyPrices.Apply(core);
        Assert.True(oracle.RunToNextStop() && oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        p.DataSegment().AsSpan(0, L.Size).CopyTo(core.Bytes);
        oracle.ResetPassed();

        var states = new List<CallState>();
        Action<OriginalProgram> Capture(byte pass) => q => states.Add(new CallState(q.Week, pass, q.DataSegment().AsSpan(0, L.Size).ToArray()));
        var handlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(
            s => s.Number,
            s => s.Number is WeekPlanningStep.BeforePlantingCall or WeekPlanningStep.BeforeTradeCall
                ? oracle.PortHandler(core, WeekPlanningStep.ForCall(s.Number), Capture(WeekPlanningStep.ForCall(s.Number).Pass))
                : WeekRuleSteps.Rules.ContainsKey(s.Number)
                    ? oracle.PortHandler(core, WeekRuleSteps.Handler(s.Number))
                    : oracle.Handler(core));
        var machine = new PhaseMachine(core, new OriginalRandom(new RecordedClock([])), handlers, nextStep: 2);
        var differences = new List<string>();
        try
        {
            for (int f = 0; f < 40 * Weeks && !(machine.Week == Weeks && machine.NextStep == 2); f++)
            {
                machine.Advance(new PhaseTick(machine.LastFrame + 1));
                machine.DrainEvents();
            }
        }
        catch (InvalidOperationException e)
        {
            differences.Add(e.Message);
        }
        differences.AddRange(oracle.Differences.Where(x => !differences.Contains(x)));
        if (differences.Count == 0)
        {
            Assert.Equal(Weeks, machine.Week);
            Assert.Equal(Weeks, p.Week);
        }
        return new GameRun(states, differences, oracle.PortedSteps);
    }
}
