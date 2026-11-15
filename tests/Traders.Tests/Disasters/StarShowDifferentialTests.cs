using Traders.Assets;
using Traders.Core;
using Traders.Core.Disasters;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using Traders.Tests.Presentation;
using Traders.Tests.Startup;
using Xunit.Abstractions;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Disasters;

/// <summary>
/// Differential tests of the star show 111D:0112 in the core (rule page, finding docs/routines/111D_0009_katastrophen.md,
/// section "Sternvorführung") and of 111D:0009 with it. The original runs from the snapshot at the main menu with its real
/// drawing (186C:0002, command list 0A96, 0609:1ED9/1256, 0A69:00D3, 21FD:060D/051F/0590); only fades, palettes, sound, typed
/// lines, the columns 111D:07E2, waits, the pause check and the wait loop are replaced by a far return. Recorded are the calls made from segment
/// 111D with their arguments; the port emits the same calls as <see cref="DisasterEvent"/>s. Compared: the call sequence, the
/// random state and the number of random() calls, the data segment except the bytes the output layers own
/// (<see cref="ProgramStartDifferentialTests.OutputOwned"/>), and the rule page against the VRAM. Expected values come only
/// from the RefEmu.
/// </summary>
public class StarShowDifferentialTests(ITestOutputHelper output)
{
    private const ushort Seg = 0x111D, Driver = 0x23CD;

    /// <summary>Return addresses of the two calls of 23CD:06DE in 111D:0112 (first and second star loop).</summary>
    private const ushort AfterRead1 = 0x018D, AfterRead2 = 0x022D;

    /// <summary>Recorded calls: segment, offset, argument words (−1 = low byte of the next word), name, replaced by retf.</summary>
    private static readonly (ushort Seg, ushort Off, int[] Args, string Name, bool Stub)[] Recorded =
    [
        (0x186C, 0x0002, [0], nameof(DisasterEventKind.LoadImage), false),
        (0x21FD, 0x037D, [0], nameof(DisasterEventKind.FadeOut), true),
        (0x21FD, 0x0329, [], nameof(DisasterEventKind.FadeIn), true),
        (0x21FD, 0x026C, [0], nameof(DisasterEventKind.BasePalette), true),
        (0x2295, 0x006D, [0, 1], nameof(DisasterEventKind.SetColour), true),
        (0x0A96, 0x001B, [], nameof(DisasterEventKind.ClearList), false),
        (0x0A96, 0x0030, [0, 1, 2, 3, 4, 5], nameof(DisasterEventKind.Blit), false),
        (0x0A96, 0x04D7, [0, 1, 2, 3, 4], nameof(DisasterEventKind.FillRect), false),
        (0x0A96, 0x03FB, [0, 1, 2], nameof(DisasterEventKind.SetPixel), false),
        (0x0A96, 0x06A8, [], nameof(DisasterEventKind.ExecuteList), false),
        (0x0609, 0x1ED9, [], nameof(DisasterEventKind.DrawBackground), false),
        (0x0609, 0x1256, [0, 1, 2, 3], nameof(DisasterEventKind.DrawFrame), false),
        (0x0A69, 0x00D3, [0, 1, 2, 3, 4, 5, 6, 7, 8], nameof(DisasterEventKind.DrawTitle), false),
        (0x0A69, 0x0288, [0, 1, 2, 3, 5, 6, 7], nameof(DisasterEventKind.TypeLines), true),
        (0x21FD, 0x060D, [], nameof(DisasterEventKind.ListOnBothPages), false),
        (0x21FD, 0x051F, [], nameof(DisasterEventKind.FlipPages), false),
        (0x21FD, 0x0590, [], nameof(DisasterEventKind.DrawOnShownPage), false),
        (0x111D, 0x07E2, [0, ~1], nameof(DisasterEventKind.Column), true),
        (0x21A7, 0x0054, [0, 1], nameof(DisasterEventKind.Sound), true),
        (0x21FD, 0x0185, [0, 1], nameof(DisasterEventKind.Delay), true),
        (0x21FD, 0x0245, [], "PauseCheck", true),
        (0x21FD, 0x0004, [0], "WaitLoop", true),
    ];

    private static readonly Lazy<IndexedImage[]> Pictures = new(() =>
        [.. ResourceTable.Pictures.OrderBy(e => (int)e.Id).Select(e => Picture(e.Vga))]);

    private static IndexedImage Picture(string file)
    {
        var image = VgaImage.Load(TestGameData.Dos(file));
        return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
    }

    private static IndexedImage Images(int n) => Pictures.Value[n];

    private sealed class Harness
    {
        public OriginalProgram Program { get; } = OriginalProgram.Resume(TestGameData.DosDir, MenuSceneHarness.AtMenu.Value);
        public List<string> Calls { get; } = [];
        public List<short> RandomArgs { get; } = [];
        public List<(int Loop, short X, short Y, ushort Value)> Reads { get; } = [];

        /// <summary>Both VRAM pages and the draw page at the entry of 111D:0112.</summary>
        public (byte[][] Pages, int DrawPage)? AtStarShow { get; private set; }

        public Harness()
        {
            var p = Program;
            var m = p.Machine;
            ushort own = p.Segment(Seg);
            foreach (var (seg, off, args, name, stub) in Recorded)
            {
                ushort s = p.Segment(seg);
                m.AddCodeHook(s, off, mm =>
                {
                    // only calls made by 111D itself (not those inside 0609, 0A69 or 21FD)
                    if (mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 2)) != own) return;
                    Calls.Add($"{name}({string.Join(", ", args.Select(i => i < 0 ? (short)(byte)p.StackArgument(~i) : (short)p.StackArgument(i)))})");
                });
                if (stub) m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
            }
            m.AddCodeHook(p.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)p.StackArgument(0)));
            m.AddCodeHook(own, 0x0112, _ => AtStarShow = (Vram(p), RulePage.DrawPageOf(Ds(p))));
            short x = 0, y = 0;
            m.AddCodeHook(p.Segment(Driver), 0x06DE, _ => { x = (short)p.StackArgument(0); y = (short)p.StackArgument(1); });
            m.AddCodeHook(own, AfterRead1, mm => Reads.Add((1, x, y, mm.Cpu.Ax)));
            m.AddCodeHook(own, AfterRead2, mm => Reads.Add((2, x, y, mm.Cpu.Ax)));
        }
    }

    /// <summary>Records the port's events; picture loads also go to the picture table (as <see cref="CatastropheStep"/> does).</summary>
    private sealed class PortHost(RulePage page) : IDisasterHost
    {
        public List<string> Calls { get; } = [];

        /// <summary>Both rule-page pages and the draw page when the star picture is loaded (the entry of 0112 follows).</summary>
        public (byte[][] Pages, int DrawPage)? AtStarShow { get; private set; }

        public Dgroup? State { get; init; }

        public void Emit(DisasterEvent e)
        {
            Calls.Add(e.ToString());
            if (e.Kind != DisasterEventKind.LoadImage) return;
            if (State is not null) ImageTable.RecordLoad(State, e.Args[0], ProgramStartDifferentialTests.Files);
            if (e.Args[0] == (short)PictureId.Berge) AtStarShow = ([page.PagePixels(0), page.PagePixels(1)], page.DrawPage);
        }

        public IEnumerable<FieldWait> PauseCheck(Dgroup d)
        {
            Calls.Add("PauseCheck()");
            return [];
        }

        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
        {
            Calls.Add($"WaitLoop({mode})");
            return [];
        }
    }

    private static Dgroup Ds(OriginalProgram p) => new(p.DataSegment().AsSpan(0, L.Size).ToArray());

    /// <summary>Both physical VRAM pages, row-major.</summary>
    private static byte[][] Vram(OriginalProgram p)
    {
        var planes = p.Machine.Vga.Planes;
        var pages = new byte[2][];
        for (int page = 0; page < 2; page++)
        {
            pages[page] = new byte[RulePage.Width * RulePage.Height];
            for (int y = 0; y < RulePage.Height; y++)
                for (int x = 0; x < RulePage.Width; x++)
                    pages[page][y * RulePage.Width + x] = planes[x & 3][page * RulePage.PageDistance + y * RulePage.RowBytes + (x >> 2)];
        }
        return pages;
    }

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

    /// <summary>Rows <paramref name="rows"/> of both pages equal; the draw pages equal.</summary>
    private static void AssertPages((byte[][] Pages, int DrawPage) original, (byte[][] Pages, int DrawPage) port, int rows, string when)
    {
        Assert.True(original.DrawPage == port.DrawPage, $"{when}: draw page original {original.DrawPage}, rule page {port.DrawPage}");
        for (int page = 0; page < 2; page++)
            for (int i = 0; i < rows * RulePage.Width; i++)
                if (original.Pages[page][i] != port.Pages[page][i])
                    Assert.Fail($"{when}: page {page} ({i % RulePage.Width}, {i / RulePage.Width}) VRAM {original.Pages[page][i]}, " +
                                $"rule page {port.Pages[page][i]}");
    }

    /// <summary>The data segment equal except the bytes owned by the output layers (taken from the original).</summary>
    private static void AssertSameState(OriginalProgram p, Dgroup core, OriginalRandom random, Harness h, string label)
    {
        var original = p.DataSegment();
        var port = (byte[])core.Bytes.Clone();
        foreach (var (offset, length, _) in ProgramStartDifferentialTests.OutputOwned)
            Array.Copy(original, offset, port, offset, length);
        Array.Copy(original, TimerTicks, port, TimerTicks, 4); // DS:117E/1180, timer interrupt 27C6:03E4 (host)
        ControlHarness.AssertSameData(label, original, port);
        Assert.Equal(p.ReadDword(L.RandomSeed), random.Seed);
        Assert.Equal(h.RandomArgs.Count, random.Calls);
    }

    private const int TimerTicks = 0x117E;

    private static OriginalRandom Random(Dgroup core) => new(new RecordedClock([]), core.U32(L.RandomSeed));

    /// <summary>
    /// [DIFFERENTIAL] 111D:0112 – the star show alone: V-BERGE in the RAM atlas, the page drawn first and the page drawn second
    /// prefilled with other pictures (V-RAKETE, V-ANBAU, V-BERGE), both draw-page parities, four random states. Equal: the
    /// calls with their arguments (stars set or not, colours), the random state and the number of draws, the data segment,
    /// both pages in full and the draw page afterwards, and the frames yielded (one flip).
    /// </summary>
    [GameDataFact]
    public void Star_show_matches_the_original()
    {
        var rakete = Images((int)PictureId.Rakete);
        var others = new[] { ("V-ANBAU", Images((int)PictureId.Anbau)), ("V-BERGE", Images((int)PictureId.Berge)) };
        int hits1 = 0, hits2 = 0, negative = 0;
        foreach (bool flipFirst in new[] { false, true })
            foreach (var (name, other) in others)
                foreach (uint seed in new uint[] { 0x0000_0001, 0x1234_5678, 0x8000_0000, 0xDEAD_BEEF })
                {
                    var h = new Harness();
                    var p = h.Program;
                    p.Call(0x186C, 0x0002, (ushort)PictureId.Berge);
                    if (flipFirst) p.Call(0x21FD, 0x051F);
                    p.WriteDword(OriginalProgram.RandomStateOffset, seed);
                    var page = new RulePage(RulePage.DrawPageOf(Ds(p)));
                    FillVram(p, page.DrawPage, rakete);
                    FillVram(p, 1 - page.DrawPage, other);
                    LoadPage(page, page.DrawPage, rakete);
                    LoadPage(page, 1 - page.DrawPage, other);
                    var core = Ds(p);
                    var random = Random(core);
                    h.Calls.Clear();
                    h.RandomArgs.Clear();
                    p.Call(Seg, 0x0112);

                    var host = new PortHost(page);
                    int frames = Catastrophes.StarShow(core, random, host, page, Images((int)PictureId.Berge)).Count();
                    core.Set(L.RandomSeed, random.Seed);
                    string label = $"{name}, seed {seed:X8}, flip {flipFirst}";
                    Assert.Equal(h.Calls, host.Calls);
                    AssertSameState(p, core, random, h, label);
                    AssertPages((Vram(p), RulePage.DrawPageOf(Ds(p))), ([page.PagePixels(0), page.PagePixels(1)], page.DrawPage),
                        RulePage.Height, label);
                    Assert.Equal(Planting.Flip(core).Count(), frames);
                    hits1 += h.Reads.Count(r => r.Loop == 1 && r.Value == 1);
                    hits2 += h.Reads.Count(r => r.Loop == 2 && r.Value == 1);
                    negative += h.Reads.Count(r => r.Y < 0);
                }
        output.WriteLine($"stars set: loop 1 {hits1}, loop 2 {hits2}; reads with negative y {negative}");
        Assert.True(hits1 > 0 && hits2 > 0 && negative > 0);
    }

    /// <summary>
    /// [DIFFERENTIAL] 111D:0009, 111D:0112, 111D:0264, 111D:03B7, 111D:0757 – 0009 after its guard with the real drawing of the
    /// original: the cases of <see cref="CatastropheDifferentialTests"/> (weeks 3, 4, 5, 15, 30, three grids, both land types,
    /// none / kind 0 / kind 1). At the entry of 0112 the rule page equals the VRAM in the rows 0112 reads (0–150) on both
    /// pages and the draw page is the same; afterwards calls, random state and data segment are equal.
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(CatastropheDifferentialTests.Seeds), MemberType = typeof(CatastropheDifferentialTests))]
    public void Catastrophe_with_star_show_matches_the_original(int seed)
    {
        var c = CatastropheDifferentialTests.CaseFor(seed);
        var h = new Harness();
        var p = h.Program;
        CatastropheDifferentialTests.SetState(p, c);
        var core = Ds(p);
        var random = Random(core);
        var page = new RulePage(RulePage.DrawPageOf(core));
        p.Call(Seg, 0x0009);

        var host = new PortHost(page) { State = core };
        foreach (var _ in Catastrophes.Run(core, random, host, page, Images)) { }
        core.Set(L.RandomSeed, random.Seed);
        Assert.Equal(h.Calls, host.Calls);
        Assert.Equal(h.AtStarShow is null, host.AtStarShow is null);
        if (h.AtStarShow is { } atOriginal)
        {
            AssertPages(atOriginal, host.AtStarShow!.Value, StarRows, $"{c}, entry of 0112");
            output.WriteLine($"{c}: stars set {h.Reads.Count(r => r.Value == 1)} of {h.Reads.Count}");
        }
        AssertSameState(p, core, random, h, c.ToString());
        Assert.Equal(RulePage.DrawPageOf(Ds(p)), page.DrawPage);
    }

    /// <summary>Rows 0112 reads: 0–150 on the page drawn first, −3–147 on the other (random(n) is 0…n).</summary>
    private const int StarRows = Catastrophes.StarRangeY + 1;

    /// <summary>
    /// [DIFFERENTIAL] 0A96:0267, 0609:1ED9, 0609:1256 – background and frames of the field picture on the rule page against the
    /// command list executed by 0A96:06A8 (V-ANBAU in the atlas): the frame of 0009, the message frame of 0757 and frames with
    /// widths and heights not a multiple of the tiles; then both pages equal the VRAM in full.
    /// </summary>
    [GameDataFact]
    public void Field_backdrop_matches_the_command_list()
    {
        var anbau = Images((int)PictureId.Anbau);
        var rakete = Images((int)PictureId.Rakete);
        var p = OriginalProgram.Resume(TestGameData.DosDir, MenuSceneHarness.AtMenu.Value);
        p.Call(0x186C, 0x0002, (ushort)PictureId.Anbau);
        var page = new RulePage(RulePage.DrawPageOf(Ds(p)));
        for (int i = 0; i < 2; i++)
        {
            FillVram(p, i, rakete);
            LoadPage(page, i, rakete);
        }
        ushort U(int v) => unchecked((ushort)v);
        (short X, short Y, short W, short H)[] frames = [(0x8C, 0x88, 0xA0, 0x2C), (0x28, 0x4B, 0xF0, 0x32), (3, 5, 0x51, 0x47), (100, 120, 0x40, 0x2D)];
        p.Call(0x0A96, 0x001B);
        p.Call(0x0609, 0x1ED9);
        FieldBackdrop.Background(page, anbau);
        p.Call(0x0A96, 0x06A8);
        p.Call(0x21FD, 0x051F);
        page.Flip();
        p.Call(0x0A96, 0x001B);
        foreach (var (x, y, w, h) in frames)
        {
            p.Call(0x0609, 0x1256, U(x), U(y), U(w), U(h));
            FieldBackdrop.Frame(page, anbau, x, y, w, h);
        }
        p.Call(0x0A96, 0x06A8);
        AssertPages((Vram(p), RulePage.DrawPageOf(Ds(p))), ([page.PagePixels(0), page.PagePixels(1)], page.DrawPage),
            RulePage.Height, "backdrop");
    }
}
