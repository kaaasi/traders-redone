using System.Reflection;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Default game (four computer players) stopped at phase marker 0 of week 0, snapshot taken there, and the uninterrupted
/// run continued to marker 1 as reference. Computed once for all tests of <see cref="SnapshotTests"/>.
/// </summary>
public sealed class MarkerSnapshotFixture
{
    private readonly Lazy<(MachineSnapshot Snapshot, SnapshotTests.Outcome Early, SnapshotTests.Outcome Reference,
        MachineSnapshot AtNext)> _run = new(Compute);

    public MachineSnapshot AtMarker0 => _run.Value.Snapshot;

    /// <summary>Uninterrupted run <see cref="SnapshotTests.Checkpoint"/> instructions after marker 0.</summary>
    public SnapshotTests.Outcome Early => _run.Value.Early;

    /// <summary>Uninterrupted run at marker 1 of week 0.</summary>
    public SnapshotTests.Outcome Reference => _run.Value.Reference;

    public MachineSnapshot ReferenceAtMarker1 => _run.Value.AtNext;

    private static (MachineSnapshot, SnapshotTests.Outcome, SnapshotTests.Outcome, MachineSnapshot) Compute()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToMarker(0, 0, 60), "marker 0 of week 0 not reached");
        var snap = p.Snapshot();
        var early = SnapshotTests.RunToCheckpoint(p);
        Assert.NotNull(early);
        var reference = SnapshotTests.RunToNextMarker(p);
        Assert.NotNull(reference);
        return (snap, early, reference, p.Snapshot());
    }
}

/// <summary>
/// Machine snapshot of the RefEmu (R3 lane takt, part 1; BESTAND.md §3.1 item 3): a run resumed from a snapshot at a phase
/// marker equals the uninterrupted run at the next marker – data segment, random state DS:10D2, instruction counter,
/// emulated time and frame. Snapshots are written only to a temporary folder (they hold memory derived from T.EXE).
/// </summary>
public class SnapshotTests(MarkerSnapshotFixture fx) : IClassFixture<MarkerSnapshotFixture>
{
    public sealed record Outcome(byte[] Data, uint Random, long Instructions, long Clock, string Frame, ushort Cs, ushort Ip)
    {
        public bool SameAs(Outcome o) => Data.AsSpan().SequenceEqual(o.Data) && Random == o.Random &&
            Instructions == o.Instructions && Clock == o.Clock && Frame == o.Frame && Cs == o.Cs && Ip == o.Ip;

        public override string ToString() =>
            $"random {Random:X8}, instructions {Instructions}, clock {Clock}, frame {Frame}, at {Cs:X4}:{Ip:X4}";
    }

    /// <summary>Instructions after marker 0 at which runs are compared first (about 0.1 s emulated: timer ticks and frames).</summary>
    internal const long Checkpoint = 300_000;

    internal static Outcome Observe(OriginalProgram p)
    {
        var m = p.Machine;
        return new Outcome(p.DataSegment(), p.RandomState, m.Cpu.InstructionCount, m.Now, p.Frame().Hash(), m.Cpu.Cs, m.Cpu.IP);
    }

    /// <summary>Runs <see cref="Checkpoint"/> instructions from the current stop; null if the emulated program breaks down.</summary>
    internal static Outcome? RunToCheckpoint(OriginalProgram p)
    {
        try
        {
            p.Machine.Run(Checkpoint);
        }
        catch (EmulationException)
        {
            return null;
        }
        return p.Machine.Exited ? null : Observe(p);
    }

    /// <summary>
    /// Runs from marker 0 (or a checkpoint after it) to marker 1 of week 0; null if marker 1 is not reached within 60 emulated seconds or, with
    /// <paramref name="maxInstructions"/>, within that many instructions (bounds the counter-tests, whose broken runs are slow).
    /// </summary>
    internal static Outcome? RunToNextMarker(OriginalProgram p, long? maxInstructions = null)
    {
        try
        {
            if (maxInstructions is { } n)
            {
                p.Machine.Run(1);
                p.Machine.Run(n, _ => p.IsAt(OriginalProgram.PhaseMarkerSegment, OriginalProgram.PhaseMarkerOffset) &&
                    p.Week == 0 && p.StackArgument(0) == 1);
                if (p.Machine.Exited || !p.IsAt(OriginalProgram.PhaseMarkerSegment, OriginalProgram.PhaseMarkerOffset)) return null;
            }
            else if (!p.RunToMarker(0, 1, 60)) return null;
        }
        catch (EmulationException)
        {
            return null; // a broken restore may crash the emulated program; that also counts as "not the same run"
        }
        return Observe(p);
    }

    [GameDataFact]
    public void Resumed_run_equals_the_uninterrupted_run_at_the_next_marker()
    {
        string dir = Path.Combine(Path.GetTempPath(), "traders-snapshot-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            string file = Path.Combine(dir, "marker0.snap");
            fx.AtMarker0.WriteTo(file);
            var loaded = MachineSnapshot.ReadFrom(file);
            foreach (var part in MachineSnapshot.PartOrder)
                Assert.True(loaded.PartBytes(part).SequenceEqual(fx.AtMarker0.PartBytes(part)), $"part {part} changed by file round trip");

            var p = OriginalProgram.Resume(TestGameData.DosDir, loaded);
            Assert.True(p.IsAt(OriginalProgram.PhaseMarkerSegment, OriginalProgram.PhaseMarkerOffset));
            Assert.Equal(0xA106A091u, p.RandomState); // P1 (BESTAND.md appendix A): seed at marker 0 of week 0
            var early = RunToCheckpoint(p);
            Assert.NotNull(early);
            Assert.True(early.SameAs(fx.Early), $"resumed: {early}\nreference: {fx.Early}");
            var resumed = RunToNextMarker(p);

            Assert.NotNull(resumed);
            Assert.Equal(0xFBBDAB8Du, fx.Reference.Random); // P1: seed at marker 1 of week 0
            Assert.True(resumed.SameAs(fx.Reference), $"resumed: {resumed}\nreference: {fx.Reference}");
            // beyond the observable outcome: every part of the machine is identical
            var after = p.Snapshot();
            foreach (var part in MachineSnapshot.PartOrder)
                Assert.True(after.PartBytes(part).SequenceEqual(fx.ReferenceAtMarker1.PartBytes(part)), $"part {part} differs at marker 1");
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    /// <summary>
    /// Counter-test: leaving out one part of the state breaks the resumed run – it differs at the checkpoint or at marker 1, or
    /// marker 1 is not reached. Pic, Keyboard, AdLib, Dos and Files are left out here because between marker 0 and marker 1 their
    /// state equals that of a fresh machine or is not used; the field inventory below keeps them complete.
    /// </summary>
    [GameDataTheory]
    [InlineData(SnapshotParts.Cpu)]
    [InlineData(SnapshotParts.Memory)]
    [InlineData(SnapshotParts.Pit)]
    [InlineData(SnapshotParts.Vga)]
    [InlineData(SnapshotParts.Clock)]
    public void Leaving_out_a_part_breaks_the_resumed_run(SnapshotParts omitted)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, fx.AtMarker0, SnapshotParts.All & ~omitted);
        var early = RunToCheckpoint(p);
        if (early is null || !early.SameAs(fx.Early)) return;
        long budget = fx.Reference.Instructions - fx.AtMarker0.InstructionCount + 1_000_000;
        var resumed = RunToNextMarker(p, budget);
        Assert.False(resumed is not null && resumed.SameAs(fx.Reference), $"run without {omitted} still equals the reference");
    }

    /// <summary>
    /// With FXDRIVER resident (sound TSR in memory, int 64h hooked) the resumed run also equals the uninterrupted one at
    /// marker 1, and <see cref="OriginalProgram.Resume"/> logs int 64h calls again. Measured: all 36 int 64h calls of week 0
    /// happen before marker 0, so between marker 0 and 1 both runs have none – the comparison still covers the driver's memory.
    /// </summary>
    [GameDataFact]
    public void Resumed_run_with_sound_driver_equals_the_uninterrupted_run()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir, runFxDriver: true);
        Assert.True(p.RunToMarker(0, 0, 60), "marker 0 of week 0 not reached");
        var snap = p.Snapshot();
        int logged = p.Machine.Log.Events.Count;
        Assert.NotEmpty(SoundCalls(p.Machine.Log.Events));
        var reference = RunToNextMarker(p);
        Assert.NotNull(reference);
        var referenceSound = SoundCalls(p.Machine.Log.Events.Skip(logged));

        var q = OriginalProgram.Resume(TestGameData.DosDir, snap);
        var m = q.Machine;
        Assert.True(m.HasCodeHook(m.Mem.Read16(0, 0x64 * 4 + 2), m.Mem.Read16(0, 0x64 * 4)), "int 64h not hooked after resume");
        var resumed = RunToNextMarker(q);
        Assert.NotNull(resumed);
        Assert.True(resumed.SameAs(reference), $"resumed: {resumed}\nreference: {reference}");
        Assert.Equal(referenceSound, SoundCalls(m.Log.Events));
    }

    private static List<string> SoundCalls(IEnumerable<EmuEvent> events) =>
        events.Where(e => e.Kind == EventKind.Sound).Select(e => $"{e.Instruction} {e.Text}").ToList();

    /// <summary>
    /// Inventory of the mutable fields of the machine. A new field fails this test until it is either saved in the snapshot
    /// or listed as an observer/constant below – so the snapshot cannot silently become incomplete.
    /// </summary>
    [Fact]
    public void Snapshot_covers_every_field_of_the_machine()
    {
        var expected = new Dictionary<Type, string[]>
        {
            [typeof(PcMachine)] =
            [
                // saved: Clock, Dos
                "_timeOffset", "_nextTimerIrq", "_stopHit", "_scheduledKeys", "_nextScheduledKey", "<Exited>k__BackingField",
                "<ExitCode>k__BackingField", "_files", "_blocks", "_psp", "_dtaSeg", "_dtaOff", "_lastError", "_findResults",
                "_consoleLine", "_envSegment", "_freeStart", "_residentBlocks",
                // devices (own parts) and inputs
                "<Options>k__BackingField", "<Mem>k__BackingField", "<Cpu>k__BackingField", "<Pic>k__BackingField",
                "<Pit>k__BackingField", "<Keyboard>k__BackingField", "<Vga>k__BackingField", "<AdLib>k__BackingField",
                "<Files>k__BackingField",
                // observers only (not part of the run)
                "<Log>k__BackingField", "_portLogCount", "_hookMap", "_hooks",
            ],
            [typeof(Cpu286)] =
            [
                "R", "S", "IP", "_flags", "<Halted>k__BackingField", "<InstructionCount>k__BackingField",
                "<InterruptShadow>k__BackingField",
                // wiring
                "<Mem>k__BackingField", "<Io>k__BackingField", "<HostTrap>k__BackingField",
                // per-instruction scratch, reset by Step before use
                "_snapR", "_snapS", "_snapIp", "_snapFlags", "_fetched", "_startIp", "_segOverride", "_rep", "_mod", "_reg",
                "_rm", "_eaSeg", "_eaOff",
            ],
            [typeof(Memory)] = ["<Ram>k__BackingField", "<A20>k__BackingField", "_device", "_devStart", "_devEnd"],
            [typeof(Pic)] = ["_irr", "_isr", "_imr", "_icwStep", "_icw4", "_readIsr", "<VectorBase>k__BackingField"],
            [typeof(Pit)] = ["_ch", "ReloadChanged"],
            [typeof(Keyboard)] = ["_queue", "_data", "_full", "_nextAllowed", "<SystemControl>k__BackingField", "<Spacing>k__BackingField"],
            [typeof(VgaAdapter)] =
            [
                "Planes", "Seq", "Gc", "Crtc", "Attr", "Dac", "<Misc>k__BackingField", "<PelMask>k__BackingField", "_latch",
                "_seqIndex", "_gcIndex", "_crtcIndex", "_attrIndex", "_attrFlipFlop", "_dacWrite", "_dacRead", "_dacWriteComp",
                "_dacReadComp", "_dacReading", "<FramePeriod>k__BackingField", "<StartAddressWrites>k__BackingField",
                "<Now>k__BackingField", "<UnknownPort>k__BackingField",
            ],
            [typeof(AdLibStub)] =
            [
                "Registers", "_index", "_timer1Start", "_timer2Start", "_flags", "<WriteCount>k__BackingField",
                "<RegisterWritten>k__BackingField",
            ],
            [typeof(DosFileSystem)] = ["_hostDir", "_overlay", "_deleted"],
        };
        var channel = typeof(Pit).GetNestedType("Channel", BindingFlags.NonPublic)!;
        expected[channel] = ["Reload", "LoadTime", "AccessMode", "Mode", "WriteHigh", "PendingLow", "ReadHigh", "Latched"];

        foreach (var (type, names) in expected)
        {
            var actual = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(f => f.Name);
            Assert.Equal(names.Order(StringComparer.Ordinal), actual.Order(StringComparer.Ordinal));
        }
    }
}
