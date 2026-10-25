namespace Traders.RefEmu.Hardware;

/// <summary>Master 8259A interrupt controller (IRQ0-7, base vector 8). Only what DOS programs typically use.</summary>
public sealed class Pic
{
    private byte _irr, _isr, _imr;
    private int _icwStep;     // >0 while an initialization sequence is in progress
    private bool _icw4;
    private bool _readIsr;

    public int VectorBase { get; private set; } = 8;

    public void Raise(int irq) => _irr |= (byte)(1 << irq);

    /// <summary>Highest-priority deliverable IRQ (not masked, nothing of equal/higher priority in service), or -1.</summary>
    public int Pending()
    {
        for (int i = 0; i < 8; i++)
        {
            int bit = 1 << i;
            if ((_isr & bit) != 0) return -1;
            if ((_irr & bit) != 0 && (_imr & bit) == 0) return i;
        }
        return -1;
    }

    /// <summary>CPU acknowledges <paramref name="irq"/>: moves it from request to in-service.</summary>
    public int Acknowledge(int irq)
    {
        _irr &= (byte)~(1 << irq);
        _isr |= (byte)(1 << irq);
        return VectorBase + irq;
    }

    public void Eoi()
    {
        for (int i = 0; i < 8; i++)
        {
            if ((_isr & (1 << i)) != 0) { _isr &= (byte)~(1 << i); return; }
        }
    }

    public bool InService(int irq) => (_isr & (1 << irq)) != 0;

    public byte Read(ushort port)
    {
        if (port == 0x21) return _imr;
        return _readIsr ? _isr : _irr;
    }

    public void Write(ushort port, byte v)
    {
        if (port == 0x20)
        {
            if ((v & 0x10) != 0) { _icwStep = 2; _icw4 = (v & 1) != 0; _imr = 0; _isr = 0; return; } // ICW1
            if ((v & 0x18) == 0) // OCW2
            {
                if ((v & 0x20) != 0)
                {
                    if ((v & 0x40) != 0) _isr &= (byte)~(1 << (v & 7)); // specific EOI
                    else Eoi();
                }
                return;
            }
            if ((v & 0x18) == 0x08 && (v & 2) != 0) _readIsr = (v & 1) != 0; // OCW3
            return;
        }
        switch (_icwStep)
        {
            case 2: VectorBase = v & 0xF8; _icwStep = 3; return;
            case 3: _icwStep = _icw4 ? 4 : 0; return;
            case 4: _icwStep = 0; return;
            default: _imr = v; return;
        }
    }
}
