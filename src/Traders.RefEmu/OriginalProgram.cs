using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.RefEmu;

/// <summary>
/// Convenience layer for differential tests against the original T.EXE: addresses are given as in
/// the analysis (load-image segments, DGROUP offsets) and translated to the running machine.
/// </summary>
public sealed class OriginalProgram
{
    /// <summary>DGROUP paragraph in the load image (data segment of T.EXE).</summary>
    public const ushort ImageDgroup = 0x28DC;

    /// <summary>Emulated time at which the main menu is fully shown after <see cref="BootToMainMenu"/>'s inputs.</summary>
    public const double MainMenuTime = 17.5;

    /// <summary>Hash of the main menu frame (<see cref="FrameDump.Hash"/>) reached by <see cref="BootToMainMenu"/>.</summary>
    public const string MainMenuFrameHash = "DBAB2787F9AA5222";

    public PcMachine Machine { get; }

    private OriginalProgram(PcMachine machine) => Machine = machine;

    public ushort Segment(ushort imageSegment) => (ushort)(Machine.LoadSegment + imageSegment);

    public ushort Dgroup => Segment(ImageDgroup);

    /// <summary>
    /// Loads T.EXE without running it: DS = DGROUP, private stack at 9000:FFF0. The data segment holds
    /// the initialised values from the executable. Suitable for calling self-contained routines.
    /// </summary>
    public static OriginalProgram LoadOnly(string dosDirectory, MachineOptions? options = null)
    {
        var m = new PcMachine(options ?? new MachineOptions { HostDirectory = dosDirectory });
        m.LoadProgram("T.EXE");
        m.Cpu.Ds = (ushort)(m.LoadSegment + ImageDgroup);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        return new OriginalProgram(m);
    }

    /// <summary>
    /// Boots T.EXE with the standard reference inputs (any four digits at the "INSERT COIN!" code
    /// request – the original accepts every answer) and runs until the main menu is displayed.
    /// </summary>
    public static OriginalProgram BootToMainMenu(string dosDirectory, MachineOptions? options = null)
    {
        var m = new PcMachine(options ?? new MachineOptions { HostDirectory = dosDirectory });
        m.LoadProgram("T.EXE");
        for (int i = 0; i < 4; i++) m.ScheduleKey(14.0 + 0.4 * i, 0x02);
        m.RunFor(MainMenuTime);
        return new OriginalProgram(m);
    }

    /// <summary>Calls a far routine given by its image address; returns DX:AX.</summary>
    public (ushort Ax, ushort Dx) Call(ushort imageSegment, ushort offset, params ushort[] args) =>
        Machine.CallFar(Segment(imageSegment), offset, args);

    public byte ReadByte(ushort dsOffset) => Machine.Mem.Read8(Dgroup, dsOffset);
    public ushort ReadWord(ushort dsOffset) => Machine.Mem.Read16(Dgroup, dsOffset);
    public uint ReadDword(ushort dsOffset) => (uint)(ReadWord(dsOffset) | (ReadWord((ushort)(dsOffset + 2)) << 16));
    public void WriteByte(ushort dsOffset, byte v) => Machine.Mem.Write8(Dgroup, dsOffset, v);
    public void WriteWord(ushort dsOffset, ushort v) => Machine.Mem.Write16(Dgroup, dsOffset, v);

    public void WriteDword(ushort dsOffset, uint v)
    {
        WriteWord(dsOffset, (ushort)v);
        WriteWord((ushort)(dsOffset + 2), (ushort)(v >> 16));
    }

    /// <summary>Copy of the whole 64 KB data segment (for state comparisons).</summary>
    public byte[] DataSegment() => Machine.Mem.ReadBlock(Dgroup, 0, 0x10000);

    public FrameDump Frame() => FrameDump.Capture(Machine.Vga);
}
