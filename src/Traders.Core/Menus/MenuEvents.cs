namespace Traders.Core.Menus;

/// <summary>What a menu event tells the scene (or the sound and resource layers).</summary>
public enum MenuEventKind
{
    /// <summary>186C:0002(resource): the menu picture is loaded (A = resource number DS:08DA).</summary>
    LoadPicture,

    /// <summary>21A7:0054(A, B): sound call (not ported yet, R10).</summary>
    Sound,

    /// <summary>
    /// A menu screen is built (192D:16AF or 0FB7): A = routine of the screen, B = plaque height, C = text pointer DS offset;
    /// <see cref="MenuEvent.Ops"/> = the random backdrop (wall and plaques) in draw order. The fade-out 21FD:03B4(420h) is part
    /// of it.
    /// </summary>
    Screen,

    /// <summary>A part of a screen is drawn or redrawn: A = drawing routine in 192D, B = its argument (player, delta …).</summary>
    Draw,

    /// <summary>21FD:026C: the loaded palette becomes the base palette.</summary>
    Palette,

    /// <summary>Fade-in: A = 21FD:0353 (fast) or 21FD:0329 (slow).</summary>
    FadeIn,

    /// <summary>21FD:051F: page flip (A = 1 when the flip waited four more retraces).</summary>
    Flip,

    /// <summary>192D:120C: message of the disk menu (A = DS offset of the text pointer).</summary>
    DiskMessage,

    /// <summary>27C6:00E6(A, B): joystick calibration step B for joystick A (C = 1 when it failed).</summary>
    Calibration,
}

/// <summary>An event of the menus with the frame in which it happened.</summary>
public sealed record MenuEvent(MenuEventKind Kind, long Frame, int A = 0, int B = 0, int C = 0, IReadOnlyList<BackdropOp>? Ops = null)
{
    public override string ToString() => $"frame {Frame} {Kind} {A:X} {B:X} {C:X}{(Ops is null ? "" : $" ops {Ops.Count}")}";
}

/// <summary>Why the menu coroutine gives up the rest of the frame.</summary>
public enum MenuWait
{
    /// <summary>Page flip 21FD:051F: the original waits for the vertical retrace.</summary>
    Flip,

    /// <summary>A key wait loop of the original polls the key table again (busy loop; the key changes in a later frame).</summary>
    Poll,

    /// <summary>A retrace wait inside a fade or a slowed flip.</summary>
    Retrace,
}

/// <summary>
/// Services of the host the menus need: the save file (decision F2 = A) and the joystick calibration (27C6:00E6, hardware;
/// decision F3 = A: the host offers only the keyboard for now).
/// </summary>
public interface IMenuHost
{
    /// <summary>2336:0539: load SAVEGAME.DAT into the data segment; true (AL = 1) on success.</summary>
    bool LoadGame(State.Dgroup state);

    /// <summary>2336:000A: write SAVEGAME.DAT from the data segment; true (AL = 1) on success.</summary>
    bool SaveGame(State.Dgroup state);

    /// <summary>27C6:00E6(stick, step): one calibration step; true when it failed (AX ≠ 0).</summary>
    bool CalibrationFailed(int stick, int step);
}
