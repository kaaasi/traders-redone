using Traders.Core.Events;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Menus;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Events;

/// <summary>
/// The bounds of the week incidents 17CB (#162, R6E-1 of review r6-ereignisse) against the original: the amount classes of
/// 17CB:047E on both sides of 200, 500, 1000 and 2000 (and of the high word), a loss of exactly 3EDh (not capped; the cap
/// starts above it) and a field takeover from a leader with exactly 7 fields (the least that is taken from). The scenarios'
/// seeds are found with the port and then checked against the original (data segment, calls, random(), frames).
/// </summary>
public sealed class IncidentsBoundaryTests
{
    // [DIFFERENTIAL] 17CB:047E
    [GameDataFact]
    public void Amount_class_matches_the_original_at_its_bounds()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        var amounts = new List<int> { -1, int.MinValue, -65536, 65535, 65536, 65736, int.MaxValue };
        foreach (int b in new[] { 0, 200, 500, 1000, 2000 })
            amounts.AddRange([b - 1, b, b + 1]);
        var classes = new HashSet<int>();
        foreach (int amount in amounts)
        {
            var (ax, _) = p.Call(0x17CB, 0x047E, unchecked((ushort)amount), unchecked((ushort)(amount >> 16)));
            Assert.True((ax & 0xFF) == WeekIncidents.AmountClass(amount),
                $"17CB:047E({amount}): original {ax & 0xFF}, port {WeekIncidents.AmountClass(amount)}");
            classes.Add(ax & 0xFF);
        }
        Assert.Equal([0, 1, 2, 3, 4], classes.Order());
    }

    private static readonly bool[] AllComputers = [true, true, true, true];

    /// <summary>
    /// Loss branch of 17CB:001E for the player of wealth rank 1: gap 3016 to rank 2 gives a third of exactly 3EDh, which is
    /// kept (the cap 3EDh − random(100) applies only above); twice the wealth behind is below his, so no random() decides.
    /// </summary>
    private static IncidentsDifferentialTests.Scenario LossAtTheCap(uint seed) =>
        new([100_000, 4_016, 1_000, 500], [4, 5, 6, 8], AllComputers, seed, new KeyScript());

    /// <summary>Field takeover 17CB:04D7: the three leaders have exactly 7 fields, the last player 3.</summary>
    private static IncidentsDifferentialTests.Scenario TakeoverFromSeven(uint seed) =>
        new([100_000, 100_000, 100_000, 1_000], [7, 7, 7, 3], AllComputers, seed, new KeyScript(), Week: 1);

    public static TheoryData<string> Bounds => new(["loss of exactly 3ED", "takeover from 7 fields"]);

    // [DIFFERENTIAL] 17CB:0005 – bounds of the money incident (3EDh) and of the takeover (7 fields)
    [GameDataTheory]
    [MemberData(nameof(Bounds))]
    public void Incident_at_its_bound_matches_the_original(string name)
    {
        var h = IncidentsDifferentialTests.Harness();
        Func<uint, IncidentsDifferentialTests.Scenario> make = name == "loss of exactly 3ED" ? LossAtTheCap : TakeoverFromSeven;
        Func<IncidentsHarness.PortOutcome, bool> reached = name == "loss of exactly 3ED"
            ? o => o.Calls.Any(c => c.Seg == 0x2295 && c.A == 0x3ED)
            : o => o.State.U8(L.FieldCount + 3) == 4;
        for (uint seed = 1; seed < 3000; seed++)
        {
            var s = make(seed);
            var input = IncidentsDifferentialTests.Input(h, s);
            IncidentsHarness.PortOutcome port;
            try { port = IncidentsHarness.RunPort(input, s.Keys.At, []); }
            catch (InvalidOperationException) { continue; } // random(0) would need a clock value
            if (!reached(port)) continue;
            var o = IncidentsDifferentialTests.RunOriginal(s);
            IncidentsHarness.AssertSameDs(o.DataSegment, port.State, $"{name}, seed {seed}");
            Assert.Equal(o.Calls, port.Calls);
            Assert.Equal(o.Random, port.Incidents.RandomCalls);
            Assert.Equal(o.Frames, port.Frames);
            return;
        }
        Assert.Fail($"{name}: no seed below 3000 reaches the bound");
    }
}
