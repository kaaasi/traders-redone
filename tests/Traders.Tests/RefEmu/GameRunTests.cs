using System.Globalization;
using System.IO.Compression;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Reference run beyond the main menu: FXDRIVER resident (as in TRADERS.BAT), code request answered,
/// RETURN in the main menu (default: four computer players), then the game runs on its own.
/// Records frames, graphics driver calls (for AP2), OPL writes and int 64h calls (for AP4).
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

    private static void WriteGz(string path, Action<TextWriter> write)
    {
        using var f = File.Create(path);
        using var gz = new GZipStream(f, CompressionLevel.SmallestSize);
        using var w = new StreamWriter(gz);
        write(w);
    }
}
