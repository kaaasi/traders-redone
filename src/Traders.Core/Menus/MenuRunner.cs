using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Core.Menus;

/// <summary>
/// Drives the main menu coroutine frame by frame: <see cref="Start"/> runs it in the start frame up to its first wait, every
/// later frame resumes it once. Keys enter as the held scancodes of a <see cref="PhaseTick"/> and are written to the key table
/// DS:3D47 before the frame (as <see cref="PhaseMachine"/> does); random() is synchronised with DS:10D2 around every resume.
/// A frame number that was already processed changes nothing.
/// </summary>
public sealed class MenuRunner
{
    private readonly Dgroup _state;
    private readonly OriginalRandom _random;
    private IEnumerator<MenuWait>? _run;

    public MenuRunner(Dgroup state, OriginalRandom random, IMenuHost host)
    {
        _state = state;
        _random = random;
        Menu = new MainMenu(state, random, host);
    }

    public MainMenu Menu { get; }

    /// <summary>Last frame processed (−1 before <see cref="Start"/>).</summary>
    public long LastFrame { get; private set; } = -1;

    public bool Finished { get; private set; }

    /// <summary>Waits by kind so far (diagnostics).</summary>
    public List<MenuWait> Waits { get; } = [];

    /// <summary>Starts 192D:0007 in frame <paramref name="frame"/> with the keys held then.</summary>
    public void Start(long frame = 0, IReadOnlyCollection<byte>? held = null)
    {
        if (_run is not null) throw new InvalidOperationException("Already started.");
        if (held is not null) KeyTable.Write(_state, held);
        _run = Menu.Run().GetEnumerator();
        LastFrame = frame;
        Resume(frame);
    }

    /// <summary>Processes every frame after <see cref="LastFrame"/> up to <paramref name="tick"/>.Frame, each exactly once.</summary>
    public void Advance(PhaseTick tick)
    {
        if (_run is null) throw new InvalidOperationException("Not started.");
        if (tick.HeldScancodes is { } keys) KeyTable.Write(_state, keys);
        while (LastFrame < tick.Frame && !Finished)
        {
            LastFrame++;
            Resume(LastFrame);
        }
    }

    private void Resume(long frame)
    {
        Menu.Frame = frame;
        uint seed = _state.U32(DgroupLayout.RandomSeed);
        _random.Seed = seed;
        long calls = _random.Calls;
        bool more = _run!.MoveNext();
        if (_random.Calls != calls || _random.Seed != seed) _state.Set(DgroupLayout.RandomSeed, _random.Seed);
        if (more) Waits.Add(_run.Current);
        else Finished = true;
    }
}

/// <summary>The key table DS:3D47 as the menus and the phase machine write it: 1 = held, 0 = released (scancodes 0…7Fh).</summary>
// [APPROX] Written per frame from the host's held keys; the keyboard handler 27C6:037A itself is not ported (lane host).
public static class KeyTable
{
    public const int Length = 0x80;

    public static void Write(Dgroup state, IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < Length; sc++) state.Set(DgroupLayout.KeyState + sc, (byte)0);
        foreach (byte sc in held)
            if (sc < Length) state.Set(DgroupLayout.KeyState + sc, (byte)1);
    }
}

/// <summary>
/// The main menu as a step of a phase machine (<see cref="IWeekStepHandler"/>): <see cref="Begin"/> runs it to the first wait,
/// every <see cref="Tick"/> one frame further; the outcome's AL is the result of 192D:0007. The machine writes the key table
/// and keeps DS:10D2 in sync.
/// </summary>
public sealed class MainMenuStep(IMenuHost host) : IWeekStepHandler
{
    private IEnumerator<MenuWait>? _run;
    private MainMenu? _menu;

    public MainMenu? Menu => _menu;

    public StepOutcome Begin(StepContext context)
    {
        _menu = new MainMenu(context.State, context.Random, host);
        _run = _menu.Run().GetEnumerator();
        return Tick(context);
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null || _menu is null) throw new InvalidOperationException("Not started.");
        _menu.Frame = context.Frame;
        return _run.MoveNext() ? StepOutcome.Running : StepOutcome.Finished(_menu.Result ?? 0);
    }
}
