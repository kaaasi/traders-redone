using Traders.Audio.Effects;
using Xunit.Abstractions;

namespace Traders.Tests.Audio;

/// <summary>
/// Differential tests of the effect machine 0000:06FE/0724/0750/0732 (finding docs/routines/0000_0750_effekte.md): the
/// original on the RefEmu and <see cref="EffectMachine"/> on the image from T.EXE get the same calls; per call the OPL2 writes
/// (tick = number of 0732 calls) and the variables CS:03EC–06FD, at the end of each case the whole segment, must be equal.
/// </summary>
public class EffectsDifferentialTests(ITestOutputHelper output)
{
    /// <summary>Effects of the start table CS:15B6 (87 entries, 00h…56h).</summary>
    public const int EffectCount = 0x57;

    /// <summary>Tick limit of one case: the longest effect ends well before (the test reports the measured lengths).</summary>
    private const int MaxTicks = 4000;

    private static int RunUntilIdle(EffectsHarness h, string label, int maxTicks)
    {
        for (int t = 1; t <= maxTicks; t++)
        {
            h.Tick($"{label} tick {t}");
            if (h.Idle()) return t;
        }
        return -1;
    }

    // [DIFFERENTIAL] 0000:06FE, 0000:0750, 0000:0732, 0000:0770, 0000:0866, 0000:121C, 0000:150E
    [GameDataFact]
    public void Every_effect_alone_matches_the_original()
    {
        var h = new EffectsHarness();
        var lengths = new List<string>();
        for (int e = 0; e < EffectCount; e++)
        {
            h.Restart();
            h.Reset($"effect {e:X2} reset");
            h.Enqueue($"effect {e:X2} enqueue", (ushort)e);
            int ticks = RunUntilIdle(h, $"effect {e:X2}", MaxTicks);
            h.AssertSameSegment($"effect {e:X2} end");
            lengths.Add($"{e:X2}:{ticks}");
        }
        output.WriteLine(string.Join(" ", lengths));
    }

    // [DIFFERENTIAL] 0000:0750, 0000:0770, 0000:0724, 0000:0953
    [GameDataFact]
    public void Fast_requests_overflow_and_stops_match_the_original()
    {
        var h = new EffectsHarness();
        var r = new Random(0x0750);
        for (int c = 0; c < 12; c++)
        {
            h.Restart();
            h.Reset($"case {c} reset");
            for (int step = 0; step < 60; step++)
            {
                string label = $"case {c} step {step}";
                // 0…20 requests in one go: more than 16 unprocessed requests wrap the ring (#76 R3H-2)
                int requests = r.Next(4) == 0 ? r.Next(15, 21) : r.Next(0, 4);
                for (int i = 0; i < requests; i++)
                {
                    ushort effect = (ushort)(r.Next(EffectCount) | (r.Next(4) == 0 ? r.Next(256) << 8 : 0));
                    h.Enqueue($"{label} request {i} ({effect:X4})", effect);
                }
                if (r.Next(25) == 0) h.Stop($"{label} stop");
                int ticks = r.Next(4) == 0 ? r.Next(20, 120) : r.Next(0, 6);
                for (int t = 0; t < ticks; t++) h.Tick($"{label} tick {t}");
            }
            h.AssertSameSegment($"case {c} end");
        }
    }
}
