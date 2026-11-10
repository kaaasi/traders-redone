namespace Traders.Core;

/// <summary>
/// Port of the game's own random generator at 21FD:01BA (T.EXE b73a2170...).
/// <code>
/// int random(int n) {            // Turbo C, far
///   if (n == 0) { seed = time(NULL); return 0; }   // re-seed from the clock
///   seed = seed * 0xBB40E62DUL + 1;
///   return (unsigned long)(seed >> 8) % (unsigned long)(unsigned)(n + 1);
/// }
/// </code>
/// Seed lives at DS:10D2 (32 bit) and starts at 1. The result range is 0..n inclusive.
/// The clock value for re-seeding is an explicit input so games can be replayed.
/// </summary>
// [ORIGINAL] 21FD:01BA – random(n), Befund docs/routines/21FD_01BA_random.md
public sealed class OriginalRandom
{
    public const uint Multiplier = 0xBB40E62Du;
    public const uint InitialSeed = 1;

    private readonly IClockInput _clock;

    public OriginalRandom(IClockInput clock, uint seed = InitialSeed)
    {
        _clock = clock;
        Seed = seed;
    }

    /// <summary>Generator state as stored at DS:10D2.</summary>
    public uint Seed { get; set; }

    /// <summary>Number of calls so far (diagnostics for differential tests).</summary>
    public long Calls { get; private set; }

    /// <summary>Exactly reproduces <c>random(n)</c>; <paramref name="n"/> is the original 16-bit int argument.</summary>
    public short Next(short n)
    {
        Calls++;
        if (n == 0)
        {
            Seed = unchecked((uint)_clock.TimeSeconds());
            return 0;
        }
        Seed = unchecked(Seed * Multiplier + 1u);
        uint divisor = unchecked((ushort)(n + 1)); // n+1 computed in 16 bit, zero-extended
        uint value = (Seed >> 8) % divisor;         // LUMOD; divisor 0 would be a DOS divide error
        return unchecked((short)value);
    }
}

/// <summary>
/// Source of the value Turbo C's <c>time()</c> returned. Live games read the real clock in the
/// host; replays and tests feed recorded values. The core never reads the system clock itself.
/// </summary>
public interface IClockInput
{
    int TimeSeconds();
}

/// <summary>Clock input that replays recorded values (and fails loudly if they run out).</summary>
public sealed class RecordedClock(IEnumerable<int> values) : IClockInput
{
    private readonly Queue<int> _values = new(values);

    public int TimeSeconds() =>
        _values.Count > 0 ? _values.Dequeue() : throw new InvalidOperationException("No recorded clock value left.");
}
