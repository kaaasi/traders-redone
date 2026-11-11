using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the line routines (vectors 1C/4C, dispatcher 06BE) and the moving-object loops
/// (06E9/071C with vectors 24/28) of the 23CD VGA driver against the original in the reference emulator.
/// </summary>
public class DriverLineAndObjectTests
{
    private const int Calls = 60;

    /// <summary>Random end points; every fifth line vertical, every fifth horizontal, the rest general.</summary>
    private static (ushort, ushort, ushort, ushort) RandomLine(DriverHarness h, int i)
    {
        ushort x1 = h.Next(0, 319), y1 = h.Next(0, 199), x2 = h.Next(0, 319), y2 = h.Next(0, 199);
        if (i % 5 == 0) x2 = x1;
        else if (i % 5 == 1) y2 = y1;
        else if (i % 5 == 2) { x2 = (ushort)Math.Min(319, x1 + h.Next(0, 6)); }   // steep lines
        return (x1, y1, x2, y2);
    }

    // [DIFFERENTIAL] 23CD:378B
    [GameDataFact]
    public void DrawLineSolid_378B_matches_original()
    {
        var h = new DriverHarness(21);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            h.SetColor(h.Next(0, 255));
            var (x1, y1, x2, y2) = RandomLine(h, i);
            h.CallOriginal(0x06CA, x1, y1, x2, y2);   // jmp far [cs:1C]
            h.Port.DrawLineSolid(x1, y1, x2, y2);
            h.AssertVramEqual($"Line({x1},{y1})-({x2},{y2}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:38F2
    [GameDataFact]
    public void DrawLinePatterned_38F2_matches_original()
    {
        var h = new DriverHarness(22);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            h.SetColor(h.Next(0, 255));
            h.SetLinePattern(i % 7 == 0 ? (ushort)0xFFFF : h.Next(0, 0xFFFF));
            var (x1, y1, x2, y2) = RandomLine(h, i);
            h.CallOriginal(0x06CF, x1, y1, x2, y2);   // jmp far [cs:4C]
            h.Port.DrawLinePatterned(x1, y1, x2, y2);
            h.AssertVramEqual($"PatternLine({h.Port.LinePattern:X4}: {x1},{y1})-({x2},{y2}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:06BE
    [GameDataFact]
    public void DrawLine_06BE_picks_the_routine_like_the_original()
    {
        var h = new DriverHarness(23);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            h.SetColor(h.Next(0, 255));
            h.SetLinePattern(i % 2 == 0 ? (ushort)0xFFFF : h.Next(0, 0xFFFF));
            var (x1, y1, x2, y2) = RandomLine(h, i);
            h.CallOriginal(0x06BE, x1, y1, x2, y2);
            h.Port.DrawLine(x1, y1, x2, y2);
            h.AssertVramEqual($"Line06BE({h.Port.LinePattern:X4}: {x1},{y1})-({x2},{y2}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:06E9
    // [DIFFERENTIAL] 23CD:071C
    // [DIFFERENTIAL] 23CD:3AAB
    // [DIFFERENTIAL] 23CD:3C42
    [GameDataFact]
    public void Moving_objects_06E9_071C_match_original()
    {
        var h = new DriverHarness(24);
        var objects = h.SetUpObjects();
        for (int i = 0; i < 30; i++)
        {
            h.RandomPage();
            ushort page = (ushort)h.Rng.Next(2);
            h.MoveObjects(objects, i);
            if (h.Rng.Next(3) != 0)
            {
                h.CallOriginal(0x071C, page);
                h.Port.RestoreObjects(page, objects);
                h.AssertObjectsEqual(objects, $"restore page {page} #{i}");
            }
            h.CallOriginal(0x06E9, page);
            h.Port.DrawObjects(page, objects);
            h.AssertObjectsEqual(objects, $"draw page {page} #{i}");
        }
    }
}
