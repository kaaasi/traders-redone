using Traders.Core.State;

namespace Traders.Core.Startup;

/// <summary>
/// The text plaque 192D:16AF(h, step, texts, extra) of lane menue: the code request shows its four texts in it
/// (192D:16AF(50h, 1Eh, DS:10E0, −3) at 21FD:0771). Drawing it draws random numbers (wall and rivets 192D:131B/138F/14B4,
/// plaque 192D:14FB) and sets the plaque top DS:5334; that part is the core's and comes with lane menue.
/// </summary>
public interface ITextPlaque
{
    void Show(Dgroup d, OriginalRandom random, short height, short step, int texts, short extra);
}

/// <summary>The code the request shows (letter and column) and the four digits 21FD:08D3 expects.</summary>
public sealed record CodeRequest(int Row, int Column, char Letter, IReadOnlyList<int> Digits)
{
    /// <summary>Text the original shows behind "CODE: ", e.g. "C-12" or "A-5".</summary>
    public string Code => $"{Letter}-{Column}";
}

/// <summary>
/// The code request 21FD:0687 ("INSERT COIN!") reduced to its state changes (decision F1 = B, finding
/// docs/routines/21FD_0687_codeabfrage.md): re-seed random(0) from the clock, load the request picture, decode the four
/// texts, draw random(3FFh), write the code into the text, show the plaque (lane menue). The picture, the boxes and the
/// four key presses are left out: the original accepts every answer.
/// </summary>
// [ORIGINAL] 21FD:0687 – random(0); 186C:0002([08DA]); 21FD:081D; x = random(3FFh) & 3FCh; row = x % 32, col = x / 32;
// digits 21FD:08D3(row + i, col); text [DS:10E8]: +6 = 'A' + row/4, +8/+9 = col (one or two digits, then blank);
// plaque 192D:16AF(50h, 1Eh, DS:10E0, -3)
// [APPROX] F1 = B: no picture, no drawing of the plaque (its random draws stay, ITextPlaque), no palette fade
// (21FD:026C/02C9/0329), no boxes (21FD:0863) and no waiting for keys (DS:1158, key table DS:3D47); the comparison of the
// digits (21FD:07F8) has no effect in the original.
public static class CodeQuery
{
    /// <summary>DS:10E0 – four far pointers to the coded texts (offsets in the data segment).</summary>
    public const int Texts = 0x10E0;

    /// <summary>DS:10E8 – pointer to the text "CODE: X-nn" (third of the four).</summary>
    public const int CodeText = 0x10E8;

    public const int TextCount = 4;

    /// <summary>Arguments of the plaque call 192D:16AF at 21FD:0771.</summary>
    public const short PlaqueHeight = 0x50, PlaqueStep = 0x1E, PlaqueExtra = -3;

    /// <summary>Byte added to every character by 21FD:081D.</summary>
    public const byte Key = 0xFD;

    /// <summary>Runs the request on the data segment with the game's generator; returns the code shown.</summary>
    public static CodeRequest Run(Dgroup d, OriginalRandom random, Func<int, ImageFileLengths> files, ITextPlaque plaque)
    {
        random.Next(0);
        ImageTable.RecordLoad(d, d.S16(ImageTable.CodeRequestNumber), files);
        DecodeTexts(d);
        int x = random.Next(0x3FF) & 0x3FC;
        int row = x % 32, column = x / 32;
        var digits = new int[4];
        for (int i = 0; i < 4; i++) digits[i] = Digit(row + i, column);
        int text = d.U16(CodeText);
        char letter = (char)('A' + row / 4);
        d.Set(text + 6, (byte)letter);
        if (column < 10)
        {
            d.Set(text + 8, (byte)('0' + column));
            d.Set(text + 9, (byte)' ');
        }
        else
        {
            d.Set(text + 8, (byte)('0' + column / 10));
            d.Set(text + 9, (byte)('0' + column % 10));
        }
        plaque.Show(d, random, PlaqueHeight, PlaqueStep, Texts, PlaqueExtra); // [ORIGINAL] 21FD:0687 – call at offset 0771
        return new CodeRequest(row, column, letter, digits);
    }

    /// <summary>21FD:081D: adds FDh to every byte of the four texts up to their terminating zero.</summary>
    // [ORIGINAL] 21FD:081D – for i in 0..3: for each byte of far [DS:10E0 + 4i] until 0: byte += FDh
    // [APPROX] The far pointers point into the data segment (segment DGROUP); only their offsets are used.
    public static void DecodeTexts(Dgroup d)
    {
        for (int i = 0; i < TextCount; i++)
        {
            int p = d.U16(Texts + 4 * i);
            for (; d.U8(p) != 0; p++) d.Set(p, unchecked((byte)(d.U8(p) + Key)));
        }
    }

    /// <summary>
    /// 21FD:08D3(a, b): expected digit 1..9 – v = (b &lt;&lt; 5) + a, then v·v, v·v, v·7AB7h (32-bit products), v / 100h % 9 + 1
    /// (unsigned long division and remainder); the arguments are ints extended to long with sign.
    /// </summary>
    // [ORIGINAL] 21FD:08D3 – LXLSH, LXMUL ×3, LUDIV 100h, LUMOD 9, +1
    public static int Digit(int a, int b)
    {
        uint v = unchecked(((uint)b << 5) + (uint)a);
        v = unchecked(v * v);
        v = unchecked(v * v);
        v = unchecked(v * 0x7AB7u);
        return (int)(v / 0x100u % 9u) + 1;
    }
}
