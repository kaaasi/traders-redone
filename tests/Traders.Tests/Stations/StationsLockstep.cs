using Traders.Assets;
using Traders.Core;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.RefEmu.Cpu;
using Traders.Tests.Presentation;

namespace Traders.Tests.Stations;

/// <summary>
/// Lockstep of the stations phase with real drawing (lane stationen part 2): the original runs a routine of 1B03 in the
/// RefEmu with its whole output (command list, VGA pages, objects, palettes) from a snapshot with the graphics set up; only
/// sounds 21A7:0054 and the retrace waits 21FD:0185 are replaced by far returns. The port runs the same routine on a copy of
/// the original's data segment with a <see cref="StationsPage"/> on a <see cref="RulePage"/> prefilled from the VRAM.
/// Compared: every read of the pixel reader 23CD:06DE (arguments and colour), the data segment and the number of random()
/// calls at every page flip of 1B03 (one frame) and at the end, the number of frames. Keys for the next pass are written
/// into the key table at every frame on both sides. Expected values come from the original only.
/// </summary>
internal static class StationsLockstep
{
    public const ushort Seg = 0x1B03;

    /// <summary>Return addresses of the calls of 23CD:06DE in 1B03 (1178, 08ED, 0BE7).</summary>
    private static readonly ushort[] AfterRead = [0x11D0, 0x09B6, 0x0C03];

    /// <summary>
    /// Data segment bytes written by the output layers in the original (the port reports these calls as events): the command
    /// list, page and display state, the object records' pointers and restore records, palette and fade buffers, the
    /// retrace and timer counters.
    /// </summary>
    public static readonly List<(int Offset, int Length, string Owner)> OutputOwned =
    [
        .. Field.PlantingDifferentialTests.OutputOwned,
        (0x5894, 8, "C runtime tzset: time zone names"),
        (0x117E, 4, "27C6 timer interrupt: tick counters"),
        (0x15BA, 6 + 100 * 0x1C, "0A96 command list: pointer, count, entries (up to 100)"),
        (0x0140, 1, "0A69:01C3 typed text"),
        (0x0238, 4, "0B67 shown page / draw on hidden page"),
        (0x082E, 4, "21FD:051F page indices"),
        (0x4516, 2, "21FD:051F draw page number"),
        (0x09C8, 2, "186C:0002 picture loader"),
        (0x10D8, 2, "21FD:04E0 palette fade"),
        (0x3068, 5 * 0x54, "21AF:00CA frame images and masks of objects 0…4"),
        (0x3424, 0x34A6 + 5 * 0x54 + 0x10 - 0x3424, "21AF:022B save buffers and restore records of objects 0…4"),
        (0x3DCC, 0x3E2B - 0x3DCC, "palette buffer"),
        (0x3FAB, 0x3FDB - 0x3FAB, "palette buffer"),
        (0x406B, 0x40C8 - 0x406B, "palette buffer"),
        (0x5018, 0x5077 - 0x5018, "palette buffer"),
        (0x51F7, 0x5227 - 0x51F7, "palette buffer"),
        (0x52B7, 0x5314 - 0x52B7, "palette buffer"),
        (0x54E8, 0x300, "21FD:04AD palette fade target"),
    ];

    public static readonly Lazy<IndexedImage> Rakete = new(() =>
    {
        var image = VgaImage.Load(TestGameData.Dos("V-RAKETE.PP"));
        return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
    });

    public static readonly Lazy<byte[]> Masks = new(() => PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos("MASKS.PP"))));

    private static readonly Lazy<byte[]> FontBytes = new(() => PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos("FONTS.PP"))));

    /// <summary>The driver's font table as 0DFD:0158 leaves it (as Traders.App's host): 0 small, 1 large, 2 digits.</summary>
    public static TextFont Font(int index) => index switch
    {
        0 => new TextFont(FontBytes.Value, 0x5A0, 5, 6, 0x20),
        1 => new TextFont(FontBytes.Value, 0, 12, 14, 0x20),
        2 => new TextFont(FontBytes.Value, 0x7F8, 5, 5, 0x2F),
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// State at a frame (1B03 calls 21FD:051F): data segment, random() calls so far, hash of both physical pages outside the
    /// panel, physical draw page before the flip.
    /// </summary>
    public sealed record Frame(byte[] Ds, long RandomCalls, ulong Pages, int DrawPage);

    /// <summary>FNV-1a over pages 0 and 1 row by row, pixels in the message panel (<see cref="InPanel"/>) left out.</summary>
    public static ulong PageHash(Func<int, byte[]> page)
    {
        ulong h = 14695981039346656037;
        for (int q = 0; q < 2; q++)
        {
            var pixels = page(q);
            for (int y = 0; y < RulePage.Height; y++)
                for (int x = 0; x < RulePage.Width; x++)
                {
                    if (InPanel((short)x, (short)y)) continue;
                    h = (h ^ pixels[y * RulePage.Width + x]) * 1099511628211;
                }
        }
        return h;
    }

    public sealed record Run(List<Frame> Frames, List<PixelRead> Reads, byte[] Ds, long RandomCalls, Exception? Fault);

    /// <summary>A prepared original: the program at the snapshot with the state set up, and the clock values random(0) saw.</summary>
    public sealed class Original
    {
        public required OriginalProgram Program { get; init; }
        public required byte[] StartDs { get; init; }
        public required IndexedImage[] StartPages { get; init; }
        public required List<int> Clock { get; init; }
        public required TextFile Texts { get; init; }
        public required Run Result { get; init; }
    }

    /// <summary>Physical page <paramref name="page"/> of the RefEmu VRAM as a picture.</summary>
    public static IndexedImage VramPage(OriginalProgram p, int page) =>
        new(RulePage.Width, RulePage.Height, VramPixels(p, page));

    /// <summary>Physical page <paramref name="page"/> of the RefEmu VRAM, row-major.</summary>
    public static byte[] VramPixels(OriginalProgram p, int page)
    {
        var planes = p.Machine.Vga.Planes;
        var pixels = new byte[RulePage.Width * RulePage.Height];
        for (int y = 0; y < RulePage.Height; y++)
            for (int x = 0; x < RulePage.Width; x++)
                pixels[y * RulePage.Width + x] = planes[x & 3][page * RulePage.PageDistance + y * RulePage.RowBytes + (x >> 2)];
        return pixels;
    }

    /// <summary>
    /// Runs 1B03:<paramref name="off"/> in the original from the menu snapshot after <paramref name="setUp"/> (which may call
    /// other routines and change the data segment) with the font <paramref name="font"/> selected by 21FD:05DB.
    /// </summary>
    public static Original RunOriginal(Action<OriginalProgram> setUp, ushort off, int font,
        Func<int, IReadOnlyCollection<byte>> keys, MachineSnapshot? from = null)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, from ?? MenuSceneHarness.AtMenu.Value);
        var m = p.Machine;
        foreach (var (seg, o) in new (ushort, ushort)[] { (0x21A7, 0x0054), (0x21FD, 0x0185) })
            m.Mem.Write8(p.Segment(seg), o, 0xCB);
        if (from is null) p.Call(0x21FD, 0x05DB, (ushort)font);
        setUp(p);
        // the rule page starts without objects: no valid restore record, objects 5…7 inactive
        for (int i = 0; i < 8; i++)
        {
            ushort rec = (ushort)(0x33E4 + 8 * i);
            if (i >= 4)
                Assert.True(p.ReadWord((ushort)(rec + 2)) == 0 && p.ReadWord((ushort)(rec + 6)) == 0, $"object {i}: restore record valid at the start");
            if (i >= 5) Assert.True(p.ReadByte((ushort)(0x330C + 0x1A * i)) == 0, $"object {i} active at the start");
        }
        var clock = new List<int>();
        long random = 0;
        var frames = new List<Frame>();
        var reads = new List<PixelRead>();
        short rx = 0, ry = 0;
        bool armed = false;
        int pass = 0;
        ushort seg1B03 = p.Segment(Seg);
        m.AddCodeHook(p.Segment(0x21FD), 0x01BA, _ => { if (armed) random++; });
        m.AddCodeHook(p.Segment(0x21FD), 0x01CE, mm => { if (armed) clock.Add(mm.Cpu.Ax | (mm.Cpu.Dx << 16)); });
        m.AddCodeHook(p.Segment(0x23CD), 0x06DE, _ => { rx = (short)p.StackArgument(0); ry = (short)p.StackArgument(1); });
        foreach (ushort after in AfterRead)
            m.AddCodeHook(seg1B03, after, mm => { if (armed) reads.Add(new PixelRead(rx, ry, (byte)mm.Cpu.Ax)); });
        m.AddCodeHook(p.Segment(0x21FD), 0x051F, mm =>
        {
            if (!armed || mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 2)) != seg1B03) return;
            frames.Add(new Frame(p.DataSegment()[..DgroupLayout.Size], random, PageHash(q => VramPixels(p, q)),
                RulePage.DrawPageOf(new Dgroup(p.DataSegment()[..DgroupLayout.Size]))));
            WriteKeys(p, keys(++pass));
        });
        var startDs = p.DataSegment();
        ushort textOff = p.ReadWord(MenuLayout.TextBuffer), textSeg = p.ReadWord(MenuLayout.TextBuffer + 2);
        var texts = new TextFile(m.Mem.ReadBlock(textSeg, textOff, 0x8000), textOff, textSeg);
        var pages = new[] { VramPage(p, 0), VramPage(p, 1) };
        WriteKeys(p, keys(0));
        armed = true;
        Exception? fault = null;
        try
        {
            m.CallFar(seg1B03, off, [], 4_000_000_000);
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        armed = false;
        var result = new Run(frames, reads, p.DataSegment()[..DgroupLayout.Size], random, fault);
        return new Original { Program = p, StartDs = startDs, StartPages = pages, Clock = clock, Texts = texts, Result = result };
    }

    private static void WriteKeys(OriginalProgram p, IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
        foreach (byte sc in held) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
    }

    /// <summary>
    /// Runs <paramref name="port"/> on the original's start state: the data segment, both VRAM pages on the rule page (draw
    /// page from DS:40DE[DS:082E]), V-RAKETE as atlas and font <paramref name="font"/>; random(0) gets the original's clock.
    /// </summary>
    public static (Run Run, StationPhase Phase) RunPort(Original o, int font, Func<StationPhase, IEnumerable<StationWait>> port,
        Func<int, IReadOnlyCollection<byte>> keys)
    {
        var d = new Dgroup(o.StartDs[..DgroupLayout.Size]);
        var page = new RulePage(RulePage.DrawPageOf(d));
        int draw = page.DrawPage;
        for (int i = 0; i < 2; i++)
        {
            page.SelectDrawPage(i);
            page.Blit(o.StartPages[i], 0, 0, RulePage.Width, RulePage.Height, 0, 0);
        }
        page.SelectDrawPage(draw);
        var random = new OriginalRandom(new RecordedClock(o.Clock.ToList()), d.U32(DgroupLayout.RandomSeed));
        var phase = new StationPhase(d, random, KeyboardOnlyDevices.Instance)
        {
            Pixels = new StationsPage(page, Rakete.Value, Masks.Value, Font(font), o.Texts, d),
        };
        var frames = new List<Frame>();
        int pass = 0;
        KeyTable.Write(d, keys(0));
        foreach (var w in port(phase))
        {
            if (w != StationWait.Flip) continue;
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            // the coroutine yields right after the flip: the draw page before it is the other one
            frames.Add(new Frame(d.Bytes[..DgroupLayout.Size], random.Calls, PageHash(page.PagePixels), 1 - page.DrawPage));
            KeyTable.Write(d, keys(++pass));
            if (pass > 100_000) throw new InvalidOperationException("Port coroutine did not finish.");
        }
        d.Set(DgroupLayout.RandomSeed, random.Seed);
        return (new Run(frames, phase.Pixels!.Reads.ToList(), d.Bytes[..DgroupLayout.Size], random.Calls, null), phase);
    }

    /// <summary>Byte offsets of the data segment both sides own (not <see cref="OutputOwned"/>).</summary>
    private static readonly Lazy<bool[]> Owned = new(() =>
    {
        var owned = new bool[DgroupLayout.Size];
        foreach (var (o, len, _) in OutputOwned)
            for (int i = o; i < o + len; i++) owned[i] = true;
        return owned;
    });

    public static List<string> Diff(byte[] original, byte[] port, int max = 8) =>
        Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => !Owned.Value[i] && original[i] != port[i])
            .Take(max)
            .Select(i => $"DS:{i:X4} original {original[i]:X2} port {port[i]:X2}")
            .ToList();

    /// <summary>
    /// The message panel 70h…CFh × AFh…C7h: its texts (0A69) are not on the rule page (see <see cref="StationsPage"/>), so a
    /// read there is compared as the original uses it, colour 1 or not; elsewhere the colour itself.
    /// </summary>
    public static bool InPanel(short x, short y) => x is >= 0x70 and <= 0xCF && y is >= 0xAF and <= 0xC7;

    public static bool SameRead(PixelRead original, PixelRead port) =>
        original.X == port.X && original.Y == port.Y &&
        (InPanel(original.X, original.Y) ? (original.Value == 1) == (port.Value == 1) : original.Value == port.Value);

    /// <summary>Original and port in lockstep; returns the original's run for coverage counts.</summary>
    public static Run AssertSame(Action<OriginalProgram> setUp, ushort off, int font, Func<StationPhase, IEnumerable<StationWait>> port,
        string label, Func<int, IReadOnlyCollection<byte>>? keys = null, MachineSnapshot? from = null)
    {
        keys ??= _ => [];
        var o = RunOriginal(setUp, off, font, keys, from);
        Assert.True(o.Result.Fault is null, $"{label}: original faulted: {o.Result.Fault?.Message}");
        var (run, _) = RunPort(o, font, port, keys);
        var orig = o.Result;
        for (int i = 0; i < Math.Min(orig.Reads.Count, run.Reads.Count); i++)
            Assert.True(SameRead(orig.Reads[i], run.Reads[i]), $"{label}: read {i}: original {orig.Reads[i]}, port {run.Reads[i]}");
        Assert.True(orig.Reads.Count == run.Reads.Count, $"{label}: reads original {orig.Reads.Count}, port {run.Reads.Count}");
        for (int i = 0; i < Math.Min(orig.Frames.Count, run.Frames.Count); i++)
        {
            var diff = Diff(orig.Frames[i].Ds, run.Frames[i].Ds);
            Assert.True(diff.Count == 0, $"{label}: frame {i}: " + string.Join(", ", diff));
            Assert.True(orig.Frames[i].DrawPage == run.Frames[i].DrawPage, $"{label}: frame {i}: draw page original {orig.Frames[i].DrawPage}, port {run.Frames[i].DrawPage}");
            Assert.True(orig.Frames[i].Pages == run.Frames[i].Pages, $"{label}: frame {i}: pages differ from the VRAM outside the panel");
            Assert.True(orig.Frames[i].RandomCalls == run.Frames[i].RandomCalls,
                $"{label}: frame {i}: random calls original {orig.Frames[i].RandomCalls}, port {run.Frames[i].RandomCalls}");
        }
        Assert.True(orig.Frames.Count == run.Frames.Count, $"{label}: frames original {orig.Frames.Count}, port {run.Frames.Count}");
        var end = Diff(orig.Ds, run.Ds);
        Assert.True(end.Count == 0, $"{label}: end: " + string.Join(", ", end));
        Assert.Equal(orig.RandomCalls, run.RandomCalls);
        return orig;
    }
}
