using Traders.Core.Input;
using Traders.Core.State;
using C = Traders.Core.Input.ControlReader;

namespace Traders.Tests.Input;

/// <summary>
/// Differential tests of the control reader 0E9E and the device routines it calls (finding docs/routines/0E9E_000E_steuerung.md):
/// the same data segment and the same device values go into the original (RefEmu, LoadOnly, direct calls) and the port; the
/// whole data segment, the random state and the device accesses must be equal afterwards. Joystick, mouse and parallel
/// adapter come from the RefEmu device models (port 201h, int 33h, LPT1) set per case.
/// </summary>
public class ControlReaderDifferentialTests
{
    private const ushort Seg = 0x0E9E;

    private static readonly byte[] RelevantKeys = [0x2C, 0x2D, 0x10, 0x1E, 0x39, 0x48, 0x4B, 0x4D, 0x50, 0x52, 0x7E, 0x0E, 0x1C];
    private static readonly uint[] Delays = [0, 0, 1, 2, 5, 100_000, 0x10000, 0xFFFF_FFFF, 0x8000_0000, 0x7FFF_FFFF];
    private static readonly short[] Motions = [0, 0, 1, -1, 2, -2, 3, -3, 40, -40, 32767, -32768, 7, -7];

    /// <summary>Random but valid case: every field the routines read, and devices for all six control types.</summary>
    private static DeviceSetup Arrange(ControlHarness h, Random r, bool playerIndexInRange = false)
    {
        h.Reset();
        byte[] types = [0, 1, 2, 3, 4, 5, 0, 1, 2, 5, 6, 0xFF, 0x80];
        for (int i = 0; i < 4; i++)
        {
            h.WriteByte(C.IsComputer + i, (byte)(r.Next(5) switch { 0 or 1 => 0, 2 or 3 => 1, _ => 2 }));
            h.WriteByte(C.ControlType + i, types[r.Next(types.Length)]);
            h.WriteByte(C.Bits + i, (byte)r.Next(256));
            h.WriteDword(C.MoveDelay + 4 * i, Delays[r.Next(Delays.Length)]);
            h.WriteDword(C.FireDelay + 4 * i, Delays[r.Next(Delays.Length)]);
            foreach (int f in new[] { C.FlagRight, C.FlagLeft, C.FlagUp, C.FlagDown, C.FlagFire, C.Action })
                h.WriteByte(f + i, (byte)r.Next(256));
        }
        for (int k = 0; k < 0x80; k++) h.WriteByte(C.KeyTable + k, 0);
        foreach (byte k in RelevantKeys)
            if (r.Next(3) == 0) h.WriteByte(C.KeyTable + k, r.Next(4) == 0 ? (byte)0x80 : (byte)1);
        h.WriteWord(C.PlayerIndex, playerIndexInRange ? (ushort)r.Next(4) : (ushort)r.Next(65536));
        h.WriteWord(C.MouseDriver, r.Next(3) switch { 0 => 0, 1 => 0xFFFF, _ => 1 });
        // Calibration of both sticks (27C6:00E6 never writes a step of 0).
        foreach (int cal in new[] { JoystickReader.Stick1, JoystickReader.Stick2 })
        {
            h.WriteWord(cal, (ushort)r.Next(0, 3000));
            h.WriteWord(cal + 2, (ushort)r.Next(0, 3000));
            for (int s = 4; s <= 10; s += 2) h.WriteWord(cal + s, (ushort)(r.Next(4) == 0 ? r.Next(1, 4) : r.Next(1, 400)));
        }

        var status = new byte[256];
        r.NextBytes(status);
        return new DeviceSetup
        {
            AxisReads = r.Next(5) == 0 ? null : [.. Enumerable.Range(0, 4).Select(_ => AxisCount(r))],
            JoystickButtons = (byte)(r.Next(16) << 4),
            Mouse = true,
            MouseMotions = [.. Enumerable.Range(0, r.Next(4)).Select(_ => (Motions[r.Next(Motions.Length)], Motions[r.Next(Motions.Length)]))],
            MouseButtons = (ushort)r.Next(8),
            ParallelStatus = r.Next(5) == 0 ? null : data => status[data],
        };
    }

    private static ushort AxisCount(Random r) => r.Next(6) switch
    {
        0 => 0,
        1 => (ushort)r.Next(1, 4),
        2 => 20_000,      // beyond the loop limit of 15 536
        _ => (ushort)r.Next(0, 6000),
    };

    // [DIFFERENTIAL] 0E9E:0035
    [GameDataFact]
    public void Read_bits_matches_the_original_for_all_control_types()
    {
        var h = new ControlHarness();
        var r = new Random(0x0035);
        for (int n = 0; n < 400; n++)
            h.Compare($"0035 case {n}", Seg, 0x0035, Arrange(h, r), C.ReadBits);
    }

    // [DIFFERENTIAL] 0E9E:000E
    // [DIFFERENTIAL] 0E9E:0264
    // [DIFFERENTIAL] 0E9E:0312
    [GameDataFact]
    public void Read_all_matches_the_original_with_delays_and_flags()
    {
        var h = new ControlHarness();
        var r = new Random(0x000E);
        for (int n = 0; n < 300; n++)
            h.Compare($"000E case {n}", Seg, 0x000E, Arrange(h, r), C.ReadAll);
    }

    // [DIFFERENTIAL] 0E9E:037A
    [GameDataFact]
    public void Read_actions_matches_the_original()
    {
        var h = new ControlHarness();
        var r = new Random(0x037A);
        for (int n = 0; n < 300; n++)
            h.Compare($"037A case {n}", Seg, 0x037A, Arrange(h, r), C.ReadActions);
    }

    [GameDataFact]
    public void Single_flag_routines_match_the_original_for_each_player()
    {
        var h = new ControlHarness();
        var r = new Random(0x0264);
        for (int n = 0; n < 200; n++)
        {
            h.Compare($"0264 case {n}", Seg, 0x0264, Arrange(h, r, playerIndexInRange: true), (d, _) => C.UpdateMoveFlags(d));
            h.Compare($"0312 case {n}", Seg, 0x0312, Arrange(h, r, playerIndexInRange: true), (d, _) => C.UpdateFireFlag(d));
        }
    }

    /// <summary>Repeated calls with held and released keys: delays count across calls exactly as in the original.</summary>
    [GameDataFact]
    public void Repeated_reads_keep_delays_in_step_with_the_original()
    {
        var h = new ControlHarness();
        var r = new Random(0x37A2);
        h.Reset();
        var setup = Arrange(h, r);
        for (int i = 0; i < 4; i++) h.WriteByte(C.IsComputer + i, (byte)(i == 3 ? 1 : 0));
        h.WriteByte(C.ControlType + 0, 4);
        h.WriteByte(C.ControlType + 1, 3);
        h.WriteByte(C.ControlType + 2, 5);
        h.WriteDword(C.MoveDelay + 4, 3);
        h.WriteDword(C.MoveDelay + 12, 4);
        for (int frame = 0; frame < 40; frame++)
        {
            for (int k = 0; k < 0x80; k++) h.WriteByte(C.KeyTable + k, 0);
            foreach (byte k in RelevantKeys)
                if (r.Next(2) == 0) h.WriteByte(C.KeyTable + k, 1);
            h.Compare($"frame {frame}", Seg, 0x037A, setup, C.ReadActions);
        }
    }

    // [DIFFERENTIAL] 27C6:0268
    [GameDataFact]
    public void Joystick_scaling_matches_the_original()
    {
        var h = new ControlHarness();
        var r = new Random(0x0268);
        var p = h.Program;
        for (int n = 0; n < 300; n++)
        {
            var setup = Arrange(h, r);
            setup = new DeviceSetup { AxisReads = setup.AxisReads ?? [0, 0, 0, 0], JoystickButtons = setup.JoystickButtons };
            int stick = r.Next(4) == 0 ? r.Next(0, 4) : r.Next(1, 3);
            setup.Apply(p);
            var ds = new Dgroup(p.DataSegment());
            var reading = setup.ForPort().ReadJoystick(stick == 1 ? 1 : 2);
            var (x, y, buttons) = JoystickReader.Read(ds, stick, reading);

            p.Machine.Mem.Write16(0x9000, 0xF000, 0xAAAA);
            var (ax, _) = p.Call(0x27C6, 0x0268, (ushort)stick, 0xF000, 0x9000);
            string label = $"0268 case {n} stick {stick}";
            Assert.True((byte)x == p.Machine.Mem.Read8(0x9000, 0xF000), $"{label}: x");
            Assert.True((byte)y == p.Machine.Mem.Read8(0x9000, 0xF001), $"{label}: y");
            Assert.True(buttons == ax, $"{label}: buttons {buttons:X} vs {ax:X}");
            ControlHarness.AssertSameData(label, p.DataSegment(), ds.Bytes);
        }
    }

    // [DIFFERENTIAL] 0D73:07E0
    [GameDataFact]
    public void Mouse_direction_matches_the_original()
    {
        var h = new ControlHarness();
        var p = h.Program;
        var cases = new List<(short X, short Y, ushort B, ushort Driver)>();
        foreach (short x in Motions.Distinct())
            foreach (short y in Motions.Distinct())
                cases.Add((x, y, (ushort)((x + 3 * y) & 3), 0xFFFF));
        cases.Add((5, 0, 3, 0));
        cases.Add((0, 0, 1, 1));
        foreach (var (x, y, b, driver) in cases)
        {
            h.Reset();
            h.WriteWord(C.MouseDriver, driver);
            var setup = new DeviceSetup { Mouse = true, MouseMotions = [(x, y)], MouseButtons = b };
            setup.Apply(p);
            var ds = new Dgroup(p.DataSegment());
            var dev = setup.ForPort();
            ushort expectedPort = MouseReader.ReadDirection(ds, dev);
            var (ax, _) = p.Call(0x0D73, 0x07E0);
            Assert.True(ax == expectedPort, $"mouse ({x},{y}) buttons {b} driver {driver}: original {ax:X}, port {expectedPort:X}");
            Assert.Equal(p.Machine.Devices.MouseCalls.GetValueOrDefault((ushort)0x0B), dev.MotionReads);
            ControlHarness.AssertSameData($"mouse ({x},{y})", p.DataSegment(), ds.Bytes);
        }
    }

    [GameDataFact]
    public void Two_mouse_players_consume_the_motion_in_player_order()
    {
        var h = new ControlHarness();
        h.Reset();
        for (int i = 0; i < 4; i++)
        {
            h.WriteByte(C.IsComputer + i, (byte)(i < 2 ? 0 : 1));
            h.WriteByte(C.ControlType + i, 2);
        }
        h.WriteWord(C.MouseDriver, 0xFFFF);
        var setup = new DeviceSetup { Mouse = true, MouseMotions = [(10, 0)], MouseButtons = 1 };
        h.Compare("two mice", Seg, 0x0035, setup, C.ReadBits);
        Assert.Equal((byte)(ControlBits.Right | ControlBits.Fire), h.Program.ReadByte(C.Bits));
        Assert.Equal((byte)ControlBits.Fire, h.Program.ReadByte(C.Bits + 1));
    }

    [GameDataFact]
    public void Parallel_adapter_selects_each_player_line()
    {
        var h = new ControlHarness();
        h.Reset();
        for (int i = 0; i < 4; i++)
        {
            h.WriteByte(C.IsComputer + i, 0);
            h.WriteByte(C.ControlType + i, 5);
        }
        // Player 2 holds up and fire (status bits 6 and 3 low), everybody else idle (bits 3..6 high, 7 low).
        var setup = new DeviceSetup { ParallelStatus = data => data == 0xFB ? (byte)0x30 : (byte)0x78 };
        h.Compare("lpt", Seg, 0x0035, setup, C.ReadBits);
        Assert.Equal([0xFE, 0xFD, 0xFB, 0xF7], h.Program.Machine.Devices.ParallelWrites);
        Assert.Equal((byte)(ControlBits.Up | ControlBits.Fire), h.Program.ReadByte(C.Bits + 2));
        Assert.Equal(0, h.Program.ReadByte(C.Bits));
    }

    [GameDataFact]
    public void Result_reports_the_state_the_original_leaves()
    {
        var h = new ControlHarness();
        h.Reset();
        for (int i = 0; i < 4; i++) h.WriteByte(C.IsComputer + i, (byte)(i == 1 ? 0 : 1));
        h.WriteByte(C.ControlType + 1, 4);
        h.WriteByte(C.KeyTable + 0x48, 1);
        h.WriteByte(C.KeyTable + 0x4D, 1);
        var setup = new DeviceSetup();
        h.Compare("result", Seg, 0x037A, setup, C.ReadActions);
        var ds = new Dgroup(h.Program.DataSegment());
        Assert.Equal(new PlayerControl(1, ControlBits.Up | ControlBits.Right, true, false, true, false, false, ControlAction.Up),
            C.Result(ds, 1));
        Assert.Equal(ControlAction.None, C.Result(ds, 0).Action);
    }
}
