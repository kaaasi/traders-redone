namespace Traders.Core.Menus;

/// <summary>
/// Data segment offsets and scancodes used by the menus of segment 192D (docs/routines/192D_0007_menues.md). Option fields
/// themselves are in <see cref="State.DgroupLayout"/>; the offsets here are the menu's own state and work fields.
/// </summary>
public static class MenuLayout
{
    /// <summary>DS:3CCC – result of the main menu: 1 = new game, 2 = continue the loaded game (192D:12A6).</summary>
    public const int Choice = 0x3CCC;

    /// <summary>DS:4581 – 1 when all four players are computers (192D:03E3); the controls menu is skipped then.</summary>
    public const int AllComputers = 0x4581;

    /// <summary>DS:5330/5332 – columns and rows edited in the settings menu (copies of DS:08FC/08FE, 192D:0092/00A3).</summary>
    public const int EditColumns = 0x5330, EditRows = 0x5332;

    /// <summary>DS:5334 – top of the menu plaque, (200 − height) / 2 (192D:16AF; disk menu: 1Bh, 192D:0FB7).</summary>
    public const int PlaqueTop = 0x5334;

    /// <summary>DS:08FA – a game is in progress or was loaded (set by the disk menu and by main 1777:000C).</summary>
    public const int GameInProgress = 0x08FA;

    /// <summary>DS:08DA – resource number of the menu picture, argument of 186C:0002.</summary>
    public const int PictureResource = 0x08DA;

    /// <summary>DS:451E/4528 – field origin x and y (192D:0058).</summary>
    public const int FieldOriginX = 0x451E, FieldOriginY = 0x4528;

    /// <summary>DS:0B90…0BAF – level table read by 192D:0592 (four levels).</summary>
    public const int LevelIq = 0x0B90, LevelOil = 0x0B98, LevelRobot = 0x0B9C, LevelGems = 0x0BA0, LevelCatastrophes = 0x0BA4,
        LevelPirates = 0x0BA8;

    /// <summary>DS:37CC – far pointers to the lines of ENGLISCH.TXT, entry k at 37CC + 4k (192D:18B6).</summary>
    public const int TextPointers = 0x37CC;

    /// <summary>Number of text pointer entries (0…12Fh; entry 0 is a null pointer).</summary>
    public const int TextPointerCount = 0x130;

    /// <summary>DS:4884 – far pointer to the loaded text file (filled by 186C:0B0A).</summary>
    public const int TextBuffer = 0x4884;

    /// <summary>DS:0C76 – file name of the text file ("Englisch.txt").</summary>
    public const int TextFileName = 0x0C76;
}

/// <summary>Set-1 make codes the menus test in the key table DS:3D47 + scancode.</summary>
public static class MenuKeys
{
    public const byte Q = 0x10, O = 0x18, P = 0x19, Enter = 0x1C, A = 0x1E, S = 0x1F, L = 0x26, V = 0x2F, N = 0x31;
    public const byte F1 = 0x3B, F2 = 0x3C, F3 = 0x3D, F4 = 0x3E, F5 = 0x3F, F6 = 0x40, F7 = 0x41, F8 = 0x42, F9 = 0x43, F10 = 0x44;
    public const byte Up = 0x48, Left = 0x4B, Right = 0x4D, Down = 0x50;

    /// <summary>Keypad minus and plus: the page flip 21FD:051F waits four more retraces resp. no retrace while held.</summary>
    public const byte KeypadMinus = 0x4A, KeypadPlus = 0x4E;
}
