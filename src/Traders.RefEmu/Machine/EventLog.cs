namespace Traders.RefEmu.Machine;

public enum EventKind
{
    Dos,
    Bios,
    Video,
    Sound,     // int 64h (FXDRIVER) calls
    File,
    Clock,     // date/time values handed to the program
    Keyboard,
    Port,      // unusual port accesses
    Console,   // text written to stdout/stderr or via BIOS teletype
    Info,
}

/// <summary>One recorded host interaction. <see cref="Time"/> is emulated time in PIT clocks.</summary>
public sealed record EmuEvent(long Instruction, long Time, EventKind Kind, string Text)
{
    public override string ToString() => $"{Instruction,12} {Time / (double)Hardware.Pit.ClockHz,10:F4}s {Kind,-8} {Text}";
}

/// <summary>Ordered record of everything the emulated program asked the host for.</summary>
public sealed class EventLog
{
    private readonly List<EmuEvent> _events = [];

    /// <summary>Upper bound to keep memory bounded on long runs; older entries are kept, newer dropped.</summary>
    public int Capacity { get; set; } = 200_000;

    public int Dropped { get; private set; }

    public IReadOnlyList<EmuEvent> Events => _events;

    public void Add(EmuEvent e)
    {
        if (_events.Count >= Capacity) { Dropped++; return; }
        _events.Add(e);
    }

    public IEnumerable<EmuEvent> OfKind(EventKind kind) => _events.Where(e => e.Kind == kind);

    public void WriteTo(TextWriter w)
    {
        foreach (var e in _events) w.WriteLine(e);
        if (Dropped > 0) w.WriteLine($"... {Dropped} further events dropped");
    }
}
