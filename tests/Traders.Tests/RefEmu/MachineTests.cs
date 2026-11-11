using Traders.RefEmu.Cpu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;

namespace Traders.Tests.RefEmu;

public class MachineTests
{
    internal static PcMachine NewMachine(bool sound = true) => new(new MachineOptions
    {
        HostDirectory = TestGameData.DosDir,
        RecordSoundDriver = sound,
    });

    [GameDataFact]
    public void Mz_header_of_t_exe_is_parsed()
    {
        var h = MzLoader.ParseHeader(File.ReadAllBytes(TestGameData.Dos("T.EXE")));
        Assert.Equal(3394, h.RelocationCount);
        Assert.Equal(0x360, h.HeaderParagraphs);
        Assert.Equal((ushort)0x0000, h.InitialCs);
        Assert.Equal((ushort)0x0000, h.InitialIp);
        Assert.Equal((ushort)0x2E66, h.InitialSs);
        Assert.Equal((ushort)0x0080, h.InitialSp);
        Assert.Equal(0x2E6E0, h.ImageSize);
    }

    [GameDataFact]
    public void Loading_applies_relocations_and_builds_psp()
    {
        var m = NewMachine();
        m.LoadProgram("T.EXE");
        ushort load = m.LoadSegment;
        // startup: mov dx,DGROUP (relocated immediate at 0000:0001)
        Assert.Equal((ushort)(0x28DC + load), m.Mem.Read16(load, 0x0001));
        // startup: call far 1777:000C (main), segment word at 0000:00E6
        Assert.Equal((ushort)0x000C, m.Mem.Read16(load, 0x00E4));
        Assert.Equal((ushort)(0x1777 + load), m.Mem.Read16(load, 0x00E6));
        Assert.Equal(load, m.Cpu.Cs);
        Assert.Equal((ushort)(0x2E66 + load), m.Cpu.Ss);
        Assert.Equal(m.PspSegment, m.Cpu.Ds);
        Assert.Equal((ushort)0x20CD, m.Mem.Read16(m.PspSegment, 0));
    }

    [Fact]
    public void Default_vectors_point_to_host_stubs_and_int64_is_optional()
    {
        var with = NewMachine(sound: true);
        var without = NewMachine(sound: false);
        Assert.Equal(PcMachine.StubSegment, with.Mem.Read16(0, 0x21 * 4 + 2));
        Assert.Equal(PcMachine.StubSegment, with.Mem.Read16(0, 0x64 * 4 + 2));
        Assert.Equal((ushort)0, without.Mem.Read16(0, 0x64 * 4 + 2));
        Assert.Equal((ushort)0, without.Mem.Read16(0, 0x64 * 4));
    }

    [Fact]
    public void Vga_unchained_write_and_render_round_trip()
    {
        var vga = new VgaAdapter();
        vga.SetMode(0x13, true);
        // mode X like the game: chain-4 off, dword off, byte mode on
        vga.Seq[4] &= 0xF7;
        vga.Crtc[0x14] &= 0xBF;
        vga.Crtc[0x17] |= 0x40;
        // pixel (5, 2) = plane 1, offset 2*80 + 1
        vga.Seq[2] = 1 << 1;
        vga.Write(0xA0000 + 2 * 80 + 1, 0x77);
        var (w, h, px) = vga.RenderFrame();
        Assert.Equal(320, w);
        Assert.Equal(200, h);
        Assert.Equal(0x77, px[2 * 320 + 5]);
        Assert.Equal(1, px.Count(p => p != 0));
    }

    [Fact]
    public void Vga_chained_mode_13h_is_linear()
    {
        var vga = new VgaAdapter();
        vga.SetMode(0x13, true);
        vga.Write(0xA0000 + 7 * 320 + 13, 42);
        var (_, _, px) = vga.RenderFrame();
        Assert.Equal(42, px[7 * 320 + 13]);
        Assert.Equal(42, vga.Read(0xA0000 + 7 * 320 + 13));
    }

    [Fact]
    public void Vga_write_mode_1_copies_latches()
    {
        var vga = new VgaAdapter();
        vga.SetMode(0x13, true);
        vga.Seq[4] &= 0xF7;
        for (int p = 0; p < 4; p++) vga.Planes[p][100] = (byte)(10 + p);
        vga.Seq[2] = 0x0F;
        vga.Gc[5] = (byte)((vga.Gc[5] & 0xFC) | 1);
        vga.Read(0xA0000 + 100);
        vga.Write(0xA0000 + 200, 0);
        for (int p = 0; p < 4; p++) Assert.Equal(10 + p, vga.Planes[p][200]);
    }
}
