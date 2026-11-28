using Traders.App;
using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Tests.Phases;
using Xunit.Abstractions;

namespace Traders.Tests.App;

/// <summary>
/// #276 (R9G-2, review R9 part 2): the host's return to the main menu after the end of a game (decision F3 = B of R9) followed by
/// "continue" (key A in the dialog 192D:12A6). The original has no such path (it ends the program), so there is no oracle: the
/// expectation comes from the core's own state just before the end screens' key wait, where the game has ended and nothing
/// but the end music has run since the end check. Continuing must not change the game: week counter DS:4B9A (the end check
/// counted the week of the end once more, 20B3:0071, so the week loop goes on with the next week), cash, stock, wealth of every
/// player and the random state DS:10D2 (neither the dialog nor the key draws) are those of the snapshot when the week loop
/// runs again, and they stay so up to its first report frame; the sound switch DS:10A5 is that of the snapshot too (the music flag DS:10A9 is the menu's: the end music has ended and the menu music runs again, so it is only noted).
/// Counter-checks (local, review): the host without <c>returnToMenu</c> (the program ends, no week loop), N instead of A (a
/// new game: week 0), the week counter not counted at the end → red.
/// </summary>
public class HostWholeGameContinueTests
{
    private readonly ITestOutputHelper _out;

    public HostWholeGameContinueTests(ITestOutputHelper output) => _out = output;

    private sealed record Snapshot(short Week, uint[] Cash, short[] Stock, uint[] Wealth, uint Random, byte SoundOn)
    {
        public static Snapshot Of(ProgramHost host)
        {
            var d = host.Core;
            return new Snapshot(d.S16(DgroupLayout.Week),
                [.. Enumerable.Range(0, 4).Select(p => d.U32(DgroupLayout.Cash + 4 * p))],
                [.. Enumerable.Range(0, 16).Select(i => d.S16(DgroupLayout.Stock + 2 * i))],
                [.. Enumerable.Range(0, 4).Select(p => d.U32(DgroupLayout.CurrentWealth + 4 * p))],
                d.U32(DgroupLayout.RandomSeed), d.U8(0x10A5));
        }

        /// <summary>The game state (not the random state) equal to <paramref name="other"/>.</summary>
        public void AssertGameEqual(Snapshot other, string where)
        {
            Assert.True(Week == other.Week, $"{where}: week {other.Week}, expected {Week}");
            Assert.True(Cash.SequenceEqual(other.Cash), $"{where}: cash {string.Join(",", other.Cash)}, expected {string.Join(",", Cash)}");
            Assert.True(Stock.SequenceEqual(other.Stock), $"{where}: stock {string.Join(",", other.Stock)}, expected {string.Join(",", Stock)}");
            Assert.True(Wealth.SequenceEqual(other.Wealth), $"{where}: wealth {string.Join(",", other.Wealth)}, expected {string.Join(",", Wealth)}");
            Assert.True(SoundOn == other.SoundOn, $"{where}: sound flag DS:10A5 {other.SoundOn:X2}, expected {SoundOn:X2}");
        }
    }

    /// <summary>Plays to the end of the game, past its end screens and back to the main menu; the snapshot is that of the key wait.</summary>
    private static (ProgramHost Host, Snapshot Before, long Frame) PlayToMenuAfterEnd()
    {
        var host = ProgramHostTests.PlayToHalt(skipRobotPhase: true);
        Assert.True(host.GameOver);
        Assert.NotNull(host.MenuReturn);
        var ending = Assert.IsType<EndingStep>(host.Screens.Ending!.Current);
        long f = host.LastFrame + 1;
        for (long until = f + 60_000; f < until && ending.Polls == 0; f++) host.Frame(f, []);
        Assert.True(ending.Polls > 0, "the end screens did not reach the key wait");
        var before = Snapshot.Of(host);
        for (int i = 0; i < 6; i++) host.Frame(f++, [MenuKeys.Enter]);
        for (long until = f + 2000; f < until && (host.Menu.Calls < 2 || ProgramHostTests.Passes(host) < 2); f++) host.Frame(f, []);
        Assert.Equal(1, host.MenuReturn.Returns);
        Assert.Equal(ProgramLoop.MainMenu, host.Machine.NextStep);
        return (host, before, f);
    }

    /// <summary>
    /// End → main menu → RETURN (dialog "new game or continue") → A (continue): the week loop goes on with the game as the end left it.
    /// </summary>
    [GameDataFact]
    public void Continue_after_the_end_goes_on_with_the_next_week_and_the_state_before_the_end()
    {
        var (host, before, f) = PlayToMenuAfterEnd();
        Assert.True(before.Week > 0);
        // the menu's own draws (plate, rivets) have used the random state since the end: it is not compared before the dialog
        for (int i = 0; i < 6; i++) host.Frame(f++, [MenuKeys.Enter]);
        for (int i = 0; i < 30; i++) host.Frame(f++, []); // the dialog is built and waits for its key
        Assert.Equal(ProgramLoop.MainMenu, host.Machine.NextStep);
        uint randomAtDialog = host.Core.U32(DgroupLayout.RandomSeed);
        var cashAtDialog = Snapshot.Of(host);
        before.AssertGameEqual(cashAtDialog, "in the dialog");

        long first = -1;
        for (long until = f + 600; f < until && host.Machine.Status != ProgramStatus.InWeek; f++)
        {
            host.Frame(f, f % 20 < 6 ? [MenuKeys.A] : []);
            first = f;
        }
        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        Assert.NotNull(host.Machine.WeekMachine);
        var atWeek = Snapshot.Of(host);
        _out.WriteLine($"continue: first week frame {first}; week {atWeek.Week}; random {randomAtDialog:X8} -> {atWeek.Random:X8}; " +
            $"sound flags DS:10A5 {host.Core.U8(0x10A5):X2}, DS:10A9 {host.Core.U8(0x10A9):X2}");
        before.AssertGameEqual(atWeek, "the week loop entered again");
        Assert.Equal(before.Week, host.Machine.WeekMachine!.Week);
        Assert.True(randomAtDialog == atWeek.Random, $"the key A drew random numbers: {randomAtDialog:X8} → {atWeek.Random:X8}");
        Assert.Equal(1, host.Core.U8(ProgramLoop.GameRunning));
        Assert.False(host.Machine.IsBlocked);

        // up to the first report frame of the week the game state stays that of the end
        for (long until = f + 200; f < until; f++)
        {
            host.Frame(f, []);
            Assert.True(host.Halt is null, $"the host halted: {host.Halt}");
            if (host.Machine.WeekMachine is { NextStep: > WeekSteps.ReportCall } or null) break;
        }
        before.AssertGameEqual(Snapshot.Of(host), "up to the week report");
        Assert.Equal(before.Week, host.Core.S16(DgroupLayout.Week));
    }

    /// <summary>
    /// The same end, but N in the dialog: a new game (week 0) – the counter-check that the continue test tells the keys apart.
    /// </summary>
    [GameDataFact]
    public void New_game_after_the_end_starts_at_week_0_not_with_the_state_before_the_end()
    {
        var (host, before, f) = PlayToMenuAfterEnd();
        Assert.True(before.Week > 0);
        for (int i = 0; i < 6; i++) host.Frame(f++, [MenuKeys.Enter]);
        for (long until = f + 600; f < until && host.Machine.Status != ProgramStatus.InWeek; f++)
            host.Frame(f, f % 20 < 6 ? [MenuKeys.N] : []);
        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        Assert.Equal(0, host.Core.U16(DgroupLayout.Week));
        Assert.NotEqual(before.Week, host.Core.S16(DgroupLayout.Week));
    }
}
