using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;

namespace Traders.Core.Harvest;

/// <summary>
/// The harvest's host in the game: the control reader 0E9E:037A, the pause key 21FD:0245 (<see cref="Planting.Pause"/>) and
/// the texts of the weather notice from the far text buffer (<paramref name="farString"/>, the host's copy of the buffer
/// loaded at the program start). Drawing and sound go to <paramref name="field"/> and <paramref name="harvest"/>.
/// </summary>
/// <param name="field">Receiver of the requests shared with the field layer (the scene).</param>
/// <param name="harvest">Receiver of the harvest's own drawing requests (the scene).</param>
/// <param name="farString">Zero-terminated string at a far address, without the terminator.</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
public sealed class HarvestHost(Action<FieldEvent> field, Action<HarvestEvent> harvest, Func<ushort, ushort, byte[]> farString, IControlDevices? devices = null)
    : IHarvestHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    public void Emit(FieldEvent e) => field(e);

    public void Emit(HarvestEvent e) => harvest(e);

    // 0E9E:037A – ControlReader.ReadActions
    public void ReadControls(Dgroup d) => ControlReader.ReadActions(d, _devices);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);

    public byte[] FarString(ushort offset, ushort segment) => farString(offset, segment);
}
