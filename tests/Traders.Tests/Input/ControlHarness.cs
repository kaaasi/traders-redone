using Traders.Core.Input;
using Traders.Core.State;
using Traders.RefEmu;

namespace Traders.Tests.Input;

/// <summary>
/// Device values of one differential case, applied to both sides: to the RefEmu as port and int 33h models
/// (<see cref="Traders.RefEmu.Machine.GameDevices"/>), to the port as <see cref="IControlDevices"/>.
/// </summary>
internal sealed class DeviceSetup
{
    /// <summary>Reads of port 201h per axis bit during which the bit stays set (null = no joystick).</summary>
    public ushort[]? AxisReads { get; init; }

    public byte JoystickButtons { get; init; } = 0xF0;
    public bool Mouse { get; init; }
    public (short X, short Y)[] MouseMotions { get; init; } = [];
    public ushort MouseButtons { get; init; }

    /// <summary>Status byte of the parallel adapter per select byte (null = no adapter, the status port reads FFh).</summary>
    public Func<byte, byte>? ParallelStatus { get; init; }

    public void Apply(OriginalProgram p)
    {
        var d = p.Machine.Devices;
        d.AxisReads = AxisReads;
        d.JoystickButtons = JoystickButtons;
        d.Mouse = Mouse;
        d.MouseMotions.Clear();
        foreach (var m in MouseMotions) d.MouseMotions.Enqueue(m);
        d.MouseButtons = MouseButtons;
        d.ParallelBase = 0x378;
        d.ParallelStatus = ParallelStatus;
        d.ParallelWrites.Clear();
        d.MouseCalls.Clear();
        p.Machine.Mem.Write16(0x40, 0x08, 0x0378); // LPT1 in the BIOS data area
    }

    /// <summary>
    /// The port's view of the same devices. The joystick reading is what the timing loop 27C6:01D5 measures on the RefEmu port
    /// model: each axis count is the number of reads with the bit set, limited by the loop's 15 536 iterations.
    /// </summary>
    public RecordingDevices ForPort() => new(this);
}

internal sealed class RecordingDevices(DeviceSetup setup) : IControlDevices
{
    private readonly Queue<(short X, short Y)> _motions = new(setup.MouseMotions);

    public List<byte> ParallelWrites { get; } = [];
    public int MotionReads { get; private set; }
    public int ButtonReads { get; private set; }
    public List<int> JoystickReads { get; } = [];

    public JoystickReading ReadJoystick(int stick)
    {
        JoystickReads.Add(stick);
        if (setup.AxisReads is not { } axes) return KeyboardOnlyDevices.Instance.ReadJoystick(stick);
        int bx = stick == 1 ? 0 : 2;
        ushort Count(int axis) => (ushort)Math.Min((int)axes[axis], KeyboardOnlyDevices.JoystickLoopLimit);
        return new JoystickReading(Count(bx), Count(bx + 1), setup.JoystickButtons);
    }

    public MouseMotion ReadMouseMotion()
    {
        MotionReads++;
        var (x, y) = _motions.Count > 0 ? _motions.Dequeue() : ((short)0, (short)0);
        return new MouseMotion(x, y);
    }

    public ushort ReadMouseButtons()
    {
        ButtonReads++;
        return setup.MouseButtons;
    }

    public byte ReadParallelStatus(byte data)
    {
        ParallelWrites.Add(data);
        return setup.ParallelStatus?.Invoke(data) ?? 0xFF;
    }
}

/// <summary>Runs a routine of 0E9E on the RefEmu and its port on a copy of the same data segment and compares everything.</summary>
internal sealed class ControlHarness
{
    private readonly OriginalProgram _p = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;

    public ControlHarness() => _initial = _p.DataSegment();

    public OriginalProgram Program => _p;

    /// <summary>Restores the data segment of the loaded image (between cases).</summary>
    public void Reset()
    {
        _p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(_p.Dgroup, 0), _initial);
        _p.Machine.Cpu.Ds = _p.Dgroup;
        _p.Machine.Cpu.Ss = 0x9000;
        _p.Machine.Cpu.Sp = 0xFFF0;
    }

    /// <summary>
    /// Calls <paramref name="segment"/>:<paramref name="offset"/> on the RefEmu and <paramref name="port"/> on a Dgroup copy taken
    /// before the call; asserts the whole data segment, the random state and the device accesses are equal.
    /// </summary>
    public void Compare(string label, ushort segment, ushort offset, DeviceSetup devices, Action<Dgroup, IControlDevices> port,
        ushort[]? args = null)
    {
        devices.Apply(_p);
        var portDs = new Dgroup(_p.DataSegment());
        var portDevices = devices.ForPort();
        port(portDs, portDevices);

        _p.Call(segment, offset, args ?? []);
        var original = _p.DataSegment();

        AssertSameData(label, original, portDs.Bytes);
        Assert.Equal(_p.ReadDword(OriginalProgram.RandomStateOffset), portDs.U32(DgroupLayout.RandomSeed));
        var emu = _p.Machine.Devices;
        if (devices.ParallelStatus is not null) // without an adapter the RefEmu leaves the port unmapped and records nothing
            Assert.True(emu.ParallelWrites.SequenceEqual(portDevices.ParallelWrites), $"{label}: LPT writes differ");
        Assert.True(emu.MouseCalls.GetValueOrDefault((ushort)0x0B) == portDevices.MotionReads, $"{label}: int 33h/0Bh calls differ");
        Assert.True(emu.MouseCalls.GetValueOrDefault((ushort)0x03) == portDevices.ButtonReads, $"{label}: int 33h/03h calls differ");
        Assert.True(emu.MouseMotions.Count == Math.Max(0, devices.MouseMotions.Length - portDevices.MotionReads),
            $"{label}: mouse motions consumed differ");
    }

    public static void AssertSameData(string label, byte[] original, byte[] port)
    {
        for (int i = 0; i < DgroupLayout.Size; i++)
            if (original[i] != port[i])
                Assert.Fail($"{label}: DS:{i:X4} original {original[i]:X2}, port {port[i]:X2}");
    }

    public void WriteByte(int offset, byte v) => _p.WriteByte((ushort)offset, v);
    public void WriteWord(int offset, ushort v) => _p.WriteWord((ushort)offset, v);
    public void WriteDword(int offset, uint v) => _p.WriteDword((ushort)offset, v);
}
