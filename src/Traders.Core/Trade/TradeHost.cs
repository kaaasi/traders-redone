using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.State;

namespace Traders.Core.Trade;

/// <summary>
/// The trade's host in the game: the control reader 0E9E:037A and the pause key 21FD:0245 (<see cref="Planting.Pause"/>);
/// drawing and sound go to <paramref name="output"/>.
/// </summary>
/// <param name="output">Receiver of the trade's drawing and sound requests (the scene).</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
public sealed class TradeHost(Action<TradeEvent> output, IControlDevices? devices = null) : ITradeHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    public void Emit(TradeEvent e) => output(e);

    // 0E9E:037A – ControlReader.ReadActions
    public void ReadControls(Dgroup d) => ControlReader.ReadActions(d, _devices);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);
}
