using Traders.RefEmu;

namespace Traders.Tests.App;

/// <summary>
/// RefEmu probes for the control reader 0E9E (finding docs/routines/0E9E_000E_steuerung.md): control type per player,
/// direction and fire bits, repeat counters and the resulting action code. The routines only read DS state (key-state
/// table, player and control type), so they are called directly on the loaded image. Expected values come from the
/// disassembly reading in the finding; the port into the core follows with the phase machine (R4).
/// </summary>
public class OriginalControlProbeTests
{
    private const ushort Seg = 0x0E9E;
    private const ushort ReadAll = 0x000E, ReadBits = 0x0035, Actions = 0x037A;

    private const ushort KeyTable = 0x3D47;
    private const ushort ControlType = 0x083C, IsComputer = 0x0840, Bits = 0x37C3, Action = 0x3308;
    private const ushort MoveDelay = 0x3786, FireDelay = 0x302E; // 32-bit counters, 4 bytes per player
    private const ushort MouseInstalled = 0x2BE6, PlayerIndex = 0x2C10;
    private const ushort Unused7E = KeyTable + 0x7E;

    // Set-1 make codes read by control types 3 and 4.
    private const byte Q = 0x10, A = 0x1E, Z = 0x2C, X = 0x2D, Space = 0x39;
    private const byte Up = 0x48, Left = 0x4B, Right = 0x4D, Down = 0x50, Ins = 0x52;

    private const byte BitRight = 0x01, BitLeft = 0x02, BitUp = 0x04, BitDown = 0x08, BitFire = 0x10;

    private static OriginalProgram Load(int player, byte type)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        for (int i = 0; i < 4; i++) p.WriteByte((ushort)(IsComputer + i), 1);
        p.WriteByte((ushort)(IsComputer + player), 0);
        p.WriteByte((ushort)(ControlType + player), type);
        return p;
    }

    private static void Hold(OriginalProgram p, params byte[] keys)
    {
        for (int k = 0; k < 0x80; k++) p.WriteByte((ushort)(KeyTable + k), 0);
        foreach (byte k in keys) p.WriteByte((ushort)(KeyTable + k), 1);
    }

    public static TheoryData<byte, byte[], byte> KeyCases => new()
    {
        { 4, [], 0 },
        { 4, [Right], BitRight },
        { 4, [Left], BitLeft },
        { 4, [Right, Left], BitRight },            // right before left
        { 4, [Up], BitUp },
        { 4, [Down], BitDown },
        { 4, [Up, Down], BitUp },                  // up before down
        { 4, [Ins], BitFire },
        { 4, [Up, Left, Ins], BitUp | BitLeft | BitFire },
        { 4, [Q, A, Z, X, Space], 0 },             // keys of type 3 are ignored
        { 3, [X], BitRight },
        { 3, [Z], BitLeft },
        { 3, [X, Z], BitRight },
        { 3, [Q], BitUp },
        { 3, [A], BitDown },
        { 3, [Q, A], BitUp },
        { 3, [Space], BitFire },
        { 3, [A, Z, Space], BitDown | BitLeft | BitFire },
        { 3, [Up, Down, Left, Right, Ins], 0 },    // keys of type 4 are ignored
        { 2, [], 0 },                              // mouse without driver (2BE6h = 0)
        { 2, [Ins, Space], 0 },
        { 2, [0x7E], BitFire },                    // the mouse type also fires with key-state byte 7Eh
    };

    [GameDataTheory]
    [MemberData(nameof(KeyCases))]
    public void Key_types_map_the_key_state_table_to_direction_and_fire_bits(byte type, byte[] keys, byte expected)
    {
        for (int player = 0; player < 4; player++)
        {
            var p = Load(player, type);
            Hold(p, keys);
            for (int i = 0; i < 4; i++) p.WriteByte((ushort)(Bits + i), 0xAA);
            p.Call(Seg, ReadBits);
            for (int i = 0; i < 4; i++)
                Assert.Equal(i == player ? expected : (byte)0xAA, p.ReadByte((ushort)(Bits + i))); // computer players untouched
        }
    }

    [GameDataFact]
    public void Mouse_type_is_idle_without_a_mouse_driver()
    {
        var p = Load(0, 2);
        Assert.Equal(0, p.ReadWord(MouseInstalled));
        Hold(p);
        p.Call(Seg, ReadBits);
        Assert.Equal(0, p.ReadByte(Bits));
        Assert.Equal(0, p.ReadByte(Unused7E));
    }

    [GameDataFact]
    public void Parallel_port_type_reads_down_from_an_absent_adapter()
    {
        // Type 5 writes ~(1 << player) to the LPT1 base from 0040:0008 and reads the status port base+1. Without an
        // adapter (status FFh: busy bit set, all other inputs high) the reader reports "down" only.
        var p = Load(1, 5);
        p.Machine.Mem.Write16(0x40, 0x08, 0x0378);
        Hold(p);
        p.Call(Seg, ReadBits);
        Assert.Equal(BitDown, p.ReadByte(Bits + 1));
    }

    [GameDataFact]
    public void Initial_control_types_are_cursor_keys_letter_keys_and_both_joysticks()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        Assert.Equal([4, 3, 0, 1], Enumerable.Range(0, 4).Select(i => p.ReadByte((ushort)(ControlType + i))).ToArray());
    }

    private static uint ReadDword(OriginalProgram p, ushort table, int player) => p.ReadDword((ushort)(table + 4 * player));

    private static sbyte RunActions(OriginalProgram p, int player)
    {
        p.Call(Seg, Actions);
        return (sbyte)p.ReadByte((ushort)(Action + player));
    }

    public static TheoryData<byte[], sbyte> ActionCases => new()
    {
        { [], -1 },
        { [Right], 0 },
        { [Left], 1 },
        { [Up], 2 },
        { [Down], 3 },
        { [Up, Right], 2 },     // up before down before right before left
        { [Down, Left], 3 },
        { [Ins], 10 },
        { [Ins, Down, Right], 10 },
    };

    [GameDataTheory]
    [MemberData(nameof(ActionCases))]
    public void Action_code_follows_the_priority_fire_up_down_right_left(byte[] keys, sbyte expected)
    {
        var p = Load(2, 4);
        Hold(p, keys);
        Assert.Equal(expected, RunActions(p, 2));
        for (int i = 0; i < 4; i++)
            if (i != 2) Assert.Equal(-1, (sbyte)p.ReadByte((ushort)(Action + i)));
        Assert.Equal(4, p.ReadWord(PlayerIndex)); // loop variable of 000E stays behind in DS
    }

    [GameDataFact]
    public void Fire_arms_a_long_delay_that_suppresses_held_fire_until_released()
    {
        var p = Load(0, 4);
        Hold(p, Ins, Up);
        Assert.Equal(10, RunActions(p, 0));
        Assert.Equal(100_000u, ReadDword(p, FireDelay, 0));

        Assert.Equal(2, RunActions(p, 0)); // fire still held: counted down, only the direction remains
        Assert.Equal(99_999u, ReadDword(p, FireDelay, 0));

        Hold(p, Up);
        Assert.Equal(2, RunActions(p, 0)); // released: counter cleared
        Assert.Equal(0u, ReadDword(p, FireDelay, 0));

        Hold(p, Ins);
        Assert.Equal(10, RunActions(p, 0));
    }

    [GameDataFact]
    public void Move_delay_suppresses_directions_and_is_cleared_when_idle()
    {
        var p = Load(1, 3);
        p.WriteDword((ushort)(MoveDelay + 4), 2);
        Hold(p, Q);
        Assert.Equal(-1, RunActions(p, 1));
        Assert.Equal(1u, ReadDword(p, MoveDelay, 1));
        Assert.Equal(-1, RunActions(p, 1));
        Assert.Equal(0u, ReadDword(p, MoveDelay, 1));
        Assert.Equal(2, RunActions(p, 1));

        p.WriteDword((ushort)(MoveDelay + 4), 5);
        Hold(p, Space);                               // no direction held: a human player's move delay is cleared
        Assert.Equal(10, RunActions(p, 1));
        Assert.Equal(0u, ReadDword(p, MoveDelay, 1));
    }

    [GameDataFact]
    public void Negative_move_delay_is_not_counted_down()
    {
        var p = Load(0, 4);
        p.WriteDword(MoveDelay, 0xFFFF_FFFF);
        Hold(p, Right);
        Assert.Equal(0, RunActions(p, 0));
        Assert.Equal(0xFFFF_FFFFu, ReadDword(p, MoveDelay, 0));
    }

    private const ushort FlagRight = 0x44BE, FlagLeft = 0x43F0, FlagUp = 0x439D, FlagDown = 0x44BA, FlagFire = 0x4894;

    private static byte[] Flags(OriginalProgram p, int player) =>
        [.. new[] { FlagRight, FlagLeft, FlagUp, FlagDown, FlagFire }.Select(f => p.ReadByte((ushort)(f + player)))];

    [GameDataFact]
    public void Computer_bits_are_taken_as_written_and_turned_into_flags()
    {
        // 0035 leaves the bits of computer players alone (their routines write them), 0264/0312 treat them like input.
        var p = Load(0, 4);
        Hold(p, Up, Ins);
        p.WriteByte(Bits + 3, BitDown | BitLeft | BitFire);
        p.Call(Seg, ReadAll);
        Assert.Equal(BitDown | BitLeft | BitFire, p.ReadByte(Bits + 3));
        Assert.Equal([0, 1, 0, 1, 1], Flags(p, 3));
        Assert.Equal([0, 0, 1, 0, 1], Flags(p, 0));
    }

    [GameDataFact]
    public void Computer_move_delay_counts_down_even_without_bits_and_fire_delay_is_cleared()
    {
        var p = Load(0, 4);
        p.WriteByte(Bits + 3, 0);
        p.WriteDword(MoveDelay + 12, 7);
        p.WriteDword(FireDelay + 12, 9);
        p.Call(Seg, ReadAll);
        Assert.Equal(6u, ReadDword(p, MoveDelay, 3)); // a human player's counter would be cleared here
        Assert.Equal(0u, ReadDword(p, FireDelay, 3));

        p.WriteByte(Bits + 3, BitUp | BitFire);
        p.WriteDword(FireDelay + 12, 9);
        p.Call(Seg, ReadAll);
        Assert.Equal(5u, ReadDword(p, MoveDelay, 3));
        Assert.Equal(8u, ReadDword(p, FireDelay, 3));
        Assert.Equal([0, 0, 0, 0, 0], Flags(p, 3));
    }
}
