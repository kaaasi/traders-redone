using System.Globalization;

namespace Traders.Vga.Layers;

/// <summary>Memory outside the data segment, addressed by far pointers (segment:offset of the original).</summary>
public interface IFarMemory
{
    byte Read(ushort segment, ushort offset);

    void Write(ushort segment, ushort offset, byte value);
}

/// <summary>
/// Draw command list 0A96: entries of 1Ch bytes collected at the far pointer DS:15BA (count DS:15BE), executed in
/// order by 0A96:06A8 through the handler table DS:01F4 (17 far pointers, type word at entry + 1Ah).
/// </summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:0142 – character advance for text shadows (signed byte); DS:0143 – shadow height (signed byte).</summary>
    public const int TextAdvance = 0x0142, ShadowHeight = 0x0143;

    /// <summary>DS:0154 – EGA colour map, 16 bytes per palette DS:09C8 (used when DS:090E ≠ 1).</summary>
    public const int EgaColorMap = 0x0154, EgaPalette = 0x09C8;

    /// <summary>DS:01F4 – far pointers to the 17 handlers (types 0…16).</summary>
    public const int HandlerTable = 0x01F4;

    /// <summary>DS:15BA/15BC – far pointer to the next entry; DS:15BE – number of entries; DS:15C0 – first entry.</summary>
    public const int ListPointer = 0x15BA, ListCount = 0x15BE, ListStart = 0x15C0;

    /// <summary>Size of one entry.</summary>
    public const int EntrySize = 0x1C;

    /// <summary>DS:1126 – fill character for leading blanks of numbers (2295:0062); DS:1127 – format "%*lu".</summary>
    public const int NumberFill = 0x1126;

    /// <summary>Segment value of the data segment in the original's address space (far pointers into DS).</summary>
    public ushort DataSegment { get; set; }

    /// <summary>Memory behind far pointers that do not point into the data segment (optional).</summary>
    public IFarMemory? OtherMemory { get; set; }

    private byte ReadFar(ushort segment, ushort offset)
    {
        if (segment == DataSegment) return Ds[offset];
        return OtherMemory is { } m ? m.Read(segment, offset)
            : throw new InvalidOperationException($"Far pointer {segment:X4}:{offset:X4} outside the data segment without OtherMemory.");
    }

    private void WriteFar(ushort segment, ushort offset, byte value)
    {
        if (segment == DataSegment) Ds[offset] = value;
        else if (OtherMemory is { } m) m.Write(segment, offset, value);
        else throw new InvalidOperationException($"Far pointer {segment:X4}:{offset:X4} outside the data segment without OtherMemory.");
    }

    private ushort ReadFar16(ushort segment, ushort offset) =>
        (ushort)(ReadFar(segment, offset) | (ReadFar(segment, (ushort)(offset + 1)) << 8));

    private void WriteFar16(ushort segment, ushort offset, ushort value)
    {
        WriteFar(segment, offset, (byte)value);
        WriteFar(segment, (ushort)(offset + 1), (byte)(value >> 8));
    }

    /// <summary>Zero-terminated string at a far address (bytes before the 0).</summary>
    private byte[] ReadString(ushort segment, ushort offset)
    {
        var bytes = new List<byte>();
        for (ushort o = offset; ; o++)
        {
            byte b = ReadFar(segment, o);
            if (b == 0) return [.. bytes];
            bytes.Add(b);
            if (bytes.Count > 0x10000) throw new InvalidOperationException("String without terminator.");
        }
    }

    // ------------------------------------------------------------------ list management

    /// <summary>[ORIGINAL] 0A96:000A(advance, height): DS:0142/0143 for the text shadow (bytes).</summary>
    public void SetTextShadow(ushort advance, ushort height)
    {
        Ds[TextAdvance] = (byte)advance;
        Ds[ShadowHeight] = (byte)height;
    }

    /// <summary>[ORIGINAL] 0A96:001B: empty the list (count 0, pointer DS:15C0).</summary>
    public void ClearList()
    {
        Set16(ListCount, 0);
        Set16(ListPointer, ListStart);
        Set16(ListPointer + 2, DataSegment);
    }

    /// <summary>
    /// [ORIGINAL] 0A96:06A8: execute all entries from DS:15C0 (pointer and count are reset first, then advance with each
    /// entry). The list is not emptied: afterwards count and pointer stand behind the last entry again, so a second
    /// call draws the same entries once more (21FD:060D uses this for both pages).
    /// </summary>
    public void ExecuteList()
    {
        short n = (short)U16(ListCount);
        Set16(ListPointer, ListStart);
        Set16(ListPointer + 2, DataSegment);
        Set16(ListCount, 0);
        while ((short)U16(ListCount) < n)
        {
            ushort type = Field(0x1A);
            ushort handler = U16(HandlerTable + (ushort)(type << 2));
            ushort handlerSegment = U16(HandlerTable + (ushort)(type << 2) + 2);
            Execute(handlerSegment, handler);
            Set16(ListPointer, (ushort)(U16(ListPointer) + EntrySize));
            Set16(ListCount, (ushort)(U16(ListCount) + 1));
        }
    }

    private ushort EntrySegment => U16(ListPointer + 2);

    private ushort EntryOffset => U16(ListPointer);

    private ushort Field(int at) => ReadFar16(EntrySegment, (ushort)(EntryOffset + at));

    private void SetField(int at, ushort value) => WriteFar16(EntrySegment, (ushort)(EntryOffset + at), value);

    /// <summary>Appends an entry: the given fields, then the type at + 1Ah, count + 1, pointer + 1Ch.</summary>
    private void Enqueue(ushort type, params (int At, ushort Value)[] fields)
    {
        foreach (var (at, value) in fields) SetField(at, value);
        SetField(0x1A, type);
        Advance();
    }

    private void Advance()
    {
        Set16(ListCount, (ushort)(U16(ListCount) + 1));
        Set16(ListPointer, (ushort)(U16(ListPointer) + EntrySize));
    }

    // ------------------------------------------------------------------ enqueue (one routine per entry type)

    /// <summary>[ORIGINAL] 0A96:0030: type 0, blit RAM atlas → page.</summary>
    public void AddBlit(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy) =>
        Enqueue(0, (0x0, sx), (0x2, sy), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy));

    /// <summary>[ORIGINAL] 0A96:0089: type 1, copy RAM atlas → RAM atlas.</summary>
    public void AddAtlasCopy(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy) =>
        Enqueue(1, (0x0, sx), (0x2, sy), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy));

    /// <summary>[ORIGINAL] 0A96:00E2: type 2, copy page → page.</summary>
    public void AddPageCopy(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy) =>
        Enqueue(2, (0x0, sx), (0x2, sy), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy));

    /// <summary>[ORIGINAL] 0A96:013B: type 3, masked blit (mask at mx, my).</summary>
    public void AddMaskedBlit(ushort sx, ushort sy, ushort mx, ushort my, ushort w, ushort h, ushort dx, ushort dy) =>
        Enqueue(3, (0x0, sx), (0x2, sy), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy), (0x8, mx), (0xA, my));

    /// <summary>[ORIGINAL] 0A96:01AA: type 4, fill the mask in a colour.</summary>
    public void AddMaskFill(ushort mx, ushort my, ushort w, ushort h, ushort dx, ushort dy, ushort color) =>
        Enqueue(4, (0x0, mx), (0x2, my), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy), (0x10, color));

    /// <summary>[ORIGINAL] 0A96:020E: type 5, OR blit.</summary>
    public void AddOrBlit(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy) =>
        Enqueue(5, (0x0, sx), (0x2, sy), (0xC, w), (0xE, h), (0x4, dx), (0x6, dy));

    /// <summary>[ORIGINAL] 0A96:0267: type 6, area w × h tiled with the tile tw × th.</summary>
    public void AddTiles(ushort sx, ushort sy, ushort tw, ushort th, ushort dx, ushort dy, ushort w, ushort h) =>
        Enqueue(6, (0x0, sx), (0x2, sy), (0xC, tw), (0xE, th), (0x4, dx), (0x6, dy), (0x8, w), (0xA, h));

    /// <summary>[ORIGINAL] 0A96:0311: type 7, blit 8 pixels wide.</summary>
    public void AddBlit8(ushort sx, ushort sy, ushort h, ushort dx, ushort dy) => AddFixed(7, sx, sy, h, dx, dy);

    /// <summary>[ORIGINAL] 0A96:035F: type 8, blit 16 pixels wide.</summary>
    public void AddBlit16(ushort sx, ushort sy, ushort h, ushort dx, ushort dy) => AddFixed(8, sx, sy, h, dx, dy);

    /// <summary>[ORIGINAL] 0A96:03AD: type 16, blit 32 pixels wide.</summary>
    public void AddBlit32(ushort sx, ushort sy, ushort h, ushort dx, ushort dy) => AddFixed(16, sx, sy, h, dx, dy);

    private void AddFixed(ushort type, ushort sx, ushort sy, ushort h, ushort dx, ushort dy) =>
        Enqueue(type, (0x0, sx), (0x2, sy), (0xE, h), (0x4, dx), (0x6, dy));

    /// <summary>[ORIGINAL] 0A96:02D6: five type-7 blits side by side (source and target x + 8 each).</summary>
    public void AddBlit40(ushort sx, ushort sy, ushort h, ushort dx, ushort dy)
    {
        for (int i = 0; i < 5; i++)
        {
            AddBlit8(sx, sy, h, dx, dy);
            sx = (ushort)(sx + 8);
            dx = (ushort)(dx + 8);
        }
    }

    /// <summary>[ORIGINAL] 0A96:03FB: type 15, pixel.</summary>
    public void AddPixel(ushort x, ushort y, ushort color) => Enqueue(15, (0x0, x), (0x2, y), (0x10, color));

    /// <summary>[ORIGINAL] 0A96:0433: type 9, line.</summary>
    public void AddLine(ushort x1, ushort y1, ushort x2, ushort y2, ushort color) =>
        Enqueue(9, (0x0, x1), (0x2, y1), (0x4, x2), (0x6, y2), (0x10, color));

    /// <summary>[ORIGINAL] 0A96:0481: type 10, rectangle outline (corners x, y and x + w − 1, y + h − 1).</summary>
    public void AddFrame(ushort x, ushort y, ushort w, ushort h, ushort color) =>
        Enqueue(10, (0x0, x), (0x2, y), (0x4, (ushort)(x + w - 1)), (0x6, (ushort)(y + h - 1)), (0x10, color));

    /// <summary>[ORIGINAL] 0A96:04D7: type 11, filled rectangle.</summary>
    public void AddFilledRect(ushort x, ushort y, ushort w, ushort h, ushort color) =>
        Enqueue(11, (0x0, x), (0x2, y), (0xC, w), (0xE, h), (0x10, color));

    /// <summary>[ORIGINAL] 0A96:0525: type 12, text at a far address with colour and shadow colour (&lt; 0 = none).</summary>
    public void AddText(ushort x, ushort y, ushort textOffset, ushort textSegment, ushort color, ushort shadow) =>
        Enqueue(12, (0x12, textOffset), (0x14, textSegment), (0x0, x), (0x2, y), (0x10, color), (0x8, shadow));

    /// <summary>
    /// [ORIGINAL] 0A96:057A: type 13, number as text: the digits are formatted by 2295:0001 into the entry itself
    /// (from + 12h, terminated after <paramref name="digits"/> characters, which may run into the next entry).
    /// </summary>
    public void AddNumber(ushort x, ushort y, uint number, ushort digits, ushort color, ushort shadow) =>
        AddNumber(x, y, number, digits, color, shadow, dollar: false);

    /// <summary>[ORIGINAL] 0A96:05FB: like 057A with a leading "$" (digits from + 13h).</summary>
    public void AddAmount(ushort x, ushort y, uint number, ushort digits, ushort color, ushort shadow) =>
        AddNumber(x, y, number, digits, color, shadow, dollar: true);

    private void AddNumber(ushort x, ushort y, uint number, ushort digits, ushort color, ushort shadow, bool dollar)
    {
        ushort seg = EntrySegment, text = (ushort)(EntryOffset + 0x12);
        SetField(0x0, x);
        SetField(0x2, y);
        SetField(0x10, color);
        SetField(0x8, shadow);
        SetField(0x4, digits);
        SetField(0x1A, 13);
        if (dollar)
        {
            WriteFar(seg, text, (byte)'$');
            text++;
        }
        FormatNumber(number, digits, text, seg);
        WriteFar(seg, (ushort)(text + digits), 0);
        Advance();
    }

    /// <summary>[ORIGINAL] 0A96:0686: type 14, line pattern.</summary>
    public void AddLinePattern(ushort pattern) => Enqueue(14, (0x0, pattern));

    /// <summary>[ORIGINAL] 2295:0062(c): DS:1126 = fill character for numbers.</summary>
    public void SetNumberFill(ushort c) => Ds[NumberFill] = (byte)c;

    /// <summary>
    /// [ORIGINAL] 2295:0001(number, width, far target): sprintf("%*lu", width, number) into a 22-byte stack buffer,
    /// leading blanks and zeros (at most width − 1) replaced by DS:1126, then exactly width characters copied (a longer
    /// number loses its last digits). Widths above 21 overflow the original's buffer and are not supported.
    /// </summary>
    public void FormatNumber(uint number, ushort width, ushort targetOffset, ushort targetSegment)
    {
        short w = (short)width;
        if (w > 21) throw new ArgumentOutOfRangeException(nameof(width), "The original's buffer holds 22 bytes.");
        string s = number.ToString(CultureInfo.InvariantCulture);
        s = w >= 0 ? s.PadLeft(w) : s.PadRight(-w);
        var buffer = new byte[Math.Max(s.Length + 1, 22)];
        for (int i = 0; i < s.Length; i++) buffer[i] = (byte)s[i];
        for (int i = 0; w - 1 > i; i++)
        {
            if (buffer[i] is (byte)' ' or (byte)'0') buffer[i] = Ds[NumberFill];
            else break;
        }
        for (int i = 0; i < w; i++) WriteFar(targetSegment, (ushort)(targetOffset + i), buffer[i]);
    }

    // ------------------------------------------------------------------ execution (handlers of DS:01F4)

    private void Execute(ushort segment, ushort offset)
    {
        if (segment != 0x0A96 && segment != (ushort)(DataSegment - 0x28DC + 0x0A96))
            throw new NotSupportedException($"Handler {segment:X4}:{offset:X4} outside 0A96.");
        switch (offset)
        {
            case 0x06E4: Driver.BlitAtlasToPage(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x0720: Driver.CopyAtlasToAtlas(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x075C: Driver.CopyPageToPage(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x0798:
                Driver.MaskedBlit(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6), Field(0x8), Field(0xA));
                break;
            case 0x07E4:
                Driver.MaskFill(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6), MapColor(Field(0x10)));
                break;
            case 0x0848: Driver.OrAtlasToPage(Field(0x0), Field(0x2), Field(0xC), Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x0884: ExecuteTiles(); break;
            case 0x094F: Driver.BlitAtlasToPageFixed(Field(0x0), Field(0x2), 1, Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x0987: Driver.BlitAtlasToPageFixed(Field(0x0), Field(0x2), 2, Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x09BF: Driver.BlitAtlasToPageFixed(Field(0x0), Field(0x2), 4, Field(0xE), Field(0x4), Field(0x6)); break;
            case 0x09F7:
                Driver.Color = MapColor(Field(0x10));
                Driver.DrawLine(Field(0x0), Field(0x2), Field(0x4), Field(0x6));
                break;
            case 0x0A51: ExecuteFrame(); break;
            case 0x0B20:
                Driver.Color = MapColor(Field(0x10));
                Driver.FillRect(Field(0x0), Field(0x2), Field(0xC), Field(0xE));
                break;
            case 0x0B7A: DrawText(Field(0x0), Field(0x2), Field(0x12), Field(0x14), Field(0x10), Field(0x8)); break;
            case 0x0BB2: DrawText(Field(0x0), Field(0x2), (ushort)(EntryOffset + 0x12), EntrySegment, Field(0x10), Field(0x8)); break;
            case 0x0BE9: Driver.LinePattern = Field(0x0); break;
            case 0x0BFB: Driver.SetPixel(Field(0x0), Field(0x2), MapColor(Field(0x10))); break;
            default: throw new NotSupportedException($"Handler 0A96:{offset:X4} is not a list handler.");
        }
    }

    /// <summary>
    /// [ORIGINAL] 0A96:07E4 and others: on cards other than VGA the colour is mapped through DS:0154 + 16·[09C8] +
    /// (colour ∧ 0Fh) (signed byte); VGA uses it unchanged.
    /// </summary>
    private ushort MapColor(ushort color)
    {
        if (IsVga) return color;
        ushort at = (ushort)((U16(EgaPalette) << 4) + (color & 0x0F));
        return (ushort)(short)(sbyte)Ds[(EgaColorMap + at) & 0xFFFF];
    }

    /// <summary>
    /// [ORIGINAL] 0A96:0884: rows si = 0, th, 2·th … while si &lt; h and columns di = 0, tw … while di &lt; w (signed);
    /// the last row/column is cut to h − si / w − di; each cell is a type-0 blit to (dx + di, dy + si).
    /// </summary>
    private void ExecuteTiles()
    {
        ushort sx = Field(0x0), sy = Field(0x2), dx = Field(0x4), dy = Field(0x6);
        ushort tw = Field(0xC), th = Field(0xE), w = Field(0x8), h = Field(0xA);
        for (ushort si = 0; (short)si < (short)h; si = (ushort)(si + th))
        {
            ushort rowH = (short)(ushort)(si + th) > (short)h ? (ushort)(h - si) : th;
            for (ushort di = 0; (short)di < (short)w; di = (ushort)(di + tw))
            {
                ushort colW = (short)(ushort)(di + tw) > (short)w ? (ushort)(w - di) : tw;
                Driver.BlitAtlasToPage(sx, sy, colW, rowH, (ushort)(dx + di), (ushort)(dy + si));
            }
        }
    }

    /// <summary>[ORIGINAL] 0A96:0A51: four lines (top, right, bottom, left) between the stored corners.</summary>
    private void ExecuteFrame()
    {
        Driver.Color = MapColor(Field(0x10));
        ushort x1 = Field(0x0), y1 = Field(0x2), x2 = Field(0x4), y2 = Field(0x6);
        Driver.DrawLine(x1, y1, x2, y1);
        Driver.DrawLine(x2, y1, x2, y2);
        Driver.DrawLine(x2, y2, x1, y2);
        Driver.DrawLine(x1, y2, x1, y1);
    }

    /// <summary>
    /// [ORIGINAL] 0A96:0C68(x, y, far text, colour, shadow): nothing for a null pointer. Colours mapped on non-VGA
    /// cards (the shadow only if ≥ 0). Shadow ≥ 0: rectangle (x, y − [0143] + 1, strlen · [0142], [0143]) in the shadow
    /// colour, both bytes signed; then the text in the colour (23CD:05DD).
    /// </summary>
    public void DrawText(ushort x, ushort y, ushort textOffset, ushort textSegment, ushort color, ushort shadow)
    {
        if (textOffset == 0 && textSegment == 0) return;
        if (!IsVga)
        {
            color = MapColor(color);
            if ((short)shadow >= 0) shadow = MapColor(shadow);
        }
        var text = ReadString(textSegment, textOffset);
        if ((short)shadow >= 0)
        {
            Driver.Color = shadow;
            short height = (sbyte)Ds[ShadowHeight], advance = (sbyte)Ds[TextAdvance];
            Driver.FillRect(x, (ushort)(y - height + 1), (ushort)(text.Length * advance), (ushort)height);
        }
        Driver.Color = color;
        Driver.DrawText(x, y, text);
    }

    /// <summary>
    /// [ORIGINAL] 0A96:0C3F(i): copy the hidden page – 0B67:0299 with the low word of the page number DS:40DE[i]; on
    /// the Tandy cards (2, 63h) 21FD:051F first.
    /// </summary>
    public void CopyGamePage(ushort index)
    {
        ushort mode = U16(CardMode);
        if (mode is 2 or 0x63) throw new NotSupportedException($"Display mode {mode}: only VGA is ported.");
        CopyPage(U16(PageNumbers + (ushort)(index << 2)));
    }
}
