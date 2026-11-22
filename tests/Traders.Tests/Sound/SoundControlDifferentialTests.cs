using Traders.Core.Sound;
using S = Traders.Core.Sound.SoundControl;

namespace Traders.Tests.Sound;

/// <summary>
/// Differential tests of the sound control 218B/21A7/27C6:041B (finding docs/routines/218B_21A7_ton.md): the same data segment
/// and the same driver answer go into the original (RefEmu, LoadOnly, direct calls) and the port; data segment, random state,
/// the commands (int 64h calls, effect machine and PC speaker calls, callback) and the callback pointer must be equal.
/// Cases: sound on/off, AdLib/PC speaker/other values of DS:0910, driver ready or not, music playing or not (DS:10A5 and the
/// driver's state independently), same/other piece, all flag bits, driver vector missing, null and unnormalised heap blocks.
/// </summary>
public class SoundControlDifferentialTests
{
    private static readonly ushort[] Devices = [1, 1, 1, 0, 0, 2, 0xFFFF, 0x0101];
    private static readonly ushort[] Flags = [0, 1, 2, 3, 0, 1, 2, 3, 4, 7, 0xFFFC, 0xFFFF];
    private static readonly byte[] States = [0, 0, 1, 2, 0x80, 0xFF];

    private static ushort Pick(Random r, ushort[] values) => values[r.Next(values.Length)];

    private static ushort Flag(Random r) => r.Next(4) switch { 0 => 0, 1 => 1, 2 => (ushort)r.Next(2, 65536), _ => 0xFFFF };

    /// <summary>Random sound state: device, sound switch, driver ready, music playing, current piece, piece names.</summary>
    private static void Arrange(SoundHarness h, Random r)
    {
        h.Reset();
        h.WriteWord(S.SoundDevice, Pick(r, Devices));
        h.WriteWord(S.SoundOn, Flag(r));
        h.WriteWord(S.DriverReady, Flag(r));
        h.WriteWord(S.MusicPlaying, Flag(r));
        h.WriteWord(S.CurrentPiece, r.Next(3) == 0 ? (ushort)0xFFFF : (ushort)r.Next(4));
        for (int i = 0; i < 4; i++)
        {
            h.WriteWord(S.PieceNames + 4 * i, (ushort)r.Next(65536));
            h.WriteWord(S.PieceNames + 4 * i + 2, (ushort)r.Next(65536));
            h.WriteByte(S.PieceDisk + i, (byte)r.Next(256));
        }
        h.Driver.PlaybackState = States[r.Next(States.Length)];
    }

    // [DIFFERENTIAL] 218B:0002, 0000:4F65, 0000:5079
    [GameDataFact]
    public void Start_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x0002);
        uint[] blocks = [0x1234_0008, 0x5000_0000, 0, 0xFFFF_0010, 0x0FFF_0010, 0xF000_FFF0, 0x0001_F3FF, 0x8000_7FFF, 0xFFFF_FFFF];
        for (int n = 0; n < 160; n++)
        {
            Arrange(h, r);
            bool installed = r.Next(5) != 0;
            uint block = r.Next(3) == 0 ? (uint)r.NextInt64(0, 0x1_0000_0000) : blocks[r.Next(blocks.Length)];
            h.SetDriverInstalled(installed);
            h.SetAllocation(block);
            int requested = -1;
            h.Compare($"0002 case {n} (installed {installed}, block {block:X8})", 0x218B, 0x0002, [],
                (d, c) => c.Start(d, installed ? (uint)SoundHarness.DriverSegment << 16 : 0, size => { requested = size; return block; }));
            if (requested != -1) Assert.Equal(S.BufferBlockSize, requested);
        }
    }

    // [DIFFERENTIAL] 218B:0094
    [GameDataFact]
    public void Play_music_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x0094);
        for (int n = 0; n < 300; n++)
        {
            Arrange(h, r);
            ushort piece = r.Next(8) == 0 ? (ushort)3 : (ushort)r.Next(3);
            if (r.Next(3) == 0) h.WriteWord(S.CurrentPiece, piece);
            ushort flags = Pick(r, Flags);
            h.Compare($"0094 case {n} (piece {piece}, flags {flags:X4})", 0x218B, 0x0094, [piece, flags],
                (d, c) => { c.PlayMusic(d, piece, flags); return null; });
        }
    }

    // [DIFFERENTIAL] 218B:013A
    [GameDataFact]
    public void Music_state_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x013A);
        for (int n = 0; n < 80; n++)
        {
            Arrange(h, r);
            h.Compare($"013A case {n}", 0x218B, 0x013A, [], (d, c) => c.MusicState(d), lowByteOnly: true);
        }
    }

    // [DIFFERENTIAL] 218B:0153
    [GameDataFact]
    public void Stop_music_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x0153);
        for (int n = 0; n < 80; n++)
        {
            Arrange(h, r);
            h.Compare($"0153 case {n}", 0x218B, 0x0153, [], (d, c) => { c.StopMusic(d); return null; });
        }
    }

    // [DIFFERENTIAL] 218B:01A4
    [GameDataFact]
    public void Fade_step_matches_the_original()
    {
        var h = new SoundHarness();
        foreach (ushort step in new ushort[] { 0, 3, 0x7F, 0x8000, 0xFFFF })
        {
            h.Reset();
            h.Compare($"01A4({step:X4})", 0x218B, 0x01A4, [step], (d, c) => { c.SetFadeStep(step); return null; });
        }
    }

    // [DIFFERENTIAL] 21A7:000D
    [GameDataFact]
    public void Select_effects_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x000D);
        for (int n = 0; n < 40; n++)
        {
            Arrange(h, r);
            h.Compare($"000D case {n}", 0x21A7, 0x000D, [], (d, c) => c.SelectEffects(d));
        }
    }

    // [DIFFERENTIAL] 21A7:0043
    [GameDataFact]
    public void Stop_effects_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x0043);
        for (int n = 0; n < 40; n++)
        {
            Arrange(h, r);
            h.Compare($"0043 case {n}", 0x21A7, 0x0043, [], (d, c) => { c.StopEffects(d); return null; }, compareAx: false);
        }
    }

    // [DIFFERENTIAL] 21A7:0054, 21AF:0002
    [GameDataFact]
    public void Play_effect_matches_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x0054);
        for (int n = 0; n < 400; n++)
        {
            Arrange(h, r);
            ushort a = (ushort)r.Next(65536);
            ushort b = r.Next(6) == 0 ? (ushort)r.Next(65536) : (ushort)r.Next(0, 0x53);
            h.Compare($"0054 case {n} (a {a:X4}, b {b:X4})", 0x21A7, 0x0054, [a, b],
                (d, c) => { c.PlayEffect(d, a, b); return null; }, compareAx: false);
        }
    }

    // [DIFFERENTIAL] 27C6:041B, 27C6:0416
    [GameDataFact]
    public void Set_tick_callback_matches_the_original()
    {
        var h = new SoundHarness();
        (ushort Seg, ushort Off)[] targets = [(0x0000, 0x0732), (0x2850, 0x0061), (0x27C6, 0x041A), (0x1234, 0xFFFF)];
        foreach (var (seg, off) in targets)
        {
            h.Reset();
            ushort abs = (ushort)(seg + h.Program.Machine.LoadSegment);
            h.Compare($"041B({seg:X4}:{off:X4})", 0x27C6, 0x041B, [off, abs],
                (d, c) => { c.SetTickCallback(seg, off); return null; }, compareAx: false);
        }
    }

    /// <summary>A sequence of calls on one state: music, effects during music, stop – the merkers carry over.</summary>
    // [DIFFERENTIAL] 21A7:0054
    [GameDataFact]
    public void Call_sequences_match_the_original()
    {
        var h = new SoundHarness();
        var r = new Random(0x5E9);
        for (int n = 0; n < 30; n++)
        {
            Arrange(h, r);
            for (int k = 0; k < 6; k++)
            {
                // The callback is compared per call; the port starts from the RefEmu's current pointer only via a fresh control,
                // so each step restores the initial callback first.
                h.ResetCallback();
                ushort piece = (ushort)r.Next(3), flags = (ushort)r.Next(4), b = (ushort)r.Next(1, 0x53);
                switch (r.Next(3))
                {
                    case 0:
                        h.Compare($"seq {n}.{k} 0094({piece}, {flags})", 0x218B, 0x0094, [piece, flags],
                            (d, c) => { c.PlayMusic(d, piece, flags); return null; });
                        break;
                    case 1:
                        h.Compare($"seq {n}.{k} 0054(1, {b:X2})", 0x21A7, 0x0054, [1, b],
                            (d, c) => { c.PlayEffect(d, 1, b); return null; }, compareAx: false);
                        break;
                    default:
                        h.Compare($"seq {n}.{k} 0153", 0x218B, 0x0153, [], (d, c) => { c.StopMusic(d); return null; });
                        break;
                }
                h.Driver.PlaybackState = States[r.Next(States.Length)];
            }
        }
    }

    [Fact]
    public void Huge_add_and_null_compare_follow_the_runtime()
    {
        Assert.Equal(((ushort)0x12F4, (ushort)0x0C), S.HugeAdd(0x1234, 0x0008, 0x0C04));
        Assert.Equal(((ushort)0x10C0, (ushort)0x04), S.HugeAdd(0x0000, 0xFFFF, 0x0C05)); // carry adds 1000h
        Assert.True(S.IsNull(0, 0));
        Assert.True(S.IsNull(0xFFFF, 0x0010));
        Assert.False(S.IsNull(0, 1));
        Assert.Equal("int 64h 12h BX=0000 CX=0000 DX=0000", SoundCommand.Driver(0x12, 5, 6, 7).ToString());
    }
}
