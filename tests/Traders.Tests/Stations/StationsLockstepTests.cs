using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Rules;
using Xunit.Abstractions;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Tests.Stations;

/// <summary>
/// The pixel-reading routines of the stations phase and the whole phase 1B03:0009 in lockstep with the original's real
/// drawing (<see cref="StationsLockstep"/>, finding docs/routines/1B03_0009_stationen.md, lane stationen part 2).
/// </summary>
public sealed class StationsLockstepTests(ITestOutputHelper output)
{
    private static IEnumerable<FieldWait> Once(Action a)
    {
        a();
        yield break;
    }

    private static void LoadPicture(OriginalProgram p) => p.Call(0x186C, 0x0002, p.ReadWord(S.Picture));

    /// <summary>Writes a random phase state (<see cref="StationsDifferentialTests.Fill"/>) into the original's data segment.</summary>
    private static void SetState(OriginalProgram p, Inputs r, bool humans)
    {
        var d = new Dgroup(p.DataSegment()[..L.Size]);
        StationsDifferentialTests.Fill(d, r, humans);
        for (int q = 0; q < 4; q++)
        {
            d.Set(ControlReader.MoveDelay + 4 * q, 0);
            d.Set(ControlReader.FireDelay + 4 * q, 0);
        }
        p.Machine.Mem.WriteBlock((uint)(p.Dgroup * 16), d.Bytes);
    }

    /// <summary>
    /// [DIFFERENTIAL] 1B03:1178 – star field on the real draw page: V-RAKETE loaded by 186C:0002, the other page holding the
    /// menu picture, eight random states and both draw-page parities. Every read equals the pixel reader's result, the
    /// random calls and the data segment match; afterwards both VRAM pages equal the rule page in full.
    /// </summary>
    // [DIFFERENTIAL] 1B03:1178
    [GameDataFact]
    public void Star_field_1178()
    {
        int hits = 0;
        for (int i = 0; i < 8; i++)
        {
            uint seed = new Inputs((uint)(0x1178 + i)).Next();
            void SetUp(OriginalProgram p)
            {
                if (i % 2 == 1) p.Call(0x21FD, 0x051F);
                LoadPicture(p);
                p.WriteDword(OriginalProgram.RandomStateOffset, seed);
            }
            var o = StationsLockstep.RunOriginal(SetUp, 0x1178, 0, _ => []);
            Assert.Null(o.Result.Fault);
            var (run, phase) = StationsLockstep.RunPort(o, 0, ph => Once(ph.StarField), _ => []);
            Assert.Equal(200, o.Result.Reads.Count);
            Assert.Equal(o.Result.Reads, run.Reads);
            Assert.Equal(o.Result.RandomCalls, run.RandomCalls);
            Assert.Empty(StationsLockstep.Diff(o.Result.Ds, run.Ds));
            for (int page = 0; page < 2; page++)
                Assert.Equal(StationsLockstep.VramPixels(o.Program, page), phase.Pixels!.Page.PagePixels(page));
            hits += o.Result.Reads.Count(r => r.Value == 1);
        }
        output.WriteLine($"star reads on colour 1: {hits} of 1600");
        Assert.InRange(hits, 100, 1500);
    }

    /// <summary>The default game (four computers) at the entry of 1B03:0009 in weeks 0, 1 and 2 (one run from the new game).</summary>
    private static readonly Lazy<MachineSnapshot[]> AtStations = new(() =>
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, Field.FieldSceneHarness.AtWeek.Value);
        var snaps = new MachineSnapshot[3];
        for (int week = 0; week < 3; week++)
        {
            Assert.True(p.RunToRoutine(StationsLockstep.Seg, 0x0009, 600), $"1B03:0009 not reached in week {week}.");
            Assert.Equal(week, p.Week);
            snaps[week] = p.Snapshot();
            p.Machine.Run(1);
        }
        return snaps;
    });

    /// <summary>
    /// [DIFFERENTIAL] 1B03:0009 – the stations phase of the default game (four computers, small font as the game leaves it)
    /// in weeks 0, 1 and 2 from the week loop's call, in lockstep with real drawing.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0009
    [GameDataTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Game_week(int week)
    {
        var run = StationsLockstep.AssertSame(_ => { }, 0x0009, 0, ph => ph.Run(), $"week {week}", from: AtStations.Value[week]);
        output.WriteLine($"week {week}: frames {run.Frames.Count}, reads {run.Reads.Count}, random calls {run.RandomCalls}, " +
                         $"lost {string.Join("/", Enumerable.Range(0, 4).Select(q => (sbyte)run.Ds[S.Lost + q]))}");
        Assert.NotEmpty(run.Frames);
    }

    /// <summary>
    /// [DIFFERENTIAL] 1B03:0009, 1B03:0705, 1B03:08ED, 1B03:0BE7 – the whole stations phase with real drawing from random
    /// states (humans with scripted keys and computers, pirates or none, occupied stations, players without rockets) with the
    /// small font: every pixel read, the data segment, the random calls and both pages outside the panel at every frame, the
    /// frame count and the end state.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0009
    // [DIFFERENTIAL] 1B03:0705
    // [DIFFERENTIAL] 1B03:08ED
    // [DIFFERENTIAL] 1B03:0BE7
    [GameDataFact]
    public void Whole_phase_0009()
    {
        var r = new Inputs(0x0009);
        int frames = 0, reads = 0, ones = 0, hits = 0;
        for (int i = 0; i < 16; i++)
        {
            const int font = 0;
            bool humans = i % 3 != 2;
            var keys = StationsDifferentialTests.ChoiceKeys(200 + i);
            var run = StationsLockstep.AssertSame(p => SetState(p, r, humans), 0x0009, font, ph => ph.Run(), $"state {i}", keys);
            frames += run.Frames.Count;
            reads += run.Reads.Count;
            ones += run.Reads.Count(x => x.Value == 1);
            hits += Enumerable.Range(0, 4).Sum(q => (sbyte)run.Ds[S.Lost + q]);
        }
        output.WriteLine($"frames {frames}, reads {reads} (colour 1: {ones}), rockets lost {hits}");
        Assert.True(ones > 0 && ones < reads && hits > 0, "no read of colour 1, none of another colour or no rocket lost");
    }
}
