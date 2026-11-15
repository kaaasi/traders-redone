using Traders.Core;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.RefEmu;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Events;

/// <summary>
/// Multi-round probe (AGENTS.md §5) of the week incidents: the default game with four computer players runs in the RefEmu with
/// all its layers; in weeks 0..3 the core takes the data segment at the entry of 17CB:0005 (call 9), runs its own incidents
/// with the wait loops to the end and is compared with the RefEmu at the return site 1777:0240. Compared are the values the
/// incidents own: cash, fields with their counters, ranks, wealth, shares, loop variables, actions and random() DS:10D2.
/// Graphics, palette and font state is written by the layers in the RefEmu and by the scene in the port, so it is not compared
/// here (see <see cref="IncidentsDifferentialTests"/> with stubbed layers).
/// </summary>
public sealed class IncidentsMultiWeekTests
{
    private const int Weeks = 4;

    private static readonly (int Offset, int Length, string Name)[] Owned =
    [
        (L.Cash, 16, "Cash"),
        (FieldGrid.Owner, 54, "FieldOwner"),
        (FieldGrid.Kind, 54, "FieldKind"),
        (FieldGrid.FreeFields, 1, "FreeFields"),
        (L.FieldCount, 4, "FieldCount"),
        (L.FieldsByKind, 16, "FieldsByKind"),
        (L.Rank, 4, "Rank"),
        (L.RankUnique, 4, "RankUnique"),
        (L.CurrentWealth, 16, "CurrentWealth"),
        (L.ProductionShares, 12, "ProductionShares"),
        (L.ActivePlayer, 1, "37AA"),
        (L.LoopX, 1, "LoopX"),
        (L.LoopY, 1, "LoopY"),
        (L.LoopWare, 2, "LoopWare"),
        (ControlReader.Action, 4, "Action"),
        (L.RandomSeed, 4, "RandomSeed"),
    ];

    /// <summary>[DIFFERENTIAL] 17CB:0005 in weeks 0..3 of the default game: the owned state equals the RefEmu's after every call, and at
    /// least one incident window opens (week 2 of the default game).</summary>
    [GameDataFact]
    public void Four_weeks_of_incidents_match_the_original()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var clock = new List<int>();
        p.Machine.AddCodeHook(p.Segment(0x21FD), 0x01CE, m => clock.Add(m.Cpu.Ax | (m.Cpu.Dx << 16)));
        var windows = new List<int>();
        for (int week = 0; week < Weeks; week++)
        {
            Assert.True(p.RunToRoutine(0x17CB, 0x0005, 1800), $"incidents of week {week} not reached");
            Assert.Equal(week, p.Week);
            var core = new Dgroup(p.DataSegment().AsSpan(0, L.Size).ToArray());
            clock.Clear();
            Assert.True(p.RunToRoutine(0x1777, 0x0240, 600), $"return of the incidents of week {week} not reached");
            var original = p.DataSegment();

            var random = new OriginalRandom(new RecordedClock(clock), core.U32(L.RandomSeed));
            var incidents = new WeekIncidents(core, random, KeyboardOnlyDevices.Instance);
            long frames = 0;
            KeyTable.Write(core, []);
            foreach (var _ in incidents.Run())
            {
                core.Set(L.RandomSeed, random.Seed);
                Assert.True(++frames < 20_000, $"port incidents of week {week} did not finish");
                incidents.Frame = frames;
            }
            core.Set(L.RandomSeed, random.Seed);
            Assert.False(incidents.Aborted);
            windows.Add(incidents.Events.Count(e => e.Kind == IncidentEventKind.FadeOut));
            foreach (var (offset, length, name) in Owned)
                for (int i = 0; i < length; i++)
                    Assert.True(original[offset + i] == core.U8(offset + i),
                        $"week {week} {name} DS:{offset + i:X4} original {original[offset + i]:X2} port {core.U8(offset + i):X2}");
        }
        Assert.True(windows.Sum() > 0, "no incident in four weeks: " + string.Join(", ", windows));
    }
}
