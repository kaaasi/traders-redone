namespace Traders.RefEmu.Hardware;

/// <summary>
/// Silent OPL2 (AdLib) at ports 388h/389h: keeps the register file and emulates the status/timer
/// behaviour that detection routines check, so programs find a card. Register writes are counted
/// and can be observed (for the sound work package); no sound is produced.
/// </summary>
public sealed class AdLibStub
{
    public readonly byte[] Registers = new byte[256];
    private int _index;
    private long _timer1Start = -1, _timer2Start = -1;
    private byte _flags;

    public long WriteCount { get; private set; }

    /// <summary>Called for every register write (register, value).</summary>
    public Action<int, byte>? RegisterWritten { get; set; }

    public byte ReadStatus(long now)
    {
        // timer 1 ticks every 80 µs, timer 2 every 320 µs (PIT clocks: 1.193182 per µs)
        if (_timer1Start >= 0 && now - _timer1Start >= (256 - Registers[2]) * 80 * Pit.ClockHz / 1_000_000)
            if ((Registers[4] & 0x40) == 0) _flags |= 0xC0;
        if (_timer2Start >= 0 && now - _timer2Start >= (256 - Registers[3]) * 320 * Pit.ClockHz / 1_000_000)
            if ((Registers[4] & 0x20) == 0) _flags |= 0xA0;
        return (byte)(_flags | 0x06);
    }

    public void WriteIndex(byte v) => _index = v;

    public void WriteData(byte v, long now)
    {
        WriteCount++;
        Registers[_index] = v;
        RegisterWritten?.Invoke(_index, v);
        if (_index != 4) return;
        if ((v & 0x80) != 0) { _flags = 0; return; }
        _timer1Start = (v & 1) != 0 ? now : -1;
        _timer2Start = (v & 2) != 0 ? now : -1;
    }
}
