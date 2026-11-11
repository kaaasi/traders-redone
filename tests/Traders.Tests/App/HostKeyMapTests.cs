using Microsoft.Xna.Framework.Input;
using Traders.App.Input;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.App;

/// <summary>Host key map and edge detection (no original routine involved; set-1 codes as in the RefEmu input scripts).</summary>
public class HostKeyMapTests
{
    [Fact]
    public void Keys_used_by_the_original_map_to_its_scancodes()
    {
        Assert.Equal([Scancodes.Enter], HostKeyMap.Bytes(Keys.Enter, true));
        Assert.Equal([Scancodes.Escape], HostKeyMap.Bytes(Keys.Escape, true));
        Assert.Equal([Scancodes.Space], HostKeyMap.Bytes(Keys.Space, true));
        Assert.Equal([(byte)(Scancodes.Space | 0x80)], HostKeyMap.Bytes(Keys.Space, false));
        for (int n = 1; n <= 10; n++) Assert.Equal([Scancodes.F(n)], HostKeyMap.Bytes(Keys.F1 + (n - 1), true));
        for (int d = 0; d <= 9; d++) Assert.Equal([Scancodes.Digit(d)], HostKeyMap.Bytes(Keys.D0 + d, true));
        foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ") Assert.Equal([Scancodes.Letter(c)], HostKeyMap.Bytes(Keys.A + (c - 'A'), true));
    }

    [Fact]
    public void Cursor_block_is_extended_and_shares_the_keypad_codes()
    {
        Assert.Equal([0xE0, Scancodes.Up], HostKeyMap.Bytes(Keys.Up, true));
        Assert.Equal([0xE0, (byte)(Scancodes.Up | 0x80)], HostKeyMap.Bytes(Keys.Up, false));
        Assert.Equal([Scancodes.Up], HostKeyMap.Bytes(Keys.NumPad8, true));
        Assert.Equal([0xE0, 0x52], HostKeyMap.Bytes(Keys.Insert, true));
        Assert.Equal([0x52], HostKeyMap.Bytes(Keys.NumPad0, true));
        Assert.Equal([0xE0, 0x1D], HostKeyMap.Bytes(Keys.RightControl, true));
        Assert.Empty(HostKeyMap.Bytes(Keys.LeftWindows, true));
    }

    [Fact]
    public void Every_mapped_key_has_a_make_code_below_80h_and_a_unique_code_extended_pair()
    {
        var seen = new HashSet<(byte, bool)>();
        foreach (var k in HostKeyMap.MappedKeys)
        {
            Assert.True(HostKeyMap.TryGetScancode(k, out byte code, out bool ext));
            Assert.InRange(code, 0x01, 0x7F);
            Assert.True(seen.Add((code, ext)), $"{k}: {code:X2} twice");
        }
    }

    [Fact]
    public void Edges_send_breaks_before_makes_and_nothing_for_held_keys()
    {
        var e = new KeyboardEdges();
        var b = new List<byte>();
        e.Update([Keys.A, Keys.Up, Keys.LeftWindows], b);
        Assert.Equal([0xE0, 0x48, 0x1E], b); // ordered by key number: Up (38) before A (65)
        b.Clear();
        e.Update([Keys.A, Keys.Up], b);
        Assert.Empty(b);
        e.Update([Keys.Up, Keys.Space], b);
        Assert.Equal([0x9E, 0x39], b);
        b.Clear();
        e.ReleaseAll(b);
        Assert.Equal([0x39 | 0x80, 0xE0, 0xC8], b);   // Space (32) before Up (38)
        Assert.Empty(e.Held);
    }

    [Fact]
    public void Table_follows_held_keys_and_the_prefix_clears_key_60h()
    {
        var e = new KeyboardEdges();
        var t = new KeyStateTable();
        var b = new List<byte>();
        t.Receive(0x60);
        e.Update([Keys.Up, Keys.LeftShift], b);
        t.Receive(b.ToArray());
        Assert.True(t.IsDown(0x48) && t.IsDown(0x2A) && !t.IsDown(0x60));
        Assert.Equal(3, t.MakeCount);
        b.Clear();
        e.Update([Keys.LeftShift], b);
        t.Receive(b.ToArray());
        Assert.False(t.IsDown(0x48));
        Assert.Equal(0x2A, t.LastMake); // Left Shift (160) came after Up (38)
    }

    [Fact]
    public void Copy_into_a_data_segment_uses_the_original_offsets()
    {
        var t = new KeyStateTable();
        t.Receive([0x1C, 0x39, 0x9C]);
        var ds = new byte[0x10000];
        t.CopyTo(ds);
        Assert.Equal(0, ds[0x3D47 + 0x1C]);
        Assert.Equal(1, ds[0x3D47 + 0x39]);
        Assert.Equal(0x39, ds[0x1158]);
        Assert.Equal(2, ds[0x115D]);
        Assert.Throws<ArgumentException>(() => t.Load(new byte[10], 0, 0));
    }
}
