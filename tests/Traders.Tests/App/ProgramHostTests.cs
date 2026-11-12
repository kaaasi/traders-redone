using Traders.App;
using Traders.App.Rendering;
using Traders.App.Timing;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Startup;

namespace Traders.Tests.App;

/// <summary>
/// The host's program run (<see cref="ProgramHost"/>): clock value of time() against the original, the main menu picture of
/// the host against the RefEmu's screen, and the run from the start through the menu into the week loop.
/// </summary>
public class ProgramHostTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>Return address of Borland time() in random(0) 21FD:01BA (DX:AX = clock value).</summary>
    private const ushort TimeReturn = 0x01CE;

    public static TheoryData<string> Starts => new() { "1991-06-01 12:00:00", "1992-01-15 08:30:01", "1991-10-27 00:30:00", "1991-04-07 01:59:50" };

    /// <summary>
    /// [DIFFERENTIAL] 21FD:01BA – random(0) takes time() of the C runtime: the host's conversion of the local clock (EST5EDT,
    /// daylight saving of the Borland runtime) gives the value the original computed from the DOS clock, in summer, in winter
    /// and around the two switches of 1991.
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(Starts))]
    public void Clock_value_of_the_host_is_the_one_time_returned_in_the_original(string start)
    {
        var t0 = DateTime.Parse(start, System.Globalization.CultureInfo.InvariantCulture);
        var p = OriginalProgram.Start(Dir, new MachineOptions { HostDirectory = Dir, RecordSoundDriver = true, StartTime = t0 });
        int? clock = null;
        DateTime at = default;
        ushort seg = p.Segment(0x21FD);
        p.Machine.AddCodeHook(seg, TimeReturn, m =>
        {
            if (clock is not null) return;
            clock = (int)(m.Cpu.Ax | (uint)m.Cpu.Dx << 16);
            at = m.CurrentTime;
        });
        p.Machine.Run(2_000_000_000, m => clock is not null);
        Assert.NotNull(clock);
        // the DOS clock was read just before the return; allow the second to turn over in between
        int host = BorlandClock.FromLocal(at);
        Assert.True(clock == host || clock == BorlandClock.FromLocal(at.AddSeconds(-1)), $"original {clock}, host {host} at {at:O}");
    }

    [Fact]
    public void Daylight_saving_follows_the_Borland_rule()
    {
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1991, 4, 7, 1, 59, 59)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1991, 4, 7, 2, 0, 0)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1991, 10, 27, 0, 59, 59)));
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1991, 10, 27, 1, 0, 0)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1986, 4, 27, 3, 0, 0))); // before 1987: last Sunday in April
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1986, 4, 6, 3, 0, 0)));
        Assert.Equal(0 + 5 * 3600, BorlandClock.FromLocal(new DateTime(1970, 1, 1)));
    }

    /// <summary>
    /// The main menu as the host shows it, against the RefEmu: same clock value (seed after the code request), and after the
    /// third pass of the main menu loop (192D:0345) seed and displayed picture – pixels through the DAC – are the original's
    /// (both pages hold the whole menu, so the picture does not depend on the page shown).
    /// </summary>
    // Counter-check (local): a wrong font height in the host's font table → red.
    // [DIFFERENTIAL] 0B67:0002 – pages of the host (VGA path) under the menu picture
    [GameDataFact]
    public void Main_menu_picture_of_the_host_is_the_original_screen()
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        int passes = 0;
        p.Machine.AddCodeHook(p.Segment(0x192D), 0x0345, _ => passes++);
        // the hook runs at the call of the 4th random(1): three draws done, the 4th pass flipped
        p.Machine.Run(4_000_000_000, _ => passes == 4);
        Assert.Equal(4, passes);
        var original = p.Frame();

        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile());
        for (long f = 0; f < 2000 && Passes(host) < 3; f++) host.Frame(f, []);
        Assert.Equal(3, Passes(host));
        var shown = new uint[FramePresenter.Width * FramePresenter.Height];
        FramePresenter.Present(host.Driver.Vram, host.Driver.DisplayStart, host.Layers.Dac.Registers, shown);

        Assert.Equal(p.RandomState, host.Core.U32(DgroupLayout.RandomSeed));
        var expected = new uint[shown.Length];
        FramePresenter.Present(OriginalScreen(original), 0, original.Palette, expected);
        Assert.True(shown.Distinct().Count() > 8, "the host shows no menu picture");
        int diff = Enumerable.Range(0, shown.Length).Count(i => shown[i] != expected[i]);
        Assert.True(diff == 0, $"{diff} pixels differ, first at {Array.FindIndex(Enumerable.Range(0, shown.Length).ToArray(), i => shown[i] != expected[i])}");
    }

    /// <summary>
    /// The whole host run without a window: start, main menu (RETURN), new game, then the week loop of the core, which stops
    /// at its first screen step (weekly balance, pending until R5). The scene never writes the core's data segment.
    /// </summary>
    [GameDataFact]
    public void Host_runs_from_the_start_through_the_menu_into_the_week_loop()
    {
        var host = new ProgramHost(Dir, new RecordedClock([12345678]), new NoSaveFile());
        long f = 0;
        for (; f < 2000 && Passes(host) < 2; f++) host.Frame(f, []);
        for (long until = f + 6; f < until; f++) host.Frame(f, [MenuKeys.Enter]);
        for (long until = f + 200; f < until && host.Machine.Status != ProgramStatus.InWeek; f++) host.Frame(f, []);

        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        Assert.True(host.Machine.IsBlocked);
        Assert.Equal(3, host.Machine.WeekMachine!.NextStep);
        Assert.Equal(1, host.Core.U8(ProgramLoop.NewGameRequest));
        Assert.Equal(1, host.Core.U8(ProgramLoop.GameRunning));
        Assert.NotSame(host.Core.Bytes, host.Layers.Ds);
        Assert.NotNull(host.Start.Code);
    }

    private static int Passes(ProgramHost host) => host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;

    /// <summary>The RefEmu's rendered picture as a mode-X page at offset 0 (pixel i in plane i % 4).</summary>
    private static Traders.Vga.ModeXMemory OriginalScreen(Traders.RefEmu.Hardware.FrameDump frame)
    {
        var vram = new Traders.Vga.ModeXMemory();
        for (int i = 0; i < frame.Width * frame.Height; i++)
        {
            int y = i / frame.Width, x = i % frame.Width;
            vram.Planes[x & 3][y * (frame.Width / 4) + (x >> 2)] = frame.Pixels[i];
        }
        return vram;
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
