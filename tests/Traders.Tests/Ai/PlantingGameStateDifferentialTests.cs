using Traders.Core.Ai;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;

namespace Traders.Tests.Ai;

/// <summary>
/// Part 3 of lane anbau-ki: the computer turn 135D on data segments from a real game (finding
/// docs/routines/135D_000E_computerzug.md). The default game with four computer players runs in the RefEmu through weeks 0..3;
/// at every entry of 135D:000E, 0935 and 0AB3 the data segment and the arguments are recorded (000E: the first calls per week,
/// player and mode; 0935/0AB3: all). Each recorded state is then replayed: the original routine runs on it (RefEmu, LoadOnly,
/// direct Call) and the port runs on a copy; the whole data segment must match. Expected values come only from the RefEmu.
/// The default game reaches modes 2 (week 0), 3 and 4 (every week) and 5 (week 2, player 3); 0935 every week, 0AB3 in week 0.
/// </summary>
public class PlantingGameStateDifferentialTests
{
    private const ushort Seg = 0x135D;
    private const int Weeks = 4;
    private const int PerKey = 4; // recorded 000E calls per (week, player, mode)

    private sealed record Capture(ushort Entry, int Week, int Player, int Mode, byte[] Data, ushort[] Args);

    private static List<Capture> RecordGame()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var captures = new List<Capture>();
        var seen = new Dictionary<(int, int, int), int>();
        ushort seg = p.Segment(Seg);
        void Hook(ushort off, int args) => p.Machine.AddCodeHook(seg, off, _ =>
        {
            int week = p.Week, player = (sbyte)p.ReadByte(0x37AA), mode = p.ReadByte((ushort)(0x43B2 + player));
            if (off == 0x000E)
            {
                var key = (week, player, mode);
                seen[key] = seen.GetValueOrDefault(key) + 1;
                if (seen[key] > PerKey) return;
            }
            captures.Add(new Capture(off, week, player, mode, p.DataSegment(),
                Enumerable.Range(0, args).Select(i => p.StackArgument(i)).ToArray()));
        });
        Hook(0x000E, 2);
        Hook(0x0935, 0);
        Hook(0x0AB3, 0);
        for (int w = 1; w <= Weeks; w++)
            Assert.True(p.RunToWeekStart(w, 900), $"week {w} not reached (at week {p.Week}, {p.Machine.Seconds:F0} s emulated, {captures.Count} captures)");
        return captures;
    }

    // [DIFFERENTIAL] 135D:000E
    // [DIFFERENTIAL] 135D:0935
    // [DIFFERENTIAL] 135D:0AB3
    [GameDataFact]
    public void Computer_turns_of_weeks_0_to_3_match_the_original()
    {
        var captures = RecordGame();
        var o = OriginalProgram.LoadOnly(TestGameData.DosDir);
        foreach (var c in captures)
        {
            o.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(o.Dgroup, 0), c.Data);
            var ds = new Dgroup(o.DataSegment());
            switch (c.Entry)
            {
                case 0x000E: PlantingAi.ComputerTurn(ds, (short)c.Args[0], (short)c.Args[1]); break;
                case 0x0935: PlantingAi.CommitCandidate(ds); break;
                default: PlantingAi.ChooseWantKind(ds); break;
            }
            o.Call(Seg, c.Entry, c.Args);
            ControlHarness.AssertSameData($"{c.Entry:X4} week {c.Week} player {c.Player} mode {c.Mode}", o.DataSegment(), ds.Bytes);
        }

        // Coverage: every week and every (computer) player reached 000E; both other entries ran.
        var turns = captures.Where(c => c.Entry == 0x000E).ToList();
        for (int w = 0; w < Weeks; w++)
            for (int pl = 0; pl < 4; pl++)
                Assert.Contains(turns, c => c.Week == w && c.Player == pl);
        foreach (int mode in new[] { 2, 3, 4, 5 }) Assert.Contains(turns, c => c.Mode == mode); // default game: 5 in week 2
        Assert.Contains(captures, c => c.Entry == 0x0935);
        Assert.Contains(captures, c => c.Entry == 0x0AB3);
    }
}
