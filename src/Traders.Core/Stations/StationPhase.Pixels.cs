using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Core.Stations;

/// <summary>
/// The routines of the stations phase that read pixels of the draw page (docs/routines/1B03_0009_stationen.md, sections
/// "Sternfeld", "Raketenflug" and "Regelseite"): the star field 1B03:1178, the flight loop 1B03:08ED with the hit test
/// 1B03:0BE7, and the frames of the whole phase 1B03:0009 and of the flight 1B03:0705. The pixels come from the rule page
/// <see cref="Pixels"/>.
/// </summary>
public sealed partial class StationPhase
{
    private const int LastProgress = 0x59;

    private byte ReadPixel(short x, short y) =>
        (Pixels ?? throw new InvalidOperationException("The stations phase reads pixels: set StationPhase.Pixels.")).Read(x, y);

    /// <summary>
    /// 1B03:0009 – the whole stations phase: station values and the rocket check (<see cref="Begin"/>), the star field, palette
    /// and fade-in, the choice of stations, the rocket flight, the flash, lost rocket fields and the bonuses. Yields after
    /// every page flip of the choice loop and of the flight loop (the controls of the next pass come from the host).
    /// </summary>
    // [ORIGINAL] 1B03:0009
    public IEnumerable<StationWait> Run()
    {
        if (!Begin()) yield break;
        StarField();
        ShowScreen();
        foreach (var w in Selection()) yield return w;
        foreach (var w in Flight()) yield return w;
        End();
    }

    /// <summary>
    /// 1B03:0705 – rocket flight: reset, messages, rocket lines, launch schedule, list emptied (<see cref="FlightBegin"/>), the
    /// flight loop 08ED, then pause, message box and the messages for total losses (<see cref="FlightEnd"/>).
    /// </summary>
    // [ORIGINAL] 1B03:0705
    public IEnumerable<StationWait> Flight()
    {
        FlightBegin();
        foreach (var w in FlightLoop()) yield return w;
        FlightEnd();
    }

    /// <summary>
    /// 1B03:1178 – star field: the picture (atlas rows 0…199) onto the draw page; 200 times a point x = random(319),
    /// y = random(199) whose pixel is read: only a pixel of colour 1 gets a star in colour DS:0DD9[random(2)], drawn at once
    /// (the next reads see it). Then the twelve station pictures (16 × 11 from atlas x 100h + 10h·DS:0DCD[i], y C8h) at
    /// (DS:0C84[i], DS:0C9C[i]) and the draw page copied to the other page.
    /// </summary>
    // [ORIGINAL] 1B03:1178
    public void StarField()
    {
        ClearList();
        Emit(StationCall.Blit, 0, 0, 0x140, 0xC8, 0, 0);
        Emit(StationCall.RunList);
        for (int si = 0; si <= 0xC7; si++)
        {
            short x = _random.Next(0x13F);
            short y = _random.Next(0xC7);
            ClearList();
            if (ReadPixel(x, y) == 1) Emit(StationCall.Plot, x, y, Byte(S.StarColor + _random.Next(2)));
            Emit(StationCall.RunList);
        }
        ClearList();
        for (int si = 0; si <= 0x0B; si++)
            Emit(StationCall.Blit, (short)((Byte(S.StationPicture + si) << 4) + 0x100), 0xC8, 0x10, 0x0B,
                Word(S.StationX + 2 * si), Byte(S.StationY + si));
        Emit(StationCall.RunList);
        Emit(StationCall.CopyPage, Page);
    }

    /// <summary>
    /// 1B03:08ED – flight loop, one pass per frame while rockets fly (DS:53B4 &gt; 0): flip, pause 3, objects restored, the
    /// list of the last frame on this page too, list emptied, pause check; then every rocket k: done (progress &gt; 59h) is
    /// skipped, waiting (progress &lt; 0) counts up; otherwise its owner becomes DS:37AA, at progress 0 the launch sound,
    /// position 0CCF, attack decision 0B7A; the attacked rocket goes to the hit test 0BE7, every other one leaves a point of
    /// its owner's colour where the pixel under it is 1 and moves on (past 59h it has arrived). After the rockets: ship
    /// lights 12D7(3), pirates 0A62, flash 0C89, list and objects drawn. At the end the sound stops and 21FD:0665.
    /// Yields after every flip.
    /// </summary>
    // [ORIGINAL] 1B03:08ED
    public IEnumerable<StationWait> FlightLoop()
    {
        while (_d.S8(S.Flying) > 0)
        {
            Emit(StationCall.Flip);
            yield return StationWait.Flip;
            Delay(3);
            Emit(StationCall.RestoreObjects, Page);
            Emit(StationCall.RunList);
            ClearList();
            Emit(StationCall.Pause);
            for (_d.Set(S.RocketIndex, (byte)0); _d.S8(S.RocketIndex) <= _d.S8(S.RocketTotal) - 1; Inc(S.RocketIndex))
            {
                int k = _d.S8(S.RocketIndex);
                int progress = At(S.Progress + 2 * k);
                if (_d.S16(progress) > LastProgress) continue;
                if (_d.S16(progress) < 0)
                {
                    _d.Set(progress, Int16Math.S(_d.S16(progress) + 1));
                    continue;
                }
                SetP(SByte(S.Owner + k));
                if (_d.S16(progress) == 0) Emit(StationCall.Sound, 0, 0x12);
                RocketPosition(Word(S.Target + 2 * k), _d.S16(progress));
                AttackDecision();
                if (_d.U8(S.RocketIndex) == _d.U8(S.Attacked))
                {
                    RocketHit();
                    continue;
                }
                short x = _d.S16(S.RocketX), y = _d.S16(S.RocketY);
                if (ReadPixel(x, y) == 1) Emit(StationCall.Plot, x, y, Word(S.RocketColor + 2 * P));
                short next = Int16Math.S(_d.S16(progress) + 1);
                _d.Set(progress, next);
                if (next > LastProgress) Inc(S.Flying, -1);
            }
            AnimateLights(3);
            PirateStep();
            FlashStep();
            Emit(StationCall.RunList);
            Emit(StationCall.DrawObjects, Page);
        }
        Emit(StationCall.Sound, 0, 0);
        Emit(StationCall.LastFrame);
    }

    /// <summary>
    /// 1B03:0BE7 – the attacked rocket, once per frame. In its first frame (explosion frame 0) the flash starts (7) and the
    /// pixel under the rocket decides: 1 → the explosion is visible, object 4 shown at (x − 8, y − 6); otherwise not. While
    /// the frame is below 15 and the explosion visible, object 4 shows frame / 3; otherwise object 4 is hidden and the rocket
    /// is lost (progress 5Ah, one fewer in flight, one more lost for its owner, his count redrawn, no rocket attacked). The
    /// explosion frame counts up.
    /// </summary>
    // [ORIGINAL] 1B03:0BE7
    public void RocketHit()
    {
        if (_d.U8(S.ExplosionFrame) == 0)
        {
            _d.Set(S.Flash, (byte)7);
            if (ReadPixel(_d.S16(S.RocketX), _d.S16(S.RocketY)) == 1)
            {
                _d.Set(S.ExplosionVisible, (byte)1);
                ShowObject(S.ExplosionObject, 0);
                _d.Set(S.ObjectX + Obj(S.ExplosionObject), Int16Math.S(_d.S16(S.RocketX) - 8));
                _d.Set(S.ObjectY + Obj(S.ExplosionObject), Int16Math.S(_d.S16(S.RocketY) - 6));
            }
            else _d.Set(S.ExplosionVisible, (byte)0);
        }
        sbyte frame = _d.S8(S.ExplosionFrame);
        if (frame < 0x0F && _d.U8(S.ExplosionVisible) == 1)
            _d.Set(S.ObjectFrame + Obj(S.ExplosionObject), (short)(frame / 3));
        else
        {
            HideObject(S.ExplosionObject);
            _d.Set(At(S.Progress + 2 * _d.S8(S.RocketIndex)), (short)0x5A);
            Inc(S.Flying, -1);
            Inc(S.Lost + P);
            DrawLostCount();
            _d.Set(S.Attacked, (sbyte)-1);
        }
        Inc(S.ExplosionFrame);
    }
}
