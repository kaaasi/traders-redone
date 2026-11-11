using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Weather of the week (docs/routines/21FD_021F_wetter.md): random(2) gives 0..2, a further random(4) == 0 adds 3
/// (byte addition), so 0..5 with 3..5 in a quarter of the weeks.
/// </summary>
public static class WeatherRule
{
    // [ORIGINAL] 21FD:021F
    public static void Apply(Dgroup d, OriginalRandom random)
    {
        d.Set(L.Weather, unchecked((byte)random.Next(2)));
        if (random.Next(4) == 0) d.Set(L.Weather, unchecked((byte)(d.U8(L.Weather) + 3)));
    }
}
