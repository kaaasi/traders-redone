using Traders.RefEmu.Cpu;

namespace Traders.RefEmu.Hardware;

/// <summary>
/// Register-level VGA: four 64 KB planes with the full write/read pipeline (latches, write modes 0-3,
/// rotate, logical function, bit mask, set/reset, read modes 0/1), chain-4 and unchained ("mode X")
/// addressing, CRTC start address/offset for page flipping, DAC palette and a retrace signal derived
/// from emulated time. <see cref="RenderFrame"/> turns the displayed page into indexed pixels.
/// Mapped at A0000-AFFFF.
/// </summary>
public sealed class VgaAdapter : IMemoryDevice
{
    public readonly byte[][] Planes = [new byte[0x10000], new byte[0x10000], new byte[0x10000], new byte[0x10000]];
    public readonly byte[] Seq = new byte[5];
    public readonly byte[] Gc = new byte[9];
    public readonly byte[] Crtc = new byte[25];
    public readonly byte[] Attr = new byte[21];
    /// <summary>256 entries × (r, g, b), 6 bit each.</summary>
    public readonly byte[] Dac = new byte[768];

    public byte Misc { get; set; } = 0x63;
    public byte PelMask { get; set; } = 0xFF;

    private readonly byte[] _latch = new byte[4];
    private int _seqIndex, _gcIndex, _crtcIndex, _attrIndex;
    private bool _attrFlipFlop;
    private int _dacWrite, _dacRead, _dacWriteComp, _dacReadComp;
    private bool _dacReading;

    /// <summary>Emulated time source in PIT clocks (set by the machine).</summary>
    public Func<long> Now { get; set; } = () => 0;

    /// <summary>Frame period in PIT clocks (70 Hz).</summary>
    public long FramePeriod { get; set; } = Pit.ClockHz / 70;

    /// <summary>Number of times the CRTC start address was changed (page flips).</summary>
    public int StartAddressWrites { get; private set; }

    /// <summary>Called for every write to an I/O port this adapter does not model (port, value).</summary>
    public Action<ushort, byte>? UnknownPort { get; set; }

    // ------------------------------------------------------------------ snapshot

    internal void SaveState(BinaryWriter w)
    {
        foreach (var p in Planes) w.Write(p);
        w.Write(Seq);
        w.Write(Gc);
        w.Write(Crtc);
        w.Write(Attr);
        w.Write(Dac);
        w.Write(Misc);
        w.Write(PelMask);
        w.Write(_latch);
        w.Write(_seqIndex);
        w.Write(_gcIndex);
        w.Write(_crtcIndex);
        w.Write(_attrIndex);
        w.Write(_attrFlipFlop);
        w.Write(_dacWrite);
        w.Write(_dacRead);
        w.Write(_dacWriteComp);
        w.Write(_dacReadComp);
        w.Write(_dacReading);
        w.Write(FramePeriod);
        w.Write(StartAddressWrites);
    }

    internal void LoadState(BinaryReader r)
    {
        foreach (var p in Planes) r.ReadExactly(p);
        r.ReadExactly(Seq);
        r.ReadExactly(Gc);
        r.ReadExactly(Crtc);
        r.ReadExactly(Attr);
        r.ReadExactly(Dac);
        Misc = r.ReadByte();
        PelMask = r.ReadByte();
        r.ReadExactly(_latch);
        _seqIndex = r.ReadInt32();
        _gcIndex = r.ReadInt32();
        _crtcIndex = r.ReadInt32();
        _attrIndex = r.ReadInt32();
        _attrFlipFlop = r.ReadBoolean();
        _dacWrite = r.ReadInt32();
        _dacRead = r.ReadInt32();
        _dacWriteComp = r.ReadInt32();
        _dacReadComp = r.ReadInt32();
        _dacReading = r.ReadBoolean();
        FramePeriod = r.ReadInt64();
        StartAddressWrites = r.ReadInt32();
    }

    // ------------------------------------------------------------------ memory

    private bool Chain4 => (Seq[4] & 0x08) != 0;

    public byte Read(uint address)
    {
        int a = (int)(address - 0xA0000) & 0xFFFF;
        if (Chain4)
        {
            int plane = a & 3, off = a & 0xFFFC;
            for (int p = 0; p < 4; p++) _latch[p] = Planes[p][off];
            return Planes[plane][off];
        }
        for (int p = 0; p < 4; p++) _latch[p] = Planes[p][a];
        if ((Gc[5] & 0x08) == 0) return _latch[Gc[4] & 3];
        // read mode 1: colour compare
        int result = 0xFF;
        for (int p = 0; p < 4; p++)
        {
            if ((Gc[7] & (1 << p)) == 0) continue;
            byte want = (Gc[2] & (1 << p)) != 0 ? (byte)0xFF : (byte)0;
            result &= ~(_latch[p] ^ want);
        }
        return (byte)result;
    }

    public void Write(uint address, byte value)
    {
        int a = (int)(address - 0xA0000) & 0xFFFF;
        int planeMask = Seq[2] & 0x0F;
        int off = a;
        if (Chain4)
        {
            planeMask &= 1 << (a & 3);
            off = a & 0xFFFC;
        }
        if (planeMask == 0) return;

        int writeMode = Gc[5] & 3;
        int func = (Gc[3] >> 3) & 3;
        int rot = Gc[3] & 7;
        byte bitMask = Gc[8];
        byte rotated = (byte)((value >> rot) | (value << (8 - rot)));

        for (int p = 0; p < 4; p++)
        {
            if ((planeMask & (1 << p)) == 0) continue;
            byte latch = _latch[p];
            byte result;
            switch (writeMode)
            {
                case 0:
                {
                    byte v = (Gc[1] & (1 << p)) != 0 ? ((Gc[0] & (1 << p)) != 0 ? (byte)0xFF : (byte)0) : rotated;
                    v = Logic(func, v, latch);
                    result = (byte)((v & bitMask) | (latch & ~bitMask));
                    break;
                }
                case 1:
                    result = latch;
                    break;
                case 2:
                {
                    byte v = (value & (1 << p)) != 0 ? (byte)0xFF : (byte)0;
                    v = Logic(func, v, latch);
                    result = (byte)((v & bitMask) | (latch & ~bitMask));
                    break;
                }
                default:
                {
                    byte m = (byte)(rotated & bitMask);
                    byte v = (Gc[0] & (1 << p)) != 0 ? (byte)0xFF : (byte)0;
                    v = Logic(func, v, latch);
                    result = (byte)((v & m) | (latch & ~m));
                    break;
                }
            }
            Planes[p][off] = result;
        }
    }

    private static byte Logic(int func, byte v, byte latch) => func switch
    {
        1 => (byte)(v & latch),
        2 => (byte)(v | latch),
        3 => (byte)(v ^ latch),
        _ => v,
    };

    // ------------------------------------------------------------------ ports

    public static bool Handles(ushort port) => port is >= 0x3C0 and <= 0x3CF or 0x3D4 or 0x3D5 or 0x3DA;

    public byte In(ushort port)
    {
        switch (port)
        {
            case 0x3C0: return (byte)_attrIndex;
            case 0x3C1: return _attrIndex < Attr.Length ? Attr[_attrIndex] : (byte)0;
            case 0x3C2: return 0x00; // input status 0
            case 0x3C4: return (byte)_seqIndex;
            case 0x3C5: return _seqIndex < Seq.Length ? Seq[_seqIndex] : (byte)0xFF;
            case 0x3C6: return PelMask;
            case 0x3C7: return (byte)(_dacReading ? 3 : 0);
            case 0x3C8: return (byte)_dacWrite;
            case 0x3C9:
            {
                byte v = Dac[_dacRead * 3 + _dacReadComp];
                if (++_dacReadComp == 3) { _dacReadComp = 0; _dacRead = (_dacRead + 1) & 0xFF; }
                return v;
            }
            case 0x3CC: return Misc;
            case 0x3CE: return (byte)_gcIndex;
            case 0x3CF: return _gcIndex < Gc.Length ? Gc[_gcIndex] : (byte)0xFF;
            case 0x3D4: return (byte)_crtcIndex;
            case 0x3D5: return _crtcIndex < Crtc.Length ? Crtc[_crtcIndex] : (byte)0xFF;
            case 0x3DA:
            {
                _attrFlipFlop = false;
                return Status();
            }
            default: return 0xFF;
        }
    }

    /// <summary>Input status 1: bit 3 vertical retrace, bit 0 display disabled (blank).</summary>
    public byte Status()
    {
        long t = Now() % FramePeriod;
        long retrace = FramePeriod * 45 / 449;
        bool vr = t >= FramePeriod - retrace;
        long line = FramePeriod / 449;
        bool blank = vr || t % line >= line * 4 / 5;
        return (byte)((vr ? 0x08 : 0) | (blank ? 0x01 : 0));
    }

    /// <summary>True during vertical retrace (for frame capture by the machine).</summary>
    public bool InRetrace => (Status() & 0x08) != 0;

    public void Out(ushort port, byte v)
    {
        switch (port)
        {
            case 0x3C0:
                if (!_attrFlipFlop) _attrIndex = v & 0x1F;
                else if (_attrIndex < Attr.Length) Attr[_attrIndex] = v;
                _attrFlipFlop = !_attrFlipFlop;
                return;
            case 0x3C2: Misc = v; return;
            case 0x3C4: _seqIndex = v; return;
            case 0x3C5: if (_seqIndex < Seq.Length) Seq[_seqIndex] = v; else UnknownPort?.Invoke(port, v); return;
            case 0x3C6: PelMask = v; return;
            case 0x3C7: _dacRead = v; _dacReadComp = 0; _dacReading = true; return;
            case 0x3C8: _dacWrite = v; _dacWriteComp = 0; _dacReading = false; return;
            case 0x3C9:
                Dac[_dacWrite * 3 + _dacWriteComp] = (byte)(v & 0x3F);
                if (++_dacWriteComp == 3) { _dacWriteComp = 0; _dacWrite = (_dacWrite + 1) & 0xFF; }
                return;
            case 0x3CE: _gcIndex = v; return;
            case 0x3CF: if (_gcIndex < Gc.Length) Gc[_gcIndex] = v; else UnknownPort?.Invoke(port, v); return;
            case 0x3D4: _crtcIndex = v; return;
            case 0x3D5:
                if (_crtcIndex < Crtc.Length)
                {
                    if (_crtcIndex is 0x0C or 0x0D) StartAddressWrites++;
                    Crtc[_crtcIndex] = v;
                }
                else UnknownPort?.Invoke(port, v);
                return;
            default:
                UnknownPort?.Invoke(port, v);
                return;
        }
    }

    // ------------------------------------------------------------------ BIOS mode set

    private static readonly byte[] Mode13Crtc =
    [
        0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0xBF, 0x1F, 0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x9C, 0x8E, 0x8F, 0x28, 0x40, 0x96, 0xB9, 0xA3, 0xFF,
    ];

    private static readonly byte[] Mode0DCrtc =
    [
        0x2D, 0x27, 0x28, 0x90, 0x2B, 0x80, 0xBF, 0x1F, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x9C, 0x8E, 0x8F, 0x14, 0x00, 0x96, 0xB9, 0xE3, 0xFF,
    ];

    private static readonly byte[] Mode03Crtc =
    [
        0x5F, 0x4F, 0x50, 0x82, 0x55, 0x81, 0xBF, 0x1F, 0x00, 0x4F, 0x0D, 0x0E, 0x00, 0x00, 0x00, 0x00,
        0x9C, 0x8E, 0x8F, 0x28, 0x1F, 0x96, 0xB9, 0xA3, 0xFF,
    ];

    /// <summary>Programs the registers like the VGA BIOS for modes 03h, 0Dh and 13h; resets the DAC to the default palette.</summary>
    public void SetMode(int mode, bool clear)
    {
        Array.Clear(Gc);
        Array.Clear(Attr);
        Gc[7] = 0x0F; // colour don't care: all planes
        Gc[8] = 0xFF;
        for (int i = 0; i < 16; i++) Attr[i] = (byte)i;
        Attr[0x12] = 0x0F;
        switch (mode)
        {
            case 0x13:
                Seq[0] = 0x03; Seq[1] = 0x01; Seq[2] = 0x0F; Seq[3] = 0x00; Seq[4] = 0x0E;
                Gc[5] = 0x40; Gc[6] = 0x05;
                Attr[0x10] = 0x41;
                Mode13Crtc.CopyTo(Crtc, 0);
                Misc = 0x63;
                break;
            case 0x0D:
                Seq[0] = 0x03; Seq[1] = 0x09; Seq[2] = 0x0F; Seq[3] = 0x00; Seq[4] = 0x06;
                Gc[5] = 0x00; Gc[6] = 0x05;
                Attr[0x10] = 0x01;
                Mode0DCrtc.CopyTo(Crtc, 0);
                Misc = 0x63;
                break;
            default: // text mode 03h (only so that programs can switch back on exit)
                Seq[0] = 0x03; Seq[1] = 0x00; Seq[2] = 0x03; Seq[3] = 0x00; Seq[4] = 0x02;
                Gc[5] = 0x10; Gc[6] = 0x0E;
                Attr[0x10] = 0x0C;
                Mode03Crtc.CopyTo(Crtc, 0);
                Misc = 0x67;
                break;
        }
        if (clear) foreach (var p in Planes) Array.Clear(p);
        SetDefaultPalette(mode == 0x13);
    }

    private void SetDefaultPalette(bool mode13)
    {
        Array.Clear(Dac);
        // 16 EGA colours (as in the standard VGA BIOS tables)
        byte[] ega = [0, 0, 0, 0, 0, 42, 0, 42, 0, 0, 42, 42, 42, 0, 0, 42, 0, 42, 42, 21, 0, 42, 42, 42,
            21, 21, 21, 21, 21, 63, 21, 63, 21, 21, 63, 63, 63, 21, 21, 63, 21, 63, 63, 63, 21, 63, 63, 63];
        Array.Copy(ega, Dac, ega.Length);
        if (!mode13) return;
        // grey ramp 16-31 as in the VGA BIOS; the remaining default colours are not needed for comparisons
        byte[] grey = [0, 5, 8, 11, 14, 17, 20, 24, 28, 32, 36, 40, 45, 50, 56, 63];
        for (int i = 0; i < 16; i++) Dac[(16 + i) * 3] = Dac[(16 + i) * 3 + 1] = Dac[(16 + i) * 3 + 2] = grey[i];
    }

    // ------------------------------------------------------------------ display

    public int StartAddress => (Crtc[0x0C] << 8) | Crtc[0x0D];

    /// <summary>Renders the currently displayed picture as palette indices (after the PEL mask).</summary>
    public (int Width, int Height, byte[] Pixels) RenderFrame()
    {
        bool graphics = (Gc[6] & 1) != 0;
        bool mode256 = (Gc[5] & 0x40) != 0;
        int hChars = Crtc[0x01] + 1;
        int vDisplay = Crtc[0x12] | ((Crtc[0x07] & 0x02) << 7) | ((Crtc[0x07] & 0x40) << 3);
        vDisplay++;
        int rowScan = (Crtc[0x09] & 0x1F) + 1;
        if ((Crtc[0x09] & 0x80) != 0) rowScan *= 2;
        int height = Math.Max(1, vDisplay / rowScan);
        int offset = Crtc[0x13] * 2;
        bool dword = (Crtc[0x14] & 0x40) != 0;
        bool byteMode = (Crtc[0x17] & 0x40) != 0;
        int shift = dword ? 2 : byteMode ? 0 : 1;
        int start = StartAddress;

        if (!graphics) return (0, 0, []);

        if (mode256)
        {
            int width = hChars * 4;
            var px = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int line = start + y * offset;
                for (int x = 0; x < width; x++)
                {
                    int addr = ((line + (x >> 2)) << shift) & 0xFFFF;
                    px[y * width + x] = (byte)(Planes[x & 3][addr] & PelMask);
                }
            }
            return (width, height, px);
        }
        else
        {
            int width = hChars * 8;
            var px = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int line = start + y * offset;
                for (int x = 0; x < width; x++)
                {
                    int addr = ((line + (x >> 3)) << shift) & 0xFFFF;
                    int bit = 7 - (x & 7);
                    int c = 0;
                    for (int p = 0; p < 4; p++) c |= ((Planes[p][addr] >> bit) & 1) << p;
                    c &= Attr[0x12] & 0x0F;
                    px[y * width + x] = (byte)(Attr[c] & 0x3F & PelMask);
                }
            }
            return (width, height, px);
        }
    }
}
