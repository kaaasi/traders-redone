using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Land quality of a new game (docs/routines/0E31_0003_neue_partie.md): random land scatters n fields per quality 0..2,
/// real land grows three regions around random seeds; all remaining fields keep quality 3. n = cols*rows/4 - 1.
/// </summary>
public static class NewGameLand
{
    /// <summary>Random or real land depending on DS:08F1 (also called from 111D:02E8).</summary>
    // [ORIGINAL] 0E31:0361
    public static void Create(Dgroup d, OriginalRandom random)
    {
        if (d.U8(L.LandType) == 0) Random(d, random);
        else Real(d, random);
    }

    /// <summary>Land type RANDOM: n fields per quality 0..2 at random positions.</summary>
    // [ORIGINAL] 0E31:0379
    public static void Random(Dgroup d, OriginalRandom random)
    {
        Span<sbyte> left = stackalloc sbyte[3];
        left.Fill(PerQuality(d));
        FillQuality3(d);

        for (int q = 0; q <= 2; q++)
            while (left[q] > 0)
            {
                int index;
                do index = DrawField(d, random);
                while (d.S8(L.FieldQuality + index) != 3);
                d.Set(L.FieldQuality + index, (sbyte)q);
                left[q]--;
            }
    }

    /// <summary>Land type REAL: one seed per quality, then fields at growing distance r around the seeds.</summary>
    // [ORIGINAL] 0E31:0465
    public static void Real(Dgroup d, OriginalRandom random)
    {
        short cols = d.S16(L.FieldColumns), rows = d.S16(L.FieldRows);
        byte maxRadius = unchecked((byte)((cols > rows ? cols : rows) - 1)); // low byte + 0FFh
        Span<sbyte> left = stackalloc sbyte[3];
        left.Fill(PerQuality(d));
        short remaining = Int16Math.S(left[0] + left[1] + left[2]);
        FillQuality3(d);

        Span<sbyte> seedX = stackalloc sbyte[3], seedY = stackalloc sbyte[3];
        for (int q = 0; q <= 2; q++)
        {
            int index;
            do index = DrawField(d, random);
            while (d.S8(L.FieldQuality + index) != 3);
            seedX[q] = d.S8(L.LoopX);
            seedY[q] = d.S8(L.LoopY);
            d.Set(L.FieldQuality + index, (sbyte)q);
            left[q]--;
            remaining--;
        }

        // 0E31:058B: growth around the seeds
        byte radius = 1;
        while (remaining > 0) // checked only after six passes
        {
            for (int pass = 0; pass <= 5; pass++)
                for (int q = 0; q <= 2; q++)
                {
                    if (left[q] <= 0) continue;
                    sbyte r = unchecked((sbyte)radius);
                    sbyte dx, dy;
                    if (random.Next(1) == 0) { dx = r; dy = unchecked((sbyte)random.Next(r)); }
                    else { dx = unchecked((sbyte)random.Next(r)); dy = r; }
                    if (random.Next(1) == 0) dx = unchecked((sbyte)-dx);
                    if (random.Next(1) == 0) dy = unchecked((sbyte)-dy);
                    sbyte x = unchecked((sbyte)(seedX[q] + dx)), y = unchecked((sbyte)(seedY[q] + dy));
                    d.Set(L.LoopX, x);
                    d.Set(L.LoopY, y);
                    if (x < 0 || x >= cols || y < 0 || y >= rows) continue;
                    int index = x * L.FieldRowsMax + y;
                    if (d.S8(L.FieldQuality + index) != 3) continue;
                    d.Set(L.FieldQuality + index, (sbyte)q);
                    left[q]--;
                    remaining--;
                }
            radius = unchecked((byte)(radius + 1));
            if (radius > maxRadius) radius = 1;
        }
    }

    /// <summary>Visits every field row by row with the global loop variables DS:2F00 (row) and DS:2EB0 (column).</summary>
    internal static void ForEachField(Dgroup d, Action<int, int> visit)
    {
        for (d.Set(L.LoopY, (sbyte)0); d.S8(L.LoopY) <= Int16Math.S(d.S16(L.FieldRows) - 1); d.Set(L.LoopY, (sbyte)(d.S8(L.LoopY) + 1)))
            for (d.Set(L.LoopX, (sbyte)0); d.S8(L.LoopX) <= Int16Math.S(d.S16(L.FieldColumns) - 1); d.Set(L.LoopX, (sbyte)(d.S8(L.LoopX) + 1)))
                visit(d.S8(L.LoopX), d.S8(L.LoopY));
    }

    // 0E31:037F: low byte of (cols*rows idiv 4) - 1; cwd on the low word of the product
    private static sbyte PerQuality(Dgroup d) =>
        unchecked((sbyte)(Int16Math.S(d.S16(L.FieldColumns) * d.S16(L.FieldRows)) / 4 - 1));

    private static void FillQuality3(Dgroup d) =>
        ForEachField(d, (x, y) => d.Set(L.FieldQuality + L.FieldIndex(x, y), (sbyte)3));

    /// <summary>x = random(cols - 1) into DS:2EB0, y = random(rows - 1) into DS:2F00; returns the field index.</summary>
    private static int DrawField(Dgroup d, OriginalRandom random)
    {
        d.Set(L.LoopX, unchecked((sbyte)random.Next(Int16Math.S(d.S16(L.FieldColumns) - 1))));
        d.Set(L.LoopY, unchecked((sbyte)random.Next(Int16Math.S(d.S16(L.FieldRows) - 1))));
        return d.S8(L.LoopX) * L.FieldRowsMax + d.S8(L.LoopY);
    }
}
