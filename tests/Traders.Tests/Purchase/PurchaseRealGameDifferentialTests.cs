using Traders.Core;
using Traders.Core.State;
using Traders.RefEmu;
using L = Traders.Core.State.DgroupLayout;
using R = Traders.Core.Purchase.PurchaseRules;

namespace Traders.Tests.Purchase;

/// <summary>
/// The rules of the purchase (finding docs/routines/08CF_0004_einkauf.md) on states of real purchases: the default game (four
/// computers) runs in the RefEmu through the purchases of weeks 0–3; at the entries of the rule routines the data segment and
/// the arguments are captured (a sample per routine and week). Each state is then called again in the original
/// (<see cref="PurchaseHarness"/>) and in the port – as captured, with players 0 and 2 made human, and with oil consumption and
/// gem trading switched off (the default game has both on). Expected values come only from the original.
/// </summary>
public class PurchaseRealGameDifferentialTests
{
    private sealed record Captured(int Week, ushort Off, byte[] DataSegment, ushort[] Args)
    {
        public override string ToString() => $"week {Week} 08CF:{Off:X4}";
    }

    /// <summary>Rule routines of 08CF and their argument count.</summary>
    private static readonly (ushort Off, int Args)[] Routines =
    [
        (0x0082, 0), (0x029A, 0), (0x039C, 0), (0x03E5, 0), (0x0455, 1), (0x0492, 0), (0x04BA, 0), (0x05F1, 0), (0x066D, 0),
        (0x069F, 0), (0x0813, 0), (0x084F, 0), (0x0886, 1), (0x0914, 0), (0x09C4, 0), (0x09F1, 0), (0x0AF0, 0), (0x0BE1, 0),
        (0x0CD5, 0), (0x0DEF, 0), (0x0E53, 1), (0x0F4D, 0), (0x115C, 0), (0x1348, 0), (0x161F, 0), (0x178D, 1),
    ];

    /// <summary>
    /// Reached by the four computers of the default game in weeks 0–3 (measured 17.11.2026; not reached: 08CF:084F, 08CF:09C4,
    /// 08CF:09F1 – no computer enters the rocket shop or buys a gem from its account in these weeks).
    /// </summary>
    private static readonly ushort[] Reached =
    [
        0x0082, 0x029A, 0x039C, 0x03E5, 0x0455, 0x0492, 0x04BA, 0x05F1, 0x066D, 0x069F, 0x0813, 0x0886, 0x0914, 0x0AF0, 0x0BE1,
        0x0CD5, 0x0DEF, 0x0E53, 0x0F4D, 0x115C, 0x1348, 0x161F, 0x178D,
    ];

    /// <summary>At most this many states per routine and week, every <see cref="Stride"/>-th call.</summary>
    private const int PerWeek = 10, Stride = 11;

    private static readonly Lazy<List<Captured>> States = new(Capture);

    private static readonly List<byte[]> EntryStates = [];

    /// <summary>The data segment at the entry of 08CF:0004 in weeks 0–3 of the same game (for the flow tests of part 2).</summary>
    internal static IReadOnlyList<byte[]> Entries
    {
        get
        {
            _ = States.Value;
            return EntryStates;
        }
    }

    private static List<Captured> Capture()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var list = new List<Captured>();
        var seen = new Dictionary<(int, ushort), int>();
        int week = -1;
        foreach (var (off, args) in Routines)
            p.Machine.AddCodeHook(p.Segment(0x08CF), off, _ =>
            {
                if (week < 0) return;
                var key = (week, off);
                int n = seen.GetValueOrDefault(key);
                seen[key] = n + 1;
                if (n % Stride != 0 || n / Stride >= PerWeek) return;
                list.Add(new Captured(week, off, p.DataSegment()[..L.Size],
                    Enumerable.Range(0, args).Select(i => p.StackArgument(i)).ToArray()));
            });
        for (int w = 0; w < 4; w++)
        {
            Assert.True(p.RunToRoutine(0x08CF, 0x0004, 1800), $"08CF:0004 not reached (week {p.Week})");
            Assert.Equal(w, p.Week);
            EntryStates.Add(p.DataSegment()[..L.Size]);
            week = w;
            Assert.True(p.RunToRoutine(0x1F63, 0x0B03, 1800), $"end of the purchase of week {w} not reached");
            week = -1;
        }
        return list;
    }

    private static readonly Lazy<PurchaseHarness> Shared = new(() => new PurchaseHarness());

    private static int? Run(ushort off, Dgroup d, OriginalRandom rnd, PurchaseHarness.Recorder o, ushort[] args)
    {
        switch (off)
        {
            case 0x0082: R.Layout(d); break;
            case 0x029A: R.Street(d, rnd, o); break;
            case 0x039C: R.Enter(d, o); break;
            case 0x03E5: R.ComputerChoice(d); break;
            case 0x0455: R.Head(d, (short)args[0]); break;
            case 0x0492: R.EnterOil(d, o); break;
            case 0x04BA: R.OilShop(d, o); break;
            case 0x05F1: return R.ComputerWantsOil(d) ? 1 : 0;
            case 0x066D: R.EnterGems(d, o); break;
            case 0x069F: R.GemShop(d, o); break;
            case 0x0813: return R.CanSellGem(d) ? 1 : 0;
            case 0x084F: return R.CanBuyGem(d) ? 1 : 0;
            case 0x0886: R.TradeGem(d, (sbyte)args[0], o); break;
            case 0x0914: return R.ComputerGemChoice(d);
            case 0x09C4: R.EnterRockets(d, o); break;
            case 0x09F1: R.RocketShop(d, o); break;
            case 0x0AF0: return R.ComputerWantsRocket(d) ? 1 : 0;
            case 0x0BE1: R.EnterLotto(d, o); break;
            case 0x0CD5: R.Lotto(d, rnd, o); break;
            case 0x0DEF: return R.CanPlayLotto(d) ? 1 : 0;
            case 0x0E53: R.Pick(d, (short)args[0], o); break;
            case 0x0F4D: R.Draw(d, rnd, o); break;
            case 0x115C: R.Blink(d, o); break;
            case 0x1348: R.Setup(d, rnd, o); break;
            case 0x161F: R.NewDraw(d, o); break;
            case 0x178D: R.ChangeGems(d, (sbyte)args[0], o); break;
            default: throw new InvalidOperationException($"08CF:{off:X4}");
        }
        return null;
    }

    private static int Mask(ushort off) => off switch
    {
        0x05F1 or 0x0813 or 0x084F or 0x0AF0 or 0x0DEF => 0xFF,
        0x0914 => 0xFFFF,
        _ => 0,
    };

    // [DIFFERENTIAL] 08CF:029A, 08CF:0CD5, 08CF:0F4D, 08CF:1348, 08CF:09F1, 08CF:0AF0
    [GameDataFact]
    public void Rules_of_the_purchase_on_states_of_weeks_0_to_3()
    {
        var states = States.Value;
        foreach (ushort off in Reached)
            Assert.True(states.Any(s => s.Off == off), $"08CF:{off:X4} not reached in weeks 0-3");
        Assert.Equal([0, 1, 2, 3], states.Select(s => s.Week).Distinct().Order());
        foreach (var s in states)
            foreach (int variant in new[] { 0, 1, 2 })
            {
                var d = new Dgroup((byte[])s.DataSegment.Clone());
                PurchaseHarness.UseTestTexts(d);
                if (variant >= 1)
                {
                    d.Set(L.IsComputer + 0, (byte)0);
                    d.Set(L.IsComputer + 2, (byte)0);
                }
                if (variant == 2)
                {
                    d.Set(L.OilConsumption, (byte)0);
                    d.Set(L.GemTrading, (byte)0);
                }
                Shared.Value.AssertSame(d, 0x08CF, s.Off, (p, rnd, rec) => Run(s.Off, p, rnd, rec, s.Args),
                    $"{s} variant {variant}", Mask(s.Off), s.Args);
            }
    }
}
