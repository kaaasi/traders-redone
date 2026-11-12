namespace Traders.Core.Menus;

/// <summary>What a menu event tells the scene (or the sound and resource layers).</summary>
public enum MenuEventKind
{
    /// <summary>186C:0002(resource): the menu picture is loaded (A = resource number DS:08DA).</summary>
    LoadPicture,

    /// <summary>21A7:0054(A, B): sound call (not ported yet, R10).</summary>
    Sound,

    /// <summary>21FD:03B4(A): fade-out to the 12-bit colour A (420h) before a screen is built.</summary>
    FadeOut,

    /// <summary>
    /// A menu screen is built (192D:00C5, 042F, 0A26, 0FB7, 1263, 12A6, 19A4, 1B90): A = routine of the screen, B = its
    /// argument (joystick for 1B90); <see cref="MenuEvent.Ops"/> = the random backdrop (wall and plaques) in draw order. The
    /// scene draws the whole routine after its fade-out (21FD:03B4) and before its fade-in.
    /// </summary>
    Screen,

    /// <summary>A part of a screen is redrawn: A = drawing routine in 192D, B = its argument (player, prompt line …).</summary>
    Draw,

    /// <summary>Fade-in: A = 21FD:0353 (fast) or 21FD:0329 (slow).</summary>
    FadeIn,

    /// <summary>21FD:051F: page flip (A = 1 when the flip waited four more retraces).</summary>
    Flip,

    /// <summary>0A96:06A8: the command list is executed.</summary>
    ListExecute,

    /// <summary>0A96:001B: the command list is emptied.</summary>
    ListClear,

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
