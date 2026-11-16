using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Startup;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Phases;

/// <summary>
/// Call 9 as bound by <see cref="WeekScreens"/> with its shared, writable text buffer (review R6E-1, issue #175): the default
/// game with four computer players runs in the RefEmu to the entry of 17CB:0005 in week 2, whose incident writes an amount with
/// 2295:0001 into the text buffer behind DS:4884; the core runs <see cref="WeekScreens.Incidents"/> in its phase machine on the
/// data segment of that moment and on a copy of the original's text buffer, handed in as <see cref="WeekScreenInputs.TextBuffer"/>.
/// After the return 1777:0240 the text buffer must equal the original's: the amount reaches the buffer the other steps read.
/// </summary>
public sealed class WeekScreensIncidentsDifferentialTests
{
    /// <summary>The week of the default game whose incidents call 2295:0001 (the only one of weeks 0..9).</summary>
    private const int AmountWeek = 2;

    /// <summary>[DIFFERENTIAL] 17CB:0005 – call 9 through WeekScreens in week 2 of the default game: 2295:0001 runs at least once
    /// inside the call in the original, the text buffer changes there, and the port's shared buffer equals the original's after it.</summary>
    [GameDataFact]
    public void Incidents_write_their_amount_into_the_shared_text_buffer()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var clock = new List<int>();
        p.Machine.AddCodeHook(p.Segment(0x21FD), 0x01CE, m => clock.Add(m.Cpu.Ax | (m.Cpu.Dx << 16)));
        int amounts = 0;
        p.Machine.AddCodeHook(p.Segment(0x2295), 0x0001, _ => amounts++);
        for (int week = 0; week <= AmountWeek; week++)
            Assert.True(p.RunToRoutine(0x17CB, 0x0005, 1800), $"incidents of week {week} not reached");
        Assert.Equal(AmountWeek, p.Week);

        var core = new Dgroup(p.DataSegment().AsSpan(0, L.Size).ToArray());
        ushort off = core.U16(MenuLayout.TextBuffer), seg = core.U16(MenuLayout.TextBuffer + 2);
        byte[] Original() => p.Machine.Mem.ReadBlock(seg, off, MenuTexts.ScanLimit + 1);
        var before = Original();
        var texts = (byte[])before.Clone();
        clock.Clear();
        amounts = 0;
        Assert.True(p.RunToRoutine(0x1777, 0x0240, 600), "return of the incidents not reached");
        var after = Original();
        Assert.True(amounts > 0, "2295:0001 did not run inside call 9 of week 2: the test no longer covers the text buffer");
        Assert.False(after.AsSpan().SequenceEqual(before), "the incidents of week 2 left the text buffer unchanged");

        var screens = new WeekScreens(
            new WeekScreenInputs(ProgramStartDifferentialTests.Files,
                _ => throw new InvalidOperationException("no picture in call 9"), [],
                () => throw new InvalidOperationException("no font in call 9"), () => texts),
            new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));
        var random = new OriginalRandom(new RecordedClock(clock), core.U32(L.RandomSeed));
        var machine = new PhaseMachine(core, random,
            new Dictionary<int, IWeekStepHandler> { [WeekSteps.IncidentsCall] = screens.Incidents }, nextStep: WeekSteps.IncidentsCall);
        KeyTable.Write(core, []);
        long frame = 0;
        machine.Advance(new PhaseTick(frame));
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(++frame < 20_000, "port incidents did not finish");
            machine.Advance(new PhaseTick(frame));
        }
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == WeekSteps.StationsCall,
            $"phase machine {machine.Status} at {machine.NextStep}");
        Assert.NotEmpty(screens.DrainIncidentEvents());

        int k = after.AsSpan().CommonPrefixLength(texts);
        Assert.True(k == after.Length, $"text buffer +{k:X4}: original {after[Math.Min(k, after.Length - 1)]:X2} port {texts[Math.Min(k, texts.Length - 1)]:X2}");
        Assert.Equal(p.DataSegment()[L.Cash..(L.Cash + 16)], core.Bytes[L.Cash..(L.Cash + 16)]);
    }
}
