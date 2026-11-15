using Traders.Assets;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Presentation;
using Xunit.Abstractions;

namespace Traders.Tests.Pixels;

/// <summary>
/// The rule page <see cref="RulePage"/> (finding docs/routines/111D_0112_regelseite.md) against the VRAM of the original. The
/// original runs from the snapshot at the main menu (graphics set up by the program start). Expected values are the pixel
/// reader's return values and the VGA planes of the RefEmu, never the port.
/// </summary>
public class RulePageDifferentialTests(ITestOutputHelper output)
{
    private const ushort Seg0A96 = 0x0A96, Seg21FD = 0x21FD, Driver = 0x23CD, Seg111D = 0x111D;

    /// <summary>Return addresses of the two calls of 23CD:06DE in 111D:0112 (first and second star loop).</summary>
    private const ushort AfterRead1 = 0x018D, AfterRead2 = 0x022D;

    private static IndexedImage Picture(string file)
    {
        var image = VgaImage.Load(TestGameData.Dos(file));
        return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
    }

    private static OriginalProgram AtMenu() => OriginalProgram.Resume(TestGameData.DosDir, MenuSceneHarness.AtMenu.Value);

    private static Dgroup Ds(OriginalProgram p) => new(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());

    /// <summary>Writes rows 0..199 of <paramref name="image"/> into physical page <paramref name="page"/> of the RefEmu VRAM.</summary>
    private static void FillVram(OriginalProgram p, int page, IndexedImage image)
    {
        var planes = p.Machine.Vga.Planes;
        for (int y = 0; y < RulePage.Height; y++)
            for (int x = 0; x < RulePage.Width; x++)
                planes[x & 3][page * RulePage.PageDistance + y * RulePage.RowBytes + (x >> 2)] = image[x, y];
    }

    private static void LoadPage(RulePage core, int page, IndexedImage image)
    {
        int keep = core.DrawPage;
        core.SelectDrawPage(page);
        core.Blit(image, 0, 0, RulePage.Width, RulePage.Height, 0, 0);
        core.SelectDrawPage(keep);
    }

    /// <summary>Both physical pages of the core equal the VRAM pages of the RefEmu.</summary>
    private static void AssertPages(OriginalProgram p, RulePage core, string when)
    {
        var planes = p.Machine.Vga.Planes;
        for (int page = 0; page < 2; page++)
        {
            var pixels = core.PagePixels(page);
            for (int y = 0; y < RulePage.Height; y++)
                for (int x = 0; x < RulePage.Width; x++)
                {
                    byte vram = planes[x & 3][page * RulePage.PageDistance + y * RulePage.RowBytes + (x >> 2)];
                    if (vram != pixels[y * RulePage.Width + x])
                        Assert.Fail($"{when}: page {page} ({x}, {y}) VRAM {vram}, rule page {pixels[y * RulePage.Width + x]}");
                }
        }
    }

    /// <summary>
    /// Replays the drawing steps of the original onto the core: the command list 0A96 (only the entry types of 111D:0112 –
    /// 0030 blit, 04D7 filled rectangle, 03FB pixel) is applied when 0A96:06A8 executes it, 21FD:051F flips the core's draw
    /// page. Each read of 23CD:06DE in 0112 is compared at its return with <see cref="RulePage.Get(int, int)"/>.
    /// </summary>
    private sealed class Replay
    {
        private readonly List<Action> _list = [];
        private short _x, _y;

        public List<(int Loop, short X, short Y, ushort Value)> Reads { get; } = [];

        public Replay(OriginalProgram p, RulePage core, IndexedImage atlas)
        {
            var m = p.Machine;
            ushort a = p.Segment(Seg0A96);
            m.AddCodeHook(a, 0x001B, _ => _list.Clear());
            m.AddCodeHook(a, 0x0030, _ =>
            {
                var (sx, sy, w, h, dx, dy) = (Arg(p, 0), Arg(p, 1), Arg(p, 2), Arg(p, 3), Arg(p, 4), Arg(p, 5));
                _list.Add(() => core.Blit(atlas, sx, sy, w, h, dx, dy));
            });
            m.AddCodeHook(a, 0x04D7, _ =>
            {
                var (x, y, w, h, c) = (Arg(p, 0), Arg(p, 1), Arg(p, 2), Arg(p, 3), (byte)Arg(p, 4));
                _list.Add(() => core.FillRect(x, y, w, h, c));
            });
            m.AddCodeHook(a, 0x03FB, _ =>
            {
                var (x, y, c) = (Arg(p, 0), Arg(p, 1), (byte)Arg(p, 2));
                _list.Add(() => core.SetPixel(x, y, c));
            });
            m.AddCodeHook(a, 0x06A8, _ =>
            {
                // only the replayed entry types may be in the list
                Assert.Equal(_list.Count, p.ReadWord(0x15BE));
                foreach (var step in _list) step();
            });
            m.AddCodeHook(p.Segment(Seg21FD), 0x051F, _ => core.Flip());
            m.AddCodeHook(p.Segment(Driver), 0x06DE, _ => { _x = (short)p.StackArgument(0); _y = (short)p.StackArgument(1); });
            ushort s = p.Segment(Seg111D);
            m.AddCodeHook(s, AfterRead1, mm => Read(1, mm.Cpu.Ax));
            m.AddCodeHook(s, AfterRead2, mm => Read(2, mm.Cpu.Ax));

            void Read(int loop, ushort ax)
            {
                Reads.Add((loop, _x, _y, ax));
                Assert.True(core.Get(_x, _y) == ax,
                    $"loop {loop} read ({_x}, {_y}) on page {core.DrawPage}: original {ax}, rule page {core.Get(_x, _y)}");
            }
        }

        private static int Arg(OriginalProgram p, int i) => (short)p.StackArgument(i);
    }

    /// <summary>
    /// [DIFFERENTIAL] 23CD:06DE – rule page at every read of the pixel reader in the star show 111D:0112: V-BERGE in the RAM atlas
    /// (186C:0002), the page drawn second (after the flip 21FD:051F) prefilled with another picture, both draw-page parities
    /// (with and without an extra flip before), four random states. At each read the core's page equals the original's
    /// return value (AX); after 0112 both pages equal the VRAM in full and the draw page equals DS:40DE[DS:082E]. Covered:
    /// reads that hit colour 1 in both loops, reads with negative y (y − 3) on both physical pages.
    /// </summary>
    [GameDataFact]
    public void Star_show_reads_the_rule_page_like_the_original()
    {
        var berge = Picture("V-BERGE.PP");
        var rakete = Picture("V-RAKETE.PP");
        var others = new[] { ("V-ANBAU.PP", Picture("V-ANBAU.PP")), ("V-BERGE.PP", berge) };
        int[,] hits = new int[3, 2], negative = new int[2, 2];
        foreach (bool flipFirst in new[] { false, true })
            foreach (var (name, other) in others)
                foreach (uint seed in new uint[] { 0x0000_0001, 0x1234_5678, 0x8000_0000, 0xDEAD_BEEF })
                {
                    var p = AtMenu();
                    p.Call(0x186C, 0x0002, (ushort)PictureId.Berge);
                    if (flipFirst) p.Call(Seg21FD, 0x051F);
                    var core = new RulePage(RulePage.DrawPageOf(Ds(p)));
                    // the page drawn first is overwritten by the blit; the second keeps what was there before 0112
                    FillVram(p, core.DrawPage, rakete);
                    FillVram(p, 1 - core.DrawPage, other);
                    LoadPage(core, core.DrawPage, rakete);
                    LoadPage(core, 1 - core.DrawPage, other);
                    p.WriteDword(OriginalProgram.RandomStateOffset, seed);
                    int second = 1 - core.DrawPage;

                    var replay = new Replay(p, core, berge);
                    p.Call(Seg111D, 0x0112);

                    Assert.Equal(180, replay.Reads.Count);
                    AssertPages(p, core, $"after 0112 ({name}, seed {seed:X8}, flip {flipFirst})");
                    Assert.Equal(RulePage.DrawPageOf(Ds(p)), core.DrawPage);
                    Assert.Equal(second, core.DrawPage);
                    foreach (var r in replay.Reads)
                    {
                        if (r.Value == 1) hits[r.Loop, name == "V-BERGE.PP" ? 1 : 0]++;
                        if (r.Y < 0) negative[r.Loop - 1, second]++;
                    }
                }
        output.WriteLine($"hits loop 1: {hits[1, 0] + hits[1, 1]}, loop 2 over V-ANBAU {hits[2, 0]}, over V-BERGE {hits[2, 1]}; " +
                         $"negative y in loop 2 on page 0: {negative[1, 0]}, page 1: {negative[1, 1]}");
        Assert.True(hits[1, 0] + hits[1, 1] > 0 && hits[2, 1] > 0, "no read of colour 1");
        Assert.Equal(0, negative[0, 0] + negative[0, 1]);
        Assert.True(negative[1, 0] > 0 && negative[1, 1] > 0, "no read with negative y on both pages");
    }

    /// <summary>
    /// [DIFFERENTIAL] 23CD:06DE, 23CD:3764 – <see cref="RulePage.Get(int, int)"/> against the pixel reader called directly through
    /// 23CD:06DE on both physical pages: corners, negative y, y ≥ 200 (the gap behind page 0 and page 1 itself), x ≥ 320 and
    /// negative x, with both pages prefilled with different pictures.
    /// </summary>
    [GameDataFact]
    public void Pixel_reader_matches_for_every_word_argument()
    {
        var anbau = Picture("V-ANBAU.PP");
        var berge = Picture("V-BERGE.PP");
        var points = new (int X, int Y)[]
        {
            (0, 0), (319, 0), (0, 199), (319, 199), (1, 1), (2, 2), (3, 3), (157, 83), (318, 149),
            (0, -1), (5, -3), (318, -2), (319, -205), (0, 200), (7, 204), (0, 205), (13, 300), (319, 404), (2, 405),
            (320, 0), (323, 10), (639, 5), (-1, 0), (-4, 7), (-321, 50), (unchecked((short)0x8000), 0), (0, short.MaxValue),
        };
        for (int draw = 0; draw < 2; draw++)
        {
            var p = AtMenu();
            var core = new RulePage(RulePage.DrawPageOf(Ds(p)));
            if (core.DrawPage != draw)
            {
                p.Call(Seg21FD, 0x051F);
                core.Flip();
            }
            Assert.Equal(draw, RulePage.DrawPageOf(Ds(p)));
            FillVram(p, 0, anbau);
            FillVram(p, 1, berge);
            LoadPage(core, 0, anbau);
            LoadPage(core, 1, berge);
            foreach (var (x, y) in points)
            {
                ushort ax = p.Call(Driver, 0x06DE, unchecked((ushort)x), unchecked((ushort)y)).Ax;
                Assert.True(core.Get(x, y) == (byte)ax, $"page {draw} ({x}, {y}): original {ax}, rule page {core.Get(x, y)}");
            }
        }
    }

    /// <summary>
    /// [DIFFERENTIAL] 0A96:0030, 0A96:04D7, 0A96:03FB – the drawing steps of the rule page against the command list executed by
    /// 0A96:06A8 on the draw page: unaligned blits (source and target x mod 4 differ, widths not a multiple of 4, rows of the
    /// atlas beyond 200), rectangles and single pixels at the page edges; then both VRAM pages equal the rule page in full.
    /// </summary>
    [GameDataFact]
    public void Drawing_steps_match_the_command_list()
    {
        var berge = Picture("V-BERGE.PP");
        var rakete = Picture("V-RAKETE.PP");
        for (int draw = 0; draw < 2; draw++)
        {
            var p = AtMenu();
            p.Call(0x186C, 0x0002, (ushort)PictureId.Berge);
            var core = new RulePage(RulePage.DrawPageOf(Ds(p)));
            if (core.DrawPage != draw)
            {
                p.Call(Seg21FD, 0x051F);
                core.Flip();
            }
            FillVram(p, 0, rakete);
            FillVram(p, 1, rakete);
            LoadPage(core, 0, rakete);
            LoadPage(core, 1, rakete);
            var replay = new Replay(p, core, berge);
            ushort U(int v) => unchecked((ushort)v);
            p.Call(Seg0A96, 0x001B);
            foreach (var (sx, sy, w, h, dx, dy) in new[]
                     {
                         (5, 201, 13, 7, 2, 190), (0, 0, 1, 1, 319, 199), (3, 250, 29, 6, 100, 0), (318, 17, 2, 40, 0, 60),
                         (1, 2, 317, 3, 3, 100), (0, 0, 320, 3, 0, 197),
                     })
                p.Call(Seg0A96, 0x0030, U(sx), U(sy), U(w), U(h), U(dx), U(dy));
            foreach (var (x, y, w, h, c) in new[] { (3, 1, 6, 2, 9), (0, 197, 320, 3, 1), (317, 50, 3, 1, 200), (1, 120, 1, 9, 0) })
                p.Call(Seg0A96, 0x04D7, U(x), U(y), U(w), U(h), U(c));
            foreach (var (x, y, c) in new[] { (0, 0, 7), (319, 199, 255), (161, 77, 1), (2, 199, 8) })
                p.Call(Seg0A96, 0x03FB, U(x), U(y), U(c));
            p.Call(Seg0A96, 0x06A8);
            AssertPages(p, core, $"draw page {draw}");
            Assert.Empty(replay.Reads);
        }
    }
}
