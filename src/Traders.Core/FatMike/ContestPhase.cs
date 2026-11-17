using System.Runtime.CompilerServices;
using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.FatMike;

/// <summary>
/// The contest 1F63:0B03 (call 30 of the week loop; finding docs/routines/1F63_0B03_wettbewerb.md): every third week (week mod
/// 3 = 2) Fat Mike's scene shows the stocks of kind 3 of the players who paid last time (DS:4BC2 = 0); those with the largest
/// stock win: their colour flashes (11AD), and each gets week·50 in cash – or, when the largest stock is at least 70 and free
/// fields are left, the notice DS:3B70 and a free field in a second field round (0609:0080). Shares 0D24, 01B2 (with 0C82,
/// 0E5C, 0EC7, 0F46) with <see cref="FatMikePhase"/>; drawing and sound are emitted as events in the original's order, every
/// yielded <see cref="FieldWait"/> is one frame.
/// </summary>
public static class ContestPhase
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane fatmike).
    public const int HeadingText = 0x3B6C;    // far pointer, the typed heading of the contest (0A69:0243)
    public const int FieldText = 0x3B70;      // far pointer, the text instead of the prize when a field is won
    public const int PlayerColours = 0x1014;  // word[4], rgb12 of the players' colours 0Ch…0Fh (restored by 11AD)

    /// <summary>Largest stock from which the prize is a free field (when DS:2F05 &gt; 0).</summary>
    public const short FieldPrizeStock = 0x46;

    private static int P(Dgroup d) => d.S8(DgroupLayout.ActivePlayer);
    private static int At(int offset) => Int16Math.U(offset);
    private static short Row(int p) => Int16Math.S(p * 0x21);

    private static void Emit(IFatMikeHost h, FieldEventKind k, params short[] args) => h.Emit(new FieldEvent(k, args));
    private static void Emit(IFatMikeHost h, FatMikeEventKind k, params short[] args) => h.Emit(new FatMikeEvent(k, args));

    /// <summary>
    /// The whole of 1F63:0B03 frame by frame: prize = week·50 (16-bit); nothing unless week mod 3 = 2 (signed). Picture, fade-out
    /// to 50h, the scene 0D24, palette, fade-in, DS:54B1 = 1, the scale 01B2 (largest stock), the heading 0A69:0243(B1h, 0Ah, 8Fh,
    /// DS:3B6C, 8, 2, −1), font 1, 50 retraces, the flashing 11AD; per winner (DS:4890 = 1) the prize (0A96:05FB and cash
    /// DS:3CD2 += prize, unsigned) or – largest stock ≥ 70 and DS:2F05 ≠ 0 – the text DS:3B70 (0A96:0525), shown on both pages,
    /// sound (0, 28h), 50 retraces; font 0, the wait loop 21FD:0004(4); with largest stock ≥ 70 and DS:2F05 &gt; 0 the second
    /// field round 0609:0080 (<see cref="IContestHost.SecondRound"/>).
    /// </summary>
    // [ORIGINAL] 1F63:0B03
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IContestHost host)
    {
        short prize = Int16Math.S(d.S16(DgroupLayout.Week) * 0x32);
        if (Int16Math.Mod(d.S16(DgroupLayout.Week), 3) != 2) yield break;
        Emit(host, FieldEventKind.LoadImage, d.S16(FatMikePhase.PictureNumber));
        Emit(host, FieldEventKind.FadeOut, 0x50);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        foreach (var w in FatMikePhase.SceneFrames(d, host)) yield return w;
        Emit(host, FieldEventKind.BasePalette, FatMikePhase.Palette);
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        d.Set(FatMikePhase.Contest, (byte)1);
        var max = new StrongBox<short>();
        foreach (var w in FatMikePhase.ScaleFrames(d, host, max)) yield return w;
        short di = max.Value;
        foreach (var w in TypeLine(d, host, 0xB1, 0x0A, 0x8F, d.U16(HeadingText), d.U16(HeadingText + 2), 8, 2, -1))
            yield return w;
        Emit(host, FatMikeEventKind.Font, 1);
        foreach (var w in Delay(host, 0x32)) yield return w;
        foreach (var w in FlashFrames(d, host)) yield return w;
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
        {
            int p = P(d);
            if (d.U8(At(FatMikePhase.Bidders + p)) != 1) continue;
            Emit(host, FieldEventKind.ClearList);
            if (di < FieldPrizeStock || d.S8(FieldGrid.FreeFields) == 0)
            {
                Emit(host, FatMikeEventKind.Number, 0x29, Int16Math.S(Row(p) + 0x22), prize, 0, 5, 0, -1);
                int cash = At(DgroupLayout.Cash + Int16Math.S(p << 2));
                d.Set(cash, unchecked(d.S32(cash) + Int16Math.U(prize))); // add/adc 0: the prize unsigned
            }
            else
            {
                Emit(host, FatMikeEventKind.Text, 0x29, Int16Math.S(Row(p) + 0x21), d.S16(FieldText), d.S16(FieldText + 2), 0, -1);
            }
            foreach (var w in BothPages(d, host)) yield return w;
            Emit(host, FieldEventKind.Sound, 0, 0x28);
            foreach (var w in Delay(host, 0x32)) yield return w;
        }
        Emit(host, FatMikeEventKind.Font, 0);
        foreach (var w in host.WaitLoop(d, random, 4)) yield return w;
        if (di >= FieldPrizeStock && d.S8(FieldGrid.FreeFields) > 0)
            foreach (var w in host.SecondRound(d, random)) yield return w;
    }

    /// <summary>
    /// 1F63:11AD – eight times (si = 0 … 7) for every winner (DS:4890 = 1) his colour 0Ch + p set (2295:006D): even si to FFFh with
    /// sound (0, 29h), odd si back to DS:1014[p] with sound (0, 2Ah); then 15 retraces.
    /// </summary>
    // [ORIGINAL] 1F63:11AD
    public static IEnumerable<FieldWait> FlashFrames(Dgroup d, IFatMikeHost host)
    {
        for (short si = 0; si <= 7; si++)
        {
            for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
                 d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
            {
                int p = P(d);
                if (d.U8(At(FatMikePhase.Bidders + p)) != 1) continue;
                if (Int16Math.Mod(si, 2) == 0)
                {
                    Emit(host, FatMikeEventKind.DacColour, Int16Math.S(p + 0x0C), 0xFFF);
                    Emit(host, FieldEventKind.Sound, 0, 0x29);
                }
                else
                {
                    Emit(host, FatMikeEventKind.DacColour, Int16Math.S(p + 0x0C), d.S16(At(PlayerColours + Int16Math.S(p << 1))));
                    Emit(host, FieldEventKind.Sound, 0, 0x2A);
                }
            }
            foreach (var w in Delay(host, 0x0F)) yield return w;
        }
    }

    /// <summary>
    /// 0609:0080 – the second field round: objects off (21AF:000D), the winners (DS:4890 = 1) in mode 5 with the purchase flag
    /// DS:43FA = 1, all others in mode 6 (done), DS:1582 = 1; then the field 0609:00D6 – set-up (without the wait loop, cursors
    /// shown, the typed notice of 0609:0204) and the field loop 0609:03E0 up to its last frame 21FD:0665. No settlement 0609:0FA9.
    /// </summary>
    // [ORIGINAL] 0609:0080
    // [ORIGINAL] 0609:00D6 – called (Planting.SetupFrames, PassFrames)
    public static IEnumerable<FieldWait> SecondRoundFrames(Dgroup d, OriginalRandom random, IPlantingHost field)
    {
        PrepareSecondRound(d, field);
        foreach (var w in Planting.SetupFrames(d, random, field)) yield return w;
        while (Planting.Running(d))
            foreach (var w in Planting.PassFrames(d, random, field)) yield return w;
        field.Emit(new FieldEvent(FieldEventKind.LastFrame));
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>0609:0080 up to its call of 0609:00D6 (see <see cref="SecondRoundFrames"/>).</summary>
    public static void PrepareSecondRound(Dgroup d, IFieldOutput field)
    {
        Planting.InitObjects(d, field);
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
        {
            int p = P(d);
            if (d.U8(At(FatMikePhase.Bidders + p)) == 1)
            {
                d.Set(At(Planting.Mode + p), (byte)5);
                d.Set(At(Planting.Purchase + p), (byte)1);
            }
            else d.Set(At(Planting.Mode + p), (byte)6);
        }
        d.Set(Planting.NoTime, (byte)1);
    }

    /// <summary>
    /// 0A69:0243(x, y, w, text, colour, wait, final wait) – one typed line centred in w (nothing for a null pointer); per
    /// character (0A69:01AD) list emptied, the character (0A96:0525 with DS:0140), 21FD:060D, the key click (1, 16h) except for
    /// blanks, 21FD:0245, 21FD:0185(wait); finally 21FD:0185(final wait) when it is &gt; 0.
    /// </summary>
    // [ORIGINAL] 0A69:0243 – frames and events; the position and the character buffer DS:0140 are the output's
    // [ORIGINAL] 0A69:01AD – frames and events
    public static IEnumerable<FieldWait> TypeLine(Dgroup d, IFatMikeHost host, short x, short y, short w, ushort offset,
        ushort segment, short colour, short wait, short finalWait)
    {
        Emit(host, FatMikeEventKind.TypeLine, x, y, w, unchecked((short)offset), unchecked((short)segment), colour, wait, finalWait);
        if (offset == 0 && segment == 0) yield break;
        byte[] text = host.FarString(offset, segment);
        for (int k = 0; k < text.Length; k++)
        {
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FatMikeEventKind.TypeCharacter, text[k], 0, (short)k, (short)text.Length);
            Emit(host, FieldEventKind.ListOnBothPages);
            foreach (var f in Planting.Flip(d)) yield return f;
            if (text[k] != (byte)' ') Emit(host, FieldEventKind.Sound, 1, 0x16);
            foreach (var f in host.PauseCheck(d)) yield return f;
            short high = (short)(wait < 0 ? -1 : 0); // cwd
            Emit(host, FieldEventKind.Delay, wait, high);
            foreach (var f in Planting.Retraces(wait, high)) yield return f;
        }
        if (finalWait > 0)
        {
            Emit(host, FieldEventKind.Delay, finalWait, 0);
            foreach (var f in Planting.Retraces(finalWait, 0)) yield return f;
        }
    }

    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8 (one event, the flip's frames)
    private static IEnumerable<FieldWait> BothPages(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.ListOnBothPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    // 21FD:0185(n) – the retraces (Planting.Retraces).
    private static IEnumerable<FieldWait> Delay(IFatMikeHost host, short n)
    {
        Emit(host, FieldEventKind.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }
}
