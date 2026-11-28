using Traders.Core.Ai;
using Traders.Core.Input;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using L = Traders.Core.Ai.RobotLineupLayout;

namespace Traders.Tests.Ai;

/// <summary>
/// Lineup AI 12C8 on data segments from real games (multi-round probe of lane aufstellung, part 1; finding
/// docs/routines/12C8_000B_aufstellung_ki.md). The default game (four computer players, RETURN after 0.5 s) runs in the
/// RefEmu with the robot battle switched on (DS:411A = 1 at every entry of 1C68:000B) and the robot cycle DS:08EC set to 2 in
/// even and 1 in odd weeks, so that weeks 0..4 all have a robot phase with both cycles; week 0 has no battle (1C68:006B), weeks
/// 1..4 reach the lineup. At every entry of 12C8:000B, 003B, 005A, 030B, 06D6, 0539, 0621 and 08EA the data segment is
/// recorded (0539, 0621 and 06D6: a seeded sample of at most <see cref="PerWeek"/> per week). Each state is replayed: the
/// original routine (RefEmu, LoadOnly, direct call) and the port run on it; the whole data segment and DS:10D2 must match.
/// The probe with a human player replays the 003B/0539 states with one computer switched to human (IsComputer = 0), the
/// state 0535:00B4 and 0535:012B leave for a human; a lineup played by a human in the RefEmu belongs to part 2 (input via
/// 0E9E). Expected values come only from the RefEmu.
/// </summary>
public class RobotLineupAiGameStateDifferentialTests
{
    private const ushort Seg = 0x12C8;
    private const int LastWeek = 4;
    private const int PerWeek = 300;
    private static readonly ushort[] Entries = [0x000B, 0x003B, 0x005A, 0x030B, 0x06D6, 0x0539, 0x0621, 0x08EA];
    private static readonly HashSet<ushort> Sampled = [0x0539, 0x0621, 0x06D6];

    private sealed record Capture(ushort Entry, int Week, int Cycle, int Player, byte[] Data);

    private static readonly Lazy<List<Capture>> Game = new(RecordGame);

    private static List<Capture> RecordGame()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var byKey = new Dictionary<(ushort, int), List<Capture>>();
        var seen = new Dictionary<(ushort, int), int>();
        var sample = new Random(0x12C8);
        p.Machine.AddCodeHook(p.Segment(0x1C68), 0x000B, _ =>
        {
            p.WriteByte(DgroupLayout.RobotBattle, 1);
            p.WriteByte(DgroupLayout.RobotCycle, (byte)(p.Week % 2 == 0 ? 2 : 1));
        });
        foreach (ushort entry in Entries)
            p.Machine.AddCodeHook(p.Segment(Seg), entry, _ =>
            {
                var key = (entry, p.Week);
                int n = seen[key] = seen.GetValueOrDefault(key) + 1;
                if (!byKey.TryGetValue(key, out var list)) byKey[key] = list = [];
                int slot = !Sampled.Contains(entry) || n <= PerWeek ? list.Count : sample.Next(n); // reservoir sample per key
                if (slot >= PerWeek && Sampled.Contains(entry)) return;
                var c = new Capture(entry, p.Week, p.ReadByte(DgroupLayout.RobotCycle), (sbyte)p.ReadByte(DgroupLayout.ActivePlayer),
                    p.Machine.Mem.ReadBlock(p.Dgroup, 0, DgroupLayout.Size));
                if (slot == list.Count) list.Add(c); else list[slot] = c;
            });
        while (true)
        {
            Assert.True(p.RunToRoutine(0x0EDE, 0x0009, 1200),
                $"battle of week {LastWeek} not reached (at week {p.Week}, {p.Machine.Seconds:F0} s emulated, {byKey.Values.Sum(l => l.Count)} captures)");
            if (p.Week >= LastWeek) break;
        }
        var captures = byKey.Values.SelectMany(l => l).ToList();
        return captures;
    }

    private static void Replay(OriginalProgram o, Capture c, string label, Action<byte[]>? change = null)
    {
        var data = (byte[])c.Data.Clone();
        change?.Invoke(data);
        o.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(o.Dgroup, 0), data);
        var ds = new Dgroup(o.DataSegment());
        Action<Dgroup> port = c.Entry switch
        {
            0x000B => RobotLineupAi.ResetMarks,
            0x003B => RobotLineupAi.Plan,
            0x005A => RobotLineupAi.MarkQuadrant,
            0x030B => RobotLineupAi.PlanRings,
            0x06D6 => RobotLineupAi.ChooseTarget,
            0x0539 => RobotLineupAi.Steer,
            0x0621 => RobotLineupAi.SteerToTarget,
            _ => RobotLineupAi.SteerByPass,
        };
        o.CallWithBudget(5_000_000, Seg, c.Entry);
        port(ds);
        ControlHarness.AssertSameData(label, o.DataSegment(), ds.Bytes);
        Assert.Equal(o.ReadDword(OriginalProgram.RandomStateOffset), ds.U32(DgroupLayout.RandomSeed));
    }

    // [DIFFERENTIAL] 12C8:000B
    // [DIFFERENTIAL] 12C8:003B
    // [DIFFERENTIAL] 12C8:005A
    // [DIFFERENTIAL] 12C8:030B
    // [DIFFERENTIAL] 12C8:06D6
    // [DIFFERENTIAL] 12C8:0539
    // [DIFFERENTIAL] 12C8:0621
    // [DIFFERENTIAL] 12C8:08EA
    [GameDataFact]
    public void Lineup_ai_of_four_robot_weeks_matches_the_original()
    {
        var captures = Game.Value;
        var o = OriginalProgram.LoadOnly(TestGameData.DosDir);
        foreach (var c in captures)
            Replay(o, c, $"{c.Entry:X4} week {c.Week} cycle {c.Cycle} player {c.Player}");

        // Coverage: lineups in weeks 1..4 with both cycles, every computer planned each week, steering every week.
        for (int w = 1; w <= LastWeek; w++)
        {
            Assert.Contains(captures, c => c.Entry == 0x000B && c.Week == w);
            for (int pl = 0; pl < 4; pl++)
                Assert.Contains(captures, c => c.Entry == 0x003B && c.Week == w && c.Player == pl);
            Assert.Contains(captures, c => c.Entry == 0x0539 && c.Week == w);
            Assert.Contains(captures, c => c.Entry == 0x0621 && c.Week == w);
        }
        Assert.Contains(captures, c => c.Entry == 0x003B && c.Cycle == 1);
        Assert.Contains(captures, c => c.Entry == 0x003B && c.Cycle == 2);
    }

    /// <summary>
    /// The recorded 003B and 0539 states with one computer player switched to human: 003B is then not called for that player
    /// by 0535:00B4 (so only 0539 is replayed for him), and 0539 must leave his control bits alone.
    /// </summary>
    // [DIFFERENTIAL] 12C8:0539
    // [DIFFERENTIAL] 12C8:003B
    [GameDataFact]
    public void Lineup_ai_with_a_human_player_matches_the_original()
    {
        var captures = Game.Value;
        var o = OriginalProgram.LoadOnly(TestGameData.DosDir);
        int replayed = 0;
        foreach (var c in captures.Where(c => c.Entry is 0x0539 or 0x003B))
            for (int human = 0; human < 4; human++)
            {
                if (c.Entry == 0x003B && human == c.Player) continue;
                int h = human;
                Replay(o, c, $"{c.Entry:X4} week {c.Week} player {c.Player} human {h}",
                    data => data[DgroupLayout.IsComputer + h] = 0);
                if (c.Entry == 0x0539)
                    Assert.Equal(c.Data[ControlReader.Bits + h], o.ReadByte((ushort)(ControlReader.Bits + h)));
                replayed++;
            }
        Assert.True(replayed > 100, $"only {replayed} states replayed");
    }

    /// <summary>The recorded states reach every pass of the target search and both steering branches of 0539.</summary>
    [GameDataFact]
    public void Recorded_lineups_reach_the_branches()
    {
        var captures = Game.Value;
        var steer = captures.Where(c => c.Entry == 0x0539).ToList();
        Assert.Contains(steer, c => Enumerable.Range(0, 4).Any(pl => c.Data[L.Presses + pl] > 0
            && c.Data[L.Mode + pl] == L.ModePlacing));
        Assert.Contains(captures, c => c.Entry == 0x06D6 && c.Player is >= 0 and < 4 && c.Data[L.Pass + c.Player] > 0);
    }
}
