using Traders.Core;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.RefEmu;

namespace Traders.Tests.Week;

/// <summary>
/// Multi-round probe (AGENTS.md §5) of the week report: the default game with four computer players runs in the RefEmu with
/// all its layers; in weeks 0..3 the core takes the data segment at the entry of 0825:000A (call 3, arg 1), runs its own
/// report with the wait loop to the end and is compared with the RefEmu at the return site 1777:020D. Compared are the values
/// the report owns: ranks DS:3D42/302A, wealth DS:2F06, scale DS:1584, loop variables DS:37AA/45D0, actions DS:3308, the
/// indicator DS:54B0 and random() DS:10D2. Graphics, palette and page state is written by the layers in the RefEmu and by the
/// scene in the port, so it is not compared here (see the differential tests with stubbed layers).
/// </summary>
public sealed class ReportMultiWeekTests
{
    private const int Weeks = 4;

    private static readonly (int Offset, int Length, string Name)[] Owned =
    [
        (DgroupLayout.Rank, 4, "Rank"),
        (DgroupLayout.RankUnique, 4, "RankUnique"),
        (DgroupLayout.CurrentWealth, 16, "CurrentWealth"),
        (ReportLayout.GraphScale, 2, "GraphScale"),
        (DgroupLayout.ActivePlayer, 1, "37AA"),
        (DgroupLayout.LoopWare, 2, "LoopWare"),
        (ControlReader.Action, 4, "Action"),
        (WaitLoopLayout.IndicatorCountdown, 1, "IndicatorCountdown"),
        (DgroupLayout.RandomSeed, 4, "RandomSeed"),
    ];

    /// <summary>
    /// [DIFFERENTIAL] 0825:000A in weeks 0..3 of the default game: the state the report owns equals the RefEmu's at the return
    /// of every week's report, and the scale changes with the wealth (it is not the same value in all weeks by accident only).
    /// </summary>
    [GameDataFact]
    public void Four_weekly_reports_match_the_original()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var scales = new List<short>();
        for (int week = 0; week < Weeks; week++)
        {
            Assert.True(p.RunToRoutine(0x0825, 0x000A, 1800), $"report of week {week} not reached");
            Assert.Equal(week, p.Week);
            Assert.Equal(1, p.StackArgument(0));
            var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
            var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
            var report = new Report(core, random, KeyboardOnlyDevices.Instance, 1);
            long frames = 0;
            KeyTable.Write(core, []);
            foreach (var _ in report.Run())
            {
                core.Set(DgroupLayout.RandomSeed, random.Seed);
                Assert.True(++frames < 10_000, $"port report of week {week} did not finish");
                report.Frame = frames;
            }
            core.Set(DgroupLayout.RandomSeed, random.Seed);
            Assert.False(report.Aborted);
            Assert.Equal([3, 2, 1, 0], report.WaitLoop.Confirmations);

            Assert.True(p.RunToRoutine(0x1777, 0x020D, 120), $"return of the report of week {week} not reached");
            var original = p.DataSegment();
            foreach (var (offset, length, name) in Owned)
                for (int i = 0; i < length; i++)
                    Assert.True(original[offset + i] == core.U8(offset + i),
                        $"week {week} {name} DS:{offset + i:X4} original {original[offset + i]:X2} port {core.U8(offset + i):X2}");
            scales.Add(core.S16(ReportLayout.GraphScale));
        }
        Assert.True(scales.Distinct().Count() > 1, "scale " + string.Join(", ", scales));
    }
}
