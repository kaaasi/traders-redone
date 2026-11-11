using Microsoft.Xna.Framework.Input;
using Traders.App.Input;
using Traders.RefEmu;

namespace Traders.Tests.App;

/// <summary>
/// Differential tests of the host key-state table against the original keyboard interrupt in the RefEmu, and of the
/// host key map against what the original control reader 0E9E makes of the resulting table.
/// </summary>
public class HostInputDifferentialTests
{
    private static void AssertSame(OriginalProgram p, KeyStateTable port, string context)
    {
        for (int k = 0; k < KeyStateTable.Size; k++)
            Assert.True(p.ReadByte((ushort)(KeyStateTable.TableOffset + k)) == port.State[k], $"{context}: key {k:X2}");
        Assert.True(p.ReadByte(KeyStateTable.LastMakeOffset) == port.LastMake, $"{context}: last make");
        Assert.True(p.ReadByte(KeyStateTable.MakeCountOffset) == port.MakeCount, $"{context}: make count");
    }

    private static KeyStateTable FromOriginal(OriginalProgram p)
    {
        var t = new KeyStateTable();
        var state = new byte[KeyStateTable.Size];
        for (int k = 0; k < state.Length; k++) state[k] = p.ReadByte((ushort)(KeyStateTable.TableOffset + k));
        t.Load(state, p.ReadByte(KeyStateTable.LastMakeOffset), p.ReadByte(KeyStateTable.MakeCountOffset));
        return t;
    }

    /// <summary>
    /// Bytes the main menu does not act on (modifiers, lock keys, unassigned codes, the E0h prefix), as makes and breaks.
    /// Keys the menu reacts to would leave the menu and are covered by the probes and the 0E9E test below.
    /// </summary>
    private static readonly byte[] Pool =
        [0x1D, 0x2A, 0x36, 0x38, 0x3A, 0x45, 0x46, .. Enumerable.Range(0x54, 0x2C).Select(i => (byte)i)];

    // [DIFFERENTIAL] 27C6:0043
    [GameDataFact]
    public void Random_scancode_stream_gives_the_same_table_last_make_and_count()
    {
        var p = OriginalProgram.BootToMainMenu(TestGameData.DosDir);
        var m = p.Machine;
        var port = FromOriginal(p);
        var rnd = new Random(4711);
        const double spacing = 0.015;
        for (int i = 0; i < 300; i++)
        {
            byte b = rnd.Next(10) switch
            {
                0 => 0xE0,
                < 5 => Pool[rnd.Next(Pool.Length)],
                _ => (byte)(Pool[rnd.Next(Pool.Length)] | 0x80),
            };
            if (i < 3) b = (byte)(i == 0 ? 0x7F : i == 1 ? 0xFF : 0x00); // edge codes: highest make, its break, code 00h
            m.ScheduleScancode(m.Seconds + spacing / 2, b);
            m.RunFor(spacing);
            port.Receive(b);
            AssertSame(p, port, $"byte {i} ({b:X2})");
        }
        Assert.True(port.MakeCount != 0 || port.LastMake != 0);
    }

    // [DIFFERENTIAL] 27C6:00AC
    [GameDataFact]
    public void Clear_releases_every_key_like_the_original()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        var rnd = new Random(7);
        var state = new byte[KeyStateTable.Size];
        for (int k = 0; k < state.Length; k++) state[k] = (byte)rnd.Next(2);
        for (int k = 0; k < state.Length; k++) p.WriteByte((ushort)(KeyStateTable.TableOffset + k), state[k]);
        p.WriteByte(KeyStateTable.LastMakeOffset, 0x33);
        p.WriteByte(KeyStateTable.MakeCountOffset, 0x44);
        ushort around0 = p.ReadWord(KeyStateTable.TableOffset - 2), around1 = p.ReadWord(KeyStateTable.TableOffset + KeyStateTable.Size);
        var port = new KeyStateTable();
        port.Load(state, 0x33, 0x44);

        p.Call(0x27C6, 0x00AC);
        port.Clear();

        AssertSame(p, port, "after clear");
        Assert.Equal(around0, p.ReadWord(KeyStateTable.TableOffset - 2));
        Assert.Equal(around1, p.ReadWord(KeyStateTable.TableOffset + KeyStateTable.Size));
    }

    public static TheoryData<byte, Keys[], byte> ControlCases => new()
    {
        { 4, [Keys.Up], 0x04 },
        { 4, [Keys.Down, Keys.Right], 0x09 },
        { 4, [Keys.Left, Keys.Insert], 0x12 },
        { 4, [Keys.NumPad8, Keys.NumPad0], 0x14 },   // keypad shares the codes of the cursor block
        { 4, [Keys.Q, Keys.Space], 0x00 },
        { 3, [Keys.Q, Keys.Z, Keys.Space], 0x16 },
        { 3, [Keys.A, Keys.X], 0x09 },
        { 3, [Keys.Up, Keys.Insert], 0x00 },
    };

    /// <summary>
    /// End to end: host keys → scancode bytes → host table → data segment of the original → 0E9E:0035. The original
    /// reads the same key bytes as it would after a real keyboard.
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(ControlCases))]
    public void Host_keys_reach_the_original_control_reader(byte type, Keys[] keys, byte expectedBits)
    {
        var edges = new KeyboardEdges();
        var bytes = new List<byte>();
        edges.Update(keys, bytes);
        var table = new KeyStateTable();
        table.Receive(bytes.ToArray());

        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        var ds = p.DataSegment();
        table.CopyTo(ds);
        for (int k = 0; k < KeyStateTable.Size; k++) p.WriteByte((ushort)(KeyStateTable.TableOffset + k), ds[KeyStateTable.TableOffset + k]);
        p.WriteByte(0x0840, 0);
        p.WriteByte(0x083C, type);
        p.Call(0x0E9E, 0x0035);
        Assert.Equal(expectedBits, p.ReadByte(0x37C3));
    }
}
