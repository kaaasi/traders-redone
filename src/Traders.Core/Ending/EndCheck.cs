using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Ending;

/// <summary>
/// The decision of the end check 20B3:0005 (call 19 of the week loop; finding docs/routines/20B3_0005_endpruefung.md): the
/// ranking 0825:06CB writes every player's wealth to DS:2F06, then the players 0..3 are compared with the target DS:08F4
/// (32 bit unsigned, high word first). The first player at or above the target stops the loop with DS:37AA = that player;
/// without one DS:37AA = 4, and the game goes on only if the key with scancode 44h (F10) is not held either.
/// </summary>
public static class EndCheck
{
    /// <summary>Scancode of the key that ends the game at the end check (F10; key table DS:3D47 + 44h = DS:3D8B).</summary>
    public const byte EndKey = 0x44;

    /// <summary>
    /// 20B3:0005–0051 on <paramref name="d"/>: ranking, comparison loop with DS:37AA, key test. True if the game ends (the
    /// original goes on to the end screens at 20B3:0053), false if it returns AL = 0 (20B3:004F).
    /// </summary>
    // [ORIGINAL] 20B3:0005 – up to the decision (20B3:0005–0051); the end screens from 20B3:0053 on are pending (F1 = B)
    public static bool Decide(Dgroup d)
    {
        Ranking.Apply(d); // 0825:06CB
        sbyte player = 0;
        for (; player < L.PlayerCount; player++)
        {
            d.Set(L.ActivePlayer, player);
            // cmp ax,[08F6] / jc / jnz / cmp dx,[08F4] / jc: unsigned, high word first
            if (d.U32(L.CurrentWealth + 4 * player) >= d.U32(L.Target)) break;
        }
        d.Set(L.ActivePlayer, player);
        return player != L.PlayerCount || d.U8(L.KeyState + EndKey) != 0;
    }

    /// <summary>True if the end check would end the game in <paramref name="d"/>; works on a copy, <paramref name="d"/> stays as it is.</summary>
    public static bool Ends(Dgroup d) => Decide(new Dgroup((byte[])d.Bytes.Clone()));
}

/// <summary>
/// Call 19 of the week loop, 20B3:0005, as a step of the phase machine: where the game goes on, the decision runs on the
/// data segment and the step returns AL = 0 at once. Where it ends, the end screens, 0825:07C4, 0825:000A(0), the any-key wait
/// 27C6:00D2 and <c>inc word [DS:4B9A]</c> at 20B3:0071 are not ported (decision F1 = B): the call is pending and the machine
/// stops there ("game over"), or <paramref name="rest"/> (for example the RefEmu oracle of a test) carries out the whole call
/// 20B3:0005 from the unchanged state. The decision for <see cref="IsPending"/> is made on a copy (R7P-2): it depends on
/// DS:2F06, which only 0825:06CB in 20B3:0005 writes fresh.
/// </summary>
/// <param name="rest">Handler of the whole call for the states in which the game ends; null = pending.</param>
public sealed class EndCheckStep(IWeekStepHandler? rest = null) : IPartialWeekStepHandler
{
    /// <summary>Call number of 20B3:0005 in <see cref="WeekLoop"/>.</summary>
    public const int Call = 19;

    private IWeekStepHandler? _running;

    /// <summary>True if the last call ended the game (it went to the rest).</summary>
    public bool GameOver { get; private set; }

    public bool IsPending(Dgroup state) => rest is null && EndCheck.Ends(state);

    public StepOutcome Begin(StepContext context)
    {
        _running = null;
        GameOver = EndCheck.Ends(context.State);
        if (!GameOver)
        {
            EndCheck.Decide(context.State);
            return StepOutcome.Finished(0); // mov al,0 at 20B3:004F
        }
        _running = rest ?? throw new InvalidOperationException("20B3:0005 beyond its decision (end of the game) is not ported");
        return _running.Begin(context);
    }

    public StepOutcome Tick(StepContext context) =>
        (_running ?? throw new InvalidOperationException("the end check finishes at once where the game goes on")).Tick(context);
}
