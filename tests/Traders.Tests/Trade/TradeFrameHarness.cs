using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Core.Trade;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Harvest;
using Traders.Tests.Startup;

namespace Traders.Tests.Trade;

/// <summary>
/// The trade 0B9F:0002 frame by frame in a running game against the original. The original runs from a snapshot at the entry
/// of 0B9F:0002 (call 23 of the week loop, reached by the game itself with four computers) with live graphics and sound; the
/// trading computer 141D is replaced by a far return on both sides until lane handelski is merged (the computers keep the
/// actions they last had); keys reach the original as real keyboard interrupts at emulated times. A frame of the original is
/// one retrace wait (entry into 0D73:00D3). The port runs <see cref="TradeStep"/> in a <see cref="PhaseMachine"/> from call 23,
/// one <see cref="PhaseTick"/> per frame, with the key table the original held at the end of the retrace before. At the k-th
/// retrace of the original the port must wait its k-th frame; there the data segment (outside <see cref="Owned"/>), DS:10D2
/// and the number of random() calls are compared; at the return once more. The picture is part 3 (scene).
/// </summary>
internal static class TradeFrameHarness
{
    private const ushort SegRetrace = 0x0D73;

    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the trade's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>A run: the snapshot (week), players made human (control 4: arrows, Ins), key presses.</summary>
    public sealed record Script(int Week, int[] Humans, Press[] Presses);

    /// <summary>
    /// Data segment bytes the original writes in the trade that the core leaves to the output layers: those of the planting
    /// and harvest phases' outputs, and the number buffer of 0C29:0E01 (2295:0001 into the far pointer DS:02A6).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. HarvestFrameHarness.Owned,
        (0x0644, 0x10, "0C29:0E01 number buffer of 2295:0001 (scene)"),
        (0x023A, 2, "0B67:01D7/0209 page flip state (Vga pages)"),
    ];

    private const long MaxFrames = 60_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtTrade = new()
    {
        [0] = new(() => Snapshot(0)),
        [2] = new(() => Snapshot(2)),
    };

    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        while (true)
        {
            Assert.True(p.RunToRoutine(0x0B9F, 0x0002, 1800), $"0B9F:0002 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    private static readonly ushort[] AiEntries = [0x0008, 0x021A, 0x0397, 0x0464, 0x09F3, 0x0E67, 0x0CCA];

    public sealed record Result(int Retraces, long RandomCalls, byte[] DataSegment, int Deals, int Biddings);

    /// <summary>Counter-tests: a mutation of the port.</summary>
    public enum Mutation { None, DoubleFrame }

    /// <summary>The trading computer replaced: no call does anything (as the retf in the original).</summary>
    private sealed class NoAi : ITradeAi
    {
        public void Prepare(Dgroup d, OriginalRandom random) { }
        public void EnterTrade(Dgroup d, OriginalRandom random) { }
        public void ChooseRole(Dgroup d, OriginalRandom random) { }
        public void StartBidding(Dgroup d, OriginalRandom random) { }
        public void Bid(Dgroup d, OriginalRandom random) { }
        public void AfterSale(Dgroup d, OriginalRandom random) { }
        public void AfterPurchase(Dgroup d, OriginalRandom random, short price) { }
    }

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtTrade[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(0x0B9F, 0x0002), "snapshot not at 0B9F:0002");
        foreach (ushort off in AiEntries) m.Mem.Write8(o.Segment(0x141D), off, 0xCB);
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        var run = new OriginalRun(o, retCs, retIp);
        var core = new Dgroup(o.DataSegment()[..DgroupLayout.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        int deals = 0, biddings = 0;
        var host = new TradeHost(e =>
        {
            if (e.Kind == TradeEventKind.DrawStockChange && e.Args[0] == -1) deals++;
            if (e.Kind == TradeEventKind.FadeInFast) biddings++;
        });
        var step = new TradeStep(host, new NoAi(), ProgramStartDifferentialTests.Files);
        IWeekStepHandler handler = mutation == Mutation.DoubleFrame ? new DoubleTick(step, 500) : step;
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [23] = handler }, nextStep: 23);

        long frame = 0;
        machine.Advance(new PhaseTick(frame, run.Held()));
        int retraces = 0;
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"trade did not end within {MaxFrames} frames");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            run.Compare(core, random.Calls, $"retrace {retraces} (port frame {frame})");
            retraces++;
            run.RunToRetraceEnd();
            run.CheckControls();
            frame++;
            machine.Advance(new PhaseTick(frame, run.Held()));
        }
        Assert.Equal(0, step.Polls);
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == 24,
            $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 0B9F:0002 after {retraces} retraces");
        Assert.Equal(retraces, step.Retraces);
        return new Result(retraces, random.Calls, core.Bytes, deals, biddings);
    }

    /// <summary>Counter-test: the step processes frame <paramref name="at"/> twice (two of its frames in one tick).</summary>
    private sealed class DoubleTick(IWeekStepHandler inner, int at) : IWeekStepHandler
    {
        private int _ticks;

        public StepOutcome Begin(StepContext context) => inner.Begin(context);

        public StepOutcome Tick(StepContext context)
        {
            var outcome = inner.Tick(context);
            if (++_ticks == at && !outcome.Done) outcome = inner.Tick(context);
            return outcome;
        }
    }

    /// <summary>The original side: run to retraces and the return, compare, controls read.</summary>
    private sealed class OriginalRun
    {
        private readonly OriginalProgram _o;
        private readonly ushort _retCs, _retIp, _retraceCs;
        private readonly List<(int Index, byte[] Keys)> _controlReads = [];
        private byte[] _keysGiven = [];
        private int _retraceIndex = -1;

        public OriginalRun(OriginalProgram o, ushort retCs, ushort retIp)
        {
            _o = o;
            _retCs = retCs;
            _retIp = retIp;
            _retraceCs = o.Segment(SegRetrace);
            o.Machine.AddCodeHook(o.Segment(0x21FD), 0x01BA, _ => RandomCalls++);
            o.Machine.AddCodeHook(o.Segment(0x0E9E), 0x037A, _ => _controlReads.Add((_retraceIndex, Keys())));
        }

        public long RandomCalls { get; private set; }

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, DgroupLayout.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

        public bool RunToRetraceOrReturn()
        {
            var m = _o.Machine;
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp) || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3));
            Assert.False(m.Exited, "original exited");
            if (At(_retraceCs, 0x00D3))
            {
                _retraceIndex++;
                return true;
            }
            Assert.True(At(_retCs, _retIp), $"original stopped at {m.Cpu.Cs - _o.Segment(0):X4}:{m.Cpu.IP:X4}");
            return false;
        }

        public void RunToRetraceEnd()
        {
            var m = _o.Machine;
            m.Run(4_000_000, mm => mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00F7);
            Assert.True(At(_retraceCs, 0x00F7), "retrace wait did not end");
        }

        public void CheckControls()
        {
            foreach (var (index, keys) in _controlReads)
                Assert.True(keys.AsSpan().SequenceEqual(_keysGiven),
                    $"keys changed between the end of retrace {index} and the control read of the original (script ambiguous)");
            _controlReads.Clear();
        }

        public void Compare(Dgroup core, long portRandomCalls, string where)
        {
            var expected = _o.DataSegment()[..DgroupLayout.Size];
            foreach (var (offset, length, _) in Owned) Array.Copy(core.Bytes, offset, expected, offset, length);
            if (!expected.AsSpan().SequenceEqual(core.Bytes))
            {
                var diffs = Enumerable.Range(0, expected.Length).Where(i => expected[i] != core.Bytes[i]).ToList();
                int first = diffs[0];
                Assert.Fail($"{where}: DS:{first:X4} original {expected[first]:X2}, port {core.Bytes[first]:X2}; " +
                            $"differing {string.Join(" ", diffs.Take(40).Select(i => i.ToString("X4")))}");
            }
            Assert.True(RandomCalls == portRandomCalls, $"{where}: original drew {RandomCalls} times, port {portRandomCalls}");
        }
    }
}
