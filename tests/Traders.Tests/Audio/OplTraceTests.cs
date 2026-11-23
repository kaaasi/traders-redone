using Traders.Audio.Opl;
using Traders.RefEmu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Audio;

/// <summary>
/// Real register sequences: FXDRIVER resident in the RefEmu, OPL writes up to and in the main menu (intro and menu
/// music) recorded with <see cref="OplTrace"/> and rendered by the synthesis. With OPL_WAV_OUT set, the trace text and a
/// WAV file are written there for the listening comparison with the video (M14) – never into the repository.
/// </summary>
public class OplTraceTests
{
    private static List<OplWrite> MenuTrace(double secondsAfterMenu, out OplTrace trace)
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, runFxDriver: true);
        trace = new OplTrace(p.Machine);
        p.Machine.RunFor(OriginalProgram.MainMenuTime + secondsAfterMenu);
        return trace.Writes.Select(w => new OplWrite(w.Time, w.Register, w.Value)).ToList();
    }

    [GameDataFact]
    public void Menu_trace_renders_deterministically_with_sound()
    {
        double seconds = double.TryParse(Environment.GetEnvironmentVariable("OPL_WAV_SECONDS"),
            System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 5.0;
        var writes = MenuTrace(seconds, out var trace);
        Assert.True(writes.Count > 1000, $"only {writes.Count} OPL writes");
        Assert.Contains(writes, w => (w.Register & 0xf0) == 0xb0 && (w.Value & 0x20) != 0); // key on

        var first = OplRenderer.Render(writes, OplClock.Pit, 44_100, tailSeconds: 0.5);
        var second = OplRenderer.Render(writes, OplClock.Pit, 44_100, tailSeconds: 0.5);
        Assert.Equal(first, second);

        // clearly audible over most of the recording (music plays from the intro on)
        int window = 44_100 / 10, loud = 0, windows = 0;
        for (int i = 0; i + window <= first.Length; i += window, windows++)
            if (first.AsSpan(i, window).ToArray().Max(v => Math.Abs((int)v)) > 500) loud++;
        Assert.True(loud > windows / 2, $"{loud} of {windows} windows audible");

        string? outDir = Environment.GetEnvironmentVariable("OPL_WAV_OUT");
        if (!string.IsNullOrEmpty(outDir))
        {
            Directory.CreateDirectory(outDir);
            using (var w = File.CreateText(Path.Combine(outDir, "opl-menu.txt"))) trace.WriteText(w);
            using var f = File.Create(Path.Combine(outDir, "opl-menu.wav"));
            OplRenderer.WriteWav(f, first, 44_100);
        }
    }

    [GameDataFact]
    public void Trace_text_of_the_refemu_reads_back_into_the_same_writes()
    {
        var writes = MenuTrace(0.0, out var trace);
        var sw = new StringWriter();
        trace.WriteText(sw);
        Assert.Equal(writes, OplRenderer.ReadTrace(new StringReader(sw.ToString())));
    }
}
