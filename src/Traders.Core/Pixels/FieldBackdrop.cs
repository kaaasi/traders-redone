namespace Traders.Core.Pixels;

/// <summary>
/// The field picture's background and frame (atlas V-ANBAU) as drawing steps of the rule page: 111D:0009 draws them on both
/// pages before the star show 111D:0112 reads the page drawn second (finding docs/routines/111D_0112_regelseite.md). Same
/// command list entries as the scene's (src/Traders.Presentation/Field/FieldScene.cs); the core has no reference to it (R6P-1).
/// All arithmetic 16-bit as in the original; the rectangles must lie inside the page (<see cref="RulePage.Blit"/>).
/// </summary>
public static class FieldBackdrop
{
    /// <summary>The whole page tiled with the atlas tile (120h, 58h), 20h × 16h.</summary>
    // [ORIGINAL] 0609:1ED9 – 0A96:0267(120h, 58h, 20h, 16h, 0, 0, 140h, C8h)
    public static void Background(RulePage page, IndexedImage atlas) =>
        page.Tiles(atlas, 0x120, 0x58, 0x20, 0x16, 0, 0, 0x140, 0xC8);

    /// <summary>
    /// Frame (x, y, w, h): four corners 10h × 16h (atlas 80h/90h, 0/42h), top and bottom edge 0Bh high tiled from (120h, 9Ah)
    /// and (120h, A5h), left and right edge 10h wide tiled from (80h, 16h) and (90h, 16h), then the interior
    /// (x + 10h, y + 0Bh, w − 20h, h − 16h) tiled with (F0h, B0h), 20h × 16h.
    /// </summary>
    // [ORIGINAL] 0609:1256, 0609:1F04
    public static void Frame(RulePage page, IndexedImage atlas, short x, short y, short w, short h)
    {
        page.Blit(atlas, 0x80, 0, 0x10, 0x16, x, y);
        page.Blit(atlas, 0x90, 0, 0x10, 0x16, S(x + w - 0x10), y);
        page.Blit(atlas, 0x80, 0x42, 0x10, 0x16, x, S(y + h - 0x16));
        page.Blit(atlas, 0x90, 0x42, 0x10, 0x16, S(x + w - 0x10), S(y + h - 0x16));
        page.Tiles(atlas, 0x120, 0x9A, 0x20, 0x0B, S(x + 0x10), y, S(w - 0x20), 0x0B);
        page.Tiles(atlas, 0x120, 0xA5, 0x20, 0x0B, S(x + 0x10), S(y + h - 0x0B), S(w - 0x20), 0x0B);
        page.Tiles(atlas, 0x80, 0x16, 0x10, 0x2C, x, S(y + 0x16), 0x10, S(h - 0x2C));
        page.Tiles(atlas, 0x90, 0x16, 0x10, 0x2C, S(x + w - 0x10), S(y + 0x16), 0x10, S(h - 0x2C));
        page.Tiles(atlas, 0xF0, 0xB0, 0x20, 0x16, S(x + 0x10), S(y + 0x0B), S(w - 0x20), S(h - 0x16));
    }

    private static short S(int v) => unchecked((short)v);
}
