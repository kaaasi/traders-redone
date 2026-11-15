using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using E = Traders.Core.Disasters.DisasterEventKind;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Disasters;

/// <summary>
/// The catastrophes 111D:0009 behind the guard (finding docs/routines/111D_0009_katastrophen.md, rest from 111D:001E): the
/// draw whether a catastrophe strikes, its kind, the ranks, the title and the star show, then one of the kinds 111D:0264,
/// 03B7 (0519 is never chosen by 0009) with its effect on the fields, and the wait loop. Every yielded wait is one frame.
/// </summary>
public static class Catastrophes
{
    /// <summary>DS:08CE – picture of the field (V-ANBAU); DS:08D2 – picture of the star show (V-BERGE).</summary>
    public const int FieldPicture = 0x08CE, StarPicture = 0x08D2;

    /// <summary>DS:385C – far pointer to the title text of the catastrophe.</summary>
    public const int TitleText = 0x385C;

    /// <summary>DS:3854, 384C, 3834 – far pointers to the message lines of the kinds 0264 (2 lines), 03B7 (2), 0519 (6).</summary>
    public const int Message0264 = 0x3854, Message03B7 = 0x384C, Message0519 = 0x3834;

    /// <summary>DS:07A0, 0780 – palettes of the kinds (0780 is shared by 03B7 and 0519).</summary>
    public const int Palette0264 = 0x07A0, Palette03B7 = 0x0780;

    /// <summary>DS:00A8 – palette of the field picture.</summary>
    public const int FieldPalette = 0x00A8;

    private const short Headquarters = 4, Rocket = 3, Empty = -2;

    // Retraces per stage of the fades 21FD:037D (0185(4)) and 21FD:0329 (0185(3)), as in Planting.
    private const int FadeOutRetraces = 4, FadeInRetraces = 3;

    /// <summary>
    /// 111D:001E–0046: random(10) = 0, or the week a multiple of 15 (signed remainder), strikes; then the kind random(2)
    /// (0 → 0264, 1 → 03B7). Null: no catastrophe (only the first number is drawn).
    /// </summary>
    // [ORIGINAL] 111D:0009 – rest 001E–0046: random(10), week % 15 (idiv), random(2)
    public static short? Choose(Dgroup d, OriginalRandom random)
    {
        if (random.Next(10) != 0 && d.S16(L.Week) % 15 != 0) return null;
        return random.Next(2);
    }

    /// <summary>111D:0009 from 001E (after the guard of <see cref="Phases.CatastropheGuardStep"/>), frame by frame.</summary>
    // [ORIGINAL] 111D:0009 – rest 001E–010E (ranks, title, star show, kind, wait loop 21FD:0004(3))
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IDisasterHost host)
    {
        short? kind = Choose(d, random);
        if (kind is not { } k) yield break;
        Ranking.Apply(d);
        Emit(host, E.LoadImage, d.S16(FieldPicture));
        foreach (var w in FadeOut(d, host, 0x550)) yield return w;
        Emit(host, E.ClearList);
        Emit(host, E.DrawBackground);
        Emit(host, E.DrawFrame, 0x8C, 0x88, 0xA0, 0x2C);
        Emit(host, E.DrawTitle, 0x8C, 0xA0, 0xA0, d.S16(TitleText), d.S16(TitleText + 2), 5, 0x0B, -1, 1);
        foreach (var w in BothPages(d, host)) yield return w;
        Emit(host, E.BasePalette, FieldPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        Emit(host, E.LoadImage, d.S16(StarPicture));
        foreach (var w in FadeOut(d, host, 0x550)) yield return w;
        foreach (var w in host.StarShow(d, random)) yield return w;
        var rest = k switch
        {
            0 => LandRedrawn(d, random, host),
            1 => FieldsChanged(d, random, host),
            2 => FieldsCleared(d, random, host),
            _ => [],
        };
        foreach (var w in rest) yield return w;
        foreach (var w in host.WaitLoop(d, random, 3)) yield return w;
    }

    /// <summary>
    /// 111D:0264 – first kind: flashes (80 double flips), the message, new land quality (0E31:0361 as at a new game), then
    /// every headquarters field gets quality 3 and every rocket field with quality 3 the quality random(2).
    /// </summary>
    // [ORIGINAL] 111D:0264
    public static IEnumerable<FieldWait> LandRedrawn(Dgroup d, OriginalRandom random, IDisasterHost host)
    {
        Emit(host, E.BasePalette, Palette0264);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 100)) yield return w;
        Emit(host, E.Sound, 0, 0x48);
        for (int i = 0; i < 0x50; i++)
        {
            foreach (var w in host.PauseCheck(d)) yield return w;
            foreach (var w in FlipAndDelay(d, host)) yield return w;
            foreach (var w in FlipAndDelay(d, host)) yield return w;
        }
        Emit(host, E.Sound, 0, 0);
        foreach (var w in Message(d, host, Message0264, 2)) yield return w;
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
    /// random(15) + 10 draws of a field (column random(columns − 1), row random(rows − 1) – the last column and row are never
    /// drawn); a field that is not a headquarters keeps its owner and gets the kind random(2) (0609:10D2).
    /// </summary>
    // [ORIGINAL] 111D:03B7
    public static IEnumerable<FieldWait> FieldsChanged(Dgroup d, OriginalRandom random, IDisasterHost host)
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
        foreach (var w in Message(d, host, Message03B7, 2)) yield return w;
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
    /// 111D:0519 – third kind (0009 never chooses it): sweeps of columns over the page shown, flashes, the six-line message,
    /// then every owned field that is not a headquarters is cleared (kind −2, owner kept, 0609:10D2) if random(19) ≤
    /// 11 − 3 · rank of its owner (DS:3D42). DS:37AA is left at the owner of the last field read.
    /// </summary>
    // [ORIGINAL] 111D:0519
    public static IEnumerable<FieldWait> FieldsCleared(Dgroup d, OriginalRandom random, IDisasterHost host)
    {
        Emit(host, E.BasePalette, Palette03B7);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 100)) yield return w;
        Emit(host, E.DrawOnShownPage);
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
            foreach (var w in FlipAndDelay(d, host)) yield return w;
            foreach (var w in FlipAndDelay(d, host)) yield return w;
        }
        Emit(host, E.Sound, 0, 0);
        foreach (var w in Message(d, host, Message0519, 6)) yield return w;
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
    /// centred in the frame.
    /// </summary>
    // [ORIGINAL] 111D:0757
    public static IEnumerable<FieldWait> Message(Dgroup d, IDisasterHost host, short lines, short count)
    {
        Emit(host, E.LoadImage, d.S16(FieldPicture));
        foreach (var w in FadeOut(d, host, 4)) yield return w;
        Emit(host, E.ClearList);
        Emit(host, E.DrawBackground);
        Emit(host, E.DrawFrame, 0x28, 0x4B, 0xF0, 0x32);
        foreach (var w in BothPages(d, host)) yield return w;
        Emit(host, E.BasePalette, FieldPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        short y = Int16Math.S((short)(0x32 - Int16Math.S(count * 6)) / 2 + 0x51);
        // [APPROX] The typed lines 0A69:0288 wait per character (texts outside the data segment, not modelled here):
        // their frames are not yielded yet (part 1, frames against the RefEmu).
        Emit(host, E.TypeLines, 0x28, y, 0xF0, lines, count, 5, 3);
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

    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    private static IEnumerable<FieldWait> BothPages(Dgroup d, IDisasterHost host)
    {
        Emit(host, E.ListOnBothPages);
        return Planting.Flip(d);
    }

    private static IEnumerable<FieldWait> Delay(IDisasterHost host, short n)
    {
        Emit(host, E.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }

    /// <summary>21FD:051F, then 21FD:0185(1) (one half of a flash in 0264 and 0519).</summary>
    private static IEnumerable<FieldWait> FlipAndDelay(Dgroup d, IDisasterHost host)
    {
        Emit(host, E.FlipPages);
        foreach (var w in Planting.Flip(d)) yield return w;
        foreach (var w in Delay(host, 1)) yield return w;
    }
}
