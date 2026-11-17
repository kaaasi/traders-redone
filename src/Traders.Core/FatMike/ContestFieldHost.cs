using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.FatMike;

/// <summary>
/// The field's host of the contest's second field round 0609:0080: the planting phase's production host
/// (<see cref="PlantingHost"/>: computer turn 135D:000E, helpers 0609:074D/100F, controls, pause, wait loop) and the texts of
/// the typed notice of 0609:0204 (<see cref="IFieldTexts"/>, the host's copy of the far text buffer). The picture load
/// 186C:0002 of 0609:00D6 is recorded in the picture table of <paramref name="state"/> (<see cref="ImageTable.RecordLoad"/>)
/// when <paramref name="files"/> is given, as <see cref="ContestStep"/> does for the contest's own events.
/// </summary>
/// <param name="output">Receiver of the field's drawing and sound requests (the scene).</param>
/// <param name="farString">Zero-terminated string at a far address, without the terminator.</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
/// <param name="state">The game's data segment (picture table), with <paramref name="files"/>.</param>
/// <param name="files">Lengths of the picture files (as for the main menu); no recording if null.</param>
public sealed class ContestFieldHost(Action<FieldEvent> output, Func<ushort, ushort, byte[]> farString, IControlDevices? devices = null,
    Dgroup? state = null, Func<int, ImageFileLengths>? files = null) : IPlantingHost, IFieldTexts
{
    private readonly PlantingHost _host = new(output, devices);

    public void Emit(FieldEvent e)
    {
        // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
        if (e.Kind == FieldEventKind.LoadImage && state is not null && files is not null) ImageTable.RecordLoad(state, e.Args[0], files);
        _host.Emit(e);
    }

    public void ComputerTurn(Dgroup d, short x, short y) => _host.ComputerTurn(d, x, y);
    public bool CanPlaceHq(Dgroup d) => _host.CanPlaceHq(d);
    public bool HasOwnNeighbour(Dgroup d, short x, short y) => _host.HasOwnNeighbour(d, x, y);
    public void ComputerPlacedHq(Dgroup d) => _host.ComputerPlacedHq(d);
    public void ComputerBoughtField(Dgroup d) => _host.ComputerBoughtField(d);
    public void ReadControls(Dgroup d) => _host.ReadControls(d);
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => _host.PauseCheck(d);
    public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => _host.WaitLoop(d, random, mode);
    public byte[] FarString(ushort offset, ushort segment) => farString(offset, segment);
}
