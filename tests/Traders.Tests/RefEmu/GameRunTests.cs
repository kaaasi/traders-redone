using System.Globalization;
using System.IO.Compression;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Reference run beyond the main menu: FXDRIVER resident (as in TRADERS.BAT), code request answered,
/// RETURN in the main menu (default: four computer players), then the game runs on its own.
/// Records frames, graphics driver calls, OPL writes and int 64h calls (experiments/refemu/spiellauf.sh).
/// Output only with REFEMU_GAME_OUT; REFEMU_GAME_SECONDS = run time after the menu.
/// </summary>
public class GameRunTests
{
    private static string Dir => TestGameData.DosDir;

    [GameDataFact]
    public void Fxdriver_stays_resident_and_installs_int64()
    {
        var p = OriginalProgram.Start(Dir, runFxDriver: true);
        var m = p.Machine;
        Assert.NotEqual((ushort)0, m.Mem.Read16(0, 0x64 * 4 + 2));
        Assert.NotEqual(PcMachine.StubSegment, m.Mem.Read16(0, 0x64 * 4 + 2));
        Assert.True(m.LoadSegment > 0x1000, $"T.EXE must load above the resident driver (load {m.LoadSegment:X4})");
    }

    [GameDataFact]
    public void Graphics_trace_and_menu_with_sound_driver()
    {
        var p = OriginalProgram.Start(Dir, runFxDriver: true);
        var gfx = p.TraceGraphicsDriver();
        var opl = new OplTrace(p.Machine);
        p.Machine.RunFor(OriginalProgram.MainMenuTime + 0.5);
        Assert.Equal(OriginalProgram.MainMenuFrameHashWithFxDriver, p.Frame().Hash());
        Assert.True(gfx.Calls.Count > 100, $"only {gfx.Calls.Count} graphics calls");
        Assert.All(gfx.Calls.Take(1000), c => Assert.True(c.Returned, $"call {c.Index} {c.TargetSeg:X4}:{c.TargetOff:X4} did not return"));
        Assert.True(opl.Writes.Count > 100, $"only {opl.Writes.Count} OPL writes");
    }

    [GameDataFact]
    public void Game_starts_from_main_menu_and_runs()
    {
        string? outDir = Environment.GetEnvironmentVariable("REFEMU_GAME_OUT");
        if (!string.IsNullOrEmpty(outDir)) Directory.CreateDirectory(outDir); else outDir = null;
        double seconds = double.TryParse(Environment.GetEnvironmentVariable("REFEMU_GAME_SECONDS"),
            CultureInfo.InvariantCulture, out var s) ? s : 15.0;

        var p = OriginalProgram.Start(Dir, runFxDriver: true);
        var m = p.Machine;
        var gfx = p.TraceGraphicsDriver();
        var opl = new OplTrace(m);
        var frames = new List<(double Time, string Hash)>();
        string last = "";
        Exception? failure = null;
        try
        {
            m.RunFor(OriginalProgram.MainMenuTime);
            p.Press(Scancodes.Enter, delay: 0.5); // RETURN: start game
            double step = 1.0 / 70;
            for (double t = 0; t < seconds && !m.Exited; t += step)
            {
                m.RunFor(step);
                var f = FrameDump.Capture(m.Vga);
                if (f.IsEmpty) continue;
                string h = f.Hash();
                if (h == last) continue;
                last = h;
                frames.Add((m.Seconds, h));
                // every changed frame for the first 100, then one per emulated second
                bool keep = frames.Count <= 100 || (int)(m.Seconds) != (int)(frames[^2].Time);
                if (outDir is not null && keep && frames.Count <= 1500)
                    f.WritePng(Path.Combine(outDir, $"frame_{m.Seconds:0000.000}s.png"));
            }
        }
        catch (Exception e)
        {
            failure = e;
        }
        finally
        {
            if (outDir is not null)
            {
                using (var w = File.CreateText(Path.Combine(outDir, "events.log")))
                {
                    m.Log.WriteTo(w);
                    w.WriteLine($"stopped at {m.Cpu.Cs:X4}:{m.Cpu.IP:X4} after {m.Cpu.InstructionCount} instructions, {m.Seconds:F3} s, exited={m.Exited}");
                    w.WriteLine($"graphics calls {gfx.TotalCalls}, OPL writes {opl.Writes.Count}");
                    if (failure is not null) w.WriteLine(failure);
                }
                WriteGz(Path.Combine(outDir, "grafik-aufrufe.txt.gz"), gfx.WriteText);
                WriteGz(Path.Combine(outDir, "opl.txt.gz"), opl.WriteText);
                using (var h = File.CreateText(Path.Combine(outDir, "grafik-histogramm.txt")))
                    foreach (var (target, count) in gfx.Histogram()) h.WriteLine($"{target} {count}");
                using (var fl = File.CreateText(Path.Combine(outDir, "frames.txt")))
                    foreach (var (time, hash) in frames) fl.WriteLine($"{time:F3} {hash}");
            }
        }

        Assert.True(failure is null, $"{failure}\nlast events:\n{string.Join('\n', m.Log.Events.TakeLast(30))}");
        Assert.False(m.Exited, $"program exited with {m.ExitCode}");
        Assert.NotEqual(OriginalProgram.MainMenuFrameHashWithFxDriver, frames[^1].Hash);
    }

    private sealed record Marker(int Week, int Number, double Seconds, uint RandomState);

    private sealed record WeekRun(List<Marker> Markers, uint RandomAtNewGame, double WeekOneStart, uint RandomAtWeekOne,
        long Instructions, byte[] Data);

    /// <summary>Plays the default game (four computer players) through week 0 up to the start of week 1.</summary>
    private static WeekRun RunFirstWeek(bool runFxDriver)
    {
        var p = OriginalProgram.StartGame(Dir, runFxDriver: runFxDriver);
        var m = p.Machine;
        var markers = new List<Marker>();
        uint atNewGame = 0;
        m.AddCodeHook(p.Segment(OriginalProgram.NewGameSegment), OriginalProgram.NewGameOffset, _ => atNewGame = p.RandomState);
        m.AddCodeHook(p.Segment(OriginalProgram.PhaseMarkerSegment), OriginalProgram.PhaseMarkerOffset,
            mm => markers.Add(new Marker(p.Week, p.StackArgument(0), mm.Seconds, p.RandomState)));
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        Assert.True(p.RunToWeekStart(1, 240), $"week 1 not reached; markers: {string.Join(", ", markers)}");
        return new WeekRun(markers, atNewGame, m.Seconds, p.RandomState, m.Cpu.InstructionCount, p.DataSegment());
    }

    /// <summary>
    /// The week run as oracle (docs/analyse/BESTAND.md §3.1): after the main menu the game passes the phase markers
    /// 0..6 of week 0 in order and starts week 1; a second run with the same inputs is bit-identical, including the
    /// random state at every marker. Expected times and random states come from the independent probes P1/P5 of BESTAND.md.
    /// </summary>
    [GameDataFact]
    public void Game_runs_through_week_0_and_repeats_bit_identically()
    {
        var runs = new[] { false, false }.AsParallel().AsOrdered().Select(RunFirstWeek).ToArray();
        var a = runs[0];

        Assert.Equal(0xF9D5A4ACu, a.RandomAtNewGame);                                  // P5, RETURN after 0.5 s
        Assert.Equal(Enumerable.Range(0, 7), a.Markers.Select(x => x.Number));
        Assert.All(a.Markers, x => Assert.Equal(0, x.Week));
        Assert.InRange(a.WeekOneStart, 220.80, 220.90);                               // P1: week 1 at 220.85 s
        Assert.True(a.Markers.Select(x => x.RandomState).Distinct().Count() > 1, "random() is not used between the phases");

        var b = runs[1];
        Assert.Equal(a.Markers, b.Markers);
        Assert.Equal(a.RandomAtWeekOne, b.RandomAtWeekOne);
        Assert.Equal(a.Instructions, b.Instructions);
        Assert.True(a.Data.AsSpan().SequenceEqual(b.Data), "data segments differ at the start of week 1");
    }

    /// <summary>With FXDRIVER resident (as TRADERS.BAT starts the game) week 0 is shorter; P1: week 1 at 214.05 s.</summary>
    [GameDataFact]
    public void Game_with_sound_driver_runs_through_week_0()
    {
        var r = RunFirstWeek(runFxDriver: true);
        Assert.Equal(Enumerable.Range(0, 7), r.Markers.Select(x => x.Number));
        Assert.InRange(r.WeekOneStart, 214.00, 214.10);
    }

    /// <summary><see cref="OriginalProgram.RunToMarker"/> stops before the marker routine and can be resumed to the next marker.</summary>
    [GameDataFact]
    public void Run_to_marker_stops_at_the_entry_and_resumes()
    {
        var p = OriginalProgram.StartGame(Dir);
        Assert.True(p.RunToMarker(0, 3, 120));
        Assert.True(p.IsAt(OriginalProgram.PhaseMarkerSegment, OriginalProgram.PhaseMarkerOffset));
        Assert.Equal(3, p.StackArgument(0));
        Assert.Equal(0, p.Week);
        double at3 = p.Machine.Seconds;
        Assert.InRange(at3, 83.70, 83.85);                                            // P1: markers 3 and 4 close together
        Assert.True(p.RunToMarker(0, 4, 1));
        Assert.True(p.Machine.Seconds > at3);
        Assert.False(p.RunToMarker(0, 2, 1), "marker 2 of week 0 must not come again");
    }

    private static void WriteGz(string path, Action<TextWriter> write)
    {
        using var f = File.Create(path);
        using var gz = new GZipStream(f, CompressionLevel.SmallestSize);
        using var w = new StreamWriter(gz);
        write(w);
    }
}
