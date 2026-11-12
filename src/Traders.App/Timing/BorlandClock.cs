using Traders.Core;

namespace Traders.App.Timing;

/// <summary>
/// The clock value Borland C time() returned in the original: DOS keeps local time, and time() converts it to seconds since
/// 1970-01-01 00:00 UTC with the default time zone of the C runtime, EST5EDT (no TZ variable): plus 5 hours, minus one hour
/// while daylight saving time is in effect. The host's local time stands for the DOS clock.
/// </summary>
// [APPROX] Daylight saving as the Borland runtime decides it (US rule of its time: from the first Sunday in April, for years
// before 1987 the last Sunday in April, 2:00, to the last Sunday in October, 1:00 standard time), not by the host's rules.
public sealed class BorlandClock(Func<DateTime> localNow) : IClockInput
{
    /// <summary>Clock of the host machine (local time).</summary>
    public static BorlandClock Host { get; } = new(() => DateTime.Now);

    public int TimeSeconds() => FromLocal(localNow());

    /// <summary>time() for the DOS local time <paramref name="local"/> (seconds truncated).</summary>
    public static int FromLocal(DateTime local)
    {
        var t = new DateTime(local.Year, local.Month, local.Day, local.Hour, local.Minute, local.Second, DateTimeKind.Unspecified);
        long seconds = (long)(t - new DateTime(1970, 1, 1)).TotalSeconds + 5 * 3600;
        if (IsDaylightSaving(t)) seconds -= 3600;
        return unchecked((int)seconds);
    }

    /// <summary>US daylight saving of the Borland runtime for a local (standard) time.</summary>
    public static bool IsDaylightSaving(DateTime local)
    {
        var start = local.Year >= 1987 ? FirstSunday(local.Year, 4) : LastSunday(local.Year, 4);
        var end = LastSunday(local.Year, 10);
        return local >= start.AddHours(2) && local < end.AddHours(1);
    }

    private static DateTime FirstSunday(int year, int month)
    {
        var d = new DateTime(year, month, 1);
        return d.AddDays(((int)DayOfWeek.Sunday - (int)d.DayOfWeek + 7) % 7);
    }

    private static DateTime LastSunday(int year, int month)
    {
        var d = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        return d.AddDays(-(int)d.DayOfWeek);
    }
}
