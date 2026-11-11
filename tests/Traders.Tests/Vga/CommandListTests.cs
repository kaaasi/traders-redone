using Traders.Tests.RefEmu;
using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the draw command list 0A96 and the helpers 0A69: random entries of all 17 types are
/// enqueued by the original routines in the RefEmu and by the port (data segment equal after each call), then the
/// list is executed twice by 0A96:06A8 (VRAM, draw page, colours and data segment equal after each run).
/// </summary>
public class CommandListTests
{
    private const ushort List = 0x0A96, Helpers = 0x0A69, Numbers = 0x2295;

    /// <summary>Texts in the data segment (zero-terminated), offsets returned.</summary>
    private const ushort TextArea = 0xC000;

    private sealed class Run(LayerHarness h)
    {
        public LayerHarness H { get; } = h;
        public List<ushort> Texts { get; } = [];

        public void Both(ushort segment, ushort offset, ushort[] args, Action<GraphicsLayers> port, string what)
        {
            H.Call(segment, offset, args);
            port(H.Port);
            H.AssertStateEqual(what);
        }
    }

    private static Run Start(int seed, ushort cardMode = 1)
    {
        var h = new LayerHarness(seed, cardMode);
        h.Driver.SetFont((ushort)h.Rng.Next(2));
        var run = new Run(h);
        ushort at = TextArea;
        for (int i = 0; i < 12; i++)
        {
            var text = new byte[i == 0 ? 0 : h.Rng.Next(1, 14)];
            for (int k = 0; k < text.Length; k++) text[k] = (byte)h.Rng.Next(32, 92);
            h.WriteBoth(at, [.. text, 0]);
            run.Texts.Add(at);
            at = (ushort)(at + text.Length + 1);
        }
        run.Both(List, 0x001B, [], l => l.ClearList(), "001B");
        return run;
    }

    /// <summary>One random entry of the given type (0…16) through its enqueue routine; 17 = 02D6, 18–19 numbers.</summary>
    private static void AddRandom(Run run, int kind, int i)
    {
        var h = run.H;
        ushort ds = h.Original.Dgroup;
        ushort W(int lo, int hi) => h.Next(lo, hi);
        string what = $"enqueue kind {kind} #{i}";
        switch (kind)
        {
            case 0 or 1 or 2 or 5:
            {
                ushort w = W(1, 64), hh = W(1, 40);
                ushort sx = W(0, 320 - w), sy = W(0, 256 - hh);
                ushort dx = kind == 1 ? W(0, 320 - w) : W(0, 319), dy = kind == 1 ? W(0, 256 - hh) : W(0, 199);
                if (kind == 2) sy = W(0, 199);
                ushort offset = kind switch { 0 => 0x0030, 1 => 0x0089, 2 => 0x00E2, _ => 0x020E };
                Action<GraphicsLayers> port = kind switch
                {
                    0 => l => l.AddBlit(sx, sy, w, hh, dx, dy),
                    1 => l => l.AddAtlasCopy(sx, sy, w, hh, dx, dy),
                    2 => l => l.AddPageCopy(sx, sy, w, hh, dx, dy),
                    _ => l => l.AddOrBlit(sx, sy, w, hh, dx, dy),
                };
                run.Both(List, offset, [sx, sy, w, hh, dx, dy], port, what);
                break;
            }
            case 3:
            {
                ushort w = W(4, 64), hh = W(1, 40);
                ushort sx = W(0, 320 - w), sy = W(0, 256 - hh), mx = W(0, 304 - w), my = W(0, 200 - hh);
                ushort dx = W(0, 319), dy = W(0, 199);
                run.Both(List, 0x013B, [sx, sy, mx, my, w, hh, dx, dy], l => l.AddMaskedBlit(sx, sy, mx, my, w, hh, dx, dy), what);
                break;
            }
            case 4:
            {
                ushort w = W(4, 64), hh = W(1, 40), mx = W(0, 304 - w), my = W(0, 200 - hh);
                ushort dx = W(0, 319), dy = W(0, 199), c = W(0, 255);
                run.Both(List, 0x01AA, [mx, my, w, hh, dx, dy, c], l => l.AddMaskFill(mx, my, w, hh, dx, dy, c), what);
                break;
            }
            case 6:
            {
                ushort tw = W(1, 24), th = W(1, 16), w = W(1, 80), hh = W(1, 50);
                ushort sx = W(0, 320 - tw), sy = W(0, 256 - th), dx = W(0, 300), dy = W(0, 190);
                run.Both(List, 0x0267, [sx, sy, tw, th, dx, dy, w, hh], l => l.AddTiles(sx, sy, tw, th, dx, dy, w, hh), what);
                break;
            }
            case 7 or 8 or 16:
            {
                int units = kind == 7 ? 1 : kind == 8 ? 2 : 4;
                ushort hh = W(1, 40), sx = W(0, 320 - units * 8), sy = W(0, 256 - hh), dx = W(0, 300), dy = W(0, 199);
                ushort offset = kind == 7 ? (ushort)0x0311 : kind == 8 ? (ushort)0x035F : (ushort)0x03AD;
                Action<GraphicsLayers> port = kind == 7 ? l => l.AddBlit8(sx, sy, hh, dx, dy)
                    : kind == 8 ? l => l.AddBlit16(sx, sy, hh, dx, dy) : l => l.AddBlit32(sx, sy, hh, dx, dy);
                run.Both(List, offset, [sx, sy, hh, dx, dy], port, what);
                break;
            }
            case 9:
            {
                ushort x1 = W(0, 319), y1 = W(0, 199), x2 = W(0, 319), y2 = W(0, 199), c = W(0, 255);
                run.Both(List, 0x0433, [x1, y1, x2, y2, c], l => l.AddLine(x1, y1, x2, y2, c), what);
                break;
            }
            case 10 or 11:
            {
                ushort w = W(1, 100), hh = W(1, 60), x = W(0, 320 - w), y = W(0, 200 - hh), c = W(0, 255);
                if (kind == 10) run.Both(List, 0x0481, [x, y, w, hh, c], l => l.AddFrame(x, y, w, hh, c), what);
                else run.Both(List, 0x04D7, [x, y, w, hh, c], l => l.AddFilledRect(x, y, w, hh, c), what);
                break;
            }
            case 12:
            {
                ushort x = W(0, 120), y = W(14, 199), c = W(0, 255), shadow = h.Rng.Next(3) == 0 ? (ushort)0xFFFF : W(0, 255);
                ushort text = run.Texts[h.Rng.Next(run.Texts.Count)];
                ushort seg = h.Rng.Next(8) == 0 ? (ushort)0 : ds;   // null pointer: nothing drawn
                if (seg == 0) text = 0;
                run.Both(List, 0x0525, [x, y, text, seg, c, shadow], l => l.AddText(x, y, text, seg, c, shadow), what);
                break;
            }
            case 13 or 18:
            {
                ushort x = W(0, 120), y = W(14, 199), c = W(0, 255), shadow = h.Rng.Next(3) == 0 ? (ushort)0xFFFF : W(0, 255);
                uint n = h.Rng.Next(3) switch { 0 => (uint)h.Rng.Next(0, 1000), 1 => 0, _ => (uint)h.Rng.NextInt64(0, 0x1_0000_0000) };
                bool dollar = kind == 18;
                // more digits run over the entry's type word at + 1Ah (the original then calls a garbage handler)
                ushort digits = h.Rng.Next(6) == 0 ? (ushort)0xFFFF : W(0, dollar ? 6 : 7);
                run.Both(List, dollar ? (ushort)0x05FB : (ushort)0x057A, [x, y, (ushort)n, (ushort)(n >> 16), digits, c, shadow],
                    l => { if (dollar) l.AddAmount(x, y, n, digits, c, shadow); else l.AddNumber(x, y, n, digits, c, shadow); }, what);
                break;
            }
            case 14:
            {
                ushort p = h.Rng.Next(3) == 0 ? (ushort)0xFFFF : W(0, 0xFFFF);
                run.Both(List, 0x0686, [p], l => l.AddLinePattern(p), what);
                break;
            }
            case 15:
            {
                ushort x = W(0, 319), y = W(0, 199), c = W(0, 255);
                run.Both(List, 0x03FB, [x, y, c], l => l.AddPixel(x, y, c), what);
                break;
            }
            case 17:
            {
                ushort hh = W(1, 30), sx = W(0, 280), sy = W(0, 256 - hh), dx = W(0, 260), dy = W(0, 199);
                run.Both(List, 0x02D6, [sx, sy, hh, dx, dy], l => l.AddBlit40(sx, sy, hh, dx, dy), what);
                break;
            }
        }
    }

    private static void ExecuteTwice(Run run, string what)
    {
        for (int k = 0; k < 2; k++)
        {
            run.H.Driver.RandomPage();
            run.Both(List, 0x06A8, [], l => l.ExecuteList(), $"{what} run {k}");
            run.H.AssertScreenEqual($"{what} run {k}");
            Assert.Equal(run.H.Driver.DriverWord(0x0588), run.H.Driver.Port.Color);
            Assert.Equal(run.H.Driver.DriverWord(0x04F4), run.H.Driver.Port.LinePattern);
        }
    }

    // [DIFFERENTIAL] 0A96:001B
    // [DIFFERENTIAL] 0A96:06A8
    // [DIFFERENTIAL] 0A96:0030
    // [DIFFERENTIAL] 0A96:0089
    // [DIFFERENTIAL] 0A96:00E2
    // [DIFFERENTIAL] 0A96:013B
    // [DIFFERENTIAL] 0A96:01AA
    // [DIFFERENTIAL] 0A96:020E
    // [DIFFERENTIAL] 0A96:0267
    // [DIFFERENTIAL] 0A96:02D6
    // [DIFFERENTIAL] 0A96:0311
    // [DIFFERENTIAL] 0A96:035F
    // [DIFFERENTIAL] 0A96:03AD
    // [DIFFERENTIAL] 0A96:03FB
    // [DIFFERENTIAL] 0A96:0433
    // [DIFFERENTIAL] 0A96:0481
    // [DIFFERENTIAL] 0A96:04D7
    // [DIFFERENTIAL] 0A96:0525
    // [DIFFERENTIAL] 0A96:057A
    // [DIFFERENTIAL] 0A96:05FB
    // [DIFFERENTIAL] 0A96:0686
    // [DIFFERENTIAL] 0A96:0C68
    // [DIFFERENTIAL] 0A96:07E4
    // [DIFFERENTIAL] 0A96:0884
    [GameDataTheory]
    [InlineData(1, 300)]
    [InlineData(0, 301)]
    public void List_with_all_types_matches_original(ushort cardMode, int seed)
    {
        var run = Start(seed, cardMode);
        var h = run.H;
        if (cardMode != 1) h.WriteWordBoth(GraphicsLayers.EgaPalette, h.Next(0, 3));
        for (int round = 0; round < 6; round++)
        {
            ushort adv = h.Next(1, 12), height = h.Next(1, 12);
            run.Both(List, 0x000A, [adv, height], l => l.SetTextShadow(adv, height), "000A");
            int count = h.Rng.Next(8, 30);
            for (int i = 0; i < count; i++)
                AddRandom(run, i < 20 && round == 0 ? i : h.Rng.Next(20), i);
            ExecuteTwice(run, $"mode {cardMode} round {round}");
            run.Both(List, 0x001B, [], l => l.ClearList(), "001B");
        }
    }

    // [DIFFERENTIAL] 0A69:000B
    // [DIFFERENTIAL] 0A69:002C
    // [DIFFERENTIAL] 0A69:004D
    // [DIFFERENTIAL] 0A69:0090
    // [DIFFERENTIAL] 0A69:00D3
    // [DIFFERENTIAL] 0A69:011B
    // [DIFFERENTIAL] 0A69:0164
    [GameDataFact]
    public void Helpers_0A69_match_original()
    {
        var run = Start(310);
        var h = run.H;
        ushort ds = h.Original.Dgroup;
        for (int round = 0; round < 5; round++)
        {
            ushort advance = h.Next(1, 16);
            h.WriteWordBoth(GraphicsLayers.FontAdvance, round == 4 ? (ushort)0xFFF0 : advance);
            for (int i = 0; i < 12; i++)
            {
                ushort x = h.Next(0, 150), y = h.Next(14, 190), c = h.Next(0, 255), s = h.Next(0, 255);
                ushort ox = h.Next(0, 3), oy = h.Next(0, 3), w = h.Next(0, 200);
                ushort text = run.Texts[h.Rng.Next(run.Texts.Count)], seg = ds;
                if (h.Rng.Next(10) == 0) text = seg = 0;
                uint n = (uint)h.Rng.NextInt64(0, 100000);
                ushort digits = h.Next(1, 6);
                string what = $"helper {i % 7} round {round} #{i}";
                switch (i % 7)
                {
                    case 0:
                    {
                        ushort len = h.Next(1, 320 - x);   // lines stay on the screen (the driver's line routine divides)
                        run.Both(Helpers, 0x000B, [x, y, len, c], l => l.AddHorizontalLine(x, y, len, c), what);
                        break;
                    }
                    case 1:
                    {
                        ushort len = h.Next(1, 200 - y);
                        run.Both(Helpers, 0x002C, [x, y, len, c], l => l.AddVerticalLine(x, y, len, c), what);
                        break;
                    }
                    case 2:
                        if (seg == 0) break;
                        run.Both(Helpers, 0x004D, [x, y, text, seg, c, s, ox, oy], l => l.AddShadowText(x, y, text, seg, c, s, ox, oy), what);
                        break;
                    case 3:
                    {
                        ushort shadow = h.Rng.Next(2) == 0 ? (ushort)0xFFFF : s;
                        run.Both(Helpers, 0x0090, [x, y, w, text, seg, c, shadow], l => l.AddCenteredText(x, y, w, text, seg, c, shadow), what);
                        break;
                    }
                    case 4:
                        run.Both(Helpers, 0x00D3, [x, y, w, text, seg, c, s, ox, oy],
                            l => l.AddCenteredShadowText(x, y, w, text, seg, c, s, ox, oy), what);
                        break;
                    case 5:
                        run.Both(Helpers, 0x011B, [x, y, (ushort)n, (ushort)(n >> 16), digits, c, s, ox, oy],
                            l => l.AddShadowNumber(x, y, n, digits, c, s, ox, oy), what);
                        break;
                    default:
                        run.Both(Helpers, 0x0164, [x, y, (ushort)n, (ushort)(n >> 16), digits, c, s, ox, oy],
                            l => l.AddShadowAmount(x, y, n, digits, c, s, ox, oy), what);
                        break;
                }
            }
            ExecuteTwice(run, $"helpers round {round}");
            run.Both(List, 0x001B, [], l => l.ClearList(), "001B");
        }
    }

    // [DIFFERENTIAL] 2295:0001
    // [DIFFERENTIAL] 2295:0062
    [GameDataFact]
    public void FormatNumber_2295_0001_matches_original()
    {
        var h = new LayerHarness(320);
        ushort ds = h.Original.Dgroup;
        uint[] numbers = [0, 1, 9, 10, 100, 12345, 99999, 4294967295, 0x8000_0000, 1000000];
        for (int i = 0; i < 120; i++)
        {
            if (i % 15 == 0)
            {
                ushort fill = i == 0 ? (ushort)' ' : h.Rng.Next(3) == 0 ? (ushort)'0' : h.Next(1, 255);
                h.Call(Numbers, 0x0062, fill);
                h.Port.SetNumberFill(fill);
            }
            uint n = i < numbers.Length ? numbers[i] : h.Rng.Next(2) == 0 ? (uint)h.Rng.Next(0, 1000) : (uint)h.Rng.NextInt64(0, 0x1_0000_0000);
            ushort width = h.Rng.Next(10) == 0 ? (ushort)(0x10000 - h.Rng.Next(1, 6)) : h.Next(0, 21);
            h.Randomize(0xD000, 32);
            h.Call(Numbers, 0x0001, (ushort)n, (ushort)(n >> 16), width, 0xD004, ds);
            h.Port.FormatNumber(n, width, 0xD004, ds);
            h.AssertStateEqual($"2295:0001({n},{(short)width})");
        }
    }

    /// <summary>
    /// Shadow rectangle sizes from DS:0142/0143 (signed bytes). Negative values give rectangles of almost 64 K rows or
    /// columns in both implementations and are left out (the game sets 8, 8 and small positive values).
    /// </summary>
    // [DIFFERENTIAL] 0A96:000A
    [GameDataFact]
    public void Text_shadow_sizes_match_original()
    {
        var run = Start(330);
        var h = run.H;
        ushort ds = h.Original.Dgroup;
        foreach (var (adv, height) in new (ushort, ushort)[] { (8, 8), (6, 4), (12, 2), (3, 1), (1, 12) })
        {
            run.Both(List, 0x000A, [adv, height], l => l.SetTextShadow(adv, height), $"000A({adv},{height})");
            ushort text = run.Texts[5];
            run.Both(List, 0x0525, [100, 100, text, ds, 15, 3], l => l.AddText(100, 100, text, ds, 15, 3), "0525");
            ExecuteTwice(run, $"shadow {adv},{height}");
            run.Both(List, 0x001B, [], l => l.ClearList(), "001B");
        }
    }

    /// <summary>Frames and lines after a line pattern (type 14): the direction of each of the four lines is visible.</summary>
    // [DIFFERENTIAL] 0A96:0A51
    [GameDataFact]
    public void Frames_with_line_pattern_match_original()
    {
        var run = Start(350);
        var h = run.H;
        foreach (ushort pattern in new ushort[] { 0x00FF, 0x8001, 0xF0F0, 0x1234 })
        {
            run.Both(List, 0x0686, [pattern], l => l.AddLinePattern(pattern), $"0686({pattern:X4})");
            for (int i = 0; i < 6; i++)
            {
                ushort w = h.Next(2, 120), hh = h.Next(2, 80), x = h.Next(0, 320 - w), y = h.Next(0, 200 - hh), c = h.Next(0, 255);
                run.Both(List, 0x0481, [x, y, w, hh, c], l => l.AddFrame(x, y, w, hh, c), "0481");
            }
        }
        ExecuteTwice(run, "patterned frames");
    }

    private sealed class Effects : ITypewriterEffects
    {
        public int Sounds { get; private set; }

        public void PlaySound(ushort a, ushort b)
        {
            Assert.Equal((1, 0x16), (a, b));
            Sounds++;
        }

        public void WaitTimerFlag() => Assert.Fail("DS:3D60 is 0 in these tests");
    }

    /// <summary>
    /// Typewriter text: sound off (DS:10A5 = DS:10A7 = 0) and timer flag DS:3D60 = 0, so the original's 21A7:0054 and
    /// 21FD:0245 return at once; their calls are counted at the entry and must match the port's effects.
    /// </summary>
    // [DIFFERENTIAL] 0A69:01AD
    // [DIFFERENTIAL] 0A69:0243
    // [DIFFERENTIAL] 0A69:0288
    [GameDataFact]
    public void Typewriter_0A69_matches_original()
    {
        var run = Start(360);
        var h = run.H;
        ushort ds = h.Original.Dgroup;
        int sounds = 0;
        h.Original.Machine.AddCodeHook(h.Original.Segment(0x21A7), 0x0054, _ => sounds++);
        var effects = new Effects();
        h.Port.Effects = effects;
        h.WriteWordBoth(0x10A5, 0);
        h.WriteWordBoth(0x10A7, 0);
        h.WriteBoth(GraphicsLayers.TimerFlag, [0]);
        h.WriteBoth(0x0141, [0]);
        h.WriteDwordBoth(GraphicsLayers.Page0, 0xA000_0000);
        h.WriteDwordBoth(GraphicsLayers.Page1, 0xA400_0000);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers, 1);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, 2);
        h.WriteBoth(GraphicsLayers.NoFlipWait, [0]);
        h.WriteBoth(GraphicsLayers.SlowFlip, [0]);
        h.WriteWordBoth(GraphicsLayers.FontAdvance, 13);
        h.WriteWordBoth(GraphicsLayers.FontLineHeight, 12);
        // texts with blanks
        h.WriteBoth(0xC400, "AB C  D\0"u8);
        // line table: three far pointers
        ushort[] lines = [0xC400, run.Texts[3], run.Texts[0]];
        for (int i = 0; i < lines.Length; i++) h.WriteDwordBoth(0xC500 + 4 * i, ((uint)ds << 16) | lines[i]);
        ushort text = run.Texts[4];
        foreach (var (wait, final) in new (ushort, ushort)[] { (0, 0), (2, 3), (0xFFFF, 0xFFFE), (1, 0) })
        {
            run.Both(Helpers, 0x01AD, [10, 40, 0xC400, ds, 15, wait, final], l => l.Typewriter(10, 40, 0xC400, ds, 15, wait, final), $"01AD {wait}");
            run.H.AssertScreenEqual($"01AD {wait}");
            run.Both(Helpers, 0x01AD, [10, 40, 0, 0, 15, wait, final], l => l.Typewriter(10, 40, 0, 0, 15, wait, final), $"01AD null {wait}");
            run.Both(Helpers, 0x0243, [0, 80, 320, text, ds, 9, wait, final], l => l.CenteredTypewriter(0, 80, 320, text, ds, 9, wait, final), $"0243 {wait}");
            run.H.AssertScreenEqual($"0243 {wait}");
            run.Both(Helpers, 0x0243, [0, 80, 320, 0, 0, 9, wait, final], l => l.CenteredTypewriter(0, 80, 320, 0, 0, 9, wait, final), $"0243 null {wait}");
        }
        run.Both(Helpers, 0x0288, [5, 100, 300, 0xC500, ds, 3, 7, 1], l => l.TypewriterLines(5, 100, 300, 0xC500, ds, 3, 7, 1), "0288");
        run.H.AssertScreenEqual("0288");
        Assert.True(sounds > 0);
        Assert.Equal(sounds, effects.Sounds);
    }

    // [DIFFERENTIAL] 0A96:0C3F
    [GameDataFact]
    public void CopyGamePage_0A96_0C3F_matches_original()
    {
        var h = new LayerHarness(340);
        h.WriteDwordBoth(GraphicsLayers.Page0, 0xA000_0000);
        h.WriteDwordBoth(GraphicsLayers.Page1, 0xA400_0000);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers, 1);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, 2);
        foreach (ushort index in new ushort[] { 0, 1, 1, 0 })
        {
            for (int p = 0; p < 4; p++)
            {
                h.Rng.NextBytes(h.Driver.Port.Vram.Planes[p]);
                h.Driver.Port.Vram.Planes[p].CopyTo(h.Original.Machine.Vga.Planes[p], 0);
            }
            h.Call(List, 0x0C3F, index);
            h.Port.CopyGamePage(index);
            h.AssertScreenEqual($"0C3F({index})");
        }
    }
}
