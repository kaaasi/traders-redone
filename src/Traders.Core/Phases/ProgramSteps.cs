using Traders.Core.Menus;
using Traders.Core.Rules;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// Program step 1, the initialisation 1777:04E4, reduced to what the game state needs: the graphics card DS:090E and the sound
/// device DS:0910. Everything else it does belongs to the host and the output layers (finding
/// docs/routines/1777_04E4_initialisierung.md, table "Zustand"): heap (186C:021E), resource table marks (186C:0544, lane part 2),
/// interrupt vectors and handler mask (0D73:00F8..049B), music driver (218B:0002) and display pages (0B67:0002).
/// </summary>
// [ORIGINAL] 1777:04E4 – DS:090E = -1 → detect the card; still -1 → result 2; DS:0910 = -1 → detect the sound device
// [APPROX] The detections 0D73:0712 (card) and 0D73:04CA (AdLib at port 388h) are replaced: the port is VGA only (card 1, as
// the RefEmu detects it) and the host passes the sound device as an explicit input. The error result 1 of 0B67:0002 (no
// memory for the page buffer) is the host's.
public sealed class ProgramInitialisation(short soundDevice) : IProgramStepHandler
{
    /// <summary>DS offset of the graphics card (1 VGA, 0 EGA, 2 Tandy, 63h Tandy emulation, -1 detect).</summary>
    public const int GraphicsCard = 0x090E;

    /// <summary>DS offset of the sound device (0 PC speaker, 1 AdLib, -1 detect).</summary>
    public const int SoundDevice = 0x0910;

    public const short Vga = 1;

    public const short AdLib = 1;

    public const short PcSpeaker = 0;

    /// <summary>Exit code of main when no graphics card was found (closing message DS:095A).</summary>
    public const byte NoGraphicsCard = 2;

    public StepOutcome Begin(ProgramStepContext context)
    {
        var d = context.State;
        if (d.S16(GraphicsCard) == -1) d.Set(GraphicsCard, Vga);
        if (d.S16(GraphicsCard) == -1) return StepOutcome.Finished(NoGraphicsCard); // unreachable with the fixed detection
        if (d.S16(SoundDevice) == -1) d.Set(SoundDevice, soundDevice);
        return StepOutcome.Finished(0);
    }

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("the initialisation finishes at once");
}

/// <summary>
/// Sound calls of main (steps 3–7: 218B:013A, 21A7:000D, 21A7:0054(0, 1), 218B:0094(1, 2)) until the sound lane (R10):
/// they return at once with AL = 0 and change nothing. That is what 218B:013A returns without a music driver (DS:10A3 = 0),
/// and 218B:0094 then does nothing either (218B:00A5); main takes the same path as in the reference run.
/// </summary>
// [APPROX] Sound pending (R10): 21A7:000D/0054 set the sound effect source and play an effect (DS:10A5, timer callback 27C6:041B);
// in the port they are no-ops. The bytes they write in the original are listed in docs/routines/1777_000C_main.md ("Ton").
public sealed class PendingSoundStep : IProgramStepHandler
{
    public static PendingSoundStep Instance { get; } = new();

    public StepOutcome Begin(ProgramStepContext context) => StepOutcome.Finished(0);

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("sound stubs finish at once");
}

/// <summary>
/// Program step 8, the main menu 192D:0007 of lane menue (<see cref="MainMenu"/>) as a step of the program machine:
/// <see cref="Begin"/> runs it in the start frame up to its first wait, every <see cref="Tick"/> one frame further; the
/// outcome's AL is the result of 192D:0007 (0 quit, 1 play). The machine writes the key table DS:3D47 from the held keys of
/// the frame and keeps DS:10D2 in sync. The menu picture load (event <see cref="MenuEventKind.LoadPicture"/>, 186C:0002) is
/// entered in the picture table of the core (<see cref="ImageTable"/>); the events go on to the scene (<see cref="DrainEvents"/>).
/// </summary>
// [ORIGINAL] 1777:000C (site 0163) – call of 192D:0007; 192D:000A – 186C:0002([08DA])
public sealed class MainMenuProgramStep(IMenuHost host, Func<int, ImageFileLengths> files) : IProgramStepHandler
{
    private readonly List<MenuEvent> _events = [];
    private IEnumerator<MenuWait>? _run;

    /// <summary>The menu of the current (or last) call; null before the first.</summary>
    public MainMenu? Menu { get; private set; }

    /// <summary>Number of calls of the menu so far (the program enters it again after exit A).</summary>
    public int Calls { get; private set; }

    /// <summary>Menu events since the last <see cref="DrainEvents"/>, oldest first (for the scene).</summary>
    public IReadOnlyList<MenuEvent> Events => _events;

    public List<MenuEvent> DrainEvents()
    {
        var list = new List<MenuEvent>(_events);
        _events.Clear();
        return list;
    }

    public StepOutcome Begin(ProgramStepContext context)
    {
        Menu = new MainMenu(context.State, context.Random, host);
        _run = Menu.Run().GetEnumerator();
        Calls++;
        return Tick(context);
    }

    public StepOutcome Tick(ProgramStepContext context)
    {
        if (_run is null || Menu is null) throw new InvalidOperationException("Not started.");
        Menu.Frame = context.Frame;
        bool more = _run.MoveNext();
        foreach (var e in Menu.DrainEvents())
        {
            if (e.Kind == MenuEventKind.LoadPicture) ImageTable.RecordLoad(context.State, e.A, files);
            _events.Add(e);
        }
        return more ? StepOutcome.Running : StepOutcome.Finished(Menu.Result ?? 0);
    }
}

/// <summary>Program step 9, the new game 0E31:0003 of lane partie (<see cref="NewGame.Start"/>); main ignores its result.</summary>
// [ORIGINAL] 1777:000C (site 0175) – call of 0E31:0003 only when DS:3CCC = 1 (guard of the machine)
public sealed class NewGameProgramStep : IProgramStepHandler
{
    public static NewGameProgramStep Instance { get; } = new();

    public StepOutcome Begin(ProgramStepContext context)
    {
        NewGame.Start(context.State, context.Random);
        return StepOutcome.Finished(0);
    }

    public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("the new game finishes at once");
}

/// <summary>Handlers of the program steps the core has.</summary>
public static class ProgramSteps
{
    /// <summary>Program steps with a sound call (handled by <see cref="PendingSoundStep"/> until R10).</summary>
    public static IReadOnlyList<int> SoundSteps { get; } = [3, 4, 5, 6, 7];

    /// <summary>
    /// Initialisation and the sound stubs only (tests of single steps). The whole program is <see cref="All"/>: the program
    /// start needs the host's file lengths and text file, the main menu the host's save file and joystick.
    /// </summary>
    public static Dictionary<int, IProgramStepHandler> Defaults(short soundDevice = ProgramInitialisation.AdLib)
    {
        var d = new Dictionary<int, IProgramStepHandler> { [1] = new ProgramInitialisation(soundDevice) };
        foreach (int n in SoundSteps) d[n] = PendingSoundStep.Instance;
        return d;
    }

    /// <summary>All nine call steps of main: initialisation, program start, sound stubs, main menu and new game.</summary>
    public static Dictionary<int, IProgramStepHandler> All(short soundDevice, ProgramStart start, MainMenuProgramStep menu)
    {
        var d = Defaults(soundDevice);
        d[2] = start;
        d[ProgramLoop.MainMenu] = menu;
        d[ProgramLoop.NewGame] = NewGameProgramStep.Instance;
        return d;
    }

    /// <summary>
    /// Initial data segment of the program as the host prepares it: the initialised data of T.EXE (<see cref="Dgroup.FromExe"/>).
    /// What the C runtime startup writes before main (heap, environment, file table) is the host's.
    /// </summary>
    // [APPROX] Turbo C startup 0000:00E3 (DS:005A..008E, 1286..12B7, 1452..1579 and others) not modelled;
    // the bytes are listed in docs/routines/1777_04E4_initialisierung.md.
    public static Dgroup InitialState(ReadOnlySpan<byte> exe) => Dgroup.FromExe(exe);
}
