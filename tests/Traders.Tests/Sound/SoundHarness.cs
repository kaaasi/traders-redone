using Traders.Core.Sound;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;

namespace Traders.Tests.Sound;

/// <summary>Playback state answered by the test driver to function 12h, on both sides.</summary>
internal sealed class FixedDriverState : IMusicDriverState
{
    public byte PlaybackState { get; set; }
}

/// <summary>
/// Runs a sound routine on the RefEmu (LoadOnly) and its port on a copy of the same data segment. On the RefEmu the int 64h
/// vector points to a small test driver (answers function 12h with BL = <see cref="FixedDriverState.PlaybackState"/>, leaves
/// all registers else unchanged) and code hooks record every int 64h call and the calls of the effect machine (0000:06FE,
/// 0724, 0750), of the PC speaker module (2850:00CC) and of 27C6:041B as <see cref="SoundCommand"/>s. The file check
/// 186C:0A6C is replaced by retf (dropped in the port, F2 = A), the heap allocation 186C:02C6 by "mov ax/mov dx/retf".
/// </summary>
internal sealed class SoundHarness
{
    /// <summary>Segment of the test driver (free memory above the loaded image, below the stack at 9000).</summary>
    public const ushort DriverSegment = 0x8800;

    private readonly OriginalProgram _p = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly byte[] _callbackInitial;
    private readonly List<SoundCommand> _recorded = [];

    public SoundHarness()
    {
        var m = _p.Machine;
        // cmp ax,12h / jnz +2 / mov bl,state / iret
        m.Mem.WriteBlock(Linear(DriverSegment, 0), [0x3D, 0x12, 0x00, 0x75, 0x02, 0xB3, 0x00, 0xCF]);
        m.AddCodeHook(DriverSegment, 0, mm =>
            _recorded.Add(SoundCommand.Driver(mm.Cpu.Ax, mm.Cpu.Bx, mm.Cpu.Cx, mm.Cpu.Dx)));
        m.Mem.Write8(Linear(_p.Segment(0x186C), 0x0A6C), 0xCB);
        Hook(SoundControl.EffectsSegment, 0x06FE, _ => new SoundCommand(SoundCommandKind.EffectsReset));
        Hook(SoundControl.EffectsSegment, 0x0724, _ => new SoundCommand(SoundCommandKind.EffectsStop));
        Hook(SoundControl.EffectsSegment, 0x0750, mm => new SoundCommand(SoundCommandKind.Effect, Bx: Arg(mm, 0)));
        Hook(SoundControl.SpeakerSegment, 0x00CC, mm => new SoundCommand(SoundCommandKind.SpeakerEffect, Bx: Arg(mm, 0)));
        Hook(0x27C6, 0x041B, mm => new SoundCommand(SoundCommandKind.TickCallback,
            Bx: (ushort)(Arg(mm, 1) - mm.LoadSegment), Cx: Arg(mm, 0)));
        _initial = _p.DataSegment();
        _callbackInitial = m.Mem.ReadBlock(_p.Segment(0x27C6), 0x0416, 4);
    }

    public OriginalProgram Program => _p;

    public FixedDriverState Driver { get; } = new();

    private static uint Linear(ushort seg, ushort off) => ((uint)seg << 4) + off;

    private static ushort Arg(Traders.RefEmu.Machine.PcMachine m, int index) =>
        m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 4 + 2 * index));

    private void Hook(ushort imageSegment, ushort offset, Func<Traders.RefEmu.Machine.PcMachine, SoundCommand> record) =>
        _p.Machine.AddCodeHook(_p.Segment(imageSegment), offset, m => _recorded.Add(record(m)));

    /// <summary>Restores the data segment and the callback pointer of the loaded image (between cases).</summary>
    public void Reset()
    {
        var m = _p.Machine;
        m.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(_p.Dgroup, 0), _initial);
        ResetCallback();
        m.Cpu.Ds = _p.Dgroup;
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        SetDriverInstalled(true);
    }

    /// <summary>Restores the callback pointer CS:0416/0418 of segment 27C6 to its initial value.</summary>
    public void ResetCallback() =>
        _p.Machine.Mem.WriteBlock(Linear(_p.Segment(0x27C6), 0x0416), _callbackInitial);

    /// <summary>int 64h vector: the test driver or null.</summary>
    public void SetDriverInstalled(bool installed)
    {
        _p.Machine.Mem.Write16(0, 0x64 * 4, 0);
        _p.Machine.Mem.Write16(0, 0x64 * 4 + 2, installed ? DriverSegment : (ushort)0);
    }

    /// <summary>The heap allocation 186C:02C6 returns <paramref name="pointer"/> (segment:offset).</summary>
    public void SetAllocation(uint pointer) =>
        _p.Machine.Mem.WriteBlock(Linear(_p.Segment(0x186C), 0x02C6),
            [0xB8, (byte)pointer, (byte)(pointer >> 8), 0xBA, (byte)(pointer >> 16), (byte)(pointer >> 24), 0xCB]);

    /// <summary>Callback pointer CS:0416/0418 of segment 27C6 as image address.</summary>
    public (ushort Segment, ushort Offset) Callback()
    {
        var m = _p.Machine;
        ushort seg = m.Mem.Read16(_p.Segment(0x27C6), 0x0418);
        return ((ushort)(seg - m.LoadSegment), m.Mem.Read16(_p.Segment(0x27C6), 0x0416));
    }

    /// <summary>
    /// Calls <paramref name="segment"/>:<paramref name="offset"/> on the RefEmu and <paramref name="port"/> on a Dgroup copy and a
    /// <see cref="SoundControl"/> with the RefEmu's callback; asserts data segment, random state, commands, callback and AX.
    /// </summary>
    public void Compare(string label, ushort segment, ushort offset, ushort[] args, Func<Dgroup, SoundControl, ushort?> port,
        bool compareAx = true, bool lowByteOnly = false)
    {
        _p.Machine.Mem.Write8(Linear(DriverSegment, 6), Driver.PlaybackState);
        var portDs = new Dgroup(_p.DataSegment());
        var control = new SoundControl(Driver);
        var (cbSeg, cbOff) = Callback();
        if (cbSeg != SoundControl.NoCallbackSegment || cbOff != SoundControl.NoCallbackOffset)
            throw new InvalidOperationException("callback not reset");
        ushort? result = port(portDs, control);

        _recorded.Clear();
        var (ax, _) = _p.Call(segment, offset, args);
        var original = _p.DataSegment();

        ControlHarness.AssertSameData(label, original, portDs.Bytes);
        Assert.Equal(_p.ReadDword(OriginalProgram.RandomStateOffset), portDs.U32(DgroupLayout.RandomSeed));
        Assert.True(_recorded.SequenceEqual(control.Commands),
            $"{label}: commands differ\n original: {string.Join("; ", _recorded)}\n port:     {string.Join("; ", control.Commands)}");
        Assert.Equal(Callback(), (control.TickCallbackSegment, control.TickCallbackOffset));
        if (compareAx && result is { } r)
            Assert.True((lowByteOnly ? (byte)ax : ax) == r, $"{label}: AX original {ax:X4}, port {r:X4}");
    }

    public void WriteWord(int offset, ushort v) => _p.WriteWord((ushort)offset, v);
    public void WriteByte(int offset, byte v) => _p.WriteByte((ushort)offset, v);
}
