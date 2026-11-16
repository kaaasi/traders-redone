using Traders.Core.Input;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Rules;
using Xunit.Abstractions;
using Traders.Core.Stations;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Tests.Stations;

/// <summary>
/// The scene of the stations phase picture by picture against the original (<see cref="StationsSceneHarness"/>, finding
/// docs/routines/1B03_0009_stationen.md, section "Szene", lane stationen part 3).
/// </summary>
public sealed class StationsSceneTests(ITestOutputHelper output)
{
    private static void SetState(OriginalProgram p, Inputs r, bool humans, bool pirates)
    {
        var d = new Dgroup(p.DataSegment()[..L.Size]);
        StationsDifferentialTests.Fill(d, r, humans);
        if (pirates) d.Set(L.PiratePresence, (short)2);
        for (int q = 0; q < 4; q++)
        {
            d.Set(ControlReader.MoveDelay + 4 * q, 0);
            d.Set(ControlReader.FireDelay + 4 * q, 0);
        }
        p.Machine.Mem.WriteBlock((uint)(p.Dgroup * 16), d.Bytes);
    }

    /// <summary>
    /// [DIFFERENTIAL] 1B03:0009 – the whole phase with humans (scripted keys) and computers from random states (every other one
    /// with pirates present), small font:
    /// at every retrace wait the four VRAM planes, the DAC, the CRTC start and the command list equal the original's, the
    /// number of pictures and the end state too. Every kind of output call is played at least once.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0009
    [GameDataFact]
    public void Scene_with_humans()
    {
        var r = new Inputs(0x5CE0);
        int pictures = 0, lost = 0;
        var calls = new HashSet<StationCall>();
        for (int i = 0; i < 12; i++)
        {
            bool humans = i % 4 != 3;
            var keys = StationsDifferentialTests.ChoiceKeys(300 + i);
            var (n, c, end) = StationsSceneHarness.AssertSame(p => SetState(p, r, humans, i % 2 == 0), 0, keys, $"state {i}");
            pictures += n;
            calls.UnionWith(c);
            lost += Enumerable.Range(0, 4).Sum(q => (sbyte)end[S.Lost + q]);
        }
        var missing = Enum.GetValues<StationCall>().Except(calls).ToList();
        output.WriteLine($"pictures {pictures}, rockets lost {lost}, calls not played: {string.Join(", ", missing)}");
        Assert.True(lost > 0, "no rocket lost");
        Assert.Empty(missing);
    }

    /// <summary>
    /// [DIFFERENTIAL] 1B03:0009 – the phase of the default game (four computers) in week 0 from the week loop's call, picture
    /// by picture.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0009
    [GameDataFact]
    public void Scene_game_week_0()
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, Field.FieldSceneHarness.AtWeek.Value);
        Assert.True(p.RunToRoutine(StationsLockstep.Seg, 0x0009, 600), "1B03:0009 not reached.");
        int n = StationsSceneHarness.AssertSame(_ => { }, 0, _ => [], "week 0", p.Snapshot()).Pictures;
        output.WriteLine($"week 0: pictures {n}");
    }

    /// <summary>
    /// Handler of call 11 (<see cref="StationsStep"/>, not wired into the week loop): the phase with humans and computers from
    /// random states and the default game's week 0 through a phase machine started at call 11, one tick per frame, with an
    /// empty rule page at the start. Per state the number of ticks equals the original's retrace waits (R6S-1); pictures, end
    /// state and the stop at call 12 as above.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0009
    [GameDataFact]
    public void Step_handler_call_11()
    {
        var r = new Inputs(0x0B11);
        int pictures = 0;
        for (int i = 0; i < 4; i++)
        {
            bool humans = i != 3;
            var keys = StationsDifferentialTests.ChoiceKeys(400 + i);
            pictures += StationsSceneHarness.AssertSame(p => SetState(p, r, humans, i % 2 == 0), 0, keys, $"state {i}",
                viaStep: true).Pictures;
        }
        var w = OriginalProgram.Resume(TestGameData.DosDir, Field.FieldSceneHarness.AtWeek.Value);
        Assert.True(w.RunToRoutine(StationsLockstep.Seg, 0x0009, 600), "1B03:0009 not reached.");
        int week0 = StationsSceneHarness.AssertSame(_ => { }, 0, _ => [], "week 0", w.Snapshot(), viaStep: true).Pictures;
        output.WriteLine($"pictures {pictures}, week 0: {week0}");
    }
}
