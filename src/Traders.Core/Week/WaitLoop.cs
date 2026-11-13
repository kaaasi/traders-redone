using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;

namespace Traders.Core.Week;

/// <summary>DS offsets of the wait loop 21FD:0004 (docs/routines/21FD_0004_warteschleife.md, docs/analyse/DATENLAYOUT.md).</summary>
public static class WaitLoopLayout
{
    /// <summary>byte[8] – bit masks 01h…80h; entry p is the bit of player p in the loop's mask.</summary>
    public const int PlayerBits = 0x08C6;

    /// <summary>Key table entry of scancode 0Eh (backspace): non-zero ends the wait loop at once (abort path A).</summary>
    public const int AbortKey = DgroupLayout.KeyState + 0x0E;

    /// <summary>byte, signed – countdown of the indicator in 1F63:113E (mode 4).</summary>
    public const int IndicatorCountdown = 0x54B0;

    /// <summary>Countdown value set when the indicator is switched on (1F63:117E).</summary>
    public const byte IndicatorFrames = 8;
}

/// <summary>
/// The common wait loop 21FD:0004(mode) of the week screens as a coroutine of the core: every player confirms once – humans
/// with fire (action 0Ah from 0E9E:037A), computers by themselves when the countdown reaches 20·player – and the loop ends when
/// all four have confirmed. Mode 1 (planting, 0609:00D6) shows the player's object, mode 2 (week report, 0825:000A) draws the
/// player's mark with a page flip, mode 4 runs the indicator 1F63:113E with its random() draw. Drawing is reported as
/// <see cref="WeekEvent"/>s; wherever the original waits (page flip, pause key) the coroutine yields a <see cref="MenuWait"/>
/// and continues in the next frame, like <see cref="MainMenu"/>. Keys are read from the key table DS:3D47.
/// </summary>
// [ORIGINAL] 21FD:0004
public sealed class WaitLoop
{
    private readonly Dgroup _d;
    private readonly OriginalRandom _random;
    private readonly IControlDevices _devices;
    private readonly int _mode;
    private readonly List<WeekEvent> _events = [];
    private byte _mask; // [bp-1]: players still to confirm

    public WaitLoop(Dgroup state, OriginalRandom random, IControlDevices devices, int mode)
    {
        _d = state;
        _random = random;
        _devices = devices;
        _mode = mode;
    }

    /// <summary>Frame stamped on new events (set by the driver).</summary>
    public long Frame { get; set; }

    /// <summary>True when the loop ended through the abort key DS:3D55 instead of all confirmations.</summary>
    public bool Aborted { get; private set; }

    /// <summary>Players in the order they confirmed.</summary>
    public List<int> Confirmations { get; } = [];

    public IReadOnlyList<WeekEvent> Events => _events;

    public List<WeekEvent> DrainEvents()
    {
        var list = new List<WeekEvent>(_events);
        _events.Clear();
        return list;
    }

    /// <summary>All random() calls so far with frame and argument (diagnostics for differential tests).</summary>
    public List<(long Frame, short Argument)> RandomCalls { get; } = [];

    /// <summary>[ORIGINAL] 21FD:0004 – the whole loop up to its return.</summary>
    public IEnumerable<MenuWait> Run()
    {
        _mask = 0x0F;
        Emit(WeekEventKind.ListClear);
        short countdown = (short)(_d.U8(MenuLayout.AllComputers) == 1 ? 150 : 100);
        while (_mask != 0) // 21FD:016E
        {
            foreach (var w in Flip()) yield return w;
            Emit(WeekEventKind.RestoreObjects);
            Emit(WeekEventKind.ListExecute);
            Emit(WeekEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            if (countdown > 0) countdown--;
            if (_d.U8(WaitLoopLayout.AbortKey) != 0)
            {
                Aborted = true;
                yield break;
            }
            if (_mode == 4) Indicator();
            ControlReader.ReadActions(_d, _devices);
            ComputerActions(countdown);
            foreach (var w in Confirm()) yield return w;
            Emit(WeekEventKind.ListExecute);
            Emit(WeekEventKind.DrawObjects);
        }
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0004 (loop at 0067) – loop over DS:37AA: a computer (DS:0840 = 1) gets action 0Ah (fire) when the countdown equals
    /// 20·player, otherwise FFh; humans keep the action of 0E9E:037A.
    /// </summary>
    private void ComputerActions(short countdown)
    {
        for (_d.Set(DgroupLayout.ActivePlayer, (sbyte)0); _d.S8(DgroupLayout.ActivePlayer) < 4; Next())
        {
            int p = _d.S8(DgroupLayout.ActivePlayer);
            if (_d.U8(DgroupLayout.IsComputer + p) != 1) continue;
            byte action = (short)(p * 0x14) == countdown ? (byte)ControlAction.Fire : unchecked((byte)ControlAction.None);
            _d.Set(ControlReader.Action + p, action);
        }
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0004 (loop at 00A5) – loop over DS:37AA: a player still in the mask whose action is 0Ah leaves the mask, gets the sound
    /// of the jump table 21FD:017D and the confirmation of the mode (1: 0609:1FCC, 2: 0825:0686).
    /// </summary>
    private IEnumerable<MenuWait> Confirm()
    {
        for (_d.Set(DgroupLayout.ActivePlayer, (sbyte)0); _d.S8(DgroupLayout.ActivePlayer) < 4; Next())
        {
            int p = _d.S8(DgroupLayout.ActivePlayer);
            byte bit = _d.U8(WaitLoopLayout.PlayerBits + p);
            if ((bit & _mask) == 0 || _d.U8(ControlReader.Action + p) != (byte)ControlAction.Fire) continue;
            _mask &= (byte)(0xFF - bit);
            Confirmations.Add(p);
            switch (p) // 21FD:00E3, table at 21FD:017D
            {
                case 0: Emit(WeekEventKind.Sound, 0, 3); break;
                case 1: Emit(WeekEventKind.Sound, 1, 4); break;
                case 2: Emit(WeekEventKind.Sound, 2, 5); break;
                case 3: Emit(WeekEventKind.Sound, 0, 6); break;
            }
            if (_mode == 1) Emit(WeekEventKind.ShowPlayerObject, p); // 0609:1FCC
            if (_mode == 2)
                foreach (var w in ReportMark(p)) yield return w;
        }
    }

    private void Next() => _d.Set(DgroupLayout.ActivePlayer, (sbyte)(_d.S8(DgroupLayout.ActivePlayer) + 1));

    /// <summary>
    /// [ORIGINAL] 0825:0686 – mark of <paramref name="player"/> in its rank row (drawing only), then 21FD:060D: list, page flip,
    /// list.
    /// </summary>
    private IEnumerable<MenuWait> ReportMark(int player)
    {
        Emit(WeekEventKind.ReportMark, player, _d.U8(DgroupLayout.RankUnique + player));
        Emit(WeekEventKind.ListExecute); // [ORIGINAL] 21FD:060D
        foreach (var w in Flip()) yield return w;
        Emit(WeekEventKind.ListExecute);
    }

    /// <summary>
    /// [ORIGINAL] 1F63:113E – indicator: a positive countdown DS:54B0 is decremented and switches the indicator off (state 1)
    /// when it reaches 0; otherwise random(200) = 0 switches it on (state 0) and sets the countdown to 8.
    /// </summary>
    private void Indicator()
    {
        sbyte n = _d.S8(WaitLoopLayout.IndicatorCountdown);
        if (n > 0)
        {
            n--;
            _d.Set(WaitLoopLayout.IndicatorCountdown, n);
            if (n == 0) Emit(WeekEventKind.Indicator, 3, 1); // [ORIGINAL] 1F63:104B
            return;
        }
        if (Random(200) != 0) return;
        Emit(WeekEventKind.Indicator, 3, 0);
        _d.Set(WaitLoopLayout.IndicatorCountdown, WaitLoopLayout.IndicatorFrames);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:051F – page flip as in <see cref="MainMenu"/>: the show-page retrace, one more retrace unless keypad plus
    /// is held (DS:3D95), four more with keypad minus held (DS:3D91); each retrace is one frame.
    /// </summary>
    private IEnumerable<MenuWait> Flip()
    {
        Emit(WeekEventKind.Flip);
        yield return MenuWait.Flip;
        if (!Key(MenuKeys.KeypadPlus)) yield return MenuWait.Retrace;
        if (!Key(MenuKeys.KeypadMinus)) yield break;
        for (int i = 0; i < 4; i++) yield return MenuWait.Retrace;
    }

    /// <summary>[ORIGINAL] 21FD:0245 – P (DS:3D60) pauses: wait for release, for the next press and for its release.</summary>
    private IEnumerable<MenuWait> Pause()
    {
        if (!Key(MenuKeys.P)) yield break;
        while (Key(MenuKeys.P)) yield return MenuWait.Poll;
        while (!Key(MenuKeys.P)) yield return MenuWait.Poll;
        while (Key(MenuKeys.P)) yield return MenuWait.Poll;
    }

    private short Random(short n)
    {
        RandomCalls.Add((Frame, n));
        return _random.Next(n);
    }

    private bool Key(byte scancode) => _d.U8(DgroupLayout.KeyState + scancode) != 0;

    private void Emit(WeekEventKind kind, int a = 0, int b = 0) => _events.Add(new WeekEvent(kind, Frame, a, b));
}
