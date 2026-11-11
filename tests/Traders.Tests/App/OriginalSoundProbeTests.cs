using Traders.RefEmu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.App;

/// <summary>
/// RefEmu probes for the sound layer (docs/analyse/TON.md): the int 64h interface of FXDRIVER.EXE, the music calls of
/// 218B during the boot, and the three ways of the effect request 21A7:0054. Expected values come from the reading of
/// the disassembly in TON.md; nothing is ported yet (the host's sound output follows with R10).
/// </summary>
public class OriginalSoundProbeTests
{
    private const ushort EffectSeg = 0x21A7, EffectRequest = 0x0054, SelectTickCallback = 0x000D;
    private const ushort SoundKind = 0x0910, SoundOn = 0x10A7, MusicPlaying = 0x10A5;
    private const ushort InputSegment = 0x27C6, TickCallback = 0x0416;
    private const ushort RuntimeSegment = 0x0000, FxHead = 0x0417, FxTail = 0x0419, FxRing = 0x041B;

    private static (ushort Ax, ushort Bx, ushort Cx, ushort Dx) Int64(PcMachine m, ushort ax, ushort bx = 0, ushort cx = 0, ushort dx = 0)
    {
        const ushort stubSeg = 0x8000;
        m.Mem.Write8(stubSeg, 0, 0xCD);
        m.Mem.Write8(stubSeg, 1, 0x64);
        m.Mem.Write8(stubSeg, 2, 0xCB); // int 64h; retf
        (m.Cpu.Ax, m.Cpu.Bx, m.Cpu.Cx, m.Cpu.Dx) = (ax, bx, cx, dx);
        m.CallFar(stubSeg, 0, []);
        return (m.Cpu.Ax, m.Cpu.Bx, m.Cpu.Cx, m.Cpu.Dx);
    }

    private static PcMachine ResidentDriver()
    {
        var m = new PcMachine(new MachineOptions { HostDirectory = TestGameData.DosDir, RecordSoundDriver = false });
        m.RunResidentProgram("FXDRIVER.EXE");
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        return m;
    }

    [GameDataFact]
    public void Driver_answers_version_buffer_size_and_status_and_keeps_the_function_number()
    {
        var m = ResidentDriver();
        Assert.NotEqual(0, m.Mem.Read16(0, 0x64 * 4 + 2));
        Assert.Equal((ushort)0x0011, Int64(m, 0x11).Ax);
        Assert.Equal(((ushort)0x11, (ushort)1, (ushort)0), (Int64(m, 0x11, 9, 9) is var v ? (v.Ax, v.Bx, v.Cx) : default));
        Assert.Equal((ushort)0x0C04, Int64(m, 0x15).Bx); // size of one of the 24 buffers the game hands over (fn 10h)
        Assert.Equal((ushort)0, Int64(m, 0x12, 0x55).Bx);  // nothing loaded: not playing
        var unknown = Int64(m, 0x1D, 0x1234, 0x5678, 0x9ABC); // above the table (0..1Ch): registers come back unchanged
        Assert.Equal(((ushort)0x1D, (ushort)0x1234, (ushort)0x5678, (ushort)0x9ABC), unknown);
    }

    [GameDataFact]
    public void Boot_with_the_driver_hands_over_24_buffers_and_starts_the_intro_by_file_name()
    {
        var p = OriginalProgram.BootToMainMenu(TestGameData.DosDir, runFxDriver: true);
        var events = p.Machine.Log.Events.Where(e => e.Kind is EventKind.Sound or EventKind.File).Select(e => e.Text).ToList();
        var calls = events.Where(t => t.StartsWith("int 64")).Select(Parse).ToList();

        Assert.Equal(0x06, calls[0].Ax); // detect card (BX = 0: try 388h, 288h, 318h, 218h)
        Assert.Equal(0, calls[0].Bx);
        Assert.Equal(0x05, calls[1].Ax);
        long first = 0;
        for (int i = 0; i < 24; i++)
        {
            var c = calls[2 + i];
            Assert.Equal((0x10, i), (c.Ax, c.Bx));
            long linear = c.Cx * 16L + c.Dx; // CX = segment, DX = offset
            if (i == 0) first = linear;
            Assert.Equal(first + 0x0C04L * i, linear);
        }
        // Intro: status query, then function 0 with the name TRADINTR.CMF at BX:CX = DS:10AB; the driver opens the file.
        int load = calls.FindIndex(c => c.Ax == 0x00);
        Assert.Equal(0x12, calls[load - 1].Ax);
        Assert.Equal((p.Dgroup, 0x10AB), (calls[load].Bx, calls[load].Cx));
        int loadEvent = events.FindIndex(t => t.StartsWith("int 64 AX=0000"));
        Assert.StartsWith("open TRADINTR.CMF", events[loadEvent + 1]);
        Assert.Equal([0x18, 0x1B, 0x1A, 0x01], calls.Skip(load + 1).Take(4).Select(c => c.Ax));
        Assert.Equal(0x7F, calls[load + 2].Bx); // 1Bh with 7Fh: flag bit 0 of 218B:0094 clear
        Assert.Equal(1, p.ReadWord(MusicPlaying));
        Assert.Equal(0, p.ReadWord(0x10A9)); // index of the loaded piece
    }

    [GameDataFact]
    public void Driver_timer_is_chained_behind_the_game_handler_by_the_dos_filter()
    {
        var p = OriginalProgram.BootToMainMenu(TestGameData.DosDir, runFxDriver: true);
        var m = p.Machine;
        ushort input = p.Segment(InputSegment), driver = m.Mem.Read16(0, 0x64 * 4 + 2);
        Assert.Equal((input, (ushort)0x03E4), (m.Mem.Read16(0, 0x22), m.Mem.Read16(0, 0x20)));
        Assert.Equal(driver, m.Mem.Read16(input, 0x0414)); // chain target CS:0412 now points into FXDRIVER
        Assert.NotEqual(driver, m.Mem.Read16(input, 0x0410)); // the remembered original vector stays the BIOS one
        Assert.Equal(0x4000, m.Pit.Channel0Period);
        Assert.DoesNotContain(m.Log.Events, e => e.Kind == EventKind.Dos && e.Text.StartsWith("set vector 08") && e.Text.Contains($"{driver:X4}:"));
    }

    private static (int Ax, int Bx, int Cx, int Dx) Parse(string t)
    {
        int Reg(string name) => Convert.ToInt32(t.Substring(t.IndexOf(name + "=", StringComparison.Ordinal) + 3, 4), 16);
        return (Reg("AX"), Reg("BX"), Reg("CX"), Reg("DX"));
    }

    private static OriginalProgram Effects(ushort kind)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        p.WriteWord(SoundKind, kind);
        p.WriteWord(SoundOn, 1);
        p.WriteWord(MusicPlaying, 0);
        return p;
    }

    [GameDataFact]
    public void Effect_request_with_adlib_stores_a_word_at_a_byte_index_of_a_16_entry_ring()
    {
        // 0000:0750 writes the request as a word at CS:041B + head (head counts bytes, mod 16) and the engine reads only
        // the low byte: the high byte overwrites the next entry, and entry 15 spills into CS:042B behind the ring.
        var p = Effects(1);
        var m = p.Machine;
        ushort rt = p.Segment(RuntimeSegment);
        byte behind = m.Mem.Read8(rt, FxRing + 16);
        for (int k = 1; k <= 17; k++) p.Call(EffectSeg, EffectRequest, 0x1000, (ushort)(0xA500 | k));
        Assert.Equal(1, m.Mem.Read16(rt, FxHead));
        Assert.Equal(0, m.Mem.Read16(rt, FxTail));    // played by the tick callback 0000:0732, not here
        Assert.Equal(17, m.Mem.Read8(rt, FxRing));     // 17th request in entry 0, no check for a full ring
        Assert.Equal(0xA5, m.Mem.Read8(rt, FxRing + 1)); // its high byte on entry 1
        for (int i = 2; i < 16; i++) Assert.Equal(i + 1, m.Mem.Read8(rt, (ushort)(FxRing + i)));
        Assert.Equal(0xA5, m.Mem.Read8(rt, FxRing + 16));
        Assert.NotEqual(0xA5, behind);
    }

    [GameDataFact]
    public void Effect_request_without_sound_or_with_speaker_does_not_touch_the_ring()
    {
        foreach (var (kind, on) in new[] { ((ushort)1, (ushort)0), ((ushort)0, (ushort)1), ((ushort)0, (ushort)0) })
        {
            var p = Effects(kind);
            p.WriteWord(SoundOn, on);
            int speaker = 0;
            var args = new List<ushort>();
            p.Machine.AddCodeHook(p.Segment(0x2850), 0x00CC, mm => { speaker++; args.Add(p.StackArgument(0)); });
            p.Call(EffectSeg, EffectRequest, 0x2222, 7);
            Assert.Equal(0, p.Machine.Mem.Read16(p.Segment(RuntimeSegment), FxHead));
            Assert.Equal(kind == 0 && on == 1 ? 1 : 0, speaker); // speaker path: direct far call 21A7:0085 (= 21AF:0005)
            if (speaker == 1) Assert.Equal([7], args);
        }
    }

    [GameDataFact]
    public void Tick_callback_is_the_adlib_effect_engine_or_the_speaker_routine()
    {
        var p = Effects(1);
        p.Call(EffectSeg, SelectTickCallback);
        var m = p.Machine;
        ushort input = p.Segment(InputSegment);
        Assert.Equal((p.Segment(RuntimeSegment), (ushort)0x0732), (m.Mem.Read16(input, TickCallback + 2), m.Mem.Read16(input, TickCallback)));

        p = Effects(0);
        p.Call(EffectSeg, SelectTickCallback);
        m = p.Machine;
        input = p.Segment(InputSegment);
        Assert.Equal((p.Segment(0x2850), (ushort)0x0061), (m.Mem.Read16(input, TickCallback + 2), m.Mem.Read16(input, TickCallback)));
    }
}
