namespace Traders.Vga.Layers;

/// <summary>Helpers 0A69 that enqueue entries of the command list (lines, centred and shadowed text, numbers).</summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:3CA8 – character advance of the current font (set with the font by 21FD:05DB).</summary>
    public const int FontAdvance = 0x3CA8;

    /// <summary>[ORIGINAL] 0A69:000B(x, y, w, colour): horizontal line from x to x + w − 1.</summary>
    public void AddHorizontalLine(ushort x, ushort y, ushort w, ushort color) =>
        AddLine(x, y, (ushort)(x + w - 1), y, color);

    /// <summary>[ORIGINAL] 0A69:002C(x, y, h, colour): vertical line from y to y + h − 1.</summary>
    public void AddVerticalLine(ushort x, ushort y, ushort h, ushort color) =>
        AddLine(x, y, x, (ushort)(y + h - 1), color);

    /// <summary>
    /// [ORIGINAL] 0A69:004D(x, y, far text, colour, shadow colour, ox, oy): the text first at (x + ox, y + oy) in the
    /// shadow colour, then at (x, y) in the colour; both without shadow rectangle (−1).
    /// </summary>
    public void AddShadowText(ushort x, ushort y, ushort textOffset, ushort textSegment, ushort color, ushort shadowColor,
        ushort ox, ushort oy)
    {
        AddText((ushort)(x + ox), (ushort)(y + oy), textOffset, textSegment, shadowColor, 0xFFFF);
        AddText(x, y, textOffset, textSegment, color, 0xFFFF);
    }

    /// <summary>
    /// [ORIGINAL] 0A69:0090(x, y, w, far text, colour, shadow): text centred in w: x + 1 + (w − strlen · [3CA8]) / 2,
    /// the difference shifted as an unsigned word; nothing for a null pointer.
    /// </summary>
    public void AddCenteredText(ushort x, ushort y, ushort w, ushort textOffset, ushort textSegment, ushort color, ushort shadow)
    {
        if (textOffset == 0 && textSegment == 0) return;
        AddText(CenteredX(x, w, textOffset, textSegment), y, textOffset, textSegment, color, shadow);
    }

    /// <summary>[ORIGINAL] 0A69:00D3(x, y, w, far text, colour, shadow colour, ox, oy): centred like 0090, shadowed like 004D.</summary>
    public void AddCenteredShadowText(ushort x, ushort y, ushort w, ushort textOffset, ushort textSegment, ushort color,
        ushort shadowColor, ushort ox, ushort oy)
    {
        if (textOffset == 0 && textSegment == 0) return;
        AddShadowText(CenteredX(x, w, textOffset, textSegment), y, textOffset, textSegment, color, shadowColor, ox, oy);
    }

    /// <summary>[ORIGINAL] 0A69:011B(x, y, number, digits, colour, shadow colour, ox, oy): number (057A) shadowed like 004D.</summary>
    public void AddShadowNumber(ushort x, ushort y, uint number, ushort digits, ushort color, ushort shadowColor, ushort ox, ushort oy)
    {
        AddNumber((ushort)(x + ox), (ushort)(y + oy), number, digits, shadowColor, 0xFFFF);
        AddNumber(x, y, number, digits, color, 0xFFFF);
    }

    /// <summary>[ORIGINAL] 0A69:0164: like 011B with "$" (05FB).</summary>
    public void AddShadowAmount(ushort x, ushort y, uint number, ushort digits, ushort color, ushort shadowColor, ushort ox, ushort oy)
    {
        AddAmount((ushort)(x + ox), (ushort)(y + oy), number, digits, shadowColor, 0xFFFF);
        AddAmount(x, y, number, digits, color, 0xFFFF);
    }

    private ushort CenteredX(ushort x, ushort w, ushort textOffset, ushort textSegment)
    {
        ushort width = (ushort)(ReadString(textSegment, textOffset).Length * (short)U16(FontAdvance));
        return (ushort)(x + 1 + ((ushort)(w - width) >> 1));
    }
}

/// <summary>
/// Effects of the typewriter text (0A69:01AD) outside the graphics layers: the key click (21A7:0054, sound driver)
/// and the wait for the timer flag (21FD:0245). Each is reported exactly once where the original calls it.
/// </summary>
public interface ITypewriterEffects
{
    /// <summary>21A7:0054(a, b) – sound effect (here always (1, 16h)); whether it sounds is decided by the sound layer.</summary>
    void PlaySound(ushort a, ushort b);

    /// <summary>21FD:0245 with DS:3D60 ≠ 0: wait until the timer interrupt has toggled the flag (to 0, to ≠ 0, to 0).</summary>
    void WaitTimerFlag();
}

public sealed partial class GraphicsLayers
{
    /// <summary>DS:0140 – one-character string of the typewriter (0141 is expected to hold 0).</summary>
    public const int TypewriterChar = 0x0140;

    /// <summary>DS:34A4 – line height of the current font (set by 21FD:05DB).</summary>
    public const int FontLineHeight = 0x34A4;

    /// <summary>DS:3D60 – flag toggled by the timer interrupt (21FD:0245 waits only while it is set).</summary>
    public const int TimerFlag = 0x3D60;

    /// <summary>Receiver of the typewriter's sound and timer waits (optional).</summary>
    public ITypewriterEffects? Effects { get; set; }

    /// <summary>
    /// [ORIGINAL] 0A69:01AD(x, y, far text, colour, wait, final wait): one character after another: DS:0140 = character,
    /// list emptied, the character as text at x + i·[3CA8] (no shadow), list drawn, pages flipped, list drawn again
    /// (21FD:060D); a key click for every character except blanks, the timer wait (21FD:0245), then wait retraces. A
    /// null pointer skips the characters. Finally final wait retraces if it is &gt; 0.
    /// </summary>
    public void Typewriter(ushort x, ushort y, ushort textOffset, ushort textSegment, ushort color, ushort wait, ushort finalWait)
    {
        if (textOffset != 0 || textSegment != 0)
        {
            for (ushort i = 0; ReadString(textSegment, textOffset).Length > i; i++)
            {
                byte c = ReadFar(textSegment, (ushort)(textOffset + i));
                Ds[TypewriterChar] = c;
                ClearList();
                AddText((ushort)(x + i * (short)U16(FontAdvance)), y, TypewriterChar, DataSegment, color, 0xFFFF);
                ExecuteList();      // 21FD:060D
                FlipPages();
                ExecuteList();
                if (Ds[TypewriterChar] != (byte)' ') Effects?.PlaySound(1, 0x16);
                if (Ds[TimerFlag] != 0) Effects?.WaitTimerFlag();
                WaitRetraces((short)wait);
            }
        }
        if ((short)finalWait > 0) WaitRetraces((short)finalWait);
    }

    /// <summary>[ORIGINAL] 0A69:0243(x, y, w, far text, colour, wait, final wait): 01AD centred in w like 0090; nothing for a null pointer.</summary>
    public void CenteredTypewriter(ushort x, ushort y, ushort w, ushort textOffset, ushort textSegment, ushort color, ushort wait,
        ushort finalWait)
    {
        if (textOffset == 0 && textSegment == 0) return;
        Typewriter(CenteredX(x, w, textOffset, textSegment), y, textOffset, textSegment, color, wait, finalWait);
    }

    /// <summary>
    /// [ORIGINAL] 0A69:0288(x, y, w, far lines, count, colour, wait): lines i = 0 … count − 1 (signed) from the far
    /// pointer array, each with 0243 at y + i·([34A4] + [34A4]/5) (signed division), final wait −1.
    /// </summary>
    public void TypewriterLines(ushort x, ushort y, ushort w, ushort linesOffset, ushort linesSegment, ushort count, ushort color,
        ushort wait)
    {
        for (short i = 0; i < (short)count; i++)
        {
            short height = (short)U16(FontLineHeight);
            ushort step = (ushort)(height + height / 5);
            ushort at = (ushort)(linesOffset + (ushort)(i << 2));
            ushort lineOffset = ReadFar16(linesSegment, at), lineSegment = ReadFar16(linesSegment, (ushort)(at + 2));
            CenteredTypewriter(x, (ushort)(y + step * i), w, lineOffset, lineSegment, color, wait, 0xFFFF);
        }
    }
}
