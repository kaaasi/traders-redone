using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Presentation.Menus;

/// <summary>Decrunched picture resources (V-*.PP) by resource number of the table DS:09CE.</summary>
public interface IPictureSource
{
    byte[] Decrunched(int resource);
}

/// <summary>
/// The menu scene: plays the <see cref="MenuEvent"/>s of the core's main menu (192D) through the graphics layers. It ports
/// the drawing side of the 192D routines call by call; option values, text pointers and the plaque top DS:5334 are read
/// from the core's data segment, which the scene never changes. The graphics state (command list, pages, palettes) lives
/// in the layers' own data segment image.
/// </summary>
public sealed class MenuScene
{
    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly IFarMemory? _far;
    private readonly IPictureSource _pictures;

    /// <param name="layers">Graphics layers with their own data segment image (DataSegment = segment value of DS).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="far">Memory behind far text pointers outside DS (the text buffer DS:4884).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public MenuScene(GraphicsLayers layers, Dgroup state, IFarMemory? far, IPictureSource pictures)
    {
        _gfx = layers;
        _state = state;
        _far = far;
        _pictures = pictures;
    }

    public GraphicsLayers Layers => _gfx;

    /// <summary>Called before an event that hands the picture to the display (flip, fade-out, fade-in) is played.</summary>
    public Action<MenuEvent>? BeforePresent { get; set; }

    private short Top => _state.S16(MenuLayout.PlaqueTop);

    /// <summary>Plays one event.</summary>
    public void Apply(MenuEvent e)
    {
        switch (e.Kind)
        {
            case MenuEventKind.LoadPicture:
                LoadPicture(e.A);
                break;
            case MenuEventKind.FadeOut:
                BeforePresent?.Invoke(e);
                _gfx.FadeOutFast((ushort)e.A); // 21FD:03B4
                break;
            case MenuEventKind.Screen:
                Screen(e.A, e.B, e.Ops ?? []);
                break;
            case MenuEventKind.Draw:
                Draw(e.A, e.B);
                break;
            case MenuEventKind.DiskMessage:
                DiskMessage(e.A);
                break;
            case MenuEventKind.FadeIn:
                BeforePresent?.Invoke(e);
                if (e.A == 0x0329) _gfx.FadeIn();
                else _gfx.FadeInFast();
                break;
            case MenuEventKind.Flip:
                BeforePresent?.Invoke(e);
                MirrorKeys();
                _gfx.FlipPages(); // 21FD:051F
                break;
            case MenuEventKind.ListExecute:
                _gfx.ExecuteList();
                break;
            case MenuEventKind.ListClear:
                _gfx.ClearList();
                break;
            case MenuEventKind.Sound:
            case MenuEventKind.Calibration:
                break; // sound and joystick are not the scene's
        }
    }

    // ------------------------------------------------------------------ resources and fonts

    /// <summary>
    /// Picture load as 186C:0002(i) does it on VGA (the loader belongs to lane programm): DS:09C8 = i, the decrunched picture's first 300h bytes become the loaded palette
    /// DS:5017, the rest is moved to the start of the atlas and turned into plane rows (23CD:3079 from atlas + 4, 320 bytes,
    /// 256 rows), then the moving objects are reset (21AF:000D).
    /// </summary>
    // [APPROX] Port of the effect only, from the resource source instead of 186C (lane programm); the caching of the loader (resource table DS:09CE, EMS/disk state DS:4C00…) is not modelled: the scene always
    // takes the picture from the resource source.
    private void LoadPicture(int resource)
    {
        var data = _pictures.Decrunched(resource);
        _gfx.Ds[0x09C8] = (byte)resource;
        _gfx.Ds[0x09C9] = (byte)(resource >> 8);
        Array.Copy(data, 0, _gfx.Ds, GraphicsLayers.LoadedPalette, 0x300);
        var atlas = _gfx.Driver.Atlas.Data;
        int n = Math.Min(atlas.Length, data.Length - 0x300);
        Array.Copy(data, 0x300, atlas, 0, n);
        PlanarAtlas.ConvertToPlanarRows(atlas, PlanarAtlas.HeaderSize, PlanarAtlas.Width, PlanarAtlas.Height);
        _gfx.InitializeObjects();
    }

    /// <summary>
    /// [ORIGINAL] 21FD:05DB(font): driver font (23CD:05B6), DS:3CA8 = advance B[10DA + font], DS:34A4 = line height
    /// B[10DD + font], text shadow 0A96:000A(advance, height).
    /// </summary>
    private void Font(int font)
    {
        _gfx.Driver.Font = (ushort)font;
        ushort advance = _gfx.Ds[0x10DA + font], height = _gfx.Ds[0x10DD + font];
        Set16(GraphicsLayers.FontAdvance, advance);
        Set16(GraphicsLayers.FontLineHeight, height);
        _gfx.SetTextShadow(advance, height);
    }

    private void Set16(int at, ushort v)
    {
        _gfx.Ds[at] = (byte)v;
        _gfx.Ds[at + 1] = (byte)(v >> 8);
    }

    /// <summary>The key table bytes the flip reads (DS:3D91 keypad minus, DS:3D95 keypad plus) as the core holds them.</summary>
    private void MirrorKeys()
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) _gfx.Ds[DgroupLayout.KeyState + sc] = _state.U8(DgroupLayout.KeyState + sc);
    }

    // ------------------------------------------------------------------ text access

    private (ushort Off, ushort Seg) Pointer(int dsOffset) => (_state.U16(dsOffset), _state.U16(dsOffset + 2));

    private byte ReadFar(ushort seg, ushort off)
    {
        if (seg == _gfx.DataSegment) return _gfx.Ds[off];
        return _far is { } m ? m.Read(seg, off)
            : throw new InvalidOperationException($"Far pointer {seg:X4}:{off:X4} outside the data segment without memory.");
    }

    private int StringLength((ushort Off, ushort Seg) p)
    {
        int n = 0;
        while (ReadFar(p.Seg, (ushort)(p.Off + n)) != 0) n++;
        return n;
    }

    private void Text(int x, int y, (ushort Off, ushort Seg) p, int color, int shadow, int ox, int oy) =>
        _gfx.AddShadowText(W(x), W(y), p.Off, p.Seg, W(color), W(shadow), W(ox), W(oy));

    private void Centered(int x, int y, int w, (ushort Off, ushort Seg) p, int color, int shadow, int ox, int oy) =>
        _gfx.AddCenteredShadowText(W(x), W(y), W(w), p.Off, p.Seg, W(color), W(shadow), W(ox), W(oy));

    private static ushort W(int v) => unchecked((ushort)v);

    // ------------------------------------------------------------------ screens

    private void Screen(int routine, int argument, IReadOnlyList<BackdropOp> ops)
    {
        switch (routine)
        {
            case 0x00C5: MainScreen(ops); break;
            case 0x042F: LevelScreen(ops); break;
            case 0x0A26: SettingsScreen(ops); break;
            case 0x0FB7: DiskScreen(ops); break;
            case 0x1263: Plaque(ops, 0x1E, 0x3BF4, 0x12); _gfx.CopyGamePage(DrawPage); break;
            case 0x12A6: Plaque(ops, 0x1E, 0x3AB0, 3); _gfx.CopyGamePage(DrawPage); break;
            case 0x19A4: ControlsScreen(ops); break;
            case 0x1B90: CalibrationScreen(ops, argument); break;
            default: throw new ArgumentOutOfRangeException(nameof(routine), $"No screen routine 192D:{routine:X4}.");
        }
    }

    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    /// <summary>The wall and plaque draw calls of 131B/14FB computed by the core.</summary>
    private void Backdrop(IReadOnlyList<BackdropOp> ops)
    {
        foreach (var o in ops)
        {
            switch (o.Kind)
            {
                case BackdropOpKind.Blit: _gfx.AddBlit(o.A, o.B, o.C, o.D, o.E, o.F); break;
                case BackdropOpKind.MaskedBlit: _gfx.AddMaskedBlit(o.A, o.B, o.C, o.D, o.E, o.F, o.G, o.H); break;
                case BackdropOpKind.FilledRect: _gfx.AddFilledRect(o.A, o.B, o.C, o.D, o.E); break;
                case BackdropOpKind.HorizontalLine: _gfx.AddHorizontalLine(o.A, o.B, o.C, o.D); break;
                case BackdropOpKind.VerticalLine: _gfx.AddVerticalLine(o.A, o.B, o.C, o.D); break;
            }
        }
    }

    /// <summary>
    /// [ORIGINAL] 192D:16AF(h, step, texts, extra) after its fade-out: the backdrop, then in font 1 the title (entry 0)
    /// centred, in font 0 the lines 1… up to "@" ("'" = 3 pixels gap, otherwise 7 lines apart), after the extra gap the
    /// line behind "@" centred. Each group is executed at once.
    /// </summary>
    private void Plaque(IReadOnlyList<BackdropOp> ops, int step, int texts, int extra)
    {
        _gfx.ClearList();
        Backdrop(ops);
        _gfx.ExecuteList();
        short si = (short)(Top + step);
        Font(1);
        _gfx.ClearList();
        Centered(0, si, 0x140, Pointer(texts), 1, 4, -4, 4);
        _gfx.ExecuteList();
        Font(0);
        si += 0x14;
        _gfx.ClearList();
        int di = 1;
        while (true)
        {
            var line = Pointer(texts + 4 * di);
            byte first = ReadFar(line.Seg, line.Off);
            if (first == 0x40) break;
            if ((unchecked((sbyte)first) & 0x7F) == 0x27) si += 3;
            else
            {
                Text(0x46, si, line, 0, 3, -1, 1);
                si += 7;
            }
            di++;
        }
        si = (short)(si + extra);
        Centered(0, si, 0x140, Pointer(texts + 4 * (di + 1)), 1, 4, -1, 1);
        _gfx.ExecuteList();
    }

    /// <summary>[ORIGINAL] 192D:00C5 – main menu: plaque, title strip, four lines of two texts, the player labels, footer.</summary>
    private void MainScreen(IReadOnlyList<BackdropOp> ops)
    {
        Plaque(ops, 0x64, 0x3994, 3);
        _gfx.ClearList();
        _gfx.AddBlit(0x47, 0x28, 0xAB, 0x1C, 0x4A, W(Top + 6));
        for (int i = 0; i < 4; i++)
        {
            Centered(0, Top + i * 0x0E + 0x2A, 0x140, Pointer(0x0B70 + 8 * i), 0, 3, -1, 1);
            Centered(0, Top + i * 0x0E + 0x31, 0x140, Pointer(0x0B74 + 8 * i), 0x0E, 3, -1, 1);
        }
        for (int p = 0; p < 4; p++) PlayerLabel(p);
        Text(0xBE, Top + 0xAC, Pointer(0x3C80), 0, 3, -1, 1);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
        _gfx.CopyLoadedToBase(); // 21FD:026C
    }

    /// <summary>[ORIGINAL] 192D:042F – level menu: plaque, key help of the user-defined level, the seven lines.</summary>
    private void LevelScreen(IReadOnlyList<BackdropOp> ops)
    {
        Plaque(ops, 0x1E, 0x39C8, 0x28);
        _gfx.ClearList();
        if (_state.S16(DgroupLayout.Level) == -1) KeyHelp();
        foreach (int r in LevelLines) Draw(r, 0);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
    }

    private static readonly int[] LevelLines = [0x076C, 0x07AC, 0x07FC, 0x084A, 0x0899, 0x08E7, 0x0935];

    /// <summary>[ORIGINAL] 192D:0A26 – settings menu: plaque, header line and the nine option lines.</summary>
    private void SettingsScreen(IReadOnlyList<BackdropOp> ops)
    {
        Plaque(ops, 0x1E, 0x3A10, 3);
        _gfx.ClearList();
        Text(0x46, Top + 0x69, Pointer(0x3C78), 0, 3, -1, 1);
        foreach (int r in (int[])[0x0DC0, 0x0E11, 0x0E64, 0x0E8E, 0x0EAE, 0x0ED9, 0x0EF8, 0x0F16, 0x0F6E]) Draw(r, 0);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
    }

    /// <summary>[ORIGINAL] 192D:0FB7 – disk menu after its fade-out: wall, two plaques, title and the texts.</summary>
    private void DiskScreen(IReadOnlyList<BackdropOp> ops)
    {
        _gfx.ClearList();
        Backdrop(ops);
        _gfx.ExecuteList();
        Font(1);
        _gfx.ClearList();
        Centered(0, Top + 0x1E, 0x140, Pointer(0x3A74), 1, 4, -4, 4);
        _gfx.ExecuteList();
        Font(0);
        _gfx.ClearList();
        Text(0x46, Top + 0x32, Pointer(0x3A78), 0, 3, -1, 1);
        Text(0x46, Top + 0x39, Pointer(0x3A7C), 0, 3, -1, 1);
        Centered(0, Top + 0x43, 0x140, Pointer(0x3A84), 1, 4, -1, 1);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
    }

    /// <summary>[ORIGINAL] 192D:19A4 – controls menu: plaque and the four control labels.</summary>
    private void ControlsScreen(IReadOnlyList<BackdropOp> ops)
    {
        Plaque(ops, 0x1E, 0x3C20, 0x0A);
        _gfx.ClearList();
        for (int p = 0; p < 4; p++) ControlLabel(p);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
        _gfx.CopyLoadedToBase();
    }

    /// <summary>
    /// [ORIGINAL] 192D:1B90 (screen part for one joystick): plaque, the joystick's name (text 125h or 126h) centred at
    /// 5Ah − strlen·3, the two fixed lines, then the first prompt.
    /// </summary>
    private void CalibrationScreen(IReadOnlyList<BackdropOp> ops, int stick)
    {
        Plaque(ops, 0x1E, 0x3C54, 0x28);
        _gfx.ClearList();
        var name = Pointer(MenuLayout.TextPointers + 4 * (stick == 1 ? 0x125 : 0x126));
        Label(W(0x5A - (ushort)(StringLength(name) * 6) / 2), 0x28, name, 0x0A);
        Label(0x28, 0x46, Pointer(0x3C74), 0x0A);
        Label(0x28, 0x37, Pointer(0x3C68), 0x14);
        _gfx.ExecuteList();
        _gfx.CopyGamePage(DrawPage);
        _gfx.CopyLoadedToBase();
    }

    // ------------------------------------------------------------------ redraw routines

    private void Draw(int routine, int argument)
    {
        switch (routine)
        {
            case 0x03AB: PlayerLabel(argument); break;
            case 0x0653: KeyHelp(); break;
            case 0x05DD: _gfx.AddFilledRect(0x45, 0x47, 0x18, 0x2A, 2); break; // 192D:0600 inside 05DD
            case 0x076C:
                // [ORIGINAL] 192D:076C – level name: "user defined" text DS:0C74 or the level number + 1
                if (_state.S16(DgroupLayout.Level) == -1) Label(0x8A, 0x32, (0x0C74, _gfx.DataSegment), 1);
                else LabelNumber(0x8A, 0x32, _state.S16(DgroupLayout.Level) + 1, 1);
                break;
            case 0x07AC:
            {
                // [ORIGINAL] 192D:07AC – computer IQ: text and the picture IQ·15h of row 6Dh
                short iq = _state.S16(DgroupLayout.ComputerIq);
                Label(0x8A, 0x39, Pointer(W(0x39F0 + iq * 4)), 9);
                _gfx.AddBlit(W(iq * 0x15), 0x6D, 0x15, 0x1C, 0x45, W(Top + 0x64));
                break;
            }
            case 0x07FC: OnOff(0x40, _state.U8(DgroupLayout.OilConsumption), 0, 0x4B, 0x0D, 0x1E, 0x1F, 2); break; // [ORIGINAL] 192D:07FC
            case 0x084A: OnOff(0x47, _state.U8(DgroupLayout.RobotBattle), 0x0D, 0x4B, 0x15, 0x1C, 0x36, 3); break; // [ORIGINAL] 192D:084A
            case 0x0899: OnOff(0x4E, _state.U8(DgroupLayout.GemTrading), 0x85, 0x44, 0x1E, 0x23, 0x55, 0); break; // [ORIGINAL] 192D:0899
            case 0x08E7: OnOff(0x55, _state.U8(DgroupLayout.Catastrophes), 0x22, 0x4B, 0x15, 0x22, 0x7D, 0); break; // [ORIGINAL] 192D:08E7
            case 0x0935:
            {
                // [ORIGINAL] 192D:0935 – pirates: text and picture (none: plaque colour)
                short pirates = _state.S16(DgroupLayout.PiratePresence);
                Label(0x8A, 0x5C, Pointer(W(0x3A00 + pirates * 4)), 9);
                if (pirates == 0) _gfx.AddFilledRect(0xE1, W(Top + 0x64), 0x1A, 0x1C, 2);
                else _gfx.AddBlit(W(pirates * 0x1A + 0x1D), 0x51, 0x1A, 0x1C, 0xE1, W(Top + 0x64));
                break;
            }
            case 0x0DC0:
                // [ORIGINAL] 192D:0DC0 – initial cash as "$" amount, 5 digits
                _gfx.AddFilledRect(0xCF, W(Top + 0x2E), 0x24, 6, 2);
                _gfx.AddShadowAmount(0xD0, W(Top + 0x32), unchecked((uint)(int)_state.S16(DgroupLayout.InitialCash)), 5, 1, 4, 0xFFFF, 1);
                break;
            case 0x0E11:
                // [ORIGINAL] 192D:0E11 – target as "$" amount, 5 digits
                _gfx.AddFilledRect(0xCF, W(Top + 0x35), 0x24, 6, 2);
                _gfx.AddShadowAmount(0xD0, W(Top + 0x39), _state.U32(DgroupLayout.Target), 5, 1, 4, 0xFFFF, 1);
                break;
            case 0x0E64: Label(0x8A, 0x40, Pointer(W(0x3A58 + _state.S8(DgroupLayout.LottoReturns) * 4)), 9); break; // [ORIGINAL] 192D:0E64
            case 0x0E8E: LabelNumber(0x8A, 0x47, _state.U8(DgroupLayout.TimeLimit), 2); break; // [ORIGINAL] 192D:0E8E
            case 0x0EAE: Label(0x8A, 0x4E, Pointer(0x3A64 + _state.U8(DgroupLayout.LandType) * 4), 9); break; // [ORIGINAL] 192D:0EAE
            case 0x0ED9: LabelNumber(0x8A, 0x55, _state.S8(DgroupLayout.RobotCycle), 1); break; // [ORIGINAL] 192D:0ED9
            case 0x0EF8: LabelNumber(0x8A, 0x5F, _state.S16(MenuLayout.EditColumns), 1); break; // [ORIGINAL] 192D:0EF8
            case 0x0F16: LabelNumber(0x96, 0x5F, _state.S16(MenuLayout.EditRows), 1); break; // [ORIGINAL] 192D:0F16
            case 0x0F6E: Label(0x87, 0x69, Pointer(W(0x398C + _state.S16(DgroupLayout.Sound) * 4)), 3); break; // [ORIGINAL] 192D:0F6E
            case 0x1B06: ControlLabel(argument); break;
            case 0x1B90:
                // [ORIGINAL] 192D:1B90 (lines 1CAB, 1D00) – next calibration prompt (argument 11h, 12h), executed and copied at once
                _gfx.ClearList();
                Label(0x28, 0x37, Pointer(argument == 0x11 ? 0x3C6C : 0x3C70), 0x14);
                _gfx.ExecuteList();
                _gfx.CopyGamePage(DrawPage);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(routine), $"No drawing routine 192D:{routine:X4}.");
        }
    }

    /// <summary>[ORIGINAL] 192D:120C(text) – disk message: plaque colour over the old line, the text centred.</summary>
    private void DiskMessage(int textPointer)
    {
        _gfx.AddFilledRect(0x51, W(Top + Top + 0x66), 0x9C, 6, 2);
        Centered(0, Top + Top + 0x6A, 0x140, Pointer(textPointer), 0, 3, -1, 1);
    }

    /// <summary>[ORIGINAL] 192D:03AB(p) – "human"/"computer" label of player p (texts DS:3BEC + 4·B[0840 + p]).</summary>
    private void PlayerLabel(int p) =>
        Label(0x78, W(p * 7 + 0x78), Pointer(W(0x3BEC + _state.U8(DgroupLayout.IsComputer + p) * 4)), 9);

    /// <summary>[ORIGINAL] 192D:0653 – key help of the user-defined level (six lines DS:0BB0).</summary>
    private void KeyHelp()
    {
        for (int i = 0; i <= 5; i++) Text(0x46, i * 7 + 0x4C, Pointer(0x0BB0 + 4 * i), 0, 3, -1, 1);
    }

    /// <summary>
    /// [ORIGINAL] 192D:1B06(p) – control label of player p: computer text DS:3C3C, device 0…4 DS:3C40 + 4·device, device 5
    /// DS:3C7C.
    /// </summary>
    private void ControlLabel(int p)
    {
        short y = (short)(p * 7 + 0x32);
        if (_state.U8(DgroupLayout.IsComputer + p) == 1) Label(0x58, y, Pointer(0x3C3C), 0x0A);
        else
        {
            sbyte device = _state.S8(DgroupLayout.ControlDevice + p);
            if (device != 5) Label(0x58, y, Pointer(W(0x3C40 + device * 4)), 0x0E);
            else Label(0x58, y, Pointer(0x3C7C), 0x0E);
        }
    }

    /// <summary>
    /// 192D:07FC / 084A / 0899 / 08E7 – an on/off option: its text (DS:398C + 4·value) and, via 09B0, the
    /// picture or the plaque colour.
    /// </summary>
    private void OnOff(int y, byte value, int sx, int sy, int w, int h, int dx, int dy)
    {
        Label(0x8A, y, Pointer(0x398C + value * 4), 9);
        // [ORIGINAL] 192D:09B0(sx, sy, w, h, dx, dy, on)
        if (value == 0) _gfx.AddFilledRect(W(dx + 0x45), W(Top + dy + 0x61), W(w), W(h), 2);
        else _gfx.AddBlit(W(sx), W(sy), W(w), W(h), W(dx + 0x45), W(Top + dy + 0x61));
    }

    /// <summary>
    /// [ORIGINAL] 192D:17F9(x, y, text, n) – plaque colour over n characters at (x + 45h, top + y − 4), then the text at
    /// (x + 46h, top + y), colour 1, shadow 4 at (−1, +1).
    /// </summary>
    private void Label(int x, int y, (ushort Off, ushort Seg) text, int n)
    {
        _gfx.AddFilledRect(W(x + 0x45), W(Top + y - 4), W((sbyte)n * 6), 6, 2);
        Text(x + 0x46, Top + y, text, 1, 4, -1, 1);
    }

    /// <summary>[ORIGINAL] 192D:1855(x, y, number, n) – like 17F9 with an n-digit number (0A69:011B).</summary>
    private void LabelNumber(int x, int y, int number, int n)
    {
        _gfx.AddFilledRect(W(x + 0x45), W(Top + y - 4), W((sbyte)n * 6), 6, 2);
        _gfx.AddShadowNumber(W(x + 0x46), W(Top + y), unchecked((uint)number), W((sbyte)n), 1, 4, 0xFFFF, 1);
    }
}
