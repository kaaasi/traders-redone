using Traders.App;
using Traders.App.Rendering;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Menus;
using Traders.Tests.Startup;
using Traders.Vga.Layers;

namespace Traders.Tests.App;

/// <summary>
/// The picture the host shows in the menus, retrace by retrace against the original (finding R4W-1, #126): both run from the
/// program start with the same clock; from the third pass of the main menu loop (192D:0345) on, keys are pressed on both
/// sides at the same retrace, and at every retrace the displayed picture (shown page through the DAC), the CRTC start and
/// the page indices DS:082E/0830 of the host's layers are the original's. Unlike <see cref="Presentation.MenuFrameHarness"/>,
/// which sets up the scene from the original's data segment at the menu entry, this runs the host's own set-up
/// (<see cref="ProgramHost"/>), so the display part of the program start (pages 0DFD:0074, flip 0DFD:0132) is covered.
/// </summary>
// [DIFFERENTIAL] 0DFD:0074 (page numbers DS:40DE and 0B67:0238 in the host's layers)
// [DIFFERENTIAL] 0DFD:0132 (first page flip of the program start in the host's layers)
public sealed class HostMenuPictureDifferentialTests
{
    private const ushort Seg192D = 0x192D, SegRetrace = 0x0D73;

    /// <summary>A key pressed at the end of retrace <c>At</c> (counted from the anchor), released three retraces later or
    /// as soon as the menu waits for its release (busy loop), whichever comes first.</summary>
    public sealed record Press(int At, byte Key);

    private const int Hold = 3;

    public static TheoryData<string> Scenarios => new(Table.Keys);

    private static readonly Dictionary<string, (Press[] Presses, int Retraces)> Table = new()
    {
        // main menu: F1 and F3 toggle human/computer; the label must change on screen with the next flip
        ["main-f1-f3"] = ([new(0, MenuKeys.F1), new(12, MenuKeys.F3)], 30),
        // settings menu: F7 opens it, F1 and F2 change initial cash and target, RETURN back to the main menu
        ["settings"] = ([new(0, MenuKeys.F7), new(80, MenuKeys.F1), new(100, MenuKeys.F2), new(130, MenuKeys.Enter)], 220),
        // level menu: F6 opens it, F1 changes the level (all option lines are redrawn), RETURN back
        ["level"] = ([new(0, MenuKeys.F6), new(80, MenuKeys.F1), new(110, MenuKeys.Enter)], 200),
    };

    private sealed record Shot(uint[] Pixels, int Start, ushort Draw, ushort Shown, uint Seed);

    private static List<Shot> RunOriginal(Press[] presses, int count)
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, ProgramStartDifferentialTests.Options(ProgramStartDifferentialTests.ReferenceStart));
        var m = p.Machine;
        int passes = 0, retraces = 0, anchor = -1;
        var shots = new List<Shot>();
        var down = new Dictionary<byte, int>(); // key → anchor-relative retrace after which it is released
        void SetKey(byte sc, byte v) => p.WriteByte((ushort)(DgroupLayout.KeyState + sc), v);
        void ReleaseAll()
        {
            foreach (byte k in down.Keys) SetKey(k, 0);
            down.Clear();
        }

        m.AddCodeHook(p.Segment(Seg192D), 0x0345, _ =>
        {
            if (++passes == 3) anchor = retraces;
        });
        m.AddCodeHook(p.Segment(SegRetrace), 0x00D3, mm =>
        {
            if (anchor >= 0 && retraces >= anchor)
            {
                var frame = FrameDumpPixels(mm.Vga);
                shots.Add(new Shot(frame, mm.Vga.StartAddress, p.ReadWord(GraphicsLayers.DrawPageIndex),
                    p.ReadWord(GraphicsLayers.ShownPageIndex), p.ReadDword(DgroupLayout.RandomSeed)));
            }
            retraces++;
        });
        // end of the retrace wait (relative index retraces − 1 − anchor): releases first, then the presses of that retrace
        m.AddCodeHook(p.Segment(SegRetrace), 0x00F7, _ =>
        {
            if (anchor < 0) return;
            int r = retraces - 1 - anchor;
            foreach (var (k, until) in down.ToArray())
                if (until <= r)
                {
                    SetKey(k, 0);
                    down.Remove(k);
                }
            foreach (var press in presses.Where(x => x.At == r))
            {
                SetKey(press.Key, 1);
                down[press.Key] = r + Hold;
            }
        });
        // the menu waits for the release of a handled key: release everything
        foreach (var (off, _) in MenuHarness.WaitEdges192D) m.AddCodeHook(p.Segment(Seg192D), off, _ => ReleaseAll());

        m.Run(8_000_000_000, _ => shots.Count >= count);
        Assert.True(shots.Count >= count, $"original: {shots.Count} retraces after the anchor (passes {passes})");
        return shots;
    }

    private static uint[] FrameDumpPixels(Traders.RefEmu.Hardware.VgaAdapter vga)
    {
        var frame = Traders.RefEmu.Hardware.FrameDump.Capture(vga);
        var vram = new Traders.Vga.ModeXMemory();
        for (int i = 0; i < frame.Width * frame.Height; i++)
        {
            int y = i / frame.Width, x = i % frame.Width;
            vram.Planes[x & 3][y * (frame.Width / 4) + (x >> 2)] = frame.Pixels[i];
        }
        var px = new uint[FramePresenter.Width * FramePresenter.Height];
        FramePresenter.Present(vram, 0, frame.Palette, px);
        return px;
    }

    private static List<Shot> RunHost(int clock, Press[] presses, int count)
    {
        var host = new ProgramHost(TestGameData.DosDir, new RecordedClock([clock]), new NoSaveFile());
        int Passes() => host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;
        var shots = new List<Shot>();
        var held = new Dictionary<byte, int>();
        int anchor = -1;
        for (long f = 0; shots.Count < count; f++)
        {
            Assert.True(f < 20_000, $"host: {shots.Count} retraces after the anchor");
            int before = host.Retraces.Count;
            host.Frame(f, held.Keys.ToArray());
            int made = host.Retraces.Count - before;
            Assert.True(made <= 1, $"host frame {f} made {made} retrace waits");
            if (anchor < 0 && Passes() >= 3) anchor = before; // the third pass's random(1) and the next flip share a frame
            if (anchor < 0) continue;
            if (made == 0)
            {
                held.Clear(); // a poll frame: the menu waits for the release of a handled key
                continue;
            }
            var px = new uint[FramePresenter.Width * FramePresenter.Height];
            FramePresenter.Present(host.Driver.Vram, host.Driver.DisplayStart, host.Layers.Dac.Registers, px);
            ushort W(int at) => (ushort)(host.Layers.Ds[at] | host.Layers.Ds[at + 1] << 8);
            shots.Add(new Shot(px, host.Driver.DisplayStart, W(GraphicsLayers.DrawPageIndex), W(GraphicsLayers.ShownPageIndex),
                host.Core.U32(DgroupLayout.RandomSeed)));
            int r = host.Retraces.Count - 1 - anchor;
            foreach (var (k, until) in held.ToArray())
                if (until <= r) held.Remove(k);
            foreach (var press in presses.Where(x => x.At == r)) held[press.Key] = r + Hold;
        }
        return shots;
    }

    /// <summary>
    /// [DIFFERENTIAL] 0DFD:0074 – keys in the main menu and in the submenus: the change is on screen in the host at the
    /// same retrace as in the original (R4W-1: before the fix the host kept showing page 0 and drew the changes only on
    /// the hidden page, so they appeared only after a submenu redrew both pages).
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Menu_picture_of_the_host_follows_the_original_at_every_retrace(string scenario)
    {
        var (presses, count) = Table[scenario];
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(ProgramStartDifferentialTests.ReferenceStart,
            ProgramStartDifferentialTests.NoExtraKeys);
        var original = RunOriginal(presses, count);
        var host = RunHost(clock, presses, count);

        // the scenario changes the picture at all (otherwise it proves nothing)
        Assert.True(original.Skip(1).Any(s => !s.Pixels.AsSpan().SequenceEqual(original[0].Pixels)), $"{scenario}: the original's picture never changes");
        for (int k = 0; k < count; k++)
        {
            var (o, h) = (original[k], host[k]);
            int diff = 0;
            for (int i = 0; i < o.Pixels.Length; i++)
                if (o.Pixels[i] != h.Pixels[i]) diff++;
            Assert.True(diff == 0 && o.Start == h.Start && o.Draw == h.Draw && o.Shown == h.Shown && o.Seed == h.Seed,
                $"{scenario}: retrace {k}: {diff} pixels differ; start {o.Start:X4}/{h.Start:X4}, DS:082E {o.Draw}/{h.Draw}, " +
                $"DS:0830 {o.Shown}/{h.Shown}, seed {o.Seed:X8}/{h.Seed:X8} (original/host)");
        }
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
