using Traders.Core;

namespace Traders.Tests;

public class OriginalRandomTests
{
    [Fact]
    public void First_step_from_initial_seed_gives_pi_digits()
    {
        var rng = new OriginalRandom(new RecordedClock([]));
        rng.Next(2);
        Assert.Equal(3141592622u, rng.Seed);
    }

    [Fact]
    public void Sequence_matches_reference()
    {
        var rng = new OriginalRandom(new RecordedClock([]));
        (short n, short value, uint seed)[] expected =
        [
            (2, 1, 3141592622u), (3, 2, 1588972055u), (1, 0, 1279602700u), (5, 2, 1481914909u),
            (6, 2, 3913565466u), (20, 13, 2610266515u), (200, 124, 1903286488u),
            (unchecked((short)0xFFFE), unchecked((short)54628), 936717817u), (7, 2, 3104230086u), (2, 2, 4091513039u),
        ];
        foreach (var (n, value, seed) in expected)
        {
            Assert.Equal(value, rng.Next(n));
            Assert.Equal(seed, rng.Seed);
        }
    }

    [Fact]
    public void Zero_argument_reseeds_from_clock_and_returns_zero()
    {
        var rng = new OriginalRandom(new RecordedClock([123456789]));
        Assert.Equal(0, rng.Next(0));
        Assert.Equal(123456789u, rng.Seed);
    }

    [Fact]
    public void Missing_clock_input_is_an_error_not_a_silent_default()
    {
        var rng = new OriginalRandom(new RecordedClock([]));
        Assert.Throws<InvalidOperationException>(() => rng.Next(0));
    }
}
