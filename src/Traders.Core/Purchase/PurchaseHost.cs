using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;

namespace Traders.Core.Purchase;

/// <summary>
/// The purchase's host in the game: the control reader 0E9E:037A and the pause key 21FD:0245 (<see cref="Planting.Pause"/>);
/// drawing and sound go to <paramref name="output"/> (purchase requests) and <paramref name="field"/> (requests of the field
/// routines and the common frame routines).
/// </summary>
/// <param name="output">Receiver of the purchase's own drawing and sound requests (the scene).</param>
/// <param name="field">Receiver of the field and frame requests (the scene).</param>
/// <param name="texts">Memory behind the far text pointers DS:37D0 + 4k (the text buffer of the program start).</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
public sealed class PurchaseHost(Action<PurchaseEvent> output, Action<FieldEvent> field, ITextMemory texts,
    IControlDevices? devices = null) : IPurchaseHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    public ITextMemory Texts => texts;

    public void Emit(PurchaseEvent e) => output(e);

    public void Emit(FieldEvent e) => field(e);

    public void ComputerTurn(Dgroup d, short x, short y) =>
        throw new InvalidOperationException("08CF calls no computer turn of the field (135D:000E).");

    // 0E9E:037A – ControlReader.ReadActions
    public void ReadControls(Dgroup d) => ControlReader.ReadActions(d, _devices);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);
}
