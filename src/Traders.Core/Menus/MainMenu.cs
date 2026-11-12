using Traders.Core.State;

namespace Traders.Core.Menus;

/// <summary>
/// The main menu 192D:0007 with all sub-menus as a coroutine of the core (docs/routines/192D_0007_menues.md). The code
/// follows the original call by call: option changes and every random() call happen here, drawing is reported as
/// <see cref="MenuEvent"/>s for the scene. Wherever the original waits – page flip, key wait loop, fade – the coroutine
/// yields a <see cref="MenuWait"/> and continues in the next frame; keys are read from the key table DS:3D47 like in the
/// original (written per frame by the runner). <see cref="Result"/> is AL of 192D:0007: 0 = quit, 1 = play.
/// </summary>
// [ORIGINAL] 192D:0007
public sealed class MainMenu
{
    private readonly Dgroup _d;
    private readonly OriginalRandom _random;
    private readonly IMenuHost _host;
    private readonly List<MenuEvent> _events = [];

    public MainMenu(Dgroup state, OriginalRandom random, IMenuHost host)
    {
        _d = state;
        _random = random;
        _host = host;
    }

    /// <summary>Frame stamped on new events (set by the runner).</summary>
    public long Frame { get; set; }

    /// <summary>AL returned by 192D:0007 once the coroutine has ended.</summary>
    public byte? Result { get; private set; }

    public IReadOnlyList<MenuEvent> Events => _events;

    public List<MenuEvent> DrainEvents()
    {
        var list = new List<MenuEvent>(_events);
        _events.Clear();
        return list;
    }

    // Retrace waits of the fades (docs/routines/21FD_026C_paletten.md): eight stages, each one wait in 2295:00C5 and then
    // 0185(n) waits – fade-out 03B4 n = 2, fade-in 0353 n = 1, slow fade-in 0329 n = 3.
    private const int FadeOutRetraces = 8 * 3, FadeInRetraces = 8 * 2, SlowFadeInRetraces = 8 * 4;

    /// <summary>[ORIGINAL] 192D:0007 – the whole main menu up to its return.</summary>
    public IEnumerable<MenuWait> Run()
    {
        Emit(MenuEventKind.LoadPicture, _d.U16(MenuLayout.PictureResource));
        CountComputers();
        _d.Set(MenuLayout.EditColumns, _d.U16(DgroupLayout.FieldColumns)); // 192D:0092
        _d.Set(MenuLayout.EditRows, _d.U16(DgroupLayout.FieldRows));
        if (_d.S16(DgroupLayout.Level) != -1) ApplyLevel();
        // 192D:00B4
        foreach (var w in DrawMainMenu()) yield return w;
        byte al = 0;
        foreach (var w in MainLoop(r => al = r)) yield return w;
        if (al == 0)
        {
            Result = 0;
            yield break;
        }
        foreach (var w in Controls()) yield return w;
        foreach (var w in ContinueDialog()) yield return w;
        if (_d.U8(MenuLayout.Choice) == 1) // 192D:00A3
        {
            _d.Set(DgroupLayout.FieldColumns, _d.U16(MenuLayout.EditColumns));
            _d.Set(DgroupLayout.FieldRows, _d.U16(MenuLayout.EditRows));
        }
        FieldOrigin(_d);
        Result = 1;
    }

    /// <summary>
    /// [ORIGINAL] 192D:0058 – origin of the playing field: x = (320 − columns·32) / 2, y = ((146 − rows·22) − 14) / 2 + 58, in
    /// 16-bit arithmetic with division toward zero.
    /// </summary>
    public static void FieldOrigin(Dgroup d)
    {
        short columns = d.S16(DgroupLayout.FieldColumns), rows = d.S16(DgroupLayout.FieldRows);
        d.Set(MenuLayout.FieldOriginX, Int16Math.Div(Int16Math.S(0x140 - Int16Math.S(columns << 5)), 2));
        short y = Int16Math.S(Int16Math.S(0x92 - Int16Math.S(rows * 0x16)) - 0x0E);
        d.Set(MenuLayout.FieldOriginY, Int16Math.S(Int16Math.Div(y, 2) + 0x3A));
    }

    /// <summary>[ORIGINAL] 192D:03E3 – DS:4581 = 1 when all four players are computers.</summary>
    private void CountComputers()
    {
        int n = 0;
        for (int i = 0; i < DgroupLayout.PlayerCount; i++)
            if (_d.U8(DgroupLayout.IsComputer + i) == 1) n++;
        _d.Set(MenuLayout.AllComputers, (byte)(n == 4 ? 1 : 0));
    }

    /// <summary>[ORIGINAL] 192D:0592 – copies the options of level DS:08EA from the level table DS:0B90….</summary>
    public static void ApplyLevel(Dgroup d)
    {
        int level = d.S16(DgroupLayout.Level);
        d.Set(DgroupLayout.ComputerIq, d.U16(Int16Math.U(MenuLayout.LevelIq + 2 * level)));
        d.Set(DgroupLayout.OilConsumption, d.U8(Int16Math.U(MenuLayout.LevelOil + level)));
        d.Set(DgroupLayout.RobotBattle, d.U8(Int16Math.U(MenuLayout.LevelRobot + level)));
        d.Set(DgroupLayout.GemTrading, d.U8(Int16Math.U(MenuLayout.LevelGems + level)));
        d.Set(DgroupLayout.Catastrophes, d.U8(Int16Math.U(MenuLayout.LevelCatastrophes + level)));
        d.Set(DgroupLayout.PiratePresence, d.U16(Int16Math.U(MenuLayout.LevelPirates + 2 * level)));
    }

    private void ApplyLevel() => ApplyLevel(_d);

    // ------------------------------------------------------------------ main menu

    /// <summary>[ORIGINAL] 192D:00C5 – main menu screen: plaque, menu lines, the four player labels (loop DS:37AA), fade-in.</summary>
    private IEnumerable<MenuWait> DrawMainMenu()
    {
        foreach (var w in Panel(0x00C5, 0xBB)) yield return w;
        _d.Set(DgroupLayout.ActivePlayer, (sbyte)4); // loop variable of the label loop is the global DS:37AA
        foreach (var w in FadeIn(slow: false)) yield return w;
    }

    /// <summary>[ORIGINAL] 192D:0234 – main menu loop: F1–F4 human/computer, F6 level, F7 settings, F8 disk, F9 info, Q, RETURN.</summary>
    private IEnumerable<MenuWait> MainLoop(Action<byte> result)
    {
        byte exit = 0;
        Emit(MenuEventKind.ListClear);
        while (exit == 0)
        {
            foreach (var w in Flip()) yield return w;
            Emit(MenuEventKind.ListExecute);
            Emit(MenuEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            for (int p = 0; p < 4; p++)
            {
                byte key = (byte)(MenuKeys.F1 + p);
                if (!Key(key)) continue;
                TogglePlayer(p);
                foreach (var w in Release(key)) yield return w;
            }
            if (Key(MenuKeys.F6))
            {
                foreach (var w in LevelMenu()) yield return w;
                foreach (var w in Release(MenuKeys.F6)) yield return w;
            }
            if (Key(MenuKeys.F7))
            {
                foreach (var w in SettingsMenu()) yield return w;
                foreach (var w in Release(MenuKeys.F7)) yield return w;
            }
            if (Key(MenuKeys.F8))
            {
                foreach (var w in DiskMenu()) yield return w;
                foreach (var w in Release(MenuKeys.F8)) yield return w;
            }
            if (Key(MenuKeys.F9))
            {
                foreach (var w in InfoScreen()) yield return w;
                foreach (var w in Release(MenuKeys.F9)) yield return w;
            }
            if (Key(MenuKeys.Q))
            {
                exit = 0x63;
                foreach (var w in Release(MenuKeys.Q)) yield return w;
            }
            if (Key(MenuKeys.Enter))
            {
                exit = 1;
                foreach (var w in Release(MenuKeys.Enter)) yield return w;
            }
            Emit(MenuEventKind.ListExecute);
            Random(1); // 192D:0345, also in the last pass
        }
        Sound(0, 1);
        result(exit == 0x63 ? (byte)0 : (byte)1);
    }

    /// <summary>[ORIGINAL] 192D:0376(p) – toggles human/computer of player p, redraws the label, sound, 03E3.</summary>
    private void TogglePlayer(int p)
    {
        _d.Set(DgroupLayout.IsComputer + p, (byte)(_d.U8(DgroupLayout.IsComputer + p) == 0 ? 1 : 0));
        Emit(MenuEventKind.Draw, 0x03AB, p);
        Sound(0, 2);
        CountComputers();
    }

    // ------------------------------------------------------------------ level menu

    /// <summary>[ORIGINAL] 192D:040E – level menu (042F, 0495), then the main menu screen again.</summary>
    private IEnumerable<MenuWait> LevelMenu()
    {
        Sound(0, 2);
        foreach (var w in Panel(0x042F, 0xA2)) yield return w;
        foreach (var w in FadeIn(slow: false)) yield return w;
        foreach (var w in LevelLoop()) yield return w;
        foreach (var w in DrawMainMenu()) yield return w;
    }

    /// <summary>Drawing routines of the level menu lines: level name, then IQ, oil, robots, gems, catastrophes, pirates.</summary>
    private static readonly int[] LevelItems = [0x076C, 0x07AC, 0x07FC, 0x084A, 0x0899, 0x08E7, 0x0935];

    /// <summary>[ORIGINAL] 192D:0495 – F5–F10 change single options when the level is user defined, F1 next level, RETURN.</summary>
    private IEnumerable<MenuWait> LevelLoop()
    {
        bool done = false;
        Emit(MenuEventKind.ListClear);
        while (!done)
        {
            foreach (var w in Flip()) yield return w;
            Emit(MenuEventKind.ListExecute);
            Emit(MenuEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            if (_d.S16(DgroupLayout.Level) == -1)
            {
                for (int k = 0; k < 6; k++)
                {
                    byte key = (byte)(MenuKeys.F5 + k);
                    if (!Key(key)) continue;
                    ChangeCustomOption(k);
                    foreach (var w in Release(key)) yield return w;
                }
            }
            if (Key(MenuKeys.F1))
            {
                NextLevel();
                foreach (var w in Release(MenuKeys.F1)) yield return w;
            }
            if (Key(MenuKeys.Enter))
            {
                done = true;
                foreach (var w in Release(MenuKeys.Enter)) yield return w;
            }
            Emit(MenuEventKind.ListExecute);
            Random(1); // 192D:0571
        }
        Sound(0, 1);
    }

    /// <summary>[ORIGINAL] 192D:0698 / 06C2 / 06E2 / 0702 / 0722 / 0742 – one user-defined option of the level menu.</summary>
    private void ChangeCustomOption(int k)
    {
        switch (k)
        {
            case 0: // 0698: IQ 0..3
            {
                short v = Int16Math.S(_d.S16(DgroupLayout.ComputerIq) + 1);
                _d.Set(DgroupLayout.ComputerIq, v >= 4 ? (short)0 : v);
                break;
            }
            case 1: _d.Set(DgroupLayout.OilConsumption, (byte)(_d.U8(DgroupLayout.OilConsumption) ^ 1)); break;
            case 2: _d.Set(DgroupLayout.RobotBattle, (byte)(_d.U8(DgroupLayout.RobotBattle) ^ 1)); break;
            case 3: _d.Set(DgroupLayout.GemTrading, (byte)(_d.U8(DgroupLayout.GemTrading) ^ 1)); break;
            case 4: _d.Set(DgroupLayout.Catastrophes, (byte)(_d.U8(DgroupLayout.Catastrophes) ^ 1)); break;
            default: // 0742: pirates 0..3
            {
                short v = Int16Math.S(_d.S16(DgroupLayout.PiratePresence) + 1);
                _d.Set(DgroupLayout.PiratePresence, v > 3 ? (short)0 : v);
                break;
            }
        }
        Emit(MenuEventKind.Draw, LevelItems[k + 1]);
        Sound(0, 2);
    }

    /// <summary>[ORIGINAL] 192D:05DD – next level 0…3, after 3 user defined (−1); a fixed level copies its options (0592).</summary>
    private void NextLevel()
    {
        short level = Int16Math.S(_d.S16(DgroupLayout.Level) + 1);
        _d.Set(DgroupLayout.Level, level);
        if (level >= 4)
        {
            _d.Set(DgroupLayout.Level, (short)-1);
            Emit(MenuEventKind.Draw, 0x0653);
        }
        else
        {
            if (level == 0) Emit(MenuEventKind.Draw, 0x05DD); // clears the key help of the user-defined level
            ApplyLevel();
            for (int i = 1; i < LevelItems.Length; i++) Emit(MenuEventKind.Draw, LevelItems[i]);
        }
        Emit(MenuEventKind.Draw, 0x076C);
        Sound(0, 2);
    }

    // ------------------------------------------------------------------ settings menu

    /// <summary>[ORIGINAL] 192D:0A05 – settings menu (0A26, 0AB4), then the main menu screen again.</summary>
    private IEnumerable<MenuWait> SettingsMenu()
    {
        Sound(0, 2);
        foreach (var w in Panel(0x0A26, 0x8E)) yield return w;
        foreach (var w in FadeIn(slow: false)) yield return w;
        foreach (var w in SettingsLoop()) yield return w;
        foreach (var w in DrawMainMenu()) yield return w;
    }

    /// <summary>[ORIGINAL] 192D:0AB4 – F1–F6 options, cursor keys field size, S sound, RETURN; no random() per pass.</summary>
    private IEnumerable<MenuWait> SettingsLoop()
    {
        bool done = false;
        Emit(MenuEventKind.ListClear);
        while (!done)
        {
            foreach (var w in Flip()) yield return w;
            Emit(MenuEventKind.ListExecute);
            Emit(MenuEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            for (int k = 0; k < 6; k++)
            {
                byte key = (byte)(MenuKeys.F1 + k);
                if (!Key(key)) continue;
                ChangeSetting(k);
                foreach (var w in Release(key)) yield return w;
            }
            (byte Key, bool Rows, short Delta)[] cursor =
                [(MenuKeys.Left, false, -1), (MenuKeys.Right, false, 1), (MenuKeys.Up, true, -1), (MenuKeys.Down, true, 1)];
            foreach (var (key, rows, delta) in cursor)
            {
                if (!Key(key)) continue;
                ChangeFieldSize(rows, delta);
                foreach (var w in Release(key)) yield return w;
            }
            if (Key(MenuKeys.S))
            {
                // 0D74: sound = (sound == 0) ? 1 : 0
                _d.Set(DgroupLayout.Sound, (short)(_d.S16(DgroupLayout.Sound) == 0 ? 1 : 0));
                Emit(MenuEventKind.Draw, 0x0F6E); // 0D74 redraws the sound line with 0F6E
                Sound(0, 2);
                foreach (var w in Release(MenuKeys.S)) yield return w;
            }
            if (Key(MenuKeys.Enter))
            {
                done = true;
                foreach (var w in Release(MenuKeys.Enter)) yield return w;
            }
            Emit(MenuEventKind.ListExecute); // 192D:0BE8
        }
        Sound(0, 1);
    }

    /// <summary>[ORIGINAL] 192D:0C08 / 0C34 / 0C75 / 0C9E / 0CC7 / 0CE7 – one option of the settings menu.</summary>
    private void ChangeSetting(int k)
    {
        int routine;
        switch (k)
        {
            case 0: // 0C08: initial cash +500, over 2000 (signed) → 500
            {
                short v = Int16Math.S(_d.S16(DgroupLayout.InitialCash) + 0x1F4);
                _d.Set(DgroupLayout.InitialCash, v > 0x7D0 ? (short)0x1F4 : v);
                routine = 0x0DC0;
                break;
            }
            case 1: // 0C34: target +10000 (32 bit), over 50000 (unsigned) → 10000
            {
                uint v = unchecked(_d.U32(DgroupLayout.Target) + 0x2710);
                _d.Set(DgroupLayout.Target, v > 0xC350 ? 0x2710u : v);
                routine = 0x0E11;
                break;
            }
            case 2: // 0C75: lotto 0..2 (signed byte)
            {
                sbyte v = unchecked((sbyte)(_d.S8(DgroupLayout.LottoReturns) + 1));
                _d.Set(DgroupLayout.LottoReturns, v > 2 ? (sbyte)0 : v);
                routine = 0x0E64;
                break;
            }
            case 3: // 0C9E: time limit +10, over 30 (unsigned byte) → 10
            {
                byte v = unchecked((byte)(_d.U8(DgroupLayout.TimeLimit) + 0x0A));
                _d.Set(DgroupLayout.TimeLimit, v > 0x1E ? (byte)0x0A : v);
                routine = 0x0E8E;
                break;
            }
            case 4: // 0CC7: land type random/real
                _d.Set(DgroupLayout.LandType, (byte)(_d.U8(DgroupLayout.LandType) ^ 1));
                routine = 0x0EAE;
                break;
            default: // 0CE7: robot cycle 1..4 (signed byte)
            {
                sbyte v = unchecked((sbyte)(_d.S8(DgroupLayout.RobotCycle) + 1));
                _d.Set(DgroupLayout.RobotCycle, v > 4 ? (sbyte)1 : v);
                routine = 0x0ED9;
                break;
            }
        }
        Emit(MenuEventKind.Draw, routine);
        Sound(0, 2);
    }

    /// <summary>[ORIGINAL] 192D:0D10 / 0D42 (delta) – columns 4…9 resp. rows 3…6 in the edit copies; outside nothing happens.</summary>
    private void ChangeFieldSize(bool rows, short delta)
    {
        int at = rows ? MenuLayout.EditRows : MenuLayout.EditColumns;
        short v = Int16Math.S(_d.S16(at) + delta);
        if (rows ? v > 6 || v < 3 : v > 9 || v < 4) return;
        _d.Set(at, v);
        Emit(MenuEventKind.Draw, rows ? 0x0F16 : 0x0EF8);
        Sound(0, 2);
    }

    // ------------------------------------------------------------------ disk menu

    /// <summary>[ORIGINAL] 192D:0F96 – disk menu (0FB7, 10F8), then the main menu screen again.</summary>
    private IEnumerable<MenuWait> DiskMenu()
    {
        Sound(0, 2);
        // 0FB7: fixed plaque position 1Bh, the wall and two plaques
        _d.Set(MenuLayout.PlaqueTop, (short)0x1B);
        foreach (var w in FadeOut()) yield return w;
        short top = _d.S16(MenuLayout.PlaqueTop);
        var ops = MenuBackdrop.Wall(Random);
        ops.AddRange(MenuBackdrop.Plaque(Random, 0x3C, top, 0xC8, 0x57));
        ops.AddRange(MenuBackdrop.Plaque(Random, 0x3C, Int16Math.S(top + top + 0x57), 0xC8, 0x20));
        Emit(MenuEventKind.Screen, 0x0FB7, ops: ops);
        foreach (var w in FadeIn(slow: false)) yield return w;
        foreach (var w in DiskLoop()) yield return w;
        foreach (var w in DrawMainMenu()) yield return w;
    }

    /// <summary>[ORIGINAL] 192D:10F8 – L load, S save, RETURN; no random() per pass.</summary>
    private IEnumerable<MenuWait> DiskLoop()
    {
        bool done = false;
        Emit(MenuEventKind.ListClear);
        while (!done)
        {
            foreach (var w in Flip()) yield return w;
            Emit(MenuEventKind.ListExecute);
            Emit(MenuEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            if (Key(MenuKeys.L))
            {
                // 1177
                Sound(0, 2);
                if (_host.LoadGame(_d))
                {
                    Emit(MenuEventKind.DiskMessage, 0x3A88);
                    _d.Set(MenuLayout.GameInProgress, (byte)1);
                }
                else
                {
                    Emit(MenuEventKind.DiskMessage, 0x3A8C);
                }
                foreach (var w in Release(MenuKeys.L)) yield return w;
            }
            if (Key(MenuKeys.S))
            {
                // 11B8
                Sound(0, 2);
                if (_d.U8(MenuLayout.GameInProgress) == 0) Emit(MenuEventKind.DiskMessage, 0x3A90);
                else Emit(MenuEventKind.DiskMessage, _host.SaveGame(_d) ? 0x3A94 : 0x3A98);
                foreach (var w in Release(MenuKeys.S)) yield return w;
            }
            if (Key(MenuKeys.Enter))
            {
                done = true;
                foreach (var w in Release(MenuKeys.Enter)) yield return w;
            }
            Emit(MenuEventKind.ListExecute); // 192D:115A
        }
        Sound(0, 1);
    }

    // ------------------------------------------------------------------ info screen, continue dialog

    /// <summary>[ORIGINAL] 192D:1263 – info screen until RETURN is pressed and released, then the main menu screen.</summary>
    private IEnumerable<MenuWait> InfoScreen()
    {
        foreach (var w in Panel(0x1263, 0x8C)) yield return w;
        foreach (var w in FadeIn(slow: false)) yield return w;
        while (!Key(MenuKeys.Enter)) yield return MenuWait.Poll;
        while (Key(MenuKeys.Enter)) yield return MenuWait.Poll;
        foreach (var w in DrawMainMenu()) yield return w;
    }

    /// <summary>
    /// [ORIGINAL] 192D:12A6 – without a game in progress DS:3CCC = 1; otherwise a dialog: N → 1 (new game), A, O or V → 2
    /// (continue). The key loop has no page flip.
    /// </summary>
    private IEnumerable<MenuWait> ContinueDialog()
    {
        if (_d.U8(MenuLayout.GameInProgress) == 0)
        {
            _d.Set(MenuLayout.Choice, (byte)1);
            yield break;
        }
        foreach (var w in Panel(0x12A6, 0x64)) yield return w;
        foreach (var w in FadeIn(slow: true)) yield return w;
        while (true)
        {
            if (Key(MenuKeys.N))
            {
                _d.Set(MenuLayout.Choice, (byte)1);
                break;
            }
            if (Key(MenuKeys.A) || Key(MenuKeys.O) || Key(MenuKeys.V))
            {
                _d.Set(MenuLayout.Choice, (byte)2);
                break;
            }
            yield return MenuWait.Poll;
        }
        Sound(0, 1);
    }

    // ------------------------------------------------------------------ controls

    /// <summary>[ORIGINAL] 192D:197F – controls menu unless all players are computers; repeated while a calibration fails.</summary>
    private IEnumerable<MenuWait> Controls()
    {
        CountComputers();
        if (_d.U8(MenuLayout.AllComputers) == 1) yield break;
        while (true)
        {
            // 19A4
            foreach (var w in Panel(0x19A4, 0x64)) yield return w;
            _d.Set(DgroupLayout.ActivePlayer, (sbyte)4); // label loop variable DS:37AA
            foreach (var w in FadeIn(slow: false)) yield return w;
            foreach (var w in ControlsLoop()) yield return w;
            bool ok = false;
            foreach (var w in Calibrate(r => ok = r)) yield return w;
            if (ok) yield break;
        }
    }

    /// <summary>[ORIGINAL] 192D:1A04 – F1–F4 next control device of a human player (1ACE), RETURN; random(1) per pass.</summary>
    private IEnumerable<MenuWait> ControlsLoop()
    {
        bool done = false;
        Emit(MenuEventKind.ListClear);
        while (!done)
        {
            foreach (var w in Flip()) yield return w;
            Emit(MenuEventKind.ListExecute);
            Emit(MenuEventKind.ListClear);
            foreach (var w in Pause()) yield return w;
            for (int p = 0; p < 4; p++)
            {
                byte key = (byte)(MenuKeys.F1 + p);
                if (!Key(key)) continue;
                if (_d.U8(DgroupLayout.IsComputer + p) != 1) // 1ACE
                {
                    sbyte v = unchecked((sbyte)(_d.S8(DgroupLayout.ControlDevice + p) + 1));
                    _d.Set(DgroupLayout.ControlDevice + p, v > 5 ? (sbyte)0 : v);
                    Emit(MenuEventKind.Draw, 0x1B06, p);
                    Sound(0, 2);
                }
                foreach (var w in Release(key)) yield return w;
            }
            if (Key(MenuKeys.Enter))
            {
                done = true;
                foreach (var w in Release(MenuKeys.Enter)) yield return w;
            }
            Emit(MenuEventKind.ListExecute);
            Random(1); // 192D:1AAD
        }
        Sound(0, 1);
    }

    /// <summary>
    /// [ORIGINAL] 192D:1B90 – for every human player with joystick 1 or 2 (device 0 → stick 1, device 1 → stick 2) the three
    /// calibration steps of 27C6:00E6; result false as soon as one step fails.
    /// </summary>
    private IEnumerable<MenuWait> Calibrate(Action<bool> result)
    {
        for (int p = 0; p < 4; p++)
        {
            if (_d.U8(DgroupLayout.IsComputer + p) == 1) continue;
            int stick = _d.U8(DgroupLayout.ControlDevice + p) switch { 0 => 1, 1 => 2, _ => 0 };
            if (stick == 0) continue;
            foreach (var w in Panel(0x1B90, 0x64, stick)) yield return w;
            foreach (var w in FadeIn(slow: false)) yield return w;
            for (int step = 0; step < 3; step++)
            {
                if (step > 0) Emit(MenuEventKind.Draw, 0x1B90, 0x10 + step); // next prompt line
                foreach (var w in Flip()) yield return w;
                bool failed = _host.CalibrationFailed(stick, step);
                Emit(MenuEventKind.Calibration, stick, step, failed ? 1 : 0);
                if (failed)
                {
                    result(false);
                    yield break;
                }
                Sound(0, 2);
            }
        }
        result(true);
    }

    // ------------------------------------------------------------------ shared pieces

    /// <summary>
    /// [ORIGINAL] 192D:16AF(h, step, texts, extra) – plaque top DS:5334 = (200 − h) / 2, fade-out to 420h, the wall (131B),
    /// the plaque 14FB(37h, top, D2h, h); the texts and the rest of the screen routine are drawn by the scene.
    /// </summary>
    private IEnumerable<MenuWait> Panel(int screen, short height, int argument = 0)
    {
        short top = Int16Math.Div(Int16Math.S(0xC8 - height), 2);
        _d.Set(MenuLayout.PlaqueTop, top);
        foreach (var w in FadeOut()) yield return w;
        var ops = MenuBackdrop.Wall(Random);
        ops.AddRange(MenuBackdrop.Plaque(Random, 0x37, top, 0xD2, height));
        Emit(MenuEventKind.Screen, screen, argument, ops: ops);
    }

    /// <summary>21FD:03B4(420h): fade-out with its retrace waits.</summary>
    private IEnumerable<MenuWait> FadeOut()
    {
        Emit(MenuEventKind.FadeOut, 0x420);
        for (int i = 0; i < FadeOutRetraces; i++) yield return MenuWait.Retrace;
    }

    /// <summary>21FD:0353 resp. 0329: fade-in with its retrace waits.</summary>
    private IEnumerable<MenuWait> FadeIn(bool slow)
    {
        Emit(MenuEventKind.FadeIn, slow ? 0x0329 : 0x0353);
        int n = slow ? SlowFadeInRetraces : FadeInRetraces;
        for (int i = 0; i < n; i++) yield return MenuWait.Retrace;
    }

    /// <summary>
    /// [ORIGINAL] 21FD:051F – page flip: the new page is shown with a retrace wait (0B67:0196/01B5), then one more retrace
    /// (0B67:028F) unless keypad plus is held (DS:3D95), and four more with keypad minus held (DS:3D91, 21FD:0185(4));
    /// each key is read just before its wait. Each retrace is one frame.
    /// </summary>
    private IEnumerable<MenuWait> Flip()
    {
        Emit(MenuEventKind.Flip);
        yield return MenuWait.Flip;
        // DS:3D95 is read after the show-page retrace, DS:3D91 after the retrace of 0B67:028F (keys of the frame then)
        if (!Key(MenuKeys.KeypadPlus)) yield return MenuWait.Retrace;
        if (!Key(MenuKeys.KeypadMinus)) yield break;
        for (int i = 0; i < 4; i++) yield return MenuWait.Retrace;
    }

    /// <summary>[ORIGINAL] 21FD:0245 – P pauses: wait for release, for the next press and for its release.</summary>
    private IEnumerable<MenuWait> Pause()
    {
        if (!Key(MenuKeys.P)) yield break;
        while (Key(MenuKeys.P)) yield return MenuWait.Poll;
        while (!Key(MenuKeys.P)) yield return MenuWait.Poll;
        while (Key(MenuKeys.P)) yield return MenuWait.Poll;
    }

    /// <summary>Key wait loop after a handled key: polls until the key is released.</summary>
    private IEnumerable<MenuWait> Release(byte key)
    {
        while (Key(key)) yield return MenuWait.Poll;
    }

    /// <summary>random(n) of the core; every call is listed in <see cref="RandomCalls"/> with its frame.</summary>
    private short Random(short n)
    {
        RandomCalls.Add((Frame, n));
        return _random.Next(n);
    }

    /// <summary>All random() calls so far with frame and argument (diagnostics for differential tests).</summary>
    public List<(long Frame, short Argument)> RandomCalls { get; } = [];

    private bool Key(byte scancode) => _d.U8(DgroupLayout.KeyState + scancode) != 0;

    private void Sound(int a, int b) => Emit(MenuEventKind.Sound, a, b);

    private void Emit(MenuEventKind kind, int a = 0, int b = 0, int c = 0, IReadOnlyList<BackdropOp>? ops = null) =>
        _events.Add(new MenuEvent(kind, Frame, a, b, c, ops));
}
