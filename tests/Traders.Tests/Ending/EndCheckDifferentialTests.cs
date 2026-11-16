using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;

namespace Traders.Tests.Ending;

/// <summary>
/// The end check 20B3:0005 up to its decision (finding docs/routines/20B3_0005_endpruefung.md) against the original: the
/// routine runs in the RefEmu (<see cref="OriginalProgram.LoadOnly"/>) with the players' cash set so that the ranking
/// 0825:06CB computes the wealth given by the case (the base wealth without cash is read from the original's 06CB first; DS:2F06
/// is then left stale, R7P-2), the target DS:08F4 and the key state of scancode 44h. The end screens from 20B3:0053 on are replaced by the routine's own
/// epilogue (pop bp, retf) and their entry is recorded. Compared: the decision (ends or AL = 0), the whole data segment –
/// DS:2F06, the ranks, DS:37AA, DS:45D0 – and DS:10D2 (no random draw). Expected values come only from the RefEmu.
/// </summary>
public class EndCheckDifferentialTests
{
    private const ushort Segment = 0x20B3, Entry = 0x0005, EndScreens = 0x0053;
    private const ushort RankingSegment = 0x0825, RankingEntry = 0x06CB;

    public static TheoryData<uint[], uint, bool> Cases() => new()
    {
        // below the target: high word smaller with low word FFFF (Ziel − 1 with carry), and an ordinary amount
        { new uint[] { 0x0000_FFFF, 0, 5, 0x7FFF }, 0x0001_0000, false },
        // the same with the key 44h held: the game ends with DS:37AA = 4
        { new uint[] { 0x0000_FFFF, 0, 5, 0x7FFF }, 0x0001_0000, true },
        // exactly the target with low word 0000 (player 1), the first at or above decides DS:37AA
        { new uint[] { 0x0000_FFFF, 0x0001_0000, 0x0002_0000, 0 }, 0x0001_0000, false },
        // one above the target (player 0)
        { new uint[] { 0x0001_0001, 0, 0, 0 }, 0x0001_0000, false },
        // target with low word FFFF: Ziel − 1 is below, Ziel and Ziel + 1 (carry into the high word) are not
        { new uint[] { 0x0001_FFFE, 0x0001_0000, 0x0000_FFFF, 0x0001_FFFE }, 0x0001_FFFF, false },
        { new uint[] { 0x0001_FFFE, 0x0001_0000, 0x0001_FFFF, 0x0002_0000 }, 0x0001_FFFF, false },
        { new uint[] { 0x0001_FFFE, 0x0001_0000, 0x0000_FFFF, 0x0002_0000 }, 0x0001_FFFF, false },
        // same high word, low word below and above: high word equal → low word decides
        { new uint[] { 0x0001_7FFF, 0x0001_8000, 0, 0 }, 0x0001_8000, false },
        // a negative wealth is a huge unsigned value: the game ends (player 2)
        { new uint[] { 9_999, 0, 0xFFFF_FFF0, 20_000 }, 10_000, false },
        { new uint[] { 9_999, 0x8000_0000, 0, 0 }, 10_000, false },
        // ordinary targets of a game, all below / one at it
        { new uint[] { 9_999, 0, 1, 9_998 }, 10_000, false },
        { new uint[] { 9_999, 0, 1, 10_000 }, 10_000, false },
        { new uint[] { 49_999, 50_000, 50_000, 1 }, 50_000, false },
        { new uint[] { 49_999, 49_999, 49_999, 49_999 }, 50_000, false },
        // a tie of all at the target: player 0
        { new uint[] { 30_000, 30_000, 30_000, 30_000 }, 30_000, true },
        // target 0: everybody is at it; target FFFFFFFF: only FFFFFFFF reaches it
        { new uint[] { 0, 0, 0, 0 }, 0, false },
        { new uint[] { 0xFFFF_FFFE, 0x7FFF_FFFF, 0x8000_0000, 0 }, 0xFFFF_FFFF, false },
        { new uint[] { 0xFFFF_FFFE, 0x7FFF_FFFF, 0x8000_0000, 0xFFFF_FFFF }, 0xFFFF_FFFF, false },
    };

    // [DIFFERENTIAL] 20B3:0005 – up to the decision (20B3:0005–0051) with 0825:06CB; end screens 20B3:0053 replaced by the epilogue
    [GameDataTheory]
    [MemberData(nameof(Cases))]
    public void Decision_matches_the_original(uint[] wealth, uint target, bool keyHeld)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        SetWealth(p, wealth);
        // DS:2F06 stale (R7P-2): the complement of the wealth, so a decision that reads it without the ranking 0825:06CB fails
        for (int i = 0; i < 4; i++) p.WriteDword((ushort)(DgroupLayout.CurrentWealth + 4 * i), ~wealth[i]);
        p.WriteDword((ushort)DgroupLayout.Target, target);
        p.WriteByte((ushort)(DgroupLayout.KeyState + EndCheck.EndKey), keyHeld ? (byte)1 : (byte)0);
        p.WriteDword((ushort)DgroupLayout.RandomSeed, 0x1234_5678);
        ushort seg = p.Segment(Segment);
        p.Machine.Mem.Write8(seg, EndScreens, 0x5D);                  // pop bp
        p.Machine.Mem.Write8(seg, (ushort)(EndScreens + 1), 0xCB);    // retf
        bool ended = false;
        p.Machine.AddCodeHook(seg, EndScreens, _ => ended = true);

        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var before = (byte[])core.Bytes.Clone();
        bool ends = EndCheck.Ends(core);
        Assert.Equal(before, core.Bytes); // Ends works on a copy
        var step = new EndCheckStep();
        Assert.Equal(ends, step.IsPending(core));
        var (ax, _) = p.Call(Segment, Entry);
        var emu = p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray();

        Assert.Equal(ended, ends);
        // the decision on the data segment: DS:2F06, ranks, DS:37AA (first player at or above the target, else 4)
        Assert.Equal(emu, Decided(before));
        Assert.Equal(0x1234_5678u, BitConverter.ToUInt32(emu, DgroupLayout.RandomSeed));
        if (ended) return;
        Assert.Equal(0, ax & 0xFF);
        // the step in the machine: returns AL = 0 at once, the machine moves on to call 20, no random draw
        var random = new OriginalRandom(new RecordedClock([]));
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [EndCheckStep.Call] = step },
            nextStep: EndCheckStep.Call);
        machine.Advance(new PhaseTick(0));
        Assert.Contains(machine.Events, e => e is { Kind: PhaseEventKind.StepFinished, Step: EndCheckStep.Call, Value: 0 });
        Assert.Equal(EndCheckStep.Call + 1, machine.NextStep);
        Assert.Equal(0, random.Calls);
        Assert.Equal(emu, core.Bytes);
        Assert.False(step.GameOver);
    }

    /// <summary>
    /// 0825:06CB is idempotent for an unchanged state (R7P-2): called twice in a row in the original, the data segment after
    /// the second call equals the one after the first – so the rest of the end check, which calls 20B3:0005 again in full from
    /// the state before the decision, sees the same ranking.
    /// </summary>
    // [DIFFERENTIAL] 0825:06CB – twice in a row on the same state
    [GameDataFact]
    public void The_ranking_is_idempotent_in_the_original()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        SetWealth(p, [0x0001_2345, 0xFFFF_0000, 77, 0x0001_2345]);
        p.Call(RankingSegment, RankingEntry);
        var first = p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray();
        p.Call(RankingSegment, RankingEntry);
        Assert.Equal(first, p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var core = new Dgroup((byte[])first.Clone());
        Traders.Core.Rules.Ranking.Apply(core);
        Assert.Equal(first, core.Bytes);
    }

    /// <summary>The port's decision on a copy of <paramref name="state"/>.</summary>
    private static byte[] Decided(byte[] state)
    {
        var d = new Dgroup((byte[])state.Clone());
        EndCheck.Decide(d);
        return d.Bytes;
    }

    /// <summary>
    /// Sets each player's cash DS:3CD2 so that 0825:0813 in the original gives <paramref name="wealth"/>: with cash 0 the
    /// original's ranking 0825:06CB writes the rest of the wealth to DS:2F06; the cash is the difference (32 bit).
    /// </summary>
    private static void SetWealth(OriginalProgram p, uint[] wealth)
    {
        for (int i = 0; i < 4; i++) p.WriteDword((ushort)(DgroupLayout.Cash + 4 * i), 0);
        p.Call(RankingSegment, RankingEntry);
        var rest = Enumerable.Range(0, 4).Select(i => p.ReadDword((ushort)(DgroupLayout.CurrentWealth + 4 * i))).ToArray();
        for (int i = 0; i < 4; i++) p.WriteDword((ushort)(DgroupLayout.Cash + 4 * i), unchecked(wealth[i] - rest[i]));
        p.Call(RankingSegment, RankingEntry);
        for (int i = 0; i < 4; i++) Assert.Equal(wealth[i], p.ReadDword((ushort)(DgroupLayout.CurrentWealth + 4 * i)));
    }
}
