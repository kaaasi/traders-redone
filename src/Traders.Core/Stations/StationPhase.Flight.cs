using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Core.Stations;

/// <summary>
/// Rocket flight 1B03:0705 without its frame loop (docs/routines/1B03_0009_stationen.md, section "Raketenflug"): setup,
/// messages, launch schedule, flight curve, pirate state machine and attack decision, ship drawing and palette flash. The
/// frame loop 1B03:08ED with the hit test 1B03:0BE7 reads the drawn page (rule page, StationPhase.Pixels.cs).
/// </summary>
public sealed partial class StationPhase
{
    /// <summary>1B03:0740 – flight state reset: no rocket flying or attacked, pirates waiting, no flash.</summary>
    // [ORIGINAL] 1B03:0740
    public void ResetFlight()
    {
        _d.Set(S.Flying, (byte)0);
        _d.Set(S.RocketTotal, (byte)0);
        _d.Set(S.Attacked, (sbyte)-1);
        _d.Set(S.PirateState, (byte)0);
        _d.Set(S.PiratesReady, (byte)0);
        _d.Set(S.Flash, (byte)0);
    }

    /// <summary>
    /// 1B03:0763 – a player whose station value names another player (not −1, not himself) gets the four lines of text 195
    /// and DS:546A = 1, every other player 0. The value is read at DS:348A + 2·station, also for station −1.
    /// </summary>
    // [ORIGINAL] 1B03:0763
    public void OccupiedMessages()
    {
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            short si = Word(L.StationValue + 2 * Word(L.Station + 2 * p));
            if (si != -1 && p != si)
            {
                Message(S.OccupiedText, 4);
                _d.Set(S.Occupied + p, (byte)1);
            }
            else _d.Set(S.Occupied + p, (byte)0);
        }
    }

    /// <summary>
    /// 1B03:07C4 – per player: launched and lost counts 0, the rocket fields added to the total, text 225 and the lost count
    /// (0D75) in his line; list on both pages. DS:37AA ends at 4.
    /// </summary>
    // [ORIGINAL] 1B03:07C4
    public void PrepareRockets()
    {
        ClearList();
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            _d.Set(S.Launched + p, (byte)0);
            _d.Set(S.Lost + p, (byte)0);
            _d.Set(S.RocketTotal, unchecked((byte)(_d.U8(S.RocketTotal) + (byte)RocketFields(p))));
            Emit(StationCall.ShadowText, 0x71, Int16Math.S(p * 6 + 0xB4), _d.S16(S.LostHeaderText), _d.S16(S.LostHeaderText + 2),
                PlayerColor(p), 0, -1, 0);
            DrawLostCount();
        }
        ListOnBothPages();
    }

    /// <summary>
    /// 1B03:0849 – launch schedule: from player random(3) on, round robin over the players with rockets left to schedule,
    /// rocket k gets start delay −5k (progress 0, −5, −10 …), target x random(299) + 10 and its owner; DS:53B4 counts the
    /// rockets in flight.
    /// </summary>
    // [ORIGINAL] 1B03:0849
    public void ScheduleLaunches()
    {
        short si = 0;
        SetP((byte)_random.Next(3));
        _d.Set(S.RocketIndex, (byte)0);
        while (_d.S8(S.RocketIndex) < _d.S8(S.RocketTotal))
        {
            int p = P;
            if (RocketFields(p) > SByte(S.Launched + p))
            {
                int k = _d.S8(S.RocketIndex);
                _d.Set(At(S.Progress + 2 * k), si);
                si = Int16Math.S(si - 5);
                _d.Set(At(S.Target + 2 * k), Int16Math.S(_random.Next(0x12B) + 0x0A));
                _d.Set(At(S.Owner + k), unchecked((byte)p));
                Inc(S.Launched + p);
                Inc(S.Flying);
                Inc(S.RocketIndex);
            }
            sbyte next = unchecked((sbyte)(P + 1));
            SetP(next > 3 ? 0 : next);
        }
    }

    private void Inc(int offset, int delta = 1) => _d.Set(At(offset), unchecked((byte)(Byte(offset) + delta)));

    /// <summary>
    /// 1B03:0CCF(target, progress) – position of a rocket on its curve from the player's station (x + 8, y + 7) to the target
    /// at the top: x = start + curve[t]·10 / (10000 / (target − start)) (a target equal to the start is moved left by one),
    /// at least 0; y = C7h − curve[59h − t]·10 / (10000 / (C7h − start y)). 16-bit products, signed divisions.
    /// </summary>
    // [ORIGINAL] 1B03:0CCF
    public void RocketPosition(short target, short progress)
    {
        int p = P;
        short station = Word(L.Station + 2 * p);
        short si = Int16Math.S(Word(S.StationX + 2 * station) + 8);
        short startY = (short)(Byte(S.StationY + station) + 7);
        const short di = 0xC7;
        short cx = target;
        if (cx == si) cx--;
        short a = Int16Math.S(Word(S.FlightCurve + 2 * progress) * 10);
        short x = Int16Math.S(si + Int16Math.Div(a, Int16Math.Div(10000, Int16Math.S(cx - si))));
        _d.Set(S.RocketX, x);
        if (_d.S16(S.RocketX) < 0) _d.Set(S.RocketX, (short)0);
        short b = Int16Math.S(Word(S.FlightCurve + 2 * Int16Math.S(0x59 - progress)) * 10);
        _d.Set(S.RocketY, Int16Math.S(di - Int16Math.Div(b, Int16Math.Div(10000, Int16Math.S(di - startY)))));
    }

    /// <summary>
    /// 1B03:0A62 – pirate ship, once per flight frame. Waiting (state 0) with pirates present and no rocket attacked, the
    /// ship starts with chance 1/DS:0D14[position]. Then the state counts up; on 2 the lights and the ship's second place,
    /// on 8 the first place again, on 11 with chance 1/10 the pirates are shot down (wreck, flash 7, sound 13h, no pirates
    /// any more, state 0) or else the gun fires (sound 14h), on 14 the gun is drawn back, on 17 the pirates are ready to
    /// attack and the state returns to 0.
    /// </summary>
    // [ORIGINAL] 1B03:0A62
    public void PirateStep()
    {
        if (_d.S8(S.PirateState) == 0)
        {
            sbyte position = _d.S8(S.PiratePosition);
            if (position == -1 || _d.S8(S.Attacked) != -1) return;
            if (_random.Next((short)Byte(S.PirateChance + position)) != 0) return;
            _d.Set(S.PirateState, (byte)1);
            return;
        }
        sbyte state = unchecked((sbyte)(_d.S8(S.PirateState) + 1));
        _d.Set(S.PirateState, state);
        switch (state - 2) // jump table at 1B03:0B5A, unsigned index 0…15
        {
            case 0:
                DrawLights(5);
                DrawShipAt(1);
                break;
            case 6:
                DrawShipAt(0);
                break;
            case 9:
                if (_random.Next(10) == 0)
                {
                    DrawShipPart(1);
                    DrawLights(4);
                    DrawWreck();
                    _d.Set(S.Flash, (byte)7);
                    Emit(StationCall.Sound, 2, 0x13);
                    _d.Set(S.PiratePosition, (sbyte)-1);
                    _d.Set(S.PirateState, (byte)0);
                }
                else
                {
                    DrawGun(1);
                    Emit(StationCall.Sound, 1, 0x14);
                }
                break;
            case 12:
                DrawGun(0);
                break;
            case 15:
                _d.Set(S.PiratesReady, (byte)1);
                _d.Set(S.PirateState, (byte)0);
                break;
        }
    }

    /// <summary>
    /// 1B03:0B7A – attack decision for rocket DS:53AC of player DS:37AA. A player marked by 0763 is attacked whenever nothing
    /// else is attacked and the ship waits; without pirates every rocket is attacked with chance 1/5000 under the same
    /// conditions; with pirates only after they got ready (0A62 state 17). Only rockets up to y BEh are hit: explosion frame 0,
    /// the rocket becomes the attacked one, sound 15h, ready flag cleared.
    /// </summary>
    // [ORIGINAL] 1B03:0B7A
    public void AttackDecision()
    {
        bool waitingAttack;
        if (Byte(S.Occupied + P) == 1) waitingAttack = true;
        else if (_d.S8(S.PiratePosition) != -1) waitingAttack = false;
        else waitingAttack = _random.Next(5000) == 0;
        if (waitingAttack)
        {
            if (_d.S8(S.Attacked) != -1) return;
            if (_d.S8(S.PirateState) > 0) return;
        }
        else if (_d.U8(S.PiratesReady) == 0) return;
        if (_d.S16(S.RocketY) > 0xBE) return;
        _d.Set(S.ExplosionFrame, (byte)0);
        _d.Set(S.Attacked, _d.U8(S.RocketIndex));
        Emit(StationCall.Sound, 2, 0x15);
        _d.Set(S.PiratesReady, (byte)0);
    }

    /// <summary>1B03:0C89 – one step of the palette flash: while DS:546F > 0 it counts down and the palette moves to grey 777h.</summary>
    // [ORIGINAL] 1B03:0C89
    public void FlashStep()
    {
        if (_d.S8(S.Flash) <= 0) return;
        sbyte f = unchecked((sbyte)(_d.S8(S.Flash) - 1));
        _d.Set(S.Flash, f);
        Emit(StationCall.PaletteStep, 0x777, f);
    }

    /// <summary>1B03:0CAC – ten flash steps with four retraces each (the flash fades out after the flight).</summary>
    // [ORIGINAL] 1B03:0CAC
    public void FlashOut()
    {
        for (int i = 0; i < 10; i++)
        {
            FlashStep();
            Delay(4);
        }
    }

    /// <summary>1B03:0D75 – the player's lost count (3 digits) at x ADh of his line.</summary>
    // [ORIGINAL] 1B03:0D75
    public void DrawLostCount()
    {
        int p = P;
        ClearLostCount();
        short lost = SByte(S.Lost + p);
        Emit(StationCall.ShadowNumber, 0xAD, Int16Math.S(p * 6 + 0xB4), lost, (short)(lost < 0 ? -1 : 0), 3, PlayerColor(p), 0, -1, 0);
    }

    /// <summary>1B03:1454 – clears the lost count of the player's line (blit 10h × 6).</summary>
    // [ORIGINAL] 1B03:1454
    public void ClearLostCount()
    {
        short y = Int16Math.S(P * 6 + 0xAF);
        Emit(StationCall.Blit, 0xB0, y, 0x10, 6, 0xB0, y);
    }

    /// <summary>1B03:0DC0 – a player who lost more than three rockets and all of his rocket fields gets the seven lines of text 199.</summary>
    // [ORIGINAL] 1B03:0DC0
    public void AllLostMessages()
    {
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            sbyte lost = SByte(S.Lost + p);
            if (lost > 3 && RocketFields(p) == lost) Message(S.AllLostText, 7);
        }
    }

    /// <summary>
    /// 1B03:12D7(period) – ship lights: with pirates and the ship at its first place the counter counts up; at the period it
    /// restarts and the next of the four light frames is drawn (131B).
    /// </summary>
    // [ORIGINAL] 1B03:12D7
    public void AnimateLights(short period)
    {
        if (_d.S8(S.PiratePosition) == -1 || _d.U8(S.ShipMoved) == 1) return;
        short c = Int16Math.S(_d.S16(S.LightCounter) + 1);
        _d.Set(S.LightCounter, c);
        if (c < period) return;
        _d.Set(S.LightCounter, (short)0);
        byte f = unchecked((byte)(_d.U8(S.LightFrame) + 1));
        if (f > 3) f = 0;
        _d.Set(S.LightFrame, f);
        DrawLights(f);
    }

    /// <summary>1B03:131B(frame) – one light frame (8-pixel blit at D0h + 8·frame).</summary>
    // [ORIGINAL] 1B03:131B
    public void DrawLights(short frame) => Emit(StationCall.Blit8, Int16Math.S((frame << 3) + 0xD0), 0xE8, 0x10, 8, 0x24);

    /// <summary>
    /// 1B03:1345(place) – the ship at its first (0) or second place, DS:53B1 = place, light counter 1000 (next 12D7 animates
    /// at once).
    /// </summary>
    // [ORIGINAL] 1B03:1345
    public void DrawShipAt(short place)
    {
        if (place == 0) Emit(StationCall.Blit, 0, 0xDB, 0x20, 0x10, 5, 0x33);
        else Emit(StationCall.Blit, 0xB0, 0xC8, 0x20, 0x10, 5, 0x33);
        _d.Set(S.ShipMoved, unchecked((byte)place));
        _d.Set(S.LightCounter, (short)1000);
    }

    /// <summary>1B03:1257 – the pirate ship: hull, the part by position (128D(0)) and the ship at its first place.</summary>
    // [ORIGINAL] 1B03:1257
    public void DrawShip()
    {
        Emit(StationCall.Blit, 0, 0xC8, 0x40, 0x35, 5, 0x20);
        DrawShipPart(0);
        DrawShipAt(0);
    }

    /// <summary>1B03:128D(variant) – the ship part of the pirate position from the tables DS:0DDC/0DDF/0DE1/0DE4.</summary>
    // [ORIGINAL] 1B03:128D
    public void DrawShipPart(byte variant)
    {
        sbyte pos = _d.S8(S.PiratePosition);
        Emit(StationCall.Blit, Byte(S.PirateSourceX + pos), Byte(S.PirateSourceY + variant), Byte(S.PirateTargetX + pos), 0x19,
            Byte(S.PirateTargetY + pos), 0x1A);
    }

    /// <summary>1B03:139D(state) – the gun drawn out (1) or back (0).</summary>
    // [ORIGINAL] 1B03:139D
    public void DrawGun(short state) => Emit(StationCall.Blit, 0xB0, Int16Math.S((state << 4) + 0xD8), 0x20, 0x10, 0x2F, 0x3A);

    /// <summary>1B03:13C9 – the wreck of the pirate ship.</summary>
    // [ORIGINAL] 1B03:13C9
    public void DrawWreck() => Emit(StationCall.Blit, 0xD0, 0xC8, 0x30, 0x20, 0x1B, 0x33);
}
