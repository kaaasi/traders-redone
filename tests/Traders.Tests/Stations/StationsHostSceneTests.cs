using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.Stations;

/// <summary>
/// Issue #264: in a game with a human the host showed the new stations 1B03:0101 (typed lines 0A69:0288, text 192) not at
/// all – its scene read the far pointer table of the lines from the layers' data segment image, which in the host never holds
/// the core's text pointers (DS:37CC, written by the program start's text loader). The scene tests had the original's whole
/// data segment as the layers' image and did not see it. Here the state comes from a real game: player 1 human (main menu
/// F1, play, confirm), the human pressing keys during the weeks, played in the RefEmu up to the stations phase (call 11) of
/// the first week in which 1B03:0101 makes the stations anew; from there the phase runs picture by picture against the
/// original (<see cref="StationsSceneHarness"/> through the handler of call 11) with the layers' image as the host has it
/// (<see cref="StationsSceneHarness.HostLayersImage"/>). Expected values come only from the RefEmu.
/// </summary>
public sealed class StationsHostSceneTests(ITestOutputHelper output)
{
    private const ushort RetraceSegment = 0x0D73, RetraceEnd = 0x00F7;

    /// <summary>Key presses in the main menu by menu pass: player 1 human, play, confirm the controls.</summary>
    private static readonly (int Pass, byte Key)[] HumanMenuPlan = [(3, MenuKeys.F1), (6, MenuKeys.Enter), (9, MenuKeys.Enter)];

    /// <summary>What the human presses in the weeks (arrows, Ins), one key every 35 retraces, held 6 retraces.</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    /// <summary>
    /// Start time of the game (the clock of random(0) at the code request): with 12:00 this game has no new stations up to week
    /// 8, with 12:01 in week 1 (RefEmu runs; checked by the test).
    /// </summary>
    private static readonly DateTime Start = new(1991, 6, 1, 12, 1, 0);

    private const int LastWeek = 1;

    /// <summary>
    /// The RefEmu game with a human up to the entry of 1B03:0009 in the first week whose stations phase calls 1B03:0101's typed
    /// lines 0A69:0288 (up to <paramref name="lastWeek"/>); returns the snapshot there and the week, null if there is none.
    /// </summary>
    private static (MachineSnapshot At, int Week)? NewStationsWithHuman(DateTime start, int lastWeek)
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, ProgramStartDifferentialTests.Options(start));
        var m = p.Machine;
        int passes = 0, retraces = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            m.AddCodeHook(menu, site, mm =>
            {
                passes++;
                foreach (var (n, key) in HumanMenuPlan)
                    if (n == passes) mm.ScheduleKey(mm.Seconds + 0.001, key, 6 / 70.0);
            });
        bool inGame = false;
        m.AddCodeHook(p.Segment(RetraceSegment), RetraceEnd, mm =>
        {
            if (!inGame) return;
            if (++retraces % KeyEvery == 0)
                mm.ScheduleKey(mm.Seconds, HumanCycle[retraces / KeyEvery % HumanCycle.Length], KeyHold / 70.0);
        });
        bool typed = false, inPhase = false;
        m.AddCodeHook(p.Segment(0x0A69), 0x0288, _ => { if (inPhase) typed = true; });
        Assert.True(p.RunToRoutine(0x0825, 0x0072, 300), "the week report of week 0 not reached");
        Assert.Equal(0, p.ReadWord((ushort)DgroupLayout.IsComputer) & 0xFF);
        inGame = true;
        while (true)
        {
            Assert.True(p.RunToRoutine(StationsLockstep.Seg, 0x0009, 3000), $"1B03:0009 not reached after week {p.Week}");
            int week = p.Week;
            if (week > lastWeek) return null;
            var at = p.Snapshot();
            // run the phase to its return (the far return address at the entry)
            ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
            inPhase = true;
            typed = false;
            m.RunFor(3000, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
            Assert.True(m.Cpu.Cs == retCs && m.Cpu.IP == retIp, $"1B03:0009 of week {week} did not return");
            inPhase = false;
            if (typed) return (at, week);
        }
    }

    /// <summary>
    /// [DIFFERENTIAL] 1B03:0101 – new stations in a real game with a human: the phase through the handler of call 11, picture
    /// by picture (VRAM, DAC, CRTC start, command list), with the layers' image of the host.
    /// </summary>
    // [DIFFERENTIAL] 1B03:0101
    [GameDataFact]
    public void New_stations_with_a_human_are_typed_as_in_the_original_with_the_hosts_layers()
    {
        var (at, week) = NewStationsWithHuman(Start, LastWeek) ?? throw new InvalidOperationException($"no new stations up to week {LastWeek}");
        var keys = StationsDifferentialTests.ChoiceKeys(264);
        var (n, calls, _) = StationsSceneHarness.AssertSame(_ => { }, 0, keys, $"week {week}", at, viaStep: true,
            layersImage: StationsSceneHarness.HostLayersImage);
        output.WriteLine($"week {week}: pictures {n}");
        Assert.Contains(StationCall.TypedLines, calls);
    }
}
