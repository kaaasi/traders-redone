using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Core.Week;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Events;

/// <summary>DS offsets of the week incidents 17CB:0005 (docs/routines/17CB_0005_wochenereignisse.md, DATENLAYOUT.md).</summary>
public static class IncidentLayout
{
    /// <summary>sbyte[5] – per amount class: line of the amount within the gain text (17CB:02BD).</summary>
    public const int GainNumberLine = 0x09AA;

    /// <summary>sbyte[5] – per amount class: first text index (table DS:37CC) of the gain text.</summary>
    public const int GainText = 0x09AF;

    /// <summary>sbyte[5] – per amount class: number of lines of the gain text.</summary>
    public const int GainLines = 0x09B4;

    /// <summary>sbyte[5] – per amount class: line of the amount within the loss text (17CB:039D).</summary>
    public const int LossNumberLine = 0x09B9;

    /// <summary>sbyte[5] – per amount class: first text index of the loss text.</summary>
    public const int LossText = 0x09BE;

    /// <summary>sbyte[5] – per amount class: number of lines of the loss text.</summary>
    public const int LossLines = 0x09C3;

    /// <summary>Text index of the field takeover (17CB:04D7: far pointer DS:3940 = DS:37CC + 4·5Dh), seven lines.</summary>
    public const int TakeoverText = (0x3940 - MenuLayout.TextPointers) / 4;

    /// <summary>Text index of the harvest loss of kind 0 (17CB:0736: DS:395C + 10h·kind), four lines per kind.</summary>
    public const int BlightText = (0x395C - MenuLayout.TextPointers) / 4;

    /// <summary>word – argument of the page copy 0A96:0C3F in the window 17CB:0986.</summary>
    public const int PageCopyArgument = 0x082E;

    /// <summary>DS offset of the palette the window loads with 21FD:026C.</summary>
    public const int Palette = 0x00A8;

    /// <summary>byte – DS:0140, the one-character string of the typewriter 0A69:01AD (DS:0141 holds 0).</summary>
    public const int TypewriterChar = 0x0140;

    /// <summary>Retraces the typewriter waits after every character (last argument of 0A69:0288 in 17CB:09E0).</summary>
    public const short TypewriterWait = 5;
}

/// <summary>
/// The week incidents 17CB:0005 (call 9 of the week loop) as a coroutine of the core: ranking 0825:06CB, then the field takeover
/// 17CB:04D7, the money incident 17CB:001E (gain 02BD or loss 039D) and the harvest loss 17CB:0736, each with its window
/// 17CB:0986 and the wait loop 21FD:0004(0) when it happens. Bookings, field changes and every random() draw are done here;
/// the drawing is reported as <see cref="IncidentEvent"/>s. Wherever the original waits (fades, 21FD:0185, page flips, wait
/// loop) the coroutine yields a <see cref="MenuWait"/> and continues in the next frame.
/// </summary>
// [ORIGINAL] 17CB:0005
public sealed class WeekIncidents
{
    private readonly Dgroup _d;
    private readonly OriginalRandom _random;
    private readonly IControlDevices _devices;
    private readonly ITextMemory _texts;
    private readonly List<IncidentEvent> _events = [];
    private WaitLoop? _waitLoop;
    private long _frame;

    /// <param name="state">The core's data segment.</param>
    /// <param name="random">random() 21FD:01BA.</param>
    /// <param name="devices">Control devices of the wait loop 21FD:0004.</param>
    /// <param name="texts">Memory behind the text pointers DS:37CC (read by the typewriter, written by 2295:0001).</param>
    public WeekIncidents(Dgroup state, OriginalRandom random, IControlDevices devices, ITextMemory texts)
    {
        _d = state;
        _random = random;
        _devices = devices;
        _texts = texts;
    }

    /// <summary>Frame stamped on new events (set by the driver).</summary>
    public long Frame
    {
        get => _frame;
        set
        {
            _frame = value;
            if (_waitLoop is not null) _waitLoop.Frame = value;
        }
    }

    /// <summary>True when a wait loop ended through the abort key DS:3D55 (17CB ignores it; the week loop reads the key).</summary>
    public bool Aborted { get; private set; }

    public IReadOnlyList<IncidentEvent> Events
    {
        get
        {
            Merge();
            return _events;
        }
    }

    public List<IncidentEvent> DrainEvents()
    {
        Merge();
        var list = new List<IncidentEvent>(_events);
        _events.Clear();
        return list;
    }

    /// <summary>All random() calls of 17CB itself with frame and argument (the wait loop keeps its own list).</summary>
    public List<(long Frame, short Argument)> RandomCalls { get; } = [];

    /// <summary>The random() calls of the wait loops, in order (21FD:0004 draws only in mode 4, so normally none).</summary>
    public List<(long Frame, short Argument)> WaitLoopRandomCalls { get; } = [];

    /// <summary>[ORIGINAL] 17CB:0005 – ranking, takeover, money incident, harvest loss.</summary>
    public IEnumerable<MenuWait> Run()
    {
        Ranking.Apply(_d); // 0825:06CB
        foreach (var w in Takeover()) yield return w;
        foreach (var w in Money()) yield return w;
        foreach (var w in Blight()) yield return w;
    }

    private int Player => _d.S8(L.ActivePlayer);

    private int Wealth(int p) => _d.S32(Int16Math.U(L.CurrentWealth + 4 * p));

    private byte RankUnique(int p) => _d.U8(Int16Math.U(L.RankUnique + p));

    private byte Rank(int p) => _d.U8(Int16Math.U(L.Rank + p));

    /// <summary>
    /// [ORIGINAL] 17CB:001E – money incident for a random player p = random(3) (DS:37AA). With RankUnique ≥ 2 (behind) it is a
    /// gain of 3/4 of the gap to the player one rank ahead (only with random(9) = 0 if twice p's wealth exceeds his); otherwise
    /// a loss against the player one rank behind (random(rank·5 + 8) = 0 needed if twice his wealth exceeds p's): 3/4 of the
    /// gap for rank 0, else a third, above 3EDh replaced by 3EDh − random(100), at most half the cash. Computers with
    /// DS:2E8E ≥ 3 lose nothing. Only a positive amount opens the window.
    /// </summary>
    private IEnumerable<MenuWait> Money()
    {
        _d.Set(L.ActivePlayer, unchecked((byte)Random(3)));
        int p = Player;
        int ru = RankUnique(p);
        if (ru >= 2)
        {
            int si = Neighbour(ru - 1);
            int gain = LongMath.Div(LongMath.Mul(Int16Math.L((long)Wealth(si) - Wealth(p)), 3), 4);
            if (Int16Math.L((long)Wealth(p) << 1) > Wealth(si) && Random(9) != 0) yield break;
            if (gain <= 0) yield break;
            foreach (var w in Window()) yield return w;
            foreach (var w in Booking(gain, true)) yield return w;
        }
        else
        {
            int si = Neighbour(ru + 1);
            if (Int16Math.L((long)Wealth(si) << 1) > Wealth(p) && Random(Int16Math.S(Rank(p) * 5 + 8)) != 0) yield break;
            int gap = Int16Math.L((long)Wealth(p) - Wealth(si));
            int loss;
            if (Rank(p) == 0) loss = LongMath.Div(LongMath.Mul(gap, 3), 4);
            else
            {
                loss = LongMath.Div(gap, 3);
                if (loss > 0x3ED) loss = (ushort)Int16Math.S(0x3ED - Random(100));
            }
            int half = LongMath.Div(_d.S32(Int16Math.U(L.Cash + 4 * p)), 2);
            if (half < loss) loss = half;
            if (_d.U8(L.IsComputer + p) != 0 && _d.S16(L.ComputerIq) >= 3) yield break;
            if (loss <= 0) yield break;
            foreach (var w in Window()) yield return w;
            foreach (var w in Booking(loss, false)) yield return w;
        }
        foreach (var w in Close()) yield return w;
    }

    /// <summary>17CB:0042/0118: first player si (0…3) with RankUnique = <paramref name="rank"/>, else 4 (word compare).</summary>
    private int Neighbour(int rank)
    {
        int si = 0;
        while (si <= 3 && RankUnique(si) != rank) si++;
        return si;
    }

    /// <summary>
    /// [ORIGINAL] 17CB:02BD, 17CB:039D – gain (02BD) and loss (039D): amount class 17CB:047E, sound 4Eh/4Fh, 21FD:0185(50), emblem of DS:37AA
    /// at x F0h with badge 0/3, 21FD:060D, 21FD:0185(30), the amount into its text line, the text 17CB:09E0 (colour 4/5), then
    /// the booking on DS:3CD2[DS:37AA].
    /// </summary>
    private IEnumerable<MenuWait> Booking(int amount, bool gain)
    {
        int cls = AmountClass(amount);
        int text = _d.S8((gain ? IncidentLayout.GainText : IncidentLayout.LossText) + cls);
        Emit(IncidentEventKind.Sound, 0, gain ? 0x4E : 0x4F);
        foreach (var w in Pause(50)) yield return w;
        Emit(IncidentEventKind.ListClear);
        Emit(IncidentEventKind.Emblem, 0xF0, 0x1B, gain ? 0 : 3, Player);
        foreach (var w in Show()) yield return w;
        foreach (var w in Pause(30)) yield return w;
        int line = _d.S8((gain ? IncidentLayout.GainNumberLine : IncidentLayout.LossNumberLine) + cls);
        Number(amount, Int16Math.S(text + line));
        int lines = _d.S8((gain ? IncidentLayout.GainLines : IncidentLayout.LossLines) + cls);
        foreach (var w in Text(text, lines, gain ? 4 : 5)) yield return w;
        int at = Int16Math.U(L.Cash + Int16Math.S(Player << 2));
        _d.Set(at, Int16Math.L((long)_d.S32(at) + (gain ? amount : -(long)amount)));
    }

    /// <summary>[ORIGINAL] 17CB:047E – amount class: &lt; 200 → 0, &lt; 500 → 1, &lt; 1000 → 2, &lt; 2000 → 3, else 4 (signed).</summary>
    public static int AmountClass(int amount) => amount < 200 ? 0 : amount < 500 ? 1 : amount < 1000 ? 2 : amount < 2000 ? 3 : 4;

    /// <summary>
    /// [ORIGINAL] 17CB:04D7 – field takeover: field rank c[p] = 3 − number of others with no more fields (DS:4BA8, ties count,
    /// loop DS:37AA); v = random(3) must have c ≤ 1, wealth rank 0 and at least 7 fields; w is the last player with c = 3 and
    /// must not have wealth rank 0; if 1.5·fields(w) exceeds fields(v), random(8) = 0 is needed. Then the window, the text, both
    /// emblems (21FD:0185(30) after the first), and a random field of v (random(columns − 1), random(rows − 1) until owned by v and not the HQ) goes to w.
    /// </summary>
    private IEnumerable<MenuWait> Takeover()
    {
        Span<sbyte> c = stackalloc sbyte[4];
        c.Fill(3);
        for (_d.Set(L.ActivePlayer, (sbyte)0); Player < 4; NextPlayer()) { } // 04DE: c[p] = 3
        for (_d.Set(L.ActivePlayer, (sbyte)0); Player < 4; NextPlayer())
        {
            int a = Player;
            for (int si = a + 1; si <= 3; si++)
            {
                byte fa = _d.U8(L.FieldCount + a), fs = _d.U8(L.FieldCount + si);
                if (fa >= fs) c[a]--;
                if (fs >= fa) c[si]--;
            }
        }
        short v = Random(3);
        if (c[v] > 1 || Rank(v) > 0 || _d.U8(L.FieldCount + v) < 7) yield break;
        short w = -1;
        for (_d.Set(L.ActivePlayer, (sbyte)0); Player < 4; NextPlayer())
            if (c[Player] == 3) w = (short)Player;
        if (w == -1 || Rank(w) == 0) yield break;
        short scaled = Int16Math.Div(Int16Math.S(_d.U8(L.FieldCount + w) * 15), 10);
        if (scaled > _d.U8(L.FieldCount + v) && Random(8) != 0) yield break;
        foreach (var x in Window()) yield return x;
        foreach (var x in Text(IncidentLayout.TakeoverText, 7, 5)) yield return x;
        _d.Set(L.ActivePlayer, unchecked((byte)v));
        Emit(IncidentEventKind.Sound, 0, 0x4F);
        foreach (var x in Pause(50)) yield return x;
        Emit(IncidentEventKind.ListClear);
        Emit(IncidentEventKind.Emblem, 0x20, 0x1B, 3, Player);
        foreach (var x in Show()) yield return x;
        foreach (var x in Pause(30)) yield return x;
        _d.Set(L.ActivePlayer, unchecked((byte)w));
        Emit(IncidentEventKind.Sound, 0, 0x4E);
        foreach (var x in Pause(50)) yield return x;
        Emit(IncidentEventKind.ListClear);
        Emit(IncidentEventKind.Emblem, 0xF0, 0x1B, 0, Player);
        foreach (var x in Show()) yield return x;
        // 17CB:06A0 – random field of v (random(n − 1) gives 0…n − 1, 21FD:01BA); like the original the loop does
        // not end if v owns no field outside its HQ (cannot happen with at least 7 fields and one HQ).
        int cell;
        do
        {
            _d.Set(L.LoopX, unchecked((byte)Random(Int16Math.S(_d.S16(L.FieldColumns) - 1))));
            _d.Set(L.LoopY, unchecked((byte)Random(Int16Math.S(_d.S16(L.FieldRows) - 1))));
            cell = Cell();
        }
        while (_d.S8(Int16Math.U(FieldGrid.Owner + cell)) != v || _d.S8(Int16Math.U(FieldGrid.Kind + cell)) == 4);
        FieldGrid.Set(_d, _d.S8(L.LoopX), _d.S8(L.LoopY), w, _d.S8(Int16Math.U(FieldGrid.Kind + cell)));
        foreach (var x in Close()) yield return x;
    }

    private int Cell() => Int16Math.U(Int16Math.S(_d.S8(L.LoopX) * 6) + _d.S8(L.LoopY));

    /// <summary>
    /// [ORIGINAL] 17CB:0736 – harvest loss: shares 22A3:0750; for di = random(3) with wealth rank 0 (random(3) = 0 needed),
    /// 1 (random(6) = 0 needed) or above 1 (never) the first kind si (0…2) of which di holds more than 60 % is struck: window,
    /// emblems of the players holding the kind (x 19h + 3Ah·n, badge 3) and of those without (x F8h − 3Ah·n, badge 0), text,
    /// then every field of the kind (rows outer, columns inner) of owner o is emptied (kind −2) unless random(3 − rank(o)) = 0.
    /// </summary>
    private IEnumerable<MenuWait> Blight()
    {
        Production.Shares(_d); // 22A3:0750
        short di = Random(3);
        byte rank = Rank(di);
        if (rank > 1) yield break;
        if (rank == 1 && Random(6) != 0) yield break;
        if (rank == 0 && Random(3) != 0) yield break;
        for (int si = 0; si <= 2; si++)
        {
            if (Share(di, si) <= 0x3C) continue;
            foreach (var w in Window()) yield return w;
            short x = 0x19;
            for (_d.Set(L.ActivePlayer, (sbyte)0); Player < 4; NextPlayer())
            {
                if (Share(Player, si) == 0) continue;
                foreach (var w in StruckEmblem(x, 0x4F, 3)) yield return w;
                x = Int16Math.S(x + 0x3A);
                foreach (var w in Pause(20)) yield return w;
            }
            x = 0xF8;
            for (_d.Set(L.ActivePlayer, (sbyte)0); Player < 4; NextPlayer())
            {
                if (Share(Player, si) != 0) continue;
                foreach (var w in StruckEmblem(x, 0x4E, 0)) yield return w;
                x = Int16Math.S(x - 0x3A);
                foreach (var w in Pause(20)) yield return w;
            }
            foreach (var w in Text(IncidentLayout.BlightText + 4 * si, 4, 5)) yield return w;
            for (_d.Set(L.LoopY, (sbyte)0); _d.S8(L.LoopY) <= Int16Math.S(_d.S16(L.FieldRows) - 1); Inc(L.LoopY))
            {
                for (_d.Set(L.LoopX, (sbyte)0); _d.S8(L.LoopX) <= Int16Math.S(_d.S16(L.FieldColumns) - 1); Inc(L.LoopX))
                {
                    int cell = Cell();
                    if (_d.S8(Int16Math.U(FieldGrid.Kind + cell)) != si) continue;
                    short owner = _d.S8(Int16Math.U(FieldGrid.Owner + cell));
                    if (Random(Int16Math.S(3 - Rank(owner))) == 0) continue;
                    FieldGrid.Set(_d, _d.S8(L.LoopX), _d.S8(L.LoopY), owner, -2);
                }
            }
            foreach (var w in Close()) yield return w;
            yield break;
        }
    }

    private byte Share(int p, int kind) => _d.U8(Int16Math.U(Int16Math.S(p * 3) + L.ProductionShares + kind));

    /// <summary>17CB:07C3/083C: sound, 21FD:0185(50), list emptied, emblem of DS:37AA at x with badge, 21FD:060D.</summary>
    private IEnumerable<MenuWait> StruckEmblem(short x, int sound, int badge)
    {
        Emit(IncidentEventKind.Sound, 0, sound);
        foreach (var w in Pause(50)) yield return w;
        Emit(IncidentEventKind.ListClear);
        Emit(IncidentEventKind.Emblem, x, 0x1B, badge, Player);
        foreach (var w in Show()) yield return w;
    }

    /// <summary>
    /// [ORIGINAL] 17CB:0986 – the incident window: fade-out to colour 4, list emptied, field picture 0609:1ED9, frame
    /// 0609:1256(0, 14h, 140h, A0h), list executed, page copy 0A96:0C3F(DS:082E), font 1, palette DS:00A8, slow fade-in.
    /// </summary>
    private IEnumerable<MenuWait> Window()
    {
        Emit(IncidentEventKind.FadeOut, 4);
        foreach (var w in Planting.Fade(_d, 4)) yield return MenuWait.Retrace; // 21FD:037D: eight stages, 0185(4)
        Emit(IncidentEventKind.ListClear);
        Emit(IncidentEventKind.FieldBackdrop);
        Emit(IncidentEventKind.Frame, 0, 0x14, 0x140, 0xA0);
        Emit(IncidentEventKind.ListExecute);
        Emit(IncidentEventKind.CopyPage, _d.U16(IncidentLayout.PageCopyArgument));
        Emit(IncidentEventKind.Font, 1);
        Emit(IncidentEventKind.LoadPalette, IncidentLayout.Palette);
        Emit(IncidentEventKind.FadeIn);
        foreach (var w in Planting.Fade(_d, 3)) yield return MenuWait.Retrace; // 21FD:0329: eight stages, 0185(3)
    }

    private (ushort Offset, ushort Segment) TextPointer(int index)
    {
        int at = Int16Math.U(MenuLayout.TextPointers + Int16Math.S(index << 2));
        return (_d.U16(at), _d.U16(Int16Math.U(at + 2)));
    }

    /// <summary>
    /// [ORIGINAL] 2295:0001 – called by 17CB:02BD/039D with (amount, 4, text pointer of line <paramref name="text"/> + 2): the amount
    /// goes into the text from its third character on (<see cref="IncidentText.FormatNumber"/>).
    /// </summary>
    private void Number(int amount, short text)
    {
        Emit(IncidentEventKind.Number, amount, 4, text);
        var (offset, segment) = TextPointer(text);
        IncidentText.FormatNumber(_d, _texts, unchecked((uint)amount), 4, Int16Math.U(offset + 2), segment);
    }

    /// <summary>
    /// [ORIGINAL] 17CB:09E0 – 0A69:0288(0, 86h − 7·lines, 140h, far pointer DS:37CC + 4·<paramref name="first"/>, lines, colour, 5):
    /// the <see cref="IncidentEventKind.Text"/> event, then the typewriter frame by frame.
    /// </summary>
    private IEnumerable<MenuWait> Text(int first, int lines, int colour)
    {
        Emit(IncidentEventKind.Text, first, lines, colour);
        // [ORIGINAL] 0A69:0288 – lines i = 0 … count − 1 (signed), each with 0A69:0243 (centred; positions are the scene's)
        for (short i = 0; i < lines; i++)
        {
            var (offset, segment) = TextPointer(first + i);
            if (offset == 0 && segment == 0) continue; // [ORIGINAL] 0A69:0243 – nothing for a null pointer
            foreach (var w in Typewriter(first + i, i, offset, segment, colour)) yield return w;
        }
    }

    /// <summary>
    /// [ORIGINAL] 0A69:01AD (with wait 5 and final wait −1) – per character i while strlen &gt; i: DS:0140 = character, list emptied,
    /// the character (<see cref="IncidentEventKind.Character"/>), 21FD:060D, a key click 21A7:0054(1, 16h) unless it is a blank,
    /// the pause key 21FD:0245, then 21FD:0185(5).
    /// </summary>
    private IEnumerable<MenuWait> Typewriter(int text, short line, ushort offset, ushort segment, int colour)
    {
        for (ushort i = 0; IncidentText.Length(_texts, offset, segment) > i; i++)
        {
            byte c = _texts.Read(segment, Int16Math.U(offset + i));
            _d.Set(IncidentLayout.TypewriterChar, c);
            Emit(IncidentEventKind.ListClear);
            Emit(IncidentEventKind.Character, text, i, colour, line);
            foreach (var w in Show()) yield return w;
            if (c != (byte)' ') Emit(IncidentEventKind.Sound, 1, 0x16);
            foreach (var _ in Planting.Pause(_d)) yield return MenuWait.Poll; // 21FD:0245
            foreach (var w in Pause(IncidentLayout.TypewriterWait)) yield return w;
        }
    }

    /// <summary>End of every incident: wait loop 21FD:0004(0), then font 0 (21FD:05DB(0)).</summary>
    private IEnumerable<MenuWait> Close()
    {
        _waitLoop = new WaitLoop(_d, _random, _devices, 0) { Frame = _frame };
        foreach (var w in _waitLoop.Run())
        {
            Merge();
            yield return w;
        }
        Merge();
        WaitLoopRandomCalls.AddRange(_waitLoop.RandomCalls);
        Aborted |= _waitLoop.Aborted;
        _waitLoop = null;
        Emit(IncidentEventKind.Font, 0);
    }

    /// <summary>[ORIGINAL] 21FD:060D – list executed, page flip 21FD:051F, list executed.</summary>
    private IEnumerable<MenuWait> Show()
    {
        Emit(IncidentEventKind.ListExecute);
        foreach (var w in Flip()) yield return w;
        Emit(IncidentEventKind.ListExecute);
    }

    /// <summary>[ORIGINAL] 21FD:051F – page flip as in <see cref="MainMenu"/> (keypad plus/minus, one frame per retrace).</summary>
    private IEnumerable<MenuWait> Flip()
    {
        Emit(IncidentEventKind.Flip);
        yield return MenuWait.Flip;
        if (!Key(MenuKeys.KeypadPlus)) yield return MenuWait.Retrace;
        if (!Key(MenuKeys.KeypadMinus)) yield break;
        for (int i = 0; i < 4; i++) yield return MenuWait.Retrace;
    }

    /// <summary>21FD:0185(n) with a positive constant n: the event, then n retraces.</summary>
    private IEnumerable<MenuWait> Pause(short n)
    {
        Emit(IncidentEventKind.Pause, n);
        foreach (var w in Planting.Retraces(n, 0)) yield return MenuWait.Retrace;
    }

    private void NextPlayer() => Inc(L.ActivePlayer);

    private void Inc(int offset) => _d.Set(offset, unchecked((sbyte)(_d.S8(offset) + 1)));

    private short Random(short n)
    {
        RandomCalls.Add((_frame, n));
        return _random.Next(n);
    }

    private bool Key(byte scancode) => _d.U8(L.KeyState + scancode) != 0;

    private void Merge()
    {
        if (_waitLoop is null) return;
        foreach (var e in _waitLoop.DrainEvents())
            _events.Add(new IncidentEvent(IncidentEventKind.WaitLoop, e.Frame, Inner: e));
    }

    private void Emit(IncidentEventKind kind, int a = 0, int b = 0, int c = 0, int d = 0)
    {
        Merge();
        _events.Add(new IncidentEvent(kind, _frame, a, b, c, d));
    }
}

/// <summary>
/// The week incidents as a step of the week loop (<see cref="IWeekStepHandler"/>, call 9 = 17CB:0005): <see cref="Begin"/> runs
/// them to their first wait, every <see cref="Tick"/> one frame further. Bound into the
/// phase machine by Phases.WeekScreens (lane ernte, part 3).
/// </summary>
public sealed class WeekIncidentsStep(ITextMemory texts, IControlDevices? devices = null) : IWeekStepHandler
{
    private IEnumerator<MenuWait>? _run;

    public WeekIncidents? Incidents { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Incidents = new WeekIncidents(context.State, context.Random, devices ?? KeyboardOnlyDevices.Instance, texts);
        _run = Incidents.Run().GetEnumerator();
        return Tick(context);
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null || Incidents is null) throw new InvalidOperationException("Not started.");
        Incidents.Frame = context.Frame;
        return _run.MoveNext() ? StepOutcome.Running : StepOutcome.Finished();
    }
}
