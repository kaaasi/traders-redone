using Traders.Core.Field;
using Traders.Core.Rules;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Core.Stations;

/// <summary>
/// End of the stations phase (docs/routines/1B03_0009_stationen.md, section "Ertrag"): the frame-free parts of 1B03:0705,
/// lost rocket fields 1B03:0E11, weekly bonuses 1B03:0EA9 and the message box 1B03:10EA.
/// </summary>
public sealed partial class StationPhase
{
    /// <summary>1B03:0705 up to its flight loop 08ED: reset (0740), messages (0763), rocket lines (07C4), schedule (0849), list cleared.</summary>
    // part of 1B03:0705 (marker at Flight)
    public IEnumerable<FieldWait> FlightBegin()
    {
        ResetFlight();
        foreach (var w in OccupiedMessages()) yield return w;
        foreach (var w in PrepareRockets()) yield return w;
        ScheduleLaunches();
        ClearList();
    }

    /// <summary>1B03:0705 after its flight loop: pause of 100 retraces, message box cleared, messages for total losses (0DC0).</summary>
    public IEnumerable<FieldWait> FlightEnd()
    {
        foreach (var w in Delay(100)) yield return w;
        foreach (var w in ClearMessageBox()) yield return w;
        foreach (var w in AllLostMessages()) yield return w;
    }

    /// <summary>1B03:0009 after the flight: flash out (0CAC), lost rocket fields (0E11), bonuses (0EA9).</summary>
    public IEnumerable<FieldWait> End()
    {
        foreach (var w in FlashOut()) yield return w;
        RemoveLostFields();
        foreach (var w in Bonuses()) yield return w;
    }

    /// <summary>
    /// 1B03:0E11 – every field in the order x = i mod columns, y = i / columns (DS:2EB0, DS:2F00) up to columns·rows − 1:
    /// a rocket field (kind 3) of a player with lost rockets left becomes empty land of the same owner (0609:10D2(x, y,
    /// owner, −2)) and his count goes down. DS:37AA holds the owner of the last field.
    /// </summary>
    // [ORIGINAL] 1B03:0E11
    public void RemoveLostFields()
    {
        for (short si = 0; Int16Math.S(_d.S16(L.FieldColumns) * _d.S16(L.FieldRows) - 1) >= si; si++)
        {
            short columns = _d.S16(L.FieldColumns);
            _d.Set(L.LoopX, unchecked((byte)Int16Math.Mod(si, columns)));
            _d.Set(L.LoopY, unchecked((byte)Int16Math.Div(si, columns)));
            int cell = At(Int16Math.S(_d.S8(L.LoopX) * 6) + _d.S8(L.LoopY));
            sbyte owner = SByte(L.FieldOwner + cell);
            SetP(owner);
            if (owner == -1) continue;
            if (SByte(S.Lost + owner) <= 0) continue;
            if (SByte(L.FieldKind + cell) != 3) continue;
            FieldGrid.Set(_d, _d.S8(L.LoopX), _d.S8(L.LoopY), owner, -2);
            Inc(S.Lost + owner, -1);
        }
    }

    /// <summary>
    /// 1B03:0EA9 – per player with rocket fields, by rank r (DS:3D42): random(22 − 4r) = 5 with gem trading adds
    /// random(2·gem price factor) + 2 to the gem credit (at most 2000, texts 210…213); random(22 − 4r) = 0 with oil
    /// consumption adds random(20) + 2 oil (at most 40, texts 206…209); random(49 − 6r) = 0 for r &gt; 0 adds random(30) + 100
    /// below 1000 cash, else cash / (10 − r), to the cash (texts 214…224). Each amount is written into its text (2295:0001).
    /// DS:37AA ends at 4.
    /// </summary>
    // [ORIGINAL] 1B03:0EA9
    public IEnumerable<FieldWait> Bonuses()
    {
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            if (RocketFields(p) == 0) continue;
            byte rank = _d.U8(L.Rank + p);
            if (_random.Next(Int16Math.S(0x16 - (rank << 2))) == 5 && _d.U8(L.GemTrading) == 1)
            {
                short amount = Int16Math.S(_random.Next(Int16Math.S(_d.S16(L.GemPriceFactor) << 1)) + 2);
                NumberText(amount, 0, 3, S.GemNumberText);
                foreach (var w in Message(S.GemBonusText, 4)) yield return w;
                short credit = Int16Math.S(_d.S16(L.GemCredit + 2 * p) + amount);
                _d.Set(L.GemCredit + 2 * p, credit);
                if (credit > 2000) _d.Set(L.GemCredit + 2 * p, (short)2000);
            }
            if (_random.Next(Int16Math.S(0x16 - (rank << 2))) == 0 && _d.U8(L.OilConsumption) == 1)
            {
                short amount = Int16Math.S(_random.Next(20) + 2);
                NumberText(amount, 0, 3, S.OilNumberText);
                foreach (var w in Message(S.OilBonusText, 4)) yield return w;
                short oil = Int16Math.S(_d.S16(L.Oil + 2 * p) + amount);
                _d.Set(L.Oil + 2 * p, oil);
                if (oil > 40) _d.Set(L.Oil + 2 * p, (short)40);
            }
            if (_random.Next(Int16Math.S(0x31 - rank * 6)) != 0 || rank == 0) continue;
            int cash = _d.S32(L.Cash + 4 * p);
            int bonus = cash < 1000 ? _random.Next(30) + 100 : LongMath.Div(cash, 10 - rank); // 0000:4F89
            Emit(StationCall.NumberText, Int16Math.S(bonus), Int16Math.S(bonus >> 16), 5,
                Int16Math.S(_d.S16(S.CashNumberText) + 2), _d.S16(S.CashNumberText + 2));
            _d.Set(L.Cash + 4 * p, unchecked(cash + bonus));
            foreach (var w in Message(S.CashBonusText, 11)) yield return w;
        }
    }

    private void NumberText(short low, short high, short width, int text) =>
        Emit(StationCall.NumberText, low, high, width, _d.S16(text), _d.S16(text + 2));

    /// <summary>
    /// 1B03:10EA(lines, count) – typed message in the player's colour: lines from the far pointer table, four per box (after
    /// four a pause of 20 retraces and the box cleared), at least one line; then a pause of 50 retraces and the box cleared.
    /// </summary>
    // [ORIGINAL] 1B03:10EA
    public IEnumerable<FieldWait> Message(int table, short count)
    {
        short di = 0, si = 0;
        while (true)
        {
            int at = At(table + 4 * di);
            foreach (var w in TypedText(0x71, Int16Math.S(si * 6 + 0xB4), 0x56, _d.S16(at), _d.S16(At(at + 2)), PlayerColor(P), 5, -1))
                yield return w;
            di++;
            if (di >= count) break;
            si++;
            if (si < 4) continue;
            foreach (var w in Delay(20)) yield return w;
            foreach (var w in ClearMessageBox()) yield return w;
            si = 0;
        }
        foreach (var w in Delay(50)) yield return w;
        foreach (var w in ClearMessageBox()) yield return w;
    }
}
