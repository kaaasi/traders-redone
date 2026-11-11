namespace Traders.App.Input;

/// <summary>
/// Key state as the original keyboard interrupt keeps it in DGROUP: one byte per scancode (1 = held), the last make
/// code and a wrapping count of make codes. Fed with the raw set-1 bytes a keyboard sends, including the E0h prefix.
/// </summary>
public sealed class KeyStateTable
{
    /// <summary>Number of entries (scancodes 00h..7Fh).</summary>
    public const int Size = 0x80;

    /// <summary>DGROUP offsets of the original fields (DS:3D47 table, DS:1158 last make, DS:115D make count).</summary>
    public const ushort TableOffset = 0x3D47, LastMakeOffset = 0x1158, MakeCountOffset = 0x115D;

    private readonly byte[] _state = new byte[Size];

    /// <summary>Key state per scancode (1 = held, 0 = released).</summary>
    public ReadOnlySpan<byte> State => _state;

    /// <summary>Last make code received (a break code leaves it unchanged).</summary>
    public byte LastMake { get; private set; }

    /// <summary>Number of make codes received, modulo 256.</summary>
    public byte MakeCount { get; private set; }

    public bool IsDown(byte scancode) => _state[scancode & 0x7F] != 0;

    /// <summary>
    /// Processes one byte from the keyboard. A byte with bit 7 clear is a make code, otherwise a break code for
    /// (byte &amp; 7Fh); the E0h prefix of extended keys is therefore a break of key 60h, as in the original.
    /// </summary>
    // [ORIGINAL] 27C6:0043
    public void Receive(byte scancode)
    {
        if ((scancode & 0x80) == 0)
        {
            LastMake = scancode;
            MakeCount++;
            _state[scancode] = 1;
        }
        else
        {
            _state[scancode & 0x7F] = 0;
        }
    }

    public void Receive(ReadOnlySpan<byte> scancodes)
    {
        foreach (byte b in scancodes) Receive(b);
    }

    /// <summary>Releases all keys (the table part of 27C6:00AC; the BIOS key buffer has no counterpart in the host).</summary>
    // [ORIGINAL] 27C6:00AC
    public void Clear() => Array.Clear(_state);

    /// <summary>Takes over a state, e.g. from a data segment image.</summary>
    public void Load(ReadOnlySpan<byte> state, byte lastMake, byte makeCount)
    {
        if (state.Length != Size) throw new ArgumentException($"Expected {Size} bytes.", nameof(state));
        state.CopyTo(_state);
        LastMake = lastMake;
        MakeCount = makeCount;
    }

    /// <summary>Writes the state into a 64 KB data segment image at the original offsets.</summary>
    public void CopyTo(Span<byte> dgroup)
    {
        _state.CopyTo(dgroup[TableOffset..]);
        dgroup[LastMakeOffset] = LastMake;
        dgroup[MakeCountOffset] = MakeCount;
    }
}
