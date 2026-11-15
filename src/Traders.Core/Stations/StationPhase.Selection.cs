using Traders.Core.Input;
using Traders.Core.Rules;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Core.Stations;

/// <summary>Station choice 1B03:00D4 and its helpers (docs/routines/1B03_0009_stationen.md, section "Stationswahl").</summary>
public sealed partial class StationPhase
{
    /// <summary>
    /// 1B03:00D4 – new stations (0101), station marks (0153), pirates (01C3), cursors (0201), the choice loop (02C0), hiding
    /// the cursors (15D7) and clearing the message box (13EE). Yields after every page flip of the choice loop.
    /// </summary>
    // [ORIGINAL] 1B03:00D4
    public IEnumerable<StationWait> Selection()
    {
        NewStations();
        DrawStationMarks();
        PlacePirates();
        PlaceCursors();
        ClearList();
        foreach (var w in Choose()) yield return w;
        HideCursors();
        ClearMessageBox();
    }

    /// <summary>
    /// 1B03:0101 – with chance 1/6 the stations are made anew: the three typed lines of text 192, 0E31:023D, a pause of 50
    /// retraces and the message box cleared.
    /// </summary>
    // [ORIGINAL] 1B03:0101
    public void NewStations()
    {
        if (_random.Next(6) != 0) return;
        Emit(StationCall.TypedLines, 0x71, 0xB4, 0x56, S.NewStationsText, Ds, 3, 7, 5);
        NewGame.CreateStations(_d, _random); // 0E31:023D
        Delay(50);
        ClearMessageBox();
    }

    /// <summary>
    /// 1B03:0153 – a mark beside every station with a value: colour 5 for value 10, else the colour of the player named by
    /// the value; then the list on both pages.
    /// </summary>
    // [ORIGINAL] 1B03:0153
    public void DrawStationMarks()
    {
        ClearList();
        for (int si = 0; si <= 11; si++)
        {
            short value = _d.S16(L.StationValue + 2 * si);
            if (value == -1) continue;
            short colour = value == 10 ? (short)5 : Word(S.PlayerColor + 2 * value);
            Emit(StationCall.Text, Int16Math.S(_d.S16(S.StationX + 2 * si) + 0x12), (short)(_d.U8(S.StationY + si) + 9),
                S.StationMarkText, Ds, colour, -1);
        }
        ListOnBothPages();
    }

    /// <summary>
    /// 1B03:01C3 – with pirate presence ≥ 1 and chance 1/2 the pirates wait at position random(presence − 1) and their ship
    /// is drawn (1257, list on both pages); otherwise DS:546E = −1.
    /// </summary>
    // [ORIGINAL] 1B03:01C3
    public void PlacePirates()
    {
        if (_d.S16(L.PiratePresence) < 1 || _random.Next(2) != 0)
        {
            _d.Set(S.PiratePosition, (sbyte)-1);
            return;
        }
        _d.Set(S.PiratePosition, unchecked((byte)_random.Next(Int16Math.S(_d.S16(L.PiratePresence) - 1))));
        ClearList();
        DrawShip();
        ListOnBothPages();
    }

    /// <summary>
    /// 1B03:0201 – all stations free; per player the move delay p·3, the cursor at the kept station (marked taken, frame 1)
    /// or at station 2p (frame 0), the cursor object placed (1617) and the cursor line drawn (0548). DS:37AA ends at 4.
    /// </summary>
    // [ORIGINAL] 1B03:0201
    public void PlaceCursors()
    {
        for (int si = 0; si <= 11; si++) _d.Set(S.Taken + si, (byte)0);
        ClearList();
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            short di = Word(L.Station + 2 * p);
            _d.Set(At(ControlReader.MoveDelay + 4 * p), (int)Int16Math.S(p * 3));
            if (di == -1)
            {
                _d.Set(At(S.Cursor + 2 * p), Int16Math.S(p * 2));
                PlaceCursorObject();
                _d.Set(At(S.ObjectFrame + Obj(p)), (short)0);
            }
            else
            {
                _d.Set(At(S.Cursor + 2 * p), di);
                PlaceCursorObject();
                _d.Set(At(S.ObjectFrame + Obj(p)), (short)1);
                _d.Set(At(S.Taken + di), (byte)1);
            }
            DrawCursorLine();
        }
        ListOnBothPages();
    }

    /// <summary>
    /// 1B03:02C0 – the choice loop: every pass flips, restores the objects, runs and empties the list, animates the ship
    /// lights (12D7(10)), checks the pause, sets the computers' controls (05E7) and reads all controls (0E9E:000E); each player
    /// still choosing moves left or right (0413) or fires: without a station he takes the one under his cursor (04D4), with
    /// one he confirms it (state 2, the mark of text DS:0E40 in his colour) and is done. Ends with 21FD:0665 when all four
    /// confirmed. Yields after each flip (the controls of the next pass come from the host).
    /// </summary>
    // [ORIGINAL] 1B03:02C0
    public IEnumerable<StationWait> Choose()
    {
        bool[] choosing = [true, true, true, true]; // local byte[4] at bp-6 in the original
        int left = 4;
        for (SetP(0); P < 4; SetP(P + 1)) { }
        while (left != 0)
        {
            Emit(StationCall.Flip);
            yield return StationWait.Flip;
            Emit(StationCall.RestoreObjects, Page);
            Emit(StationCall.RunList);
            ClearList();
            AnimateLights(10);
            Emit(StationCall.Pause);
            SetComputerControls();
            ControlReader.ReadAll(_d, _devices); // 0E9E:000E
            for (SetP(0); P < 4; SetP(P + 1))
            {
                int p = P;
                if (!choosing[p]) continue;
                if (_d.U8(ControlReader.FlagLeft + p) != 0) MoveCursor(-1);
                else if (_d.U8(ControlReader.FlagRight + p) != 0) MoveCursor(1);
                else if (_d.U8(ControlReader.FlagFire + p) != 0)
                {
                    short station = Word(L.Station + 2 * p);
                    if (station == -1) TakeStation();
                    else
                    {
                        _d.Set(At(S.Taken + station), (byte)2);
                        left--;
                        choosing[p] = false;
                        Emit(StationCall.Text, 2, Int16Math.S((p << 3) + 0x0A), S.ConfirmMarkText, Ds,
                            Word(S.RocketColor + 2 * p), -1);
                    }
                }
            }
            Emit(StationCall.RunList);
            Emit(StationCall.DrawObjects, Page);
        }
        Emit(StationCall.LastFrame);
    }

    /// <summary>
    /// 1B03:0413(step) – a kept station is given up (free, cursor frame 0); the cursor moves by the step with wrap 11 → 0 and
    /// 0 → 11, its object follows (1617), sound 16, the line is drawn (0548), and the move delay is 43 − 10·IQ for a computer,
    /// 20 for a human.
    /// </summary>
    // [ORIGINAL] 1B03:0413
    public void MoveCursor(short step)
    {
        int p = P;
        short station = Word(L.Station + 2 * p);
        if (station != -1)
        {
            _d.Set(At(S.Taken + station), (byte)0);
            _d.Set(At(L.Station + 2 * p), (short)-1);
            _d.Set(At(S.ObjectFrame + Obj(p)), (short)0);
        }
        short si = Int16Math.S(Word(S.Cursor + 2 * p) + step);
        if (si > 11) si = 0;
        else if (si < 0) si = 11;
        _d.Set(At(S.Cursor + 2 * p), si);
        PlaceCursorObject();
        Emit(StationCall.Sound, 0, 0x10);
        DrawCursorLine();
        short delay = _d.U8(At(L.IsComputer + p)) == 1 ? Int16Math.S(0x2B - _d.S16(L.ComputerIq) * 10) : (short)0x14;
        _d.Set(At(ControlReader.MoveDelay + 4 * p), (int)delay); // cwd
    }

    /// <summary>
    /// 1B03:04D4 – takes the station under the cursor if it is free: kept station, state 1, cursor frame 1, sound 17, line
    /// drawn; a human's fire delay becomes 10000.
    /// </summary>
    // [ORIGINAL] 1B03:04D4
    public void TakeStation()
    {
        int p = P;
        short si = Word(S.Cursor + 2 * p);
        if (Byte(S.Taken + si) != 0) return;
        _d.Set(At(L.Station + 2 * p), si);
        _d.Set(At(S.Taken + si), (byte)1);
        _d.Set(At(S.ObjectFrame + Obj(p)), (short)1);
        Emit(StationCall.Sound, 0, 0x11);
        DrawCursorLine();
        if (_d.U8(At(L.IsComputer + p)) == 0) _d.Set(At(ControlReader.FireDelay + 4 * p), 10000);
    }

    /// <summary>
    /// 1B03:0548 – the player's line in the message box: cleared (141D), the name of the station under the cursor at x 89h and,
    /// for a visited station, the name of its kind at x 71h, both in the player's colour.
    /// </summary>
    // [ORIGINAL] 1B03:0548
    public void DrawCursorLine()
    {
        int p = P;
        short si = Word(S.Cursor + 2 * p);
        ClearCursorLine();
        short y = Int16Math.S(p * 6 + 0xB4);
        int name = At(S.StationName + 4 * si);
        Emit(StationCall.ShadowText, 0x89, y, _d.S16(name), _d.S16(At(name + 2)), PlayerColor(p), 0, -1, 0);
        if (Byte(L.StationVisited + si) != 1) return;
        int kind = At(S.KindName + 4 * Word(L.StationKind + 2 * si));
        Emit(StationCall.ShadowText, 0x71, y, _d.S16(kind), _d.S16(At(kind + 2)), PlayerColor(p), 0, -1, 0);
    }

    /// <summary>1B03:05E7 – every computer player fires when his cursor is on the best station (0632), else steps right.</summary>
    // [ORIGINAL] 1B03:05E7
    public void SetComputerControls()
    {
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            if (_d.U8(L.IsComputer + p) != 1) continue;
            byte bits = BestStation() == Word(S.Cursor + 2 * p) ? (byte)ControlBits.Fire : (byte)ControlBits.Right;
            _d.Set(ControlReader.Bits + p, bits);
        }
    }

    /// <summary>
    /// 1B03:0632 – the computer's choice: from the cursor on, twelve stations (wrapping above 11) are scored, skipping
    /// confirmed ones and ones taken by another player unless the value names the player: not visited 3, kind 0 → 1, kind k →
    /// 2k + 3; value −1 adds 10, the player's own value 9, a value of another computer negates the score. The first station
    /// with the highest score above −1000 wins.
    /// </summary>
    // [ORIGINAL] 1B03:0632
    public short BestStation()
    {
        int p = P;
        short best = -1000, bestIndex = 0; // [APPROX] bestIndex: uninitialised stack word in the original, always set in play
        short si = Word(S.Cursor + 2 * p);
        for (int cx = 0; cx <= 11; cx++, si++)
        {
            if (si > 11) si = 0;
            short dx = Word(L.StationValue + 2 * si);
            byte taken = Byte(S.Taken + si);
            if (taken == 2) continue;
            if (taken == 1 && p != dx && Word(L.Station + 2 * p) != si) continue;
            short di;
            if (Byte(L.StationVisited + si) == 0) di = 3;
            else
            {
                short kind = Word(L.StationKind + 2 * si);
                di = kind == 0 ? (short)1 : Int16Math.S((kind << 1) + 3);
            }
            if (dx == -1) di = Int16Math.S(di + 10);
            else if (p == dx) di = Int16Math.S(di + 9);
            else if (Byte(L.IsComputer + dx) == 1) di = Int16Math.S(-di);
            if (di <= best) continue;
            bestIndex = si;
            best = di;
        }
        return bestIndex;
    }

    /// <summary>1B03:1617 – the player's cursor object at the station under his cursor: x of the station, y − 2.</summary>
    // [ORIGINAL] 1B03:1617
    public void PlaceCursorObject()
    {
        int p = P;
        short si = Word(S.Cursor + 2 * p);
        _d.Set(At(S.ObjectX + Obj(p)), Word(S.StationX + 2 * si));
        _d.Set(At(S.ObjectY + Obj(p)), Int16Math.S(Byte(S.StationY + si) - 2));
    }

    /// <summary>1B03:15D7 – the four cursor objects hidden (DS:37AA ends at 4), both pages restored and redrawn.</summary>
    // [ORIGINAL] 1B03:15D7
    public void HideCursors()
    {
        for (SetP(0); P < 4; SetP(P + 1)) HideObject(P);
        Emit(StationCall.RestoreObjects, Page);
        Emit(StationCall.DrawOnShownPage);
        Emit(StationCall.RestoreObjects, _d.S16(S.ShownPage));
        Emit(StationCall.DrawOnHiddenPage);
    }

    /// <summary>1B03:141D – clears the player's line of the message box (blit 60h × 6 from the atlas).</summary>
    // [ORIGINAL] 1B03:141D
    public void ClearCursorLine()
    {
        short y = Int16Math.S(P * 6 + 0xAF);
        Emit(StationCall.Blit, 0x70, y, 0x60, 6, 0x70, y);
    }

    /// <summary>1B03:13EE – clears the message box on both pages.</summary>
    // [ORIGINAL] 1B03:13EE
    public void ClearMessageBox()
    {
        ClearList();
        Emit(StationCall.Blit, 0x70, 0xAF, 0x60, 0x19, 0x70, 0xAF);
        ListOnBothPages();
    }
}
