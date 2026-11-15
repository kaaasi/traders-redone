using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.Rules;
using Traders.Core.State;
using E = Traders.Core.Disasters.DisasterEventKind;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Disasters;

/// <summary>
/// The catastrophes 111D:0009 behind the guard (finding docs/routines/111D_0009_katastrophen.md, rest from 111D:001E): the
/// draw whether a catastrophe strikes, its kind, the ranks, the title and the star show 111D:0112 (on the core's rule page,
/// src/Traders.Core/Pixels), then one of the kinds 111D:0264,
/// 03B7, 0519 with its effect on the fields, and the wait loop. Every yielded wait is one frame. random(n) yields 0…n
/// (OriginalRandom.Next).
/// </summary>
public static class Catastrophes
{
    /// <summary>DS:08CE – picture of the field (V-ANBAU); DS:08D2 – picture of the star show (V-BERGE).</summary>
    public const int FieldPicture = 0x08CE, StarPicture = 0x08D2;

    /// <summary>DS:385C – far pointer to the title text of the catastrophe.</summary>
    public const int TitleText = 0x385C;

    /// <summary>DS:3854, 384C, 3834 – far pointers to the message lines of the kinds 0264 (2 lines), 03B7 (2), 0519 (6).</summary>
    public const int Message0264 = 0x3854, Message03B7 = 0x384C, Message0519 = 0x3834;

    /// <summary>
    /// DS:07A0, 0780 – 12-bit palettes of the kinds (0780 is shared by 03B7 and 0519): 111D pushes them for 21FD:026C, which
    /// ignores its argument; 03B7 reads DS:0780 for its colours.
    /// </summary>
    public const int Palette0264 = 0x07A0, Palette03B7 = 0x0780;

    /// <summary>DS:00A8 – palette of the field picture.</summary>
    public const int FieldPalette = 0x00A8;

    private const short Headquarters = 4, Rocket = 3, Empty = -2;

    // Retraces per stage of the fades 21FD:037D (0185(4)) and 21FD:0329 (0185(3)), as in Planting.
    private const int FadeOutRetraces = 4, FadeInRetraces = 3;

    /// <summary>
    /// 111D:001E–0046: random(10) = 0 (1 in 11), or the week a multiple of 15 (signed remainder), strikes; then the kind
    /// random(2) (0 → 0264, 1 → 03B7, 2 → 0519). Null: no catastrophe (only the first number is drawn).
    /// </summary>
    // [ORIGINAL] 111D:0009 – rest 001E–0046: random(10), week % 15 (idiv), random(2)
    public static short? Choose(Dgroup d, OriginalRandom random)
    {
        if (random.Next(10) != 0 && d.S16(L.Week) % 15 != 0) return null;
        return random.Next(2);
    }

    /// <summary>
    /// 111D:0009 from 001E (after the guard of <see cref="Phases.CatastropheGuardStep"/>), frame by frame. The rule page
    /// <paramref name="page"/> follows the original's drawing where the star show reads it: background and frame of the field
    /// picture on both pages (21FD:060D), then <see cref="StarShow"/>; every page flip and 21FD:0590 change its draw page.
    /// <paramref name="images"/> gives the pixels of picture n (the RAM atlas after 186C:0002(n)). Pixels drawn after the star
    /// show (message windows, flashes, columns) are not applied: nothing in 111D reads them.
    /// </summary>
    // [ORIGINAL] 111D:0009 – rest 001E–010E (ranks, title, star show, kind, wait loop 21FD:0004(3))
    public static IEnumerable<FieldWait> Run(
        Dgroup d, OriginalRandom random, IDisasterHost host, RulePage page, Func<int, IndexedImage> images,
        DisasterTexts texts)
    {
        short? kind = Choose(d, random);
        if (kind is not { } k) yield break;
        Ranking.Apply(d);
        var atlas = Load(d, host, images, FieldPicture);
        foreach (var w in FadeOut(d, host, 0x550)) yield return w;
        Emit(host, E.ClearList);
        Emit(host, E.DrawBackground);
        Emit(host, E.DrawFrame, 0x8C, 0x88, 0xA0, 0x2C);
        Emit(host, E.DrawTitle, 0x8C, 0xA0, 0xA0, d.S16(TitleText), d.S16(TitleText + 2), 5, 0x0B, -1, 1);
        // The title 0A69:00D3 is not drawn on the rule page: it lies below row 146, the lowest row 0112 reads on this page
        // (test: rule page equal to the VRAM in the read rows at the entry of 0112).
        void Backdrop()
        {
            FieldBackdrop.Background(page, atlas);
            FieldBackdrop.Frame(page, atlas, 0x8C, 0x88, 0xA0, 0x2C);
        }
        foreach (var w in BothPages(d, host, page, Backdrop)) yield return w;
        Emit(host, E.BasePalette, FieldPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        atlas = Load(d, host, images, StarPicture);
        foreach (var w in FadeOut(d, host, 0x550)) yield return w;
        foreach (var w in StarShow(d, random, host, page, atlas)) yield return w;
        var rest = k switch
        {
            0 => LandRedrawn(d, random, host, page, texts),
            1 => FieldsChanged(d, random, host, page, texts),
            2 => FieldsCleared(d, random, host, page, texts),
            _ => [],
        };
        foreach (var w in rest) yield return w;
        foreach (var w in host.WaitLoop(d, random, 3)) yield return w;
    }

    /// <summary>Stars per loop of the star show and the arguments of their positions: x = random(13Fh) ∈ 0…319, y = random(96h) ∈ 0…150.</summary>
    public const int Stars = 0x5A, StarRangeX = 0x13F, StarRangeY = 0x96;

    /// <summary>
    /// 111D:0112 – the star show on the picture in the atlas (V-BERGE). The picture to the draw page; 90 stars at
    /// (random(13Fh), random(96h)), each read on the draw page (23CD:06DE) and, where it is colour 1, set in colour
    /// random(2) + 8; the list executed, the pages flipped. Then on the other page the picture three rows lower with three
    /// rows of colour 1 above, and the same 90 positions read at (x, y − 3) – on that page as it was before 0112, since the
    /// list is executed only afterwards – and set at (x, y) where colour 1. The number of draws depends on the pixels read.
    /// </summary>
    // [ORIGINAL] 111D:0112
    public static IEnumerable<FieldWait> StarShow(
        Dgroup d, OriginalRandom random, IDisasterHost host, RulePage page, IndexedImage atlas)
    {
        var list = new List<Action>();
        void Execute()
        {
            Emit(host, E.ExecuteList);
            foreach (var step in list) step();
        }
        void Clear()
        {
            Emit(host, E.ClearList);
            list.Clear();
        }
        void Blit(short sx, short sy, short w, short h, short dx, short dy)
        {
            Emit(host, E.Blit, sx, sy, w, h, dx, dy);
            list.Add(() => page.Blit(atlas, sx, sy, w, h, dx, dy));
        }
        void Star(short x, short y)
        {
            short colour = Int16Math.S(random.Next(2) + 8);
            Emit(host, E.SetPixel, x, y, colour);
            list.Add(() => page.SetPixel(x, y, (byte)colour));
        }

        Clear();
        Blit(0, 0, 0x140, 0xC8, 0, 0);
        Execute();
        Clear();
        var stars = new (short X, short Y)[Stars];
        for (int i = 0; i < Stars; i++)
        {
            short x = random.Next(StarRangeX);
            short y = random.Next(StarRangeY);
            stars[i] = (x, y);
            if (page.Get(x, y) == 1) Star(x, y);
        }
        Execute();
        foreach (var w in FlipPages(d, host, page)) yield return w;
        Clear();
        Blit(0, 0, 0x140, 0xC5, 0, 3);
        Emit(host, E.FillRect, 0, 0, 0x140, 3, 1);
        list.Add(() => page.FillRect(0, 0, 0x140, 3, 1));
        foreach (var (x, y) in stars)
        {
            short above = Int16Math.S(y - 3);
            if (page.Get(x, above) == 1) Star(x, Int16Math.S(above + 3));
        }
        Execute();
    }

    /// <summary>
    /// 111D:0264 – first kind: flashes (80 double flips), the message, new land quality (0E31:0361 as at a new game), then
    /// every headquarters field gets quality 3 and every rocket field with quality 3 the quality random(2).
    /// </summary>
    // [ORIGINAL] 111D:0264
    public static IEnumerable<FieldWait> LandRedrawn(
        Dgroup d, OriginalRandom random, IDisasterHost host, RulePage page, DisasterTexts texts)
    {
        Emit(host, E.BasePalette, Palette0264);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 100)) yield return w;
        Emit(host, E.Sound, 0, 0x48);
        for (int i = 0; i < 0x50; i++)
        {
            foreach (var w in host.PauseCheck(d)) yield return w;
            foreach (var w in FlipAndDelay(d, host, page)) yield return w;
            foreach (var w in FlipAndDelay(d, host, page)) yield return w;
        }
        Emit(host, E.Sound, 0, 0);
        foreach (var w in Message(d, host, page, texts, Message0264, 2)) yield return w;
        NewGameLand.Create(d, random);
        ForFields(d, cell =>
        {
            sbyte kind = d.S8(Int16Math.U(L.FieldKind + cell));
            int quality = Int16Math.U(L.FieldQuality + cell);
            if (kind == Headquarters) d.Set(quality, (sbyte)3);
            else if (kind == Rocket && d.S8(quality) == 3) d.Set(quality, unchecked((byte)random.Next(2)));
        });
        foreach (var w in Delay(host, 100)) yield return w;
    }

    /// <summary>
    /// 111D:03B7 – second kind: colours 2…7 − i of palette DS:0780 brightened step by step and back, the message, then
    /// random(15) + 10 draws of a field (column random(columns − 1), row random(rows − 1), i.e. every column and row); a field that is not a headquarters keeps its owner and gets the kind random(2) (0609:10D2).
    /// </summary>
    // [ORIGINAL] 111D:03B7
    public static IEnumerable<FieldWait> FieldsChanged(
        Dgroup d, OriginalRandom random, IDisasterHost host, RulePage page, DisasterTexts texts)
    {
        Emit(host, E.BasePalette, Palette03B7);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 100)) yield return w;
        for (short di = 0; di <= 5; di++)
        {
            for (short si = 2; 7 - di >= si; si++)
                Emit(host, E.SetColour, si, Int16Math.S(d.U16(Int16Math.U(Palette03B7 + 2 * si)) + di + 1));
            foreach (var w in Delay(host, 10)) yield return w;
        }
        Emit(host, E.Sound, 0, 0x49);
        foreach (var w in Delay(host, 250)) yield return w;
        Emit(host, E.Sound, 0, 0);
        for (short di = 5; di >= 0; di--)
        {
            for (short si = 2; 7 - di >= si; si++)
                Emit(host, E.SetColour, si, Int16Math.S(d.U16(Int16Math.U(Palette03B7 + 2 * si)) + di));
            foreach (var w in Delay(host, 10)) yield return w;
        }
        foreach (var w in Message(d, host, page, texts, Message03B7, 2)) yield return w;
        short count = Int16Math.S(random.Next(15) + 10);
        for (short si = 0; Int16Math.S(count - 1) >= si; si++)
        {
            d.Set(L.LoopX, unchecked((byte)random.Next(Int16Math.S(d.S16(L.FieldColumns) - 1))));
            d.Set(L.LoopY, unchecked((byte)random.Next(Int16Math.S(d.S16(L.FieldRows) - 1))));
            int cell = Cell(d);
            if (d.S8(Int16Math.U(L.FieldKind + cell)) == Headquarters) continue;
            short kind = random.Next(2);
            FieldGrid.Set(d, d.S8(L.LoopX), d.S8(L.LoopY), d.S8(Int16Math.U(L.FieldOwner + cell)), kind);
        }
    }

    /// <summary>
    /// 111D:0519 – third kind (random(2) = 2 in 0009): sweeps of columns over the page shown, flashes, the six-line message,
    /// then every owned field that is not a headquarters is cleared (kind −2, owner kept, 0609:10D2) if random(19) ≤
    /// 11 − 3 · rank of its owner (DS:3D42). DS:37AA is left at the owner of the last field read.
    /// </summary>
    // [ORIGINAL] 111D:0519
    public static IEnumerable<FieldWait> FieldsCleared(
        Dgroup d, OriginalRandom random, IDisasterHost host, RulePage page, DisasterTexts texts)
    {
        Emit(host, E.BasePalette, Palette03B7);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 100)) yield return w;
        foreach (var w in DrawOnShownPage(host, page)) yield return w;
        Emit(host, E.SetColour, 0x0C, 0x770);
        for (int n = 0; n < 4; n++)
        {
            foreach (var w in host.PauseCheck(d)) yield return w;
            Emit(host, E.Sound, 1, 0x4C);
            for (short si = 0xE5; si >= 0; si--) Emit(host, E.Column, si, 1);
            foreach (var w in Delay(host, 5)) yield return w;
            foreach (var w in host.PauseCheck(d)) yield return w;
            for (short si = 0xE5; si >= 0; si--) Emit(host, E.Column, si, 0);
        }
        foreach (var w in Delay(host, 100)) yield return w;
        Emit(host, E.SetColour, 0x0C, 0x700);
        Emit(host, E.Sound, 1, 0x4B);
        foreach (var w in Delay(host, 1)) yield return w;
        Emit(host, E.Sound, 2, 0x4B);
        for (short si = 0; si <= 0xE5; si++) Emit(host, E.Column, si, 1);
        foreach (var w in Delay(host, 10)) yield return w;
        foreach (var w in host.PauseCheck(d)) yield return w;
        for (short si = 0; si <= 0xE5; si++) Emit(host, E.Column, si, 0);
        Emit(host, E.Sound, 0, 0x4A);
        for (int n = 0; n < 0x32; n++)
        {
            foreach (var w in host.PauseCheck(d)) yield return w;
            foreach (var w in FlipAndDelay(d, host, page)) yield return w;
            foreach (var w in FlipAndDelay(d, host, page)) yield return w;
        }
        Emit(host, E.Sound, 0, 0);
        foreach (var w in Message(d, host, page, texts, Message0519, 6)) yield return w;
        ForFields(d, cell =>
        {
            sbyte owner = d.S8(Int16Math.U(L.FieldOwner + cell));
            d.Set(L.ActivePlayer, owner);
            if (owner == -1 || d.S8(Int16Math.U(L.FieldKind + cell)) == Headquarters) return;
            short r = random.Next(0x13);
            short limit = Int16Math.S(0x0B - d.U8(Int16Math.U(L.Rank + owner)) * 3);
            if (r > limit) return;
            FieldGrid.Set(d, d.S8(L.LoopX), d.S8(L.LoopY), owner, Empty);
        });
    }

    /// <summary>
    /// 111D:0757(lines, count) – message window: field picture, fade-out, background and frame on both pages, field palette,
    /// fade-in, then <paramref name="count"/> lines typed from the far pointers at DS:<paramref name="lines"/>, vertically
    /// centred in the frame (<see cref="TypeLines"/>).
    /// </summary>
    // [ORIGINAL] 111D:0757
    public static IEnumerable<FieldWait> Message(
        Dgroup d, IDisasterHost host, RulePage page, DisasterTexts texts, short lines, short count)
    {
        Emit(host, E.LoadImage, d.S16(FieldPicture));
        foreach (var w in FadeOut(d, host, 4)) yield return w;
        Emit(host, E.ClearList);
        Emit(host, E.DrawBackground);
        Emit(host, E.DrawFrame, 0x28, 0x4B, 0xF0, 0x32);
        foreach (var w in BothPages(d, host, page, null)) yield return w;
        Emit(host, E.BasePalette, FieldPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        short y = Int16Math.S((short)(0x32 - Int16Math.S(count * 6)) / 2 + 0x51);
        foreach (var w in TypeLines(d, host, page, texts, 0x28, y, 0xF0, lines, count, 5, 3)) yield return w;
    }

    /// <summary>
    /// 0A69:0288(x, y, w, lines, count, colour, wait), its frames: for every line i = 0 … count − 1 with a non-null far pointer
    /// at DS:lines + 4i (0A69:0243) every character of the text (0A69:01AD, while strlen &gt; i, unsigned) is drawn alone into
    /// the emptied list (<see cref="DisasterEventKind.TypeCharacter"/>), then 21FD:060D (list, flip, list), the typing sound
    /// unless a space, the pause check 21FD:0245 and 21FD:0185(wait). No final wait (−1). The drawing is the scene's; the
    /// rule page only follows the flips.
    /// </summary>
    // [ORIGINAL] 0A69:0288, 0A69:0243, 0A69:01AD
    public static IEnumerable<FieldWait> TypeLines(Dgroup d, IDisasterHost host, RulePage page, DisasterTexts texts,
        short x, short y, short w, short lines, short count, short colour, short wait)
    {
        Emit(host, E.TypeLines, x, y, w, lines, count, colour, wait);
        for (short i = 0; i < count; i++)
        {
            int at = Int16Math.U(lines + 4 * i);
            ushort offset = d.U16(at), segment = d.U16(Int16Math.U(at + 2));
            if (offset == 0 && segment == 0) continue;
            for (int c = 0; c < texts.Length(d, offset, segment); c++)
            {
                Emit(host, E.TypeCharacter, i, (short)c, texts.At(d, offset, segment, c));
                page.Flip();
                foreach (var f in Planting.Flip(d)) yield return f;
                foreach (var f in Planting.Pause(d)) yield return f;
                foreach (var f in Planting.Retraces(wait, (short)(wait >> 15))) yield return f;
            }
        }
    }

    /// <summary>
    /// The global field loop of 0264 and 0519: DS:2F00 (row) outside, DS:2EB0 (column) inside, both bytes compared as signed
    /// words with rows − 1 and columns − 1; the cell is column · 6 + row. Both are left one past the end.
    /// </summary>
    private static void ForFields(Dgroup d, Action<int> body)
    {
        for (d.Set(L.LoopY, (byte)0); d.S8(L.LoopY) <= Int16Math.S(d.S16(L.FieldRows) - 1);
             d.Set(L.LoopY, unchecked((byte)(d.U8(L.LoopY) + 1))))
            for (d.Set(L.LoopX, (byte)0); d.S8(L.LoopX) <= Int16Math.S(d.S16(L.FieldColumns) - 1);
                 d.Set(L.LoopX, unchecked((byte)(d.U8(L.LoopX) + 1))))
                body(Cell(d));
    }

    private static int Cell(Dgroup d) => Int16Math.S(d.S8(L.LoopX) * 6 + d.S8(L.LoopY));

    private static void Emit(IDisasterHost host, E kind, params short[] args) => host.Emit(new DisasterEvent(kind, args));

    private static IEnumerable<FieldWait> FadeOut(Dgroup d, IDisasterHost host, short rgb)
    {
        Emit(host, E.FadeOut, rgb);
        return Planting.Fade(d, FadeOutRetraces);
    }

    private static IEnumerable<FieldWait> FadeIn(Dgroup d, IDisasterHost host)
    {
        Emit(host, E.FadeIn);
        return Planting.Fade(d, FadeInRetraces);
    }

    /// <summary>21FD:060D; <paramref name="draw"/> (if any) is applied to the rule page before and after its flip.</summary>
    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    private static IEnumerable<FieldWait> BothPages(Dgroup d, IDisasterHost host, RulePage page, Action? draw)
    {
        Emit(host, E.ListOnBothPages);
        draw?.Invoke();
        page.Flip();
        draw?.Invoke();
        return Planting.Flip(d);
    }

    private static IndexedImage Load(Dgroup d, IDisasterHost host, Func<int, IndexedImage> images, int picture)
    {
        short n = d.S16(picture);
        Emit(host, E.LoadImage, n);
        return images(n);
    }

    private static IEnumerable<FieldWait> FlipPages(Dgroup d, IDisasterHost host, RulePage page)
    {
        Emit(host, E.FlipPages);
        page.Flip();
        return Planting.Flip(d);
    }

    private static IEnumerable<FieldWait> Delay(IDisasterHost host, short n)
    {
        Emit(host, E.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }

    /// <summary>
    /// 21FD:0590 – draw on the page shown: 0B67:0238 shows the page of DS:0830 once more (0196/01B5, one retrace), then
    /// 0B67:028F (one retrace); no key is read, unlike 21FD:051F.
    /// </summary>
    // [ORIGINAL] 21FD:0590 – 0B67:0238 (show, retrace), 0B67:028F (retrace)
    private static IEnumerable<FieldWait> DrawOnShownPage(IDisasterHost host, RulePage page)
    {
        Emit(host, E.DrawOnShownPage);
        page.DrawOnShownPage();
        yield return FieldWait.Retrace;
        yield return FieldWait.Retrace;
    }

    /// <summary>21FD:051F, then 21FD:0185(1) (one half of a flash in 0264 and 0519).</summary>
    private static IEnumerable<FieldWait> FlipAndDelay(Dgroup d, IDisasterHost host, RulePage page)
    {
        foreach (var w in FlipPages(d, host, page)) yield return w;
        foreach (var w in Delay(host, 1)) yield return w;
    }
}
