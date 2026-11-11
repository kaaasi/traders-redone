using System.Globalization;
using System.Text;
using Traders.RefEmu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Probes P1-P5 of docs/analyse/BESTAND.md (appendix A) against the original T.EXE in the reference emulator.
/// Not part of the test project: experiments/bestand/proben.sh copies this file into a throw-away worktree that
/// has the commits of dev/ap1-refemu applied (Start, Press, AddCodeHook) and runs it there.
/// Results go to BESTAND_OUT; BESTAND_SECONDS = emulated seconds after the main menu for P1 (default 900).
/// </summary>
public class BestandProben
{
    private static string Dir => Path.GetDirectoryName(TestGameData.Dos("T.EXE"))!;
    private static string Out(string name) => Path.Combine(Environment.GetEnvironmentVariable("BESTAND_OUT") ?? Path.GetTempPath(), name);
    private static ushort StackWord(PcMachine m, int offset) => m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + offset));

    /// <summary>P1: week loop with four computer players; time, week counter and random seed at every phase marker.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void P1_week_loop_progress(bool residentSoundDriver)
    {
        double seconds = double.Parse(Environment.GetEnvironmentVariable("BESTAND_SECONDS") ?? "900", CultureInfo.InvariantCulture);
        if (residentSoundDriver) seconds = Math.Min(seconds, 240);
        var p = OriginalProgram.Start(Dir, runFxDriver: residentSoundDriver);
        var m = p.Machine;
        var log = new StringBuilder();
        void Hook(ushort seg, ushort off, string name, bool arg = false) =>
            m.AddCodeHook(p.Segment(seg), off, mm => log.AppendLine(
                $"{mm.Seconds,9:F2}s week={p.ReadWord(0x4B9A)} seed={p.ReadDword(0x10D2):X8} {name}{(arg ? $"({StackWord(mm, 4)})" : "")}"));
        Hook(0x192D, 0x0007, "main_menu");
        Hook(0x0E31, 0x0003, "new_game");
        Hook(0x1777, 0x01F8, "week_loop_enter");
        Hook(0x1777, 0x02DC, "weekly_prices");
        Hook(0x21FD, 0x0977, "marker", true);
        Hook(0x1C68, 0x000B, "robot_phase");
        Hook(0x20B3, 0x0005, "end_check");
        Hook(0x0B9F, 0x0002, "trade");
        Hook(0x1F63, 0x000F, "fat_mike");
        Hook(0x08CF, 0x0004, "shop");
        Exception? failure = null;
        try
        {
            m.RunFor(OriginalProgram.MainMenuTime);
            p.Press(0x1C, delay: 0.5);   // RETURN in the main menu: start with the default four computer players
            m.RunFor(seconds);
        }
        catch (Exception e) { failure = e; }
        log.AppendLine($"END t={m.Seconds:F2}s exited={m.Exited} at image {(ushort)(m.Cpu.Cs - m.LoadSegment):X4}:{m.Cpu.IP:X4} " +
                       $"week={p.ReadWord(0x4B9A)} failure={failure?.GetType().Name} {failure?.Message}");
        File.WriteAllText(Out($"p1-{(residentSoundDriver ? "fxdriver" : "plain")}.log"), log.ToString());
        Assert.Null(failure);
    }

    /// <summary>P2: 1777:02DC (weekly prices) uses neither the clock nor random().</summary>
    [Fact]
    public void P2_weekly_prices_ignore_clock_and_random()
    {
        var log = new StringBuilder();
        byte[]? first = null;
        foreach (var start in new[] { new DateTime(1991, 6, 1, 12, 0, 0), new DateTime(1993, 2, 3, 4, 5, 6) })
        {
            var p = OriginalProgram.LoadOnly(Dir, new MachineOptions { HostDirectory = Dir, StartTime = start });
            uint[] cash = [500, 1200, 80000, 37];
            for (int i = 0; i < 4; i++) p.WriteDword((ushort)(0x3CD2 + 4 * i), cash[i]);
            p.WriteWord(0x4B9A, 7);    // week
            p.WriteByte(0x2F05, 48);   // number of fields
            int events = p.Machine.Log.Events.Count;
            uint seed = p.ReadDword(0x10D2);
            p.Call(0x1777, 0x02DC);
            var ds = p.DataSegment();
            log.AppendLine($"start={start:s} host events={p.Machine.Log.Events.Count - events} seed {seed:X8}->{p.ReadDword(0x10D2):X8} " +
                           $"oil={p.ReadWord(0x43F4)} rocket={p.ReadWord(0x48A0)} lotto={p.ReadWord(0x40CC)} land={p.ReadWord(0x3746)} shop oil={p.ReadWord(0x4B98)}");
            Assert.Equal(events, p.Machine.Log.Events.Count);
            Assert.Equal(seed, p.ReadDword(0x10D2));
            if (first is null) first = ds; else Assert.True(first.AsSpan().SequenceEqual(ds), "data segments differ");
        }
        log.AppendLine("data segments equal: True");
        File.WriteAllText(Out("p2.log"), log.ToString());
    }

    /// <summary>P3: 0609:1178 derives DS:2F26 and DS:48A2 from the number of production fields; 22A3:026B cuts stocks to 8/10.</summary>
    [Fact]
    public void P3_field_limits_and_weekly_stock_decay()
    {
        var log = new StringBuilder();
        foreach (int perKind in new[] { 0, 1, 2, 5, 9 })
        {
            var p = OriginalProgram.LoadOnly(Dir);
            for (int player = 0; player < 4; player++)
                for (int kind = 0; kind < 4; kind++)
                    p.WriteByte((ushort)(0x2EE7 + player * 4 + kind), (byte)(kind < 3 ? perKind : 7));
            p.Call(0x0609, 0x1178);
            log.AppendLine($"production fields of all players={perKind * 12} DS:2F26={p.ReadByte(0x2F26)} DS:48A2={p.ReadByte(0x48A2)}");
        }
        var q = OriginalProgram.LoadOnly(Dir);
        ushort[] stock = [70, 999, 5, 13];
        for (int k = 0; k < 4; k++) q.WriteWord((ushort)(0x3CF4 + k * 2), stock[k]);
        q.Call(0x22A3, 0x026B);
        log.AppendLine("stock 70,999,5,13 -> " + string.Join(",", Enumerable.Range(0, 4).Select(k => q.ReadWord((ushort)(0x3CF4 + k * 2)))));
        File.WriteAllText(Out("p3.log"), log.ToString());
    }

    /// <summary>P4: random() calls of the first week by section (between phase markers), return address and argument.</summary>
    [Fact]
    public void P4_random_calls_by_caller_and_section()
    {
        var p = OriginalProgram.Start(Dir);
        var m = p.Machine;
        string section = "boot";
        var counts = new SortedDictionary<string, int>(StringComparer.Ordinal);
        m.AddCodeHook(p.Segment(0x21FD), 0x01BA, mm =>
        {
            string key = $"{section,-10} return {(ushort)(StackWord(mm, 2) - mm.LoadSegment):X4}:{StackWord(mm, 0):X4} n={(short)StackWord(mm, 4)}";
            counts[key] = counts.GetValueOrDefault(key) + 1;
        });
        m.AddCodeHook(p.Segment(0x21FD), 0x0977, mm => section = "marker" + StackWord(mm, 4));
        m.AddCodeHook(p.Segment(0x192D), 0x0007, _ => section = "menu");
        m.AddCodeHook(p.Segment(0x0E31), 0x0003, _ => section = "newgame");
        m.AddCodeHook(p.Segment(0x1777), 0x02DC, _ => section = "week" + p.ReadWord(0x4B9A));
        m.RunFor(OriginalProgram.MainMenuTime);
        p.Press(0x1C, delay: 0.5);
        m.RunFor(215);
        File.WriteAllLines(Out("p4.log"), counts.Select(c => $"{c.Value,6} {c.Key}"));
    }

    /// <summary>P5: the random seed at the start of a new game depends on the time spent in the main menu.</summary>
    [Fact]
    public void P5_seed_at_new_game_depends_on_menu_time()
    {
        var log = new StringBuilder();
        var seeds = new List<uint>();
        foreach (double delay in new[] { 0.5, 0.5, 1.5, 3.0 })
        {
            var p = OriginalProgram.Start(Dir);
            var m = p.Machine;
            string seen = "";
            uint atNewGame = 0;
            m.AddCodeHook(p.Segment(0x192D), 0x0007, mm => seen += $" menu@{mm.Seconds:F2}s seed={p.ReadDword(0x10D2):X8}");
            m.AddCodeHook(p.Segment(0x0E31), 0x0003, mm => { atNewGame = p.ReadDword(0x10D2); seen += $" newgame@{mm.Seconds:F2}s seed={atNewGame:X8}"; });
            m.RunFor(OriginalProgram.MainMenuTime);
            p.Press(0x1C, delay: delay);
            m.RunFor(delay + 3);
            seeds.Add(atNewGame);
            log.AppendLine($"RETURN after {delay:F1}s:{seen}");
        }
        File.WriteAllText(Out("p5.log"), log.ToString());
        Assert.Equal(seeds[0], seeds[1]);      // same inputs: same run
        Assert.NotEqual(seeds[0], seeds[2]);
        Assert.NotEqual(seeds[2], seeds[3]);
    }
}
