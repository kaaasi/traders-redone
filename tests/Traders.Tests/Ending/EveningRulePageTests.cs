using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Ending;

/// <summary>
/// The pixel reads of the evening's reveal 20B3:069A (23CD:06DE at 20B3:06DC) against the original running unchanged (R6P-1):
/// a real game (four computer players) ends at the end check of week 0 or 1 (target DS:08F4 set to 0); the RefEmu runs
/// 20B3:0005 with all its drawing and records every read (x, y, AL) and DS:10D2 at 20B3:0071. The port runs
/// <see cref="EndCheck.Decide"/> and <see cref="EndScreens.Run"/> from the data segment at the entry with the rule page
/// <see cref="EndingPage"/> (draw page from DS:40DE/082E, the pictures of FONTS.PP and V-*.PP as explicit inputs) and reads
/// there. Compared: the whole sequence of reads with their values (2340 accepted cells and the rejected ones in between) and
/// the random state after the ranking. The reveal ends only once every cell is drawn: a wrong page model reads colour 2 for
/// ever, so the port's side has an abort counter (<see cref="ReadLimit"/>).
/// </summary>
public class EveningRulePageTests
{
    private const int ReadLimit = 100_000;

    // [DIFFERENTIAL] 20B3:069A – pixel reads of the reveal on the core's rule page (20B3:0525, 05B4 and the drawing they read)
    [GameDataTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void Reveal_reads_the_rule_page_like_the_original(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(week, 900), $"week {week} not reached");
        p.WriteDword((ushort)L.Target, 0);
        Assert.True(p.RunToRoutine(0x20B3, 0x0005, 900), "end check not reached");
        var state = p.DataSegment().AsSpan(0, L.Size).ToArray();

        ushort seg = p.Segment(0x20B3);
        var reads = new List<(int X, int Y, int V)>();
        uint seedAfterRanking = 0;
        p.Machine.AddCodeHook(seg, 0x06E3, m => reads.Add((m.Cpu.Di, (short)(m.Cpu.Si + 6), m.Cpu.Ax)));
        p.Machine.AddCodeHook(seg, 0x0071, _ => seedAfterRanking = p.RandomState);
        for (int k = 1; k <= 60; k++) p.Press(Scancodes.Enter, delay: 5 * k);
        Assert.True(p.RunToRoutine(0x20B3, 0x0088, 600), "20B3:0005 did not finish");

        var d = new Dgroup(state);
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var page = new EndingPage(new RulePage(RulePage.DrawPageOf(d)), Fonts.Value);
        var host = new Reader(d);
        Assert.True(EndCheck.Decide(d));
        foreach (var _ in EndScreens.Run(d, random, host, page, n => Pictures.Value[n]))
            if (host.Reads.Count > ReadLimit) break;

        Assert.True(host.Reads.Count <= ReadLimit, "the port's reveal needed the abort counter");
        Assert.Equal(reads.Count, host.Reads.Count);
        for (int i = 0; i < reads.Count; i++)
            Assert.True(reads[i] == host.Reads[i], $"read {i}: original {reads[i]}, port {host.Reads[i]}");
        Assert.Equal(seedAfterRanking, random.Seed);
    }

    private static Lazy<IndexedImage[]> Pictures => EndingInputs.Pictures;

    private static Lazy<RuleFont[]> Fonts => EndingInputs.Fonts;

    /// <summary>Reads the rule page; everything else is dropped (the report and the key wait do nothing).</summary>
    private sealed class Reader(Dgroup d) : IEndingHost
    {
        public List<(int X, int Y, int V)> Reads { get; } = [];

        public void ComputerTurn(Dgroup state, short x, short y) => throw new InvalidOperationException();

        public void Emit(FieldEvent e) { }

        public void Emit(EndingEvent e)
        {
            if (e.Kind == EndingEventKind.WaitKey) d.Set(EndScreens.MakeCount, (byte)1);
        }

        public byte ReadPixel(RulePage page, short x, short y)
        {
            byte v = page.Get(x, y);
            Reads.Add((x, y, v));
            return v;
        }

        public IEnumerable<FieldWait> Report(Dgroup state, OriginalRandom random) => [];
    }
}
