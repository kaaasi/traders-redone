namespace Traders.RefEmu.Hardware;

/// <summary>
/// Keyboard controller as seen through ports 60h/61h/64h. Scancodes come from a scripted queue
/// (see <see cref="Enqueue"/>); one byte is presented per IRQ1, the next only after the previous
/// one was read and the interrupt acknowledged, plus a minimum spacing in PIT clocks.
/// </summary>
public sealed class Keyboard
{
    private readonly Queue<byte> _queue = new();
    private byte _data;
    private bool _full;
    private long _nextAllowed;

    /// <summary>Port 61h value (speaker gate, keyboard acknowledge bit).</summary>
    public byte SystemControl { get; set; }

    /// <summary>Minimum distance between two scancodes (≈ 10 ms).</summary>
    public long Spacing { get; set; } = Pit.ClockHz / 100;

    public int QueuedCount => _queue.Count + (_full ? 1 : 0);

    internal void SaveState(BinaryWriter w)
    {
        w.Write(_queue.Count);
        foreach (var b in _queue) w.Write(b);
        w.Write(_data);
        w.Write(_full);
        w.Write(_nextAllowed);
        w.Write(SystemControl);
        w.Write(Spacing);
    }

    internal void LoadState(BinaryReader r)
    {
        _queue.Clear();
        int n = r.ReadInt32();
        for (int i = 0; i < n; i++) _queue.Enqueue(r.ReadByte());
        _data = r.ReadByte();
        _full = r.ReadBoolean();
        _nextAllowed = r.ReadInt64();
        SystemControl = r.ReadByte();
        Spacing = r.ReadInt64();
    }

    public void Enqueue(byte scancode) => _queue.Enqueue(scancode);

    /// <summary>Make and break code of a key (set 1 scancode).</summary>
    public void Press(byte make)
    {
        _queue.Enqueue(make);
        _queue.Enqueue((byte)(make | 0x80));
    }

    /// <summary>Returns true if a new byte was placed in the output buffer (caller raises IRQ1).</summary>
    public bool Poll(long now)
    {
        if (_full || _queue.Count == 0 || now < _nextAllowed) return false;
        _data = _queue.Dequeue();
        _full = true;
        return true;
    }

    public byte Read(ushort port, long now)
    {
        switch (port)
        {
            case 0x60:
                if (_full) { _full = false; _nextAllowed = now + Spacing; }
                return _data;
            case 0x61: return SystemControl;
            default: return (byte)(_full ? 0x15 : 0x14); // 64h: system flag, output buffer full
        }
    }

    public void Write(ushort port, byte value)
    {
        if (port == 0x61) SystemControl = value;
    }
}
