using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Ai;

/// <summary>
/// Calls 11A3:0000 with a prepared data segment and the pass argument in the original (T.EXE loaded with
/// <see cref="OriginalProgram.LoadOnly"/>) and runs <see cref="WeekPlanning.Apply"/> on the same bytes; compares the whole
/// data segment, the random state DS:10D2, the number and arguments of the draws and the host log. Records which of the
/// <see cref="Branches"/> the original passed, so the tests can show that every path was exercised. Expected values come
/// only from the original run.
/// </summary>
internal sealed class WeekPlanningHarness
{
    /// <summary>Instructions in 11A3 that mark one path each (offset, meaning).</summary>
    public static readonly (ushort Offset, string Path)[] Branches =
    [
        (0x015F, "007D shared lead"), (0x0161, "007D single lead"),
        (0x0310, "0180 candidate by 4BBE"), (0x0331, "0180 small field, no draw"), (0x0398, "0180 free target, selection"),
        (0x0392, "0180 free target, draw not 0"), (0x03F8, "0180 candidate by rank"), (0x041F, "0180 target kept if flags[4]"),
        (0x0430, "0180 target dropped"),
        (0x04D9, "043F weight 0"), (0x04DD, "043F weight 2"), (0x04E1, "043F weight 5"), (0x0553, "043F no good land"),
        (0x05E3, "0599 main kind changes"), (0x065B, "061A human share >= 70"), (0x0759, "067D kind taken over"),
        (0x08AE, "078A target, not first"), (0x08B9, "078A target, lead < 100"), (0x08C5, "078A target, lead < 200"),
        (0x08DD, "078A target, lead < 300"), (0x08ED, "078A target, lead >= 300"), (0x090A, "078A 4BC2 = 1"),
        (0x0919, "078A 4BC2 != 1"), (0x093A, "078A ware 3 < 200"), (0x0953, "078A ware 3 < 80, at least 2"),
        (0x095A, "078A rockets cut to good land"), (0x0962, "078A second kind alone"), (0x0971, "078A second kind taken"),
        (0x09CC, "078A launchable limits"), (0x0A09, "078A stock limits second kind"), (0x0A3D, "078A stock limits main kind"),
        (0x0CA2, "0B3B stage 0"), (0x0CBA, "0B3B stage 1"), (0x0CCB, "0B3B stage 2"), (0x0CE7, "0B3B field planned"),
        (0x0DF5, "0D35 match"), (0x0EAC, "0E01 match"), (0x0F04, "0EB8 rocket field"), (0x0F20, "0EB8 empty field"),
        (0x0F24, "0EB8 any field"), (0x0FE9, "0F2E list entry"), (0x10E7, "104B field surplus negative"),
        (0x1189, "104B rocket shortfall"), (0x120C, "104B price limit at least 30"),
    ];

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<short> _draws = [];

    public WeekPlanningHarness()
    {
        _initial = _program.DataSegment();
        _program.Machine.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _draws.Add((short)_program.StackArgument(0)));
        ushort seg = _program.Segment(0x11A3);
        foreach (var (offset, _) in Branches) _program.Machine.AddCodeHook(seg, offset, _ => Hits.Add(offset));
    }

    /// <summary>Offsets of <see cref="Branches"/> the original passed so far (over all runs).</summary>
    public HashSet<ushort> Hits { get; } = [];

    /// <summary>Number of runs in which the original drew random(5).</summary>
    public int RunsWithDraw { get; private set; }

    /// <summary>Runs original and port on <paramref name="input"/>; asserts identical results and returns the port's state.</summary>
    public Dgroup AssertSame(Dgroup input, byte pass, string label)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        int events = m.Log.Events.Count;
        _draws.Clear();
        try
        {
            // argument as pushed by the week loop (mov al,n / push ax): AH arbitrary, here 0x5A
            _program.Call(0x11A3, 0x0000, (ushort)(0x5A00 | pass));
        }
        catch (EmulationException e)
        {
            Assert.Fail($"{label}: original faulted: {e.Message}");
        }
        var original = _program.DataSegment();

        var ported = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), ported.U32(DgroupLayout.RandomSeed));
        WeekPlanning.Apply(ported, random, pass);
        ported.Set(DgroupLayout.RandomSeed, random.Seed);

        Assert.True(m.Log.Events.Count == events, $"{label}: original used a host service");
        Assert.Equal(_initial[DgroupLayout.Size..], original[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.True(_draws.Count == random.Calls, $"{label}: original drew {_draws.Count} times, port {random.Calls}");
        Assert.All(_draws, n => Assert.Equal(5, n));
        if (_draws.Count > 0) RunsWithDraw++;
        return ported;
    }
}
