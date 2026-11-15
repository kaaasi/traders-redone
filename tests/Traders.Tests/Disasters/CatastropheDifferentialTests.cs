using Traders.Core;
using Traders.Core.Disasters;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using Xunit.Abstractions;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Disasters;

/// <summary>
/// Differential tests of the third kind 111D:0519 with the message 111D:0757 and the cases of 111D:0009 (finding
/// docs/routines/111D_0009_katastrophen.md; 0009 with the star show and real drawing: <see cref="StarShowDifferentialTests"/>).
/// The original runs in the RefEmu (LoadOnly, direct calls). Its output calls, the pause check, the wait loop and the star show
/// 111D:0112 are replaced by a far return and recorded with their arguments; the port emits the same calls as
/// <see cref="DisasterEvent"/>s or host calls. 0825:06CB, 0E31:0361 and 0609:10D2
/// run in the original. Compared: the whole data segment, the random state, the arguments of all random() calls and the
/// sequence of recorded calls. Expected values come only from the RefEmu.
/// </summary>
public class CatastropheDifferentialTests(ITestOutputHelper output)
{
    private const ushort Seg = 0x111D;

    /// <summary>Calls replaced by retf: name and the recorded argument words (−1 = low byte of the next word).</summary>
    private static readonly (ushort Seg, ushort Off, int[] Args, string Name)[] Stubs =
    [
        (0x186C, 0x0002, [0], nameof(DisasterEventKind.LoadImage)),
        (0x21FD, 0x037D, [0], nameof(DisasterEventKind.FadeOut)),
        (0x21FD, 0x0329, [], nameof(DisasterEventKind.FadeIn)),
        (0x21FD, 0x026C, [0], nameof(DisasterEventKind.BasePalette)),
        (0x2295, 0x006D, [0, 1], nameof(DisasterEventKind.SetColour)),
        (0x0A96, 0x001B, [], nameof(DisasterEventKind.ClearList)),
        (0x0609, 0x1ED9, [], nameof(DisasterEventKind.DrawBackground)),
        (0x0609, 0x1256, [0, 1, 2, 3], nameof(DisasterEventKind.DrawFrame)),
        (0x0A69, 0x00D3, [0, 1, 2, 3, 4, 5, 6, 7, 8], nameof(DisasterEventKind.DrawTitle)),
        (0x0A69, 0x0288, [0, 1, 2, 3, 5, 6, 7], nameof(DisasterEventKind.TypeLines)),
        (0x21FD, 0x060D, [], nameof(DisasterEventKind.ListOnBothPages)),
        (0x21FD, 0x051F, [], nameof(DisasterEventKind.FlipPages)),
        (0x21FD, 0x0590, [], nameof(DisasterEventKind.DrawOnShownPage)),
        (0x111D, 0x07E2, [0, ~1], nameof(DisasterEventKind.Column)),
        (0x21A7, 0x0054, [0, 1], nameof(DisasterEventKind.Sound)),
        (0x21FD, 0x0185, [0, 1], nameof(DisasterEventKind.Delay)),
        (0x21FD, 0x0245, [], "PauseCheck"),
        (0x21FD, 0x0004, [0], "WaitLoop"),
        (0x111D, 0x0112, [], "StarShow"),
    ];

    private sealed class Harness
    {
        public OriginalProgram Program { get; } = OriginalProgram.LoadOnly(TestGameData.DosDir);
        public List<string> Calls { get; } = [];
        public List<short> RandomArgs { get; } = [];

        public Harness()
        {
            var m = Program.Machine;
            foreach (var (seg, off, args, name) in Stubs)
            {
                ushort s = Program.Segment(seg);
                m.AddCodeHook(s, off, _ => Calls.Add(
                    $"{name}({string.Join(", ", args.Select(i => i < 0 ? (short)(byte)Program.StackArgument(~i) : (short)Program.StackArgument(i)))})"));
                m.Mem.Write8(s, off, 0xCB); // retf: the caller removes the arguments
            }
            m.AddCodeHook(Program.Segment(0x21FD), 0x01BA, _ => RandomArgs.Add((short)Program.StackArgument(0)));
        }
    }

    private sealed class PortHost : IDisasterHost
    {
        public List<string> Calls { get; } = [];

        public void Emit(DisasterEvent e)
        {
            // the typed characters happen inside 0A69:0288, which the original runs as a stub here
            if (e.Kind != DisasterEventKind.TypeCharacter) Calls.Add(e.ToString());
        }

        public IEnumerable<FieldWait> PauseCheck(Dgroup d)
        {
            Calls.Add("PauseCheck()");
            return [];
        }

        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
        {
            Calls.Add($"WaitLoop({mode})");
            return [];
        }
    }

    /// <summary>One input state: name, generator seed of the field, week, grid, land type DS:08F1, random state DS:10D2.</summary>
    internal sealed record Case(string Name, int Seed, short Week, short Columns, short Rows, byte LandType, uint Random);

    /// <summary>
    /// A game-like field: <paramref name="c"/> columns × rows with owners −1..3, kinds −2..4 (headquarters only for owners),
    /// qualities 0..3, distinct or tied cash per player (ranks), catastrophes on, week and land type as given. Input only.
    /// </summary>
    internal static void SetState(OriginalProgram p, Case c)
    {
        var rng = new Random(c.Seed);
        p.WriteWord(L.Week, (ushort)c.Week);
        p.WriteByte(L.Catastrophes, 1);
        p.WriteWord(L.FieldColumns, (ushort)c.Columns);
        p.WriteWord(L.FieldRows, (ushort)c.Rows);
        p.WriteByte(L.LandType, c.LandType);
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                int cell = L.FieldIndex(x, y);
                sbyte owner = (sbyte)(rng.Next(5) - 1);
                sbyte kind = owner < 0 ? (sbyte)-2 : (sbyte)(rng.Next(7) - 2);
                if (kind == -1) kind = 3;
                p.WriteByte((ushort)(L.FieldOwner + cell), unchecked((byte)owner));
                p.WriteByte((ushort)(L.FieldKind + cell), unchecked((byte)kind));
                p.WriteByte((ushort)(L.FieldQuality + cell), (byte)rng.Next(4));
            }
        for (int pl = 0; pl < L.PlayerCount; pl++)
            p.WriteDword((ushort)(L.Cash + 4 * pl), (uint)(rng.Next(3) * 1000 + pl % 2 * 500));
        p.WriteDword(L.RandomSeed, c.Random);
    }

    private static void AssertSame(Harness h, PortHost host, Dgroup core, OriginalRandom random, string label)
    {
        Assert.Equal(h.Calls, host.Calls);
        ControlHarness.AssertSameData(label, h.Program.DataSegment(), core.Bytes);
        Assert.Equal(h.Program.ReadDword(L.RandomSeed), random.Seed);
        Assert.Equal(h.RandomArgs.Count, random.Calls);
    }

    private static (Harness H, Dgroup Core, OriginalRandom Random) Prepare(Case c)
    {
        var h = new Harness();
        SetState(h.Program, c);
        var core = new Dgroup(h.Program.DataSegment().AsSpan(0, L.Size).ToArray());
        return (h, core, new OriginalRandom(new RecordedClock([]), core.U32(L.RandomSeed)));
    }

    public static TheoryData<int> Seeds() => [.. Enumerable.Range(0, 24)];

    internal static Case CaseFor(int seed)
    {
        short[] weeks = [3, 4, 5, 15, 30];
        (short C, short R)[] grids = [(9, 6), (6, 4), (4, 3)];
        var g = grids[seed % 3];
        return new Case($"seed {seed}", seed, weeks[seed % 5], g.C, g.R, (byte)(seed / 3 % 2), (uint)(seed * 0x9E3779B9u + 7));
    }

    /// <summary>The cases of <see cref="StarShowDifferentialTests.Catastrophe_with_star_show_matches_the_original"/> reach none and the kinds 0, 1 and 2 (random(2) is 0…2), and week 15/30 strikes.</summary>
    [GameDataFact]
    public void Every_branch_of_0009_is_covered()
    {
        var seen = new HashSet<string>();
        foreach (int seed in Enumerable.Range(0, 24))
        {
            var c = CaseFor(seed);
            var (h, _, _) = Prepare(c);
            h.Program.Call(Seg, 0x0009);
            bool star = h.Calls.Contains("StarShow()");
            int kind = !star ? -1 : h.Calls.Contains("BasePalette(1952)") ? 0 : h.Calls.Contains("DrawOnShownPage()") ? 2 : 1;
            bool tenth = h.RandomArgs.Count > 0 && h.RandomArgs[0] == 10;
            seen.Add($"kind {kind}");
            if (star && c.Week % 15 == 0) seen.Add("week multiple of 15");
            output.WriteLine($"{c}: kind {kind}, randoms {h.RandomArgs.Count}, first {(tenth ? 10 : -1)}");
        }
        Assert.Contains("kind -1", seen);
        Assert.Contains("kind 0", seen);
        Assert.Contains("kind 1", seen);
        Assert.Contains("kind 2", seen);
        Assert.Contains("week multiple of 15", seen);
    }

    /// <summary>
    /// [DIFFERENTIAL] 111D:0519 – the third kind, called directly (also reached from 0009 with random(2) = 2): grids 9 × 6, 6 × 4, 4 × 3, ranks with
    /// ties, owners −1..3 and headquarters, several random states.
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(Seeds))]
    public void Third_kind_matches_the_original(int seed)
    {
        var c = CaseFor(seed);
        var (h, core, random) = Prepare(c);
        // ranks as 0009 leaves them (0825:06CB runs in both before)
        h.Program.Call(0x0825, 0x06CB);
        Traders.Core.Rules.Ranking.Apply(core);
        h.Program.Call(Seg, 0x0519);
        var host = new PortHost();
        foreach (var _ in Catastrophes.FieldsCleared(core, random, host, new RulePage(), DisasterTestTexts.From(h.Program))) { }
        core.Set(L.RandomSeed, random.Seed);
        AssertSame(h, host, core, random, c.ToString());
    }
}
