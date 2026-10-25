using Traders.RefEmu.Cpu;
using Traders.RefEmu.Hardware;

namespace Traders.RefEmu.Machine;

public sealed partial class PcMachine
{
    // ------------------------------------------------------------------ keyboard (IRQ1 default handler, int 16h)

    private static readonly string ScanAscii =
        "\0\u001b1234567890-=\b\tqwertyuiop[]\r\0asdfghjkl;'`\0\\zxcvbnm,./\0*\0 ";

    private void KeyboardIrq()
    {
        byte sc = Keyboard.Read(0x60, Now);
        if ((sc & 0x80) == 0)
        {
            char ch = sc < ScanAscii.Length ? ScanAscii[sc] : '\0';
            BufferPut((ushort)((sc << 8) | ch));
        }
        Pic.Eoi();
    }

    /// <summary>Puts a key (scan &lt;&lt; 8 | ascii) into the BIOS keyboard buffer.</summary>
    public bool BufferPut(ushort key)
    {
        ushort head = Mem.Read16(0x40, 0x1A), tail = Mem.Read16(0x40, 0x1C);
        ushort start = Mem.Read16(0x40, 0x80), end = Mem.Read16(0x40, 0x82);
        ushort next = (ushort)(tail + 2);
        if (next >= end) next = start;
        if (next == head) return false;
        Mem.Write16(0x40, tail, key);
        Mem.Write16(0x40, 0x1C, next);
        return true;
    }

    private bool BufferPeek(out ushort key)
    {
        ushort head = Mem.Read16(0x40, 0x1A), tail = Mem.Read16(0x40, 0x1C);
        key = head == tail ? (ushort)0 : Mem.Read16(0x40, head);
        return head != tail;
    }

    private ushort BufferGet()
    {
        ushort head = Mem.Read16(0x40, 0x1A);
        ushort key = Mem.Read16(0x40, head);
        head += 2;
        if (head >= Mem.Read16(0x40, 0x82)) head = Mem.Read16(0x40, 0x80);
        Mem.Write16(0x40, 0x1A, head);
        return key;
    }

    private void Int16(ushort trapIp)
    {
        byte ah = (byte)(Cpu.Ax >> 8);
        switch (ah)
        {
            case 0x00:
            case 0x10:
                if (!BufferPeek(out _)) { RetryLater(trapIp); return; }
                Cpu.Ax = BufferGet();
                LogEvent(EventKind.Keyboard, $"int 16 read {Cpu.Ax:X4}");
                return;
            case 0x01:
            case 0x11:
            {
                bool any = BufferPeek(out ushort k);
                if (any) Cpu.Ax = k;
                Cpu.SetFlag(Cpu286.ZF, !any);
                return;
            }
            case 0x02:
            case 0x12:
                Cpu.Ax &= 0xFF00;
                return;
            case 0x05:
                Cpu.Ax = (ushort)(Cpu.Ax & 0xFF00 | (BufferPut(Cpu.Cx) ? 0 : 1));
                return;
            default:
                LogEvent(EventKind.Bios, $"int 16 AH={ah:X2} ignored");
                return;
        }
    }

    // ------------------------------------------------------------------ time (int 1Ah)

    /// <summary>Emulated wall-clock time.</summary>
    public DateTime CurrentTime => Options.StartTime + TimeSpan.FromTicks((long)(Now * (TimeSpan.TicksPerSecond / (double)Pit.ClockHz)));

    private static byte Bcd(int v) => (byte)(((v / 10) << 4) | (v % 10));

    private void Int1A()
    {
        byte ah = (byte)(Cpu.Ax >> 8);
        var t = CurrentTime;
        switch (ah)
        {
            case 0x00:
                Cpu.Dx = Mem.Read16(0x40, 0x6C);
                Cpu.Cx = Mem.Read16(0x40, 0x6E);
                Cpu.Ax = (ushort)(Cpu.Ax & 0xFF00 | Mem.Read8(0x40, 0x70));
                Mem.Write8(0x40, 0x70, 0);
                LogEvent(EventKind.Clock, $"int 1A ticks {Cpu.Cx:X4}:{Cpu.Dx:X4}");
                return;
            case 0x02:
                Cpu.Cx = (ushort)((Bcd(t.Hour) << 8) | Bcd(t.Minute));
                Cpu.Dx = (ushort)(Bcd(t.Second) << 8);
                SetCarry(false);
                LogEvent(EventKind.Clock, $"int 1A RTC time {t:HH:mm:ss}");
                return;
            case 0x04:
                Cpu.Cx = (ushort)((Bcd(t.Year / 100) << 8) | Bcd(t.Year % 100));
                Cpu.Dx = (ushort)((Bcd(t.Month) << 8) | Bcd(t.Day));
                SetCarry(false);
                LogEvent(EventKind.Clock, $"int 1A RTC date {t:yyyy-MM-dd}");
                return;
            default:
                LogEvent(EventKind.Bios, $"int 1A AH={ah:X2} ignored");
                return;
        }
    }

    // ------------------------------------------------------------------ video (int 10h)

    private void SetVideoBda(int mode)
    {
        Mem.Write8(0x40, 0x49, (byte)mode);
        Mem.Write16(0x40, 0x4A, (ushort)(mode == 0x13 || mode == 0x0D ? 40 : 80));
        Mem.Write16(0x40, 0x4C, (ushort)(mode == 0x13 ? 0xFA00 : mode == 0x0D ? 0x2000 : 0x1000));
        Mem.Write16(0x40, 0x4E, 0);
        Mem.Write16(0x40, 0x50, 0);
        Mem.Write8(0x40, 0x62, 0);
        Mem.Write8(0x40, 0x84, 24);
    }

    private void Int10()
    {
        var c = Cpu;
        byte ah = (byte)(c.Ax >> 8), al = (byte)c.Ax;
        switch (ah)
        {
            case 0x00:
            {
                int mode = al & 0x7F;
                if (mode is not (0x03 or 0x0D or 0x13))
                {
                    LogEvent(EventKind.Video, $"set mode {mode:X2} not supported");
                    throw new EmulationException($"Video mode {mode:X2} not supported");
                }
                Vga.SetMode(mode, (al & 0x80) == 0);
                SetVideoBda(mode);
                if (mode == 0x03 && (al & 0x80) == 0)
                    for (int i = 0; i < 80 * 25; i++) Mem.Write16(0xB800, (ushort)(i * 2), 0x0720);
                LogEvent(EventKind.Video, $"set mode {al:X2}");
                return;
            }
            case 0x02:
                Mem.Write16(0x40, (ushort)(0x50 + (c.Bx >> 8) * 2), c.Dx);
                return;
            case 0x03:
                c.Dx = Mem.Read16(0x40, (ushort)(0x50 + (c.Bx >> 8) * 2));
                c.Cx = 0x0607;
                return;
            case 0x05:
                Mem.Write8(0x40, 0x62, al);
                LogEvent(EventKind.Video, $"select page {al:X2}");
                return;
            case 0x08:
                c.Ax = 0x0720;
                return;
            case 0x09:
            case 0x0A:
                LogEvent(EventKind.Console, $"int 10 write char '{(char)al}' x{c.Cx}");
                return;
            case 0x0B:
                LogEvent(EventKind.Video, $"int 10 AH=0B BX={c.Bx:X4} ignored");
                return;
            case 0x0E:
                LogEvent(EventKind.Console, $"teletype '{(al >= 32 ? (char)al : '.')}' ({al:X2})");
                return;
            case 0x0F:
                c.Ax = (ushort)((Mem.Read8(0x40, 0x4A) << 8) | Mem.Read8(0x40, 0x49));
                c.Bx = (ushort)((Mem.Read8(0x40, 0x62) << 8) | (c.Bx & 0xFF));
                return;
            case 0x10:
                Int10Palette(al);
                return;
            case 0x12:
                if ((byte)c.Bx == 0x10)
                {
                    c.Bx = 0x0003; // colour, 256 KB
                    c.Cx = 0x0009;
                }
                else LogEvent(EventKind.Video, $"int 10 AH=12 BL={(byte)c.Bx:X2} ignored");
                return;
            case 0x1A:
                if (al == 0x00) { c.Ax = (ushort)(c.Ax & 0xFF00 | 0x1A); c.Bx = 0x0008; } // VGA with colour display
                return;
            default:
                LogEvent(EventKind.Video, $"int 10 AX={c.Ax:X4} BX={c.Bx:X4} ignored");
                return;
        }
    }

    private void Int10Palette(byte al)
    {
        var c = Cpu;
        switch (al)
        {
            case 0x00:
                Vga.Attr[c.Bx & 0x0F] = (byte)(c.Bx >> 8);
                return;
            case 0x01:
                Vga.Attr[0x11] = (byte)(c.Bx >> 8);
                return;
            case 0x02:
                for (int i = 0; i < 16; i++) Vga.Attr[i] = Mem.Read8(c.Es, (ushort)(c.Dx + i));
                Vga.Attr[0x11] = Mem.Read8(c.Es, (ushort)(c.Dx + 16));
                return;
            case 0x03:
                return;
            case 0x07:
                c.Bx = (ushort)((Vga.Attr[c.Bx & 0x1F] << 8) | (c.Bx & 0xFF));
                return;
            case 0x10:
            {
                int i = c.Bx & 0xFF;
                Vga.Dac[i * 3] = (byte)((c.Dx >> 8) & 0x3F);
                Vga.Dac[i * 3 + 1] = (byte)((c.Cx >> 8) & 0x3F);
                Vga.Dac[i * 3 + 2] = (byte)(c.Cx & 0x3F);
                return;
            }
            case 0x12:
                for (int n = 0; n < c.Cx; n++)
                {
                    int i = (c.Bx + n) & 0xFF;
                    for (int k = 0; k < 3; k++)
                        Vga.Dac[i * 3 + k] = (byte)(Mem.Read8(c.Es, (ushort)(c.Dx + n * 3 + k)) & 0x3F);
                }
                return;
            case 0x15:
            {
                int i = c.Bx & 0xFF;
                c.Dx = (ushort)((Vga.Dac[i * 3] << 8) | (c.Dx & 0xFF));
                c.Cx = (ushort)((Vga.Dac[i * 3 + 1] << 8) | Vga.Dac[i * 3 + 2]);
                return;
            }
            case 0x17:
                for (int n = 0; n < c.Cx; n++)
                {
                    int i = (c.Bx + n) & 0xFF;
                    for (int k = 0; k < 3; k++) Mem.Write8(c.Es, (ushort)(c.Dx + n * 3 + k), Vga.Dac[i * 3 + k]);
                }
                return;
            default:
                LogEvent(EventKind.Video, $"int 10 AX=10{al:X2} ignored");
                return;
        }
    }
}
