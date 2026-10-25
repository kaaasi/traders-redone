using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Runs the original T.EXE in the reference emulator. With REFEMU_OUT set (CI experiment) every
/// changed frame is written as PNG together with the event log; REFEMU_SECONDS sets the run time.
/// </summary>
public class BootTests
{
    [Fact]
    public void T_exe_boots_into_vga_graphics()
    {
        var m = MachineTests.NewMachine();
        m.LoadProgram("T.EXE");
        // "INSERT COIN!" code request (21FD:0687) appears after about 12.2 s; answer with four digit keys.
        for (int i = 0; i < 4; i++) m.ScheduleKey(14.0 + 0.4 * i, 0x02);
        string? outDir = RefEmuPaths.OutputDir;
        double seconds = double.TryParse(Environment.GetEnvironmentVariable("REFEMU_SECONDS"),
            System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 20.0;

        var frames = new List<(double Time, string Hash)>();
        string last = "";
        Exception? failure = null;
        try
        {
            // capture once per emulated video frame
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
                if (outDir is not null && frames.Count <= 150)
                    f.WritePng(Path.Combine(outDir, $"frame_{frames.Count:D4}_{m.Seconds:000.000}s.png"));
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
                using var w = File.CreateText(Path.Combine(outDir, "events.log"));
                m.Log.WriteTo(w);
                w.WriteLine($"stopped at {m.Cpu.Cs:X4}:{m.Cpu.IP:X4} after {m.Cpu.InstructionCount} instructions, {m.Seconds:F3} s, exited={m.Exited}");
                w.WriteLine($"OPL register writes: {m.AdLib.WriteCount}, int 64h calls: {m.Log.OfKind(EventKind.Sound).Count()}");
                if (failure is not null) w.WriteLine(failure);
                using var fl = File.CreateText(Path.Combine(outDir, "frames.txt"));
                foreach (var (time, hash) in frames) fl.WriteLine($"{time:F3} {hash}");
            }
        }

        Assert.True(failure is null, $"{failure}\nlast events:\n{string.Join('\n', m.Log.Events.TakeLast(30))}");
        Assert.False(m.Exited, $"program exited with {m.ExitCode}:\n{string.Join('\n', m.Log.Events.TakeLast(30))}");
        Assert.Contains(m.Log.Events, e => e.Kind == EventKind.Video && e.Text.StartsWith("set mode 13"));
        Assert.NotEmpty(frames);
    }
}
