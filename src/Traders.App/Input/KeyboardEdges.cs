using Microsoft.Xna.Framework.Input;

namespace Traders.App.Input;

/// <summary>
/// Turns successive host keyboard snapshots into the scancode bytes a keyboard would have sent in between.
/// </summary>
// [APPROX] The host only shows which keys are held once per update, so changes within one update are ordered here:
// all releases before all presses, each group by key number. Typematic repeat (repeated make codes while a key is
// held) is not generated; the original only reads the held state and make codes, and 0E9E only the held state.
public sealed class KeyboardEdges
{
    private readonly HashSet<Keys> _held = [];

    /// <summary>Keys currently considered held.</summary>
    public IReadOnlyCollection<Keys> Held => _held;

    /// <summary>Compares <paramref name="pressed"/> with the previous snapshot and appends the resulting bytes.</summary>
    public void Update(IEnumerable<Keys> pressed, List<byte> output)
    {
        var now = pressed.Where(k => HostKeyMap.TryGetScancode(k, out _, out _)).ToHashSet();
        foreach (var k in _held.Where(k => !now.Contains(k)).Order().ToList())
        {
            output.AddRange(HostKeyMap.Bytes(k, make: false));
            _held.Remove(k);
        }
        foreach (var k in now.Where(k => !_held.Contains(k)).Order().ToList())
        {
            output.AddRange(HostKeyMap.Bytes(k, make: true));
            _held.Add(k);
        }
    }

    /// <summary>Releases all held keys (e.g. when the window loses focus) and appends their break codes.</summary>
    public void ReleaseAll(List<byte> output) => Update([], output);
}
