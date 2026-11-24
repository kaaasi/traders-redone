using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.State;

namespace Traders.Tests.Phases;

/// <summary>
/// Binding of the end of the game (lane partie part 1): with the driver's fonts as input <see cref="WeekScreens.Week"/> gives
/// the end check (call 19) its rest, the end screens (<see cref="EndingStep"/>, made anew at every call), so the end check is
/// no longer pending where the game ends; without the fonts, or with <c>ending: false</c>, it stays pending as up to round 8.
/// The make counter DS:115D of the any-key wait 27C6:00D2 is an input of the tick (<see cref="PhaseTick.Makes"/>).
/// </summary>
public class WeekScreensEndingTests
{
    private static WeekScreens Screens(bool fonts) => new(
        new WeekScreenInputs(_ => default, _ => new IndexedImage(1, 1, new byte[1]), [], () => default!, () => null,
            Fonts: fonts ? () => [] : null),
        new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));

    /// <summary>A data segment in which the end check ends the game: player 0 at the target 0.</summary>
    private static Dgroup Ending()
    {
        var d = new Dgroup();
        Assert.True(EndCheck.Ends(d));
        return d;
    }

    [Fact]
    public void With_the_fonts_the_end_check_gets_the_end_screens_as_its_rest()
    {
        var screens = Screens(fonts: true);
        Assert.NotNull(screens.Ending);
        var week = screens.Week(_ => { }, _ => default);
        Assert.Same(week.End, week.Handlers[WeekSteps.EndCall]);
        Assert.False(week.End.IsPending(Ending()));

        Assert.True(screens.Week(_ => { }, _ => default, ending: false).End.IsPending(Ending()));
        var without = Screens(fonts: false);
        Assert.Null(without.Ending);
        Assert.True(without.Week(_ => { }, _ => default).End.IsPending(Ending()));
    }

    /// <summary>
    /// Regression (lane partie, continuation 3, PR #259): the font state DS:3CA8/34A4 belongs to the output layers (21FD:05DB
    /// runs in the scenes); without it in the core's data segment the evening's rule page found no font, drew no text and the
    /// reveal copied the picture's cells instead of the blocks at the text (host end screens from retrace 1166 of call 19). At
    /// the start of call 19 the core takes it from the output together with the page state.
    /// </summary>
    [Fact]
    public void The_end_screens_take_the_font_and_the_pages_of_the_output_at_the_call()
    {
        var output = new byte[DgroupLayout.Size];
        output[EndingPage.Advance] = 6;
        output[EndingPage.Height] = 5;
        output[RulePage.DrawPageIndex] = 1;
        output[RulePage.PageNumbers] = 1; // page numbers 1 and 2 as the program start sets them up
        output[RulePage.PageNumbers + 4] = 2;
        var screens = new WeekScreens(
            new WeekScreenInputs(_ => default, _ => new IndexedImage(320, 200, new byte[320 * 200]), [], () => default!, () => null,
                OutputPages: () => output, Fonts: () => []),
            new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));
        var state = Ending();
        var m = new PhaseMachine(state, new OriginalRandom(new RecordedClock([]), 1),
            new Dictionary<int, IWeekStepHandler> { [EndCheckStep.Call] = screens.Ending! }, nextStep: EndCheckStep.Call);
        m.Advance(new PhaseTick(0));
        Assert.IsType<EndingStep>(screens.Ending!.Current);
        Assert.Equal(6, state.U16(EndingPage.Advance));
        Assert.Equal(5, state.U16(EndingPage.Height));
        Assert.Equal(1, state.U16(RulePage.DrawPageIndex));
        Assert.Equal(1, ((EndingStep)screens.Ending.Current).Page!.Page.DrawPage);
    }

    [Fact]
    public void Makes_of_a_tick_are_added_to_the_make_counter_once()
    {
        var state = new Dgroup();
        state.Set(PhaseTick.MakeCounter, (byte)0xFF);
        var m = new PhaseMachine(state, new OriginalRandom(new RecordedClock([]), 1), new Dictionary<int, IWeekStepHandler>());
        m.Advance(new PhaseTick(3, [0x1C], Makes: 2)); // four frames, the makes counted once; 8 bit
        Assert.Equal(1, state.U8(PhaseTick.MakeCounter));
        m.Advance(new PhaseTick(3, [0x1C], Makes: 1)); // no frame left to process: nothing counted
        Assert.Equal(1, state.U8(PhaseTick.MakeCounter));

        var program = new ProgramMachine(state, new OriginalRandom(new RecordedClock([]), 1), new Dictionary<int, IProgramStepHandler>(),
            new Dictionary<int, IWeekStepHandler>());
        program.Advance(new PhaseTick(0, Makes: 1));
        Assert.Equal(2, state.U8(PhaseTick.MakeCounter));
    }
}
