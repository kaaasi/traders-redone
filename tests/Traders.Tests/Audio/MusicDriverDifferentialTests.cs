using Traders.Audio.Music;

namespace Traders.Tests.Audio;

/// <summary>
/// <see cref="FxMusicDriver"/> against the resident FXDRIVER in the RefEmu (finding docs/analyse/FXDRIVER.md): same
/// int 64h calls, same timer calls; OPL writes of every call and tick, return registers, the driver's data segment and
/// the pattern buffers must be equal. Expected values come only from the emulated driver.
/// </summary>
public class MusicDriverDifferentialTests
{
    /// <summary>Ticks of the driver (18,2 Hz) in 60 s.</summary>
    private const int Minute = 1093;

    private static void SameWrites(MusicDriverHarness h, string step)
    {
        var (o, p) = h.TakeWrites();
        if (o.SequenceEqual(p)) return;
        int i = 0;
        while (i < o.Count && i < p.Count && o[i] == p[i]) i++;
        int from = Math.Max(0, i - 6);
        Assert.Fail($"{step}: OPL-Schreibungen verschieden ab Nr. {i} von {o.Count}/{p.Count}\n" +
            $"Original: {MusicDriverHarness.Hex(o.Skip(from).Take(24))}\nPort:     {MusicDriverHarness.Hex(p.Skip(from).Take(24))}");
    }

    private static void SameData(MusicDriverHarness h, string step)
    {
        var d = h.DataDifferences();
        if (d.Count > 0) Assert.Fail($"{step}: Datensegment verschieden bei {MusicDriverHarness.Hex(d)}");
    }

    private static void Calls(MusicDriverHarness h, Action<MusicDriverHarness> calls, string step)
    {
        calls(h);
        SameWrites(h, step);
        SameData(h, step);
    }

    private static void Ticks(MusicDriverHarness h, int count, string step)
    {
        for (int t = 0; t < count; t++)
        {
            h.Tick();
            SameWrites(h, $"{step}, Takt {t}");
            if (t % 16 == 0 || !h.Port.TimerActive) SameData(h, $"{step}, Takt {t}");
        }
        SameData(h, $"{step}, Ende");
    }

    // [DIFFERENTIAL-FX] 4630
    [GameDataFact]
    public void Installed_driver_has_the_data_segment_of_the_port()
    {
        var h = new MusicDriverHarness();
        SameData(h, "Installation");
    }

    // [DIFFERENTIAL-FX] 427D, 2928, 4324, 296B, 3403, 2993, 2A3F, 2AA1, 2AF5, 2B1D, 2DE1, 2F55, 2EFA, 3031, 307D, 3151, 3176, 31D8, 322B, 327E, 3326, 337E
    [GameDataFact]
    public void Boot_calls_write_the_same_registers()
    {
        var h = new MusicDriverHarness();
        Calls(h, x => x.Call(0x06), "Fn 06h");
        Calls(h, x => x.Call(0x05), "Fn 05h");
        h.Boot();
        SameWrites(h, "Hochlauf");
        SameData(h, "Hochlauf");
    }

    // [DIFFERENTIAL-FX] 3657, 4150, 3B82, 3BED, 40B4, 2B45, 2BFA, 2C79, 2CBB, 2D75, 33C8, 34AD
    [GameDataTheory]
    [InlineData("TRADINTR", 0)] // intro: 0DFD:02FE, flag 0
    [InlineData("TRADMENU", 2)] // menu: 1777:015C, flag 2
    [InlineData("TRAD_END", 3)] // end: 20B3:0060, flag 3 (slow fade-in)
    public void Piece_plays_like_the_driver_for_a_minute(string name, int flag)
    {
        var h = new MusicDriverHarness();
        h.Boot();
        h.TakeWrites();
        h.Play(name, flag);
        SameWrites(h, "Start");
        SameData(h, "Start");
        Assert.Empty(h.BufferDifferences());
        Ticks(h, Minute, name);
        Assert.True(h.Port.TimerActive);
    }

    // [DIFFERENTIAL-FX] 405C, 4236
    [GameDataFact]
    public void Intro_without_repeat_ends_and_unhooks_the_timer()
    {
        var h = new MusicDriverHarness();
        h.Boot();
        h.Play("TRADINTR", 0);
        h.TakeWrites();
        int t = 0;
        for (; h.Port.TimerActive && t < 6000; t++)
        {
            h.Tick();
            SameWrites(h, $"Takt {t}");
        }
        Assert.InRange(t, Minute, 5999);
        SameData(h, "Ende");
        Assert.Equal(0, h.Call(0x12).Original.Item2); // fn 12h: stopped
        for (int k = 0; k < 8; k++) h.Tick(); // the original's handler still runs when called, the port's is unhooked
        h.TakeWrites();
    }

    [GameDataFact]
    public void Menu_with_repeat_starts_the_order_again()
    {
        var h = new MusicDriverHarness();
        h.Boot();
        h.Play("TRADMENU", 2);
        h.TakeWrites();
        int wraps = 0, last = 0;
        for (int t = 0; t < 4000; t++)
        {
            h.Tick();
            SameWrites(h, $"Takt {t}");
            int pos = h.Port.DataByte(0x6C);
            if (pos < last) wraps++;
            last = pos;
        }
        SameData(h, "Ende");
        Assert.True(wraps >= 1, "Reihenfolge nicht wiederholt");
        Assert.True(h.Port.TimerActive);
    }

    [GameDataFact]
    public void Stop_in_the_middle_of_a_row_and_switch_to_another_piece()
    {
        var h = new MusicDriverHarness();
        h.Boot();
        h.Play("TRADMENU", 2);
        Ticks(h, 301, "Menü"); // 301 ticks: TRADMENU has 4 ticks per row, so the stop falls inside a row
        Assert.NotEqual(0, h.Port.DataByte(0x4A));
        Calls(h, x => x.Call(0x03), "Stopp");
        Calls(h, x => x.Play("TRAD_END", 3), "Wechsel");
        Ticks(h, 400, "Ende-Musik");
    }

    [GameDataFact]
    public void Fade_out_with_a_step_ends_the_piece()
    {
        var h = new MusicDriverHarness();
        h.Boot();
        h.Play("TRADMENU", 2);
        Ticks(h, 200, "Menü");
        Calls(h, x => { x.Call(0x1B, 5); x.Call(0x19); }, "Ausblenden");
        int t = 0;
        for (; h.Port.TimerActive && t < 100; t++)
        {
            h.Tick();
            SameWrites(h, $"Fahrt, Takt {t}");
        }
        Assert.InRange(t, 1, 99);
        SameData(h, "Fahrt, Ende");
    }

    [GameDataFact]
    public void Status_version_and_buffer_size_answer_like_the_driver()
    {
        var h = new MusicDriverHarness();
        h.Boot();
        foreach (ushort fn in new ushort[] { 0x11, 0x12, 0x15, 0x1D })
        {
            var (o, p) = h.Call(fn, 0x1234, 0x5678, 0x9ABC);
            Assert.Equal(o, p);
        }
    }

    [GameDataFact]
    public void Writes_carry_the_number_of_the_timer_call()
    {
        var h = new MusicDriverHarness();
        var ticks = new List<long>();
        h.Port.WrittenAt += w => ticks.Add(w.Tick);
        h.Boot();
        h.Play("TRADMENU", 2);
        Assert.All(ticks, t => Assert.Equal(0, t));
        ticks.Clear();
        h.Tick();
        h.Tick();
        Assert.NotEmpty(ticks);
        Assert.Equal(ticks.OrderBy(t => t), ticks);
        Assert.Equal([1L, 2L], ticks.Distinct());
    }
}
