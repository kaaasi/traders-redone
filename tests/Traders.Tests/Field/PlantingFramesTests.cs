using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using P = Traders.Core.Field.Planting;

namespace Traders.Tests.Field;

/// <summary>
/// Frame structure of the planting phase without original data (the frames themselves are compared with the original in
/// <see cref="PlantingFrameDifferentialTests"/>): page flip, delay, fades, pause, and the step in the phase machine.
/// </summary>
public class PlantingFramesTests
{
    private static Dgroup State()
    {
        var d = new Dgroup(new byte[DgroupLayout.Size]);
        d.Set(P.CardMode, (short)1);
        return d;
    }

    [Theory]
    [InlineData(false, false, 2)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 6)]
    [InlineData(true, true, 5)]
    public void Flip_waits_two_retraces_one_with_keypad_plus_four_more_with_keypad_minus(bool plus, bool minus, int expected)
    {
        var d = State();
        if (plus) d.Set(DgroupLayout.KeyState + 0x4E, (byte)1);
        if (minus) d.Set(DgroupLayout.KeyState + 0x4A, (byte)1);
        Assert.Equal(expected, P.Flip(d).Count());
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(3, 2)]
    [InlineData(5, 0)]
    [InlineData(7, 0)]
    [InlineData(-32768, 0)] // 5 − IQ overflows to a negative word: cwd, high word −1, no wait
    [InlineData(-32763, 0)]     // 5 + 32763 = 8000h, negative as a word
    [InlineData(-32762, 32767)]
    public void Pass_delay_is_five_minus_the_level_as_a_signed_long(short iq, int expected)
    {
        var d = State();
        d.Set(DgroupLayout.ComputerIq, iq);
        short low = Int16Math.S(5 - iq);
        Assert.Equal(expected, P.Retraces(low, (short)(low < 0 ? -1 : 0)).Count());
    }

    [Fact]
    public void Fades_wait_five_and_four_retraces_per_stage_in_VGA_and_one_less_otherwise()
    {
        var d = State();
        Assert.Equal(40, P.Fade(d, 4).Count());
        Assert.Equal(32, P.Fade(d, 3).Count());
        d.Set(P.CardMode, (short)0);
        Assert.Equal(32, P.Fade(d, 4).Count());
    }

    [Fact]
    public void Pause_polls_until_P_is_released_pressed_and_released_again()
    {
        var d = State();
        Assert.Empty(P.Pause(d));
        d.Set(DgroupLayout.KeyState + 0x19, (byte)1);
        int polls = 0;
        foreach (var w in P.Pause(d))
        {
            Assert.Equal(FieldWait.Poll, w);
            polls++;
            // held 2 polls, released 3, held 1, released
            d.Set(DgroupLayout.KeyState + 0x19, (byte)(polls is 1 or 5 ? 1 : 0));
        }
        Assert.Equal(6, polls);
    }

    /// <summary>A host for a phase without players: no wait loop, nothing to ask.</summary>
    private sealed class EmptyHost : IPlantingHost
    {
        public List<FieldEvent> Events { get; } = [];
        public void Emit(FieldEvent e) => Events.Add(e);
        public void ComputerTurn(Dgroup d, short x, short y) => throw new InvalidOperationException();
        public bool CanPlaceHq(Dgroup d) => throw new InvalidOperationException();
        public bool HasOwnNeighbour(Dgroup d, short x, short y) => throw new InvalidOperationException();
        public void ComputerPlacedHq(Dgroup d) => throw new InvalidOperationException();
        public void ComputerBoughtField(Dgroup d) => throw new InvalidOperationException();
        public void ReadControls(Dgroup d) { }
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => P.Pause(d);
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => [FieldWait.Retrace, FieldWait.Retrace];
    }

    /// <summary>
    /// A phase whose players leave in the wait loop (an empty board, the test host ends them there): the step waits the
    /// set-up frames, the wait loop and the last flip, records the picture load and finishes; a repeated frame number changes
    /// nothing.
    /// </summary>
    [Fact]
    public void Step_runs_frame_by_frame_and_finishes_with_the_phase()
    {
        var d = State();
        d.Set(P.PictureNumber, (short)2);
        d.Set(FieldGrid.Columns, (short)0);
        d.Set(FieldGrid.Rows, (short)0);
        for (int p = 0; p < 4; p++) d.Set(DgroupLayout.HqX + p, (sbyte)-1);
        var host = new EmptyHost();
        var step = new PlantingStep(new EndingHost(host), _ => new ImageFileLengths(100, 200));
        var machine = new PhaseMachine(d, new OriginalRandom(new RecordedClock([])),
            new Dictionary<int, IWeekStepHandler> { [8] = step }, nextStep: 8);
        long frame = 0;
        machine.Advance(new PhaseTick(frame));
        while (machine.Status == PhaseStatus.Waiting)
        {
            machine.Advance(new PhaseTick(frame)); // the same frame again: nothing happens
            frame++;
            machine.Advance(new PhaseTick(frame));
            Assert.True(frame < 1000);
        }
        // fade-out 40, fade-in 32, status flip 2, panels flip 2, wait loop 2, last frame flip 2
        Assert.Equal(80, frame);
        Assert.Equal(80, step.Retraces);
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(9, machine.NextStep);
        Assert.Equal(2, d.S16(ImageTable.CurrentImage));
        Assert.Equal(200u, d.U32(ImageTable.Entry(2) + ImageTable.UnpackedLengthField));
        Assert.Equal([FieldEventKind.InitObjects, FieldEventKind.LoadImage], host.Events.Take(2).Select(e => e.Kind));
    }

    /// <summary>Ends every player in the wait loop (stands in for players that leave at once).</summary>
    private sealed class EndingHost(EmptyHost inner) : IPlantingHost
    {
        public void Emit(FieldEvent e) => inner.Emit(e);
        public void ComputerTurn(Dgroup d, short x, short y) => inner.ComputerTurn(d, x, y);
        public bool CanPlaceHq(Dgroup d) => inner.CanPlaceHq(d);
        public bool HasOwnNeighbour(Dgroup d, short x, short y) => inner.HasOwnNeighbour(d, x, y);
        public void ComputerPlacedHq(Dgroup d) => inner.ComputerPlacedHq(d);
        public void ComputerBoughtField(Dgroup d) => inner.ComputerBoughtField(d);
        public void ReadControls(Dgroup d) => inner.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => inner.PauseCheck(d);

        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
        {
            foreach (var w in inner.WaitLoop(d, random, mode)) yield return w;
            d.Set(P.Players, (byte)0);
        }
    }
}
