using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;
using S = Traders.Core.Stations.StationsLayout;

namespace Traders.Core.Stations;

/// <summary>
/// Stations phase 1B03:0009 (docs/routines/1B03_0009_stationen.md): station draw, choice of a station by every player (humans
/// through the controls 0E9E, computers through 1B03:0632), rocket flight with pirates, lost rocket fields and the weekly
/// bonuses. Works on the data segment like the original; every call into the output layers is reported as a
/// <see cref="StationEvent"/> in the original order, random numbers come from <see cref="OriginalRandom"/>. The coroutines
/// halt at every frame of the original: one <see cref="FieldWait.Retrace"/> per retrace wait (21FD:0185, the flips of
/// 21FD:051F/060D/0665 and of the typed texts 0A69:01AD, the fades 21FD:037D/0329 and the palette step 21FD:03EB), one
/// <see cref="FieldWait.Poll"/> per busy poll of the pause 21FD:0245; the events reported before a wait belong to its frame.
/// The star field 1B03:1178, the flight loop 1B03:08ED and the hit test 1B03:0BE7 read the drawn page: the core reads
/// its rule page <see cref="Pixels"/> instead (R6P-1).
/// </summary>
public sealed partial class StationPhase
{
    private readonly Dgroup _d;
    private readonly OriginalRandom _random;
    private readonly IControlDevices _devices;
    private readonly List<StationEvent> _events = [];

    public StationPhase(Dgroup state, OriginalRandom random, IControlDevices devices)
    {
        _d = state;
        _random = random;
        _devices = devices;
    }

    /// <summary>
    /// Rule page of the phase: every output call is also played onto it; the star field, the flight loop and the
    /// hit test read their pixels there. Required for <see cref="StarField"/>, <see cref="FlightLoop"/> and <see cref="Run"/>.
    /// </summary>
    public Pixels.StationsPage? Pixels { get; init; }

    /// <summary>
    /// Receiver of every output call at the moment it is reported (the scene plays it with the data segment of that moment:
    /// object records, key table). Optional; <see cref="Events"/> keeps the calls either way.
    /// </summary>
    public Action<StationEvent>? Output { get; init; }

    /// <summary>
    /// strlen of a typed text by far pointer (offset, segment), for the frames of 0A69:01AD; by default from the text buffer of
    /// <see cref="Pixels"/>.
    /// </summary>
    public Func<ushort, ushort, int>? TextLength { get; init; }

    /// <summary>Page flips 21FD:051F called by 1B03 itself so far (the controls of the next pass are read after each).</summary>
    public int Flips { get; private set; }

    /// <summary>Output calls reported so far.</summary>
    public IReadOnlyList<StationEvent> Events => _events;

    /// <summary>Returns and clears the reported output calls.</summary>
    public List<StationEvent> DrainEvents()
    {
        var list = new List<StationEvent>(_events);
        _events.Clear();
        return list;
    }

    private const short Ds = StationEvent.DataSegment;

    private void Emit(StationCall call, params short[] args)
    {
        var e = new StationEvent(call, args);
        _events.Add(e);
        Pixels?.Apply(e);
        Output?.Invoke(e);
    }

    private sbyte P => _d.S8(L.ActivePlayer);

    private void SetP(int p) => _d.Set(L.ActivePlayer, unchecked((sbyte)p));

    // Effective addresses wrap at 64 KB like the original's 16-bit index arithmetic.
    private static int At(int offset) => Int16Math.U(offset);

    private static int Obj(int p) => Int16Math.U(Int16Math.S(p * S.ObjectSize));

    private short Word(int offset) => _d.S16(At(offset));

    private byte Byte(int offset) => _d.U8(At(offset));

    private sbyte SByte(int offset) => _d.S8(At(offset));

    /// <summary>[082E], the draw page index: the rule page's (the core's data segment leaves it to the output layers).</summary>
    private short Page => Pixels?.DrawIndex ?? _d.S16(S.DrawPage);

    /// <summary>[0830], the shown page index: the rule page's.</summary>
    private short ShownPage => Pixels?.ShownIndex ?? _d.S16(S.ShownPage);

    /// <summary>The rocket fields of player p (DS:2EE7 + 4p + 3, i.e. DS:2EEA + 4p).</summary>
    private sbyte RocketFields(int p) => SByte(L.FieldsByKind + 4 * p + 3);

    private short PlayerColor(int p) => Word(S.PlayerColor + 2 * p);

    // ------------------------------------------------------------------ 21FD helpers reported as calls

    // Retraces per stage of the fades 21FD:037D (0185(4)) and 21FD:0329 (0185(3)), as in Planting.
    private const int FadeOutRetraces = 4, FadeInRetraces = 3;

    /// <summary>21FD:0185(n, 0) – n retraces (the high word is 0 at every call in 1B03: xor ax, ax).</summary>
    // [ORIGINAL] 21FD:0185
    private IEnumerable<FieldWait> Delay(short retraces)
    {
        Emit(StationCall.Delay, retraces, 0);
        foreach (var w in Planting.Retraces(retraces, 0)) yield return w;
    }

    /// <summary>21FD:051F called by 1B03 – flip with its retraces (<see cref="Planting.Flip"/>); counted in <see cref="Flips"/>.</summary>
    // [ORIGINAL] 21FD:051F
    private IEnumerable<FieldWait> Flip()
    {
        Emit(StationCall.Flip);
        Flips++;
        foreach (var w in Planting.Flip(_d)) yield return w;
    }

    /// <summary>21FD:060D – list, flip (its retraces), list.</summary>
    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    private IEnumerable<FieldWait> ListOnBothPages()
    {
        Emit(StationCall.ListOnBothPages);
        foreach (var w in Planting.Flip(_d)) yield return w;
    }

    /// <summary>21FD:0665 – flip (its retraces), restore, list, objects.</summary>
    // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
    private IEnumerable<FieldWait> LastFrame()
    {
        Emit(StationCall.LastFrame);
        foreach (var w in Planting.Flip(_d)) yield return w;
    }

    /// <summary>21FD:0245 – pause check: busy polls while P is held, released, pressed again (<see cref="Planting.Pause"/>).</summary>
    // [ORIGINAL] 21FD:0245
    private IEnumerable<FieldWait> Pause()
    {
        Emit(StationCall.Pause);
        foreach (var w in Planting.Pause(_d)) yield return w;
    }

    /// <summary>21FD:037D(rgb12) – fade out: eight stages of 03EB and 0185(4) (<see cref="Planting.Fade"/>).</summary>
    // [ORIGINAL] 21FD:037D
    private IEnumerable<FieldWait> FadeOut(short rgb12)
    {
        Emit(StationCall.FadeOut, rgb12);
        foreach (var w in Planting.Fade(_d, FadeOutRetraces)) yield return w;
    }

    /// <summary>21FD:0329 – fade in: eight stages of 03EB and 0185(3).</summary>
    // [ORIGINAL] 21FD:0329
    private IEnumerable<FieldWait> FadeIn()
    {
        Emit(StationCall.FadeIn);
        foreach (var w in Planting.Fade(_d, FadeInRetraces)) yield return w;
    }

    /// <summary>21FD:03EB(rgb12, steps) – one palette step; on VGA (DS:090E = 1) the DAC through 2295:00C5 (one retrace).</summary>
    // [ORIGINAL] 21FD:03EB – 2295:00C5 only when DS:090E = 1
    private IEnumerable<FieldWait> PaletteStep(short rgb12, short steps)
    {
        Emit(StationCall.PaletteStep, rgb12, steps);
        if (_d.S16(Planting.CardMode) == 1) yield return FieldWait.Retrace;
    }

    /// <summary>
    /// 0A69:0243(x, y, w, far text, colour, wait, final wait) – centred typed text: nothing for a null pointer, else per
    /// character (0A69:01AD) 21FD:060D (flip), the key click, 21FD:0245 and 21FD:0185(wait); then 0185(final wait) if it is
    /// &gt; 0. The length of the text is read from the text buffer of <see cref="Pixels"/>.
    /// </summary>
    // [ORIGINAL] 0A69:0243, 0A69:01AD
    private IEnumerable<FieldWait> TypedText(short x, short y, short w, short offset, short segment, short colour, short wait,
        short finalWait)
    {
        Emit(StationCall.TypedText, x, y, w, offset, segment, colour, wait, finalWait);
        foreach (var f in Typewriter(offset, segment, wait, finalWait)) yield return f;
    }

    /// <summary>
    /// 0A69:0288(x, y, w, far lines, count, colour, wait) – the lines i = 0 … count − 1 (signed) of the far pointer table, each
    /// typed with 0A69:0243 and final wait −1.
    /// </summary>
    // [ORIGINAL] 0A69:0288
    private IEnumerable<FieldWait> TypedLines(short x, short y, short w, short lines, short segment, short count, short colour,
        short wait)
    {
        Emit(StationCall.TypedLines, x, y, w, lines, segment, count, colour, wait);
        for (short i = 0; i < count; i++)
        {
            int entry = Int16Math.U(lines + 4 * i); // the table lies in DS at every call in 1B03
            foreach (var f in Typewriter(_d.S16(entry), _d.S16(Int16Math.U(entry + 2)), wait, -1)) yield return f;
        }
    }

    // [ORIGINAL] 0A69:01AD – strlen(text) passes of 060D, 21A7:0054, 21FD:0245, 21FD:0185(wait); 0185(final wait) if > 0
    private IEnumerable<FieldWait> Typewriter(short offset, short segment, short wait, short finalWait)
    {
        int length = offset == 0 && segment == 0 ? 0
            : TextLength?.Invoke((ushort)offset, (ushort)segment)
              ?? (Pixels ?? throw new InvalidOperationException("Typed texts need the text buffer: set StationPhase.Pixels."))
                  .TextLength((ushort)offset, (ushort)segment);
        for (int i = 0; i < length; i++)
        {
            foreach (var f in Planting.Flip(_d)) yield return f;
            foreach (var f in Planting.Pause(_d)) yield return f;
            foreach (var f in Planting.Retraces(wait, (short)(wait >> 15))) yield return f;
        }
        if (finalWait > 0)
            foreach (var f in Planting.Retraces(finalWait, (short)(finalWait >> 15))) yield return f;
    }

    private void ClearList() => Emit(StationCall.ClearList);

    // ------------------------------------------------------------------ 21AF core parts

    /// <summary>21AF:0412(object, frame) – core part: frame (+18h) and active (+0); restore records are the output's.</summary>
    // [ORIGINAL] 21AF:0412 – core part only (object records +0, +18h)
    private void ShowObject(int obj, short frame)
    {
        Emit(StationCall.ShowObject, (short)obj, frame);
        _d.Set(At(S.ObjectFrame + Obj(obj)), frame);
        _d.Set(At(S.Objects + Obj(obj)), (byte)1);
    }

    /// <summary>21AF:045E(object) – inactive (+0 = 0).</summary>
    // [ORIGINAL] 21AF:045E
    private void HideObject(int obj)
    {
        Emit(StationCall.HideObject, (short)obj);
        _d.Set(At(S.Objects + Obj(obj)), (byte)0);
    }

    // ------------------------------------------------------------------ 1B03:0009 entry

    /// <summary>
    /// 1B03:0079 – four times a random station 0…10: a station without value gets 10 (chance 1/2) or random(3), a station with
    /// a value loses it (−1). Station 11 is never drawn.
    /// </summary>
    // [ORIGINAL] 1B03:0079
    public void DrawStationValues()
    {
        for (int i = 0; i <= 3; i++)
        {
            short si = _random.Next(11);
            int at = At(L.StationValue + 2 * si);
            if (_d.S16(at) == -1) _d.Set(at, _random.Next(2) != 0 ? (short)10 : _random.Next(3));
            else _d.Set(at, (short)-1);
        }
    }

    /// <summary>True when any player has a rocket field (1B03:0009 checks DS:2EEA, 2EEE, 2EF2, 2EF6).</summary>
    public bool AnyRockets()
    {
        for (int p = 0; p < L.PlayerCount; p++)
            if (RocketFields(p) != 0) return true;
        return false;
    }

    /// <summary>
    /// 1B03:148B – object setup: the explosion object 4 (layer 13, save buffers, five frames) and one rocket object per player
    /// (layer 15, save buffers, two frames, shown with frame 0); DS:37AA ends at 4.
    /// </summary>
    // [ORIGINAL] 1B03:148B
    public void SetUpObjects()
    {
        _d.Set(S.ObjectLayer + Obj(S.ExplosionObject), (short)0x0D);
        Emit(StationCall.ObjectBuffers, 4, 0, 0x78, 0, 0x85);
        for (short si = 0; si < 3; si++)
            Emit(StationCall.ObjectImage, 4, si, Int16Math.S((si << 4) + 0x100), 0xD3, Int16Math.S((si << 4) + 0x50), 0x28);
        for (short si = 0; si < 2; si++)
            Emit(StationCall.ObjectImage, 4, Int16Math.S(si + 3), Int16Math.S(0x110 - (si << 4)), 0xD3, Int16Math.S(0x60 - (si << 4)), 0x28);
        for (SetP(0); P < 4; SetP(P + 1))
        {
            int p = P;
            _d.Set(At(S.ObjectLayer + Obj(p)), (short)0x0F);
            Emit(StationCall.ObjectBuffers, (short)p, 0, Int16Math.S(p * 15), 0, Int16Math.S(p * 15 + 0x3C));
            Emit(StationCall.ObjectImage, (short)p, 0, Int16Math.S((p << 4) + 0x100), 0xE2, Int16Math.S(p << 4), 0x28);
            Emit(StationCall.ObjectImage, (short)p, 1, Int16Math.S((p << 4) + 0x100), 0xF1, 0x40, 0x28);
            ShowObject(p, 0);
        }
    }

    /// <summary>
    /// 1B03:0009 after the rocket check (<see cref="AnyRockets"/>) up to the star field: picture, objects and the fade-out.
    /// </summary>
    // part of 1B03:0009 (marker at Run)
    public IEnumerable<FieldWait> Begin()
    {
        Emit(StationCall.LoadPicture, Word(S.Picture));
        SetUpObjects();
        foreach (var w in FadeOut(2)) yield return w;
    }

    /// <summary>1B03:0009 after the star field 1B03:1178: palette and fade-in, then the selection 1B03:00D4 follows.</summary>
    public IEnumerable<FieldWait> ShowScreen()
    {
        Emit(StationCall.BasePalette, S.Palette, Ds);
        foreach (var w in FadeIn()) yield return w;
    }
}
