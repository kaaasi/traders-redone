using Traders.Core;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

/// <summary>
/// Differential test of 21FD:01BA random(n): the original code is executed in the reference
/// emulator and compared with <see cref="OriginalRandom"/> (return value and seed at DS:10D2).
/// </summary>
// [DIFFERENTIAL] 21FD:01BA
public class RandomDifferentialTests
{
    private const ushort RandomSeg = 0x21FD, RandomOff = 0x01BA;
    private const ushort TimeSeg = 0x0000, TimeOff = 0x3405;
    private const ushort SeedOffset = 0x10D2;

    private static PcMachine Prepare(uint seed)
    {
        var m = MachineTests.NewMachine();
        m.LoadProgram("T.EXE");
        m.Cpu.Ds = (ushort)(m.LoadSegment + 0x28DC);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        WriteSeed(m, seed);
        return m;
    }

    private static uint ReadSeed(PcMachine m) =>
        (uint)(m.Mem.Read16(m.Cpu.Ds, SeedOffset) | (m.Mem.Read16(m.Cpu.Ds, SeedOffset + 2) << 16));

    private static void WriteSeed(PcMachine m, uint seed)
    {
        m.Mem.Write16(m.Cpu.Ds, SeedOffset, (ushort)seed);
        m.Mem.Write16(m.Cpu.Ds, SeedOffset + 2, (ushort)(seed >> 16));
    }

    private static short CallRandom(PcMachine m, short n) =>
        (short)m.CallFar((ushort)(m.LoadSegment + RandomSeg), RandomOff, [(ushort)n]).Ax;

    [GameDataFact]
    public void Initial_seed_in_the_data_segment_is_one()
    {
        var m = MachineTests.NewMachine();
        m.LoadProgram("T.EXE");
        m.Cpu.Ds = (ushort)(m.LoadSegment + 0x28DC);
        Assert.Equal(OriginalRandom.InitialSeed, ReadSeed(m));
    }

    [GameDataFact]
    public void Sequence_matches_port_for_many_arguments()
    {
        var m = Prepare(OriginalRandom.InitialSeed);
        var port = new OriginalRandom(new RecordedClock([]));
        uint pick = 12345;
        short[] edge = [1, 2, 3, 5, 6, 7, 20, 99, 100, 200, 255, 256, 1000, 32766, short.MaxValue, -2, -3, -100, -32768];
        for (int i = 0; i < 3000; i++)
        {
            short n;
            if (i < edge.Length) n = edge[i];
            else
            {
                pick = pick * 1103515245 + 12345;
                n = (short)(pick >> 16);
                if (n is 0 or -1) n = 7;
            }
            short expected = port.Next(n);
            short actual = CallRandom(m, n);
            Assert.True(expected == actual, $"call {i}: random({n}) original {actual}, port {expected}");
            Assert.Equal(port.Seed, ReadSeed(m));
        }
    }

    [GameDataFact]
    public void Arbitrary_start_seeds_match()
    {
        foreach (uint seed in new uint[] { 0, 0xFFFFFFFF, 0x80000000, 0x12345678, 3141592622 })
        {
            var m = Prepare(seed);
            var port = new OriginalRandom(new RecordedClock([]), seed);
            for (int i = 0; i < 50; i++)
            {
                short n = (short)(i * 37 + 1);
                Assert.Equal(port.Next(n), CallRandom(m, n));
                Assert.Equal(port.Seed, ReadSeed(m));
            }
        }
    }

    [GameDataFact]
    public void Minus_one_divides_by_zero_in_both()
    {
        var m = Prepare(1);
        var ex = Assert.Throws<EmulationException>(() => CallRandom(m, -1));
        Assert.Contains("Divide error", ex.Message);
        var port = new OriginalRandom(new RecordedClock([]));
        Assert.Throws<DivideByZeroException>(() => port.Next(-1));
    }

    [GameDataFact]
    public void Zero_reseeds_from_borland_time()
    {
        var m = Prepare(1);
        var (ax, dx) = m.CallFar((ushort)(m.LoadSegment + TimeSeg), TimeOff, [0, 0]);
        uint timeValue = (uint)(ax | (dx << 16));
        Assert.Equal(0, CallRandom(m, 0));
        uint seed = ReadSeed(m);
        Assert.Equal(timeValue, seed);

        // Borland time() converts local DOS time assuming TZ=EST5EDT (timezone 18000 s, daylight saving on)
        var start = m.Options.StartTime;
        long localUnix = (long)(start - new DateTime(1970, 1, 1)).TotalSeconds;
        Assert.Equal(localUnix + 18000 - 3600, (long)seed);

        // the port takes exactly this value as clock input
        var port = new OriginalRandom(new RecordedClock([(int)seed]));
        Assert.Equal(0, port.Next(0));
        Assert.Equal(seed, port.Seed);
        for (int i = 0; i < 20; i++) Assert.Equal(port.Next(99), CallRandom(m, 99));
    }
}
