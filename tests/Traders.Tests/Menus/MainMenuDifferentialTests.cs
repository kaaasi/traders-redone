using Traders.Core.Menus;
using Traders.Core.State;

namespace Traders.Tests.Menus;

// Routines covered by the scenarios of Main_menu_192D_0007_matches_original:
// [DIFFERENTIAL] 192D:0058 (through 192D:0007)
// [DIFFERENTIAL] 192D:00C5 (through 192D:0007)
// [DIFFERENTIAL] 192D:0234 (through 192D:0007)
// [DIFFERENTIAL] 192D:0376 (through 192D:0007)
// [DIFFERENTIAL] 192D:03E3 (through 192D:0007)
// [DIFFERENTIAL] 192D:040E (through 192D:0007)
// [DIFFERENTIAL] 192D:0495 (through 192D:0007)
// [DIFFERENTIAL] 192D:0592 (through 192D:0007)
// [DIFFERENTIAL] 192D:05DD (through 192D:0007)
// [DIFFERENTIAL] 192D:0698 (through 192D:0007)
// [DIFFERENTIAL] 192D:0A05 (through 192D:0007)
// [DIFFERENTIAL] 192D:0AB4 (through 192D:0007)
// [DIFFERENTIAL] 192D:0C08 (through 192D:0007)
// [DIFFERENTIAL] 192D:0D10 (through 192D:0007)
// [DIFFERENTIAL] 192D:0F96 (through 192D:0007)
// [DIFFERENTIAL] 192D:10F8 (through 192D:0007)
// [DIFFERENTIAL] 192D:1263 (through 192D:0007)
// [DIFFERENTIAL] 192D:12A6 (through 192D:0007)
// [DIFFERENTIAL] 192D:131B (through 192D:0007)
// [DIFFERENTIAL] 192D:138F (through 192D:0007)
// [DIFFERENTIAL] 192D:14B4 (through 192D:0007)
// [DIFFERENTIAL] 192D:14FB (through 192D:0007)
// [DIFFERENTIAL] 192D:16AF (through 192D:0007)
// [DIFFERENTIAL] 192D:197F (through 192D:0007)
// [DIFFERENTIAL] 192D:1A04 (through 192D:0007)
// [DIFFERENTIAL] 192D:1B90 (through 192D:0007)
// [DIFFERENTIAL] 21FD:0245 (through 192D:0007)

/// <summary>
/// [DIFFERENTIAL] 192D:0007 – main menu with all sub-menus against the original (stubbed layers, see <see cref="MenuHarness"/>):
/// whole data segment, frame count, random() per frame, sound calls and the backdrop draw calls.
/// </summary>
public sealed class MainMenuDifferentialTests
{
    private static MenuHarness? _harness;
    private static Dgroup? _withTexts;
    private static readonly object Lock = new();

    private static (MenuHarness Harness, Dgroup Base) Setup()
    {
        lock (Lock)
        {
            _harness ??= new MenuHarness();
            _withTexts ??= _harness.WithTexts(_harness.Initial());
            return (_harness, new Dgroup((byte[])_withTexts.Bytes.Clone()));
        }
    }

    private const long Start = 60, Gap = 90, Hold = 3;

    public static TheoryData<string> Scenarios => new(ScenarioTable.Keys);

    private static readonly Dictionary<string, Func<Dgroup, (KeyScript Keys, bool[] Loads, bool[] Saves, bool[] Calibrations)>>
        ScenarioTable = new()
        {
            ["enter"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.Enter), [], [], []),
            ["quit"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.Q), [], [], []),
            ["quit-and-enter-same-frame"] = _ => (new KeyScript().Hold(Start, 5, MenuKeys.Q).Hold(Start, 8, MenuKeys.Enter), [], [], []),
            ["players-and-controls"] = d =>
            {
                d.Set(DgroupLayout.ControlDevice + 0, (sbyte)3);
                d.Set(DgroupLayout.ControlDevice + 1, (sbyte)5);
                return (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F1, MenuKeys.F2, MenuKeys.F3, MenuKeys.F3, MenuKeys.F4,
                    MenuKeys.F4, MenuKeys.Enter, MenuKeys.F1, MenuKeys.F2, MenuKeys.F2, MenuKeys.F3, MenuKeys.Enter), [], [], []);
            },
            ["controls-joystick-calibration"] = d =>
            {
                d.Set(DgroupLayout.ControlDevice + 0, (sbyte)5);
                d.Set(DgroupLayout.ControlDevice + 1, (sbyte)1);
                // player 1 → device 0 (joystick 1), player 2 device 1 (joystick 2); the first run fails in step 1, the second
                // fails in step 0 of player 2, the third passes
                return (KeyScript.Sequence(Start, 2 * Gap, Hold, MenuKeys.F1, MenuKeys.F2, MenuKeys.Enter, MenuKeys.F1, MenuKeys.Enter,
                    MenuKeys.Enter, MenuKeys.Enter), [], [], [false, true, false, false, false, true, false, false, false, false, false, false]);
            },
            ["level-menu"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F6, MenuKeys.F1, MenuKeys.F5, MenuKeys.F1,
                MenuKeys.F1, MenuKeys.F5, MenuKeys.F6, MenuKeys.F7, MenuKeys.F8, MenuKeys.F9, MenuKeys.F10, MenuKeys.F5, MenuKeys.F5,
                MenuKeys.F10, MenuKeys.F10, MenuKeys.F10, MenuKeys.F1, MenuKeys.Enter, MenuKeys.F6, MenuKeys.F1, MenuKeys.F1,
                MenuKeys.F5, MenuKeys.Enter, MenuKeys.Enter), [], [], []),
            ["settings-menu"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F7, MenuKeys.F1, MenuKeys.F1, MenuKeys.F1,
                MenuKeys.F1, MenuKeys.F2, MenuKeys.F2, MenuKeys.F2, MenuKeys.F2, MenuKeys.F3, MenuKeys.F3, MenuKeys.F4, MenuKeys.F4,
                MenuKeys.F4, MenuKeys.F5, MenuKeys.F6, MenuKeys.F6, MenuKeys.F6, MenuKeys.F6, MenuKeys.Left, MenuKeys.Left,
                MenuKeys.Left, MenuKeys.Left, MenuKeys.Left, MenuKeys.Right, MenuKeys.Up, MenuKeys.Up, MenuKeys.Up, MenuKeys.Up,
                MenuKeys.Down, MenuKeys.S, MenuKeys.S, MenuKeys.S, MenuKeys.Enter, MenuKeys.Enter), [], [], []),
            ["settings-then-quit"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F7, MenuKeys.Right, MenuKeys.Down,
                MenuKeys.Enter, MenuKeys.Q), [], [], []),
            ["disk-menu-continue"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F8, MenuKeys.S, MenuKeys.L, MenuKeys.L,
                MenuKeys.S, MenuKeys.S, MenuKeys.Enter, MenuKeys.Enter, MenuKeys.V), [false, true], [true, false], []),
            ["settings-new-game"] = d =>
            {
                d.Set(MenuLayout.GameInProgress, (byte)1);
                return (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F7, MenuKeys.Left, MenuKeys.Enter, MenuKeys.Enter,
                    MenuKeys.N, MenuKeys.A), [], [], []);
            },
            ["continue-keys-o"] = d =>
            {
                d.Set(MenuLayout.GameInProgress, (byte)1);
                return (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.Enter, MenuKeys.O), [], [], []);
            },
            ["info-and-pause"] = _ => (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F9, MenuKeys.Enter, MenuKeys.P, MenuKeys.F1,
                MenuKeys.P, MenuKeys.F1, MenuKeys.Enter, MenuKeys.Enter), [], [], []),
            ["slow-flips-and-long-holds"] = _ => (new KeyScript().Hold(Start, 40, MenuKeys.KeypadMinus).Hold(Start + 10, 25, MenuKeys.F1)
                .Hold(Start + 50, 1, MenuKeys.F2).Hold(Start + 51, 1, MenuKeys.F2).Hold(Start + 60, 30, MenuKeys.F6)
                .Hold(Start + 200, 2, MenuKeys.Enter).Hold(Start + 300, 2, MenuKeys.Enter).Hold(Start + 400, 2, MenuKeys.Enter),
                [], [], []),
            ["fast-flips"] = _ => (new KeyScript().Hold(Start, 60, MenuKeys.KeypadPlus).Hold(Start + 10, 3, MenuKeys.F1)
                .Hold(Start + 20, 3, MenuKeys.F2).Hold(Start + 100, 2, MenuKeys.Enter).Hold(Start + 200, 2, MenuKeys.Enter), [], [], []),
            ["custom-level-start"] = d =>
            {
                d.Set(DgroupLayout.Level, (short)-1);
                d.Set(DgroupLayout.ComputerIq, (short)3);
                d.Set(DgroupLayout.PiratePresence, (short)3);
                return (KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F6, MenuKeys.F5, MenuKeys.F10, MenuKeys.Enter,
                    MenuKeys.Enter), [], [], []);
            },
        };

    /// <summary>What a scenario must reach (guards the scripts; the values themselves are compared with the original).</summary>
    private static readonly Dictionary<string, (byte Al, byte Choice, int Calibrations)> Expected = new()
    {
        ["enter"] = (1, 1, 0),
        ["quit"] = (0, 0, 0),
        ["quit-and-enter-same-frame"] = (1, 1, 0), // Q first, RETURN still held after the release of Q overrides it
        ["controls-joystick-calibration"] = (1, 1, 12),
        ["disk-menu-continue"] = (1, 2, 0),
        ["settings-new-game"] = (1, 1, 0),
        ["continue-keys-o"] = (1, 2, 0),
        ["settings-then-quit"] = (0, 0, 0),
    };

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Main_menu_192D_0007_matches_original(string scenario)
    {
        var (h, d) = Setup();
        foreach (uint seed in new uint[] { 1, 0x12345678, 0xDEADBEEF })
        {
            var input = new Dgroup((byte[])d.Bytes.Clone());
            input.Set(DgroupLayout.RandomSeed, seed);
            var (keys, loads, saves, calibrations) = ScenarioTable[scenario](input);
            string label = $"{scenario} seed {seed:X8}";

            h.LoadResults.Clear();
            h.SaveResults.Clear();
            h.CalibrationFailures.Clear();
            foreach (bool b in loads) h.LoadResults.Enqueue(b);
            foreach (bool b in saves) h.SaveResults.Enqueue(b);
            foreach (bool b in calibrations) h.CalibrationFailures.Enqueue(b);
            var original = h.RunOriginal(input, 0x0007, keys.At);

            var host = new ScriptedMenuHost();
            foreach (bool b in loads) host.LoadResults.Enqueue(b);
            foreach (bool b in saves) host.SaveResults.Enqueue(b);
            foreach (bool b in calibrations) host.CalibrationFailures.Enqueue(b);
            var port = MenuHarness.RunPort(input, keys.At, host);

            Assert.True((original.Ax & 0xFF) == port.Runner.Menu.Result, $"{label}: AL original {original.Ax & 0xFF}, port {port.Runner.Menu.Result}");
            if (Expected.TryGetValue(scenario, out var expected))
            {
                Assert.Equal(expected.Al, port.Runner.Menu.Result);
                if (expected.Al == 1) Assert.Equal(expected.Choice, port.State.U8(MenuLayout.Choice));
                Assert.Equal(expected.Calibrations, port.Runner.Menu.Events.Count(e => e.Kind == MenuEventKind.Calibration));
            }
            Assert.True(original.Frames == port.Runner.LastFrame, $"{label}: frames original {original.Frames}, port {port.Runner.LastFrame}");
            MenuHarness.AssertSameDs(original.DataSegment, port.State, label);

            var originalPerFrame = original.Draws.GroupBy(x => x.Frame).ToDictionary(g => g.Key, g => g.Count());
            var portPerFrame = port.DrawsPerFrame.Where(x => x.Count > 0).ToDictionary(x => x.Frame, x => x.Count);
            Assert.Equal(originalPerFrame, portPerFrame);
            Assert.Equal(original.Draws, port.Runner.Menu.RandomCalls);

            var originalCalibrations = original.Calls.Where(c => c.Seg == 0x27C6).Select(c => ((int)c.Args[0], (int)c.Args[1])).ToList();
            var portCalibrations = port.Runner.Menu.Events.Where(e => e.Kind == MenuEventKind.Calibration).Select(e => (e.A, e.B)).ToList();
            Assert.Equal(originalCalibrations, portCalibrations);

            var originalSounds = original.Calls.Where(c => c.Seg == 0x21A7).Select(c => (c.Args[0], c.Args[1])).ToList();
            var portSounds = port.Runner.Menu.Events.Where(e => e.Kind == MenuEventKind.Sound)
                .Select(e => ((ushort)e.A, (ushort)e.B)).ToList();
            Assert.Equal(originalSounds, portSounds);

            var originalOps = original.Calls.Where(c => c.Caller is >= 0x131B and < 0x16AF).Select(ToOp).ToList();
            var portOps = port.Runner.Menu.Events.Where(e => e.Kind == MenuEventKind.Screen).SelectMany(e => e.Ops!).ToList();
            Assert.Equal(originalOps.Count, portOps.Count);
            Assert.Equal(originalOps, portOps);
        }
    }

    private static BackdropOp ToOp((ushort Seg, ushort Off, ushort[] Args, ushort Caller) c)
    {
        var a = c.Args;
        return (c.Seg, c.Off) switch
        {
            (0x0A96, 0x0030) => new BackdropOp(BackdropOpKind.Blit, a[0], a[1], a[2], a[3], a[4], a[5]),
            (0x0A96, 0x013B) => new BackdropOp(BackdropOpKind.MaskedBlit, a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7]),
            (0x0A96, 0x04D7) => new BackdropOp(BackdropOpKind.FilledRect, a[0], a[1], a[2], a[3], a[4]),
            (0x0A69, 0x000B) => new BackdropOp(BackdropOpKind.HorizontalLine, a[0], a[1], a[2], a[3]),
            (0x0A69, 0x002C) => new BackdropOp(BackdropOpKind.VerticalLine, a[0], a[1], a[2], a[3]),
            _ => throw new InvalidOperationException($"unexpected call {c.Seg:X4}:{c.Off:X4} from 192D:{c.Caller:X4}"),
        };
    }

    [GameDataFact]
    public void Text_table_192D_18B6_matches_original()
    {
        // [DIFFERENTIAL] 192D:18B6 – pointer table DS:37CC…3C8F as offsets relative to the buffer, buffer contents afterwards.
        var (h, _) = Setup();
        var file = File.ReadAllBytes(TestGameData.Dos("ENGLISCH.TXT"));
        foreach (ushort baseOffset in new ushort[] { 0x0000, 0x0010, 0xF800 })
        {
            var buffer = new byte[0x2000];
            new Random(baseOffset).NextBytes(buffer);
            file.CopyTo(buffer, 0);
            var port = CompareTextTable(h, baseOffset, buffer, $"base {baseOffset:X4}");
            // the entries are the lines of the file (TextTable from the assets): entry k = line k − 1
            var table = Traders.Assets.TextTable.Parse(file);
            for (int k = 1; k <= table.Count && k < MenuLayout.TextPointerCount; k++)
            {
                int start = MenuTexts.RelativeOffset(port.State, k)!.Value;
                var line = table.RawLines[k - 1].Select(b => b == 9 ? (byte)0x20 : b).ToArray();
                Assert.Equal(line, port.Buffer.AsSpan(start, line.Length).ToArray());
                Assert.Equal(0, port.Buffer[start + line.Length]);
            }
            Assert.Null(MenuTexts.RelativeOffset(port.State, 0));
        }
    }

    /// <summary>
    /// [DIFFERENTIAL] 192D:18B6 (#101 R4M-1) – buffers the text file does not cover: long lines with TAB and CR, so the scan
    /// ends at 1194h bytes before the entry limit (the TAB at 1194h and everything after it stays), and short lines, so it
    /// ends at entry 130h before 1194h bytes (TAB and CR after that stay).
    /// </summary>
    [GameDataFact]
    public void Text_table_192D_18B6_tab_cr_and_both_limits_match_original()
    {
        var (h, _) = Setup();
        var random = new Random(18);
        byte Letter() => (byte)random.Next(0x21, 0x7F);

        // long lines: an LF every 40 bytes at most, TAB and CR spread in, a TAB exactly at the byte limit
        var longLines = new byte[0x2000];
        for (int i = 0; i < longLines.Length; i++)
            longLines[i] = (i % 40) switch { 39 => 0x0A, 7 or 23 => 0x09, 38 => 0x0D, _ => Letter() };
        const int limit = 0x1194; // from 192D:18B6, not from the port
        longLines[limit - 1] = 0x09;
        longLines[limit] = 0x09;
        longLines[limit + 1] = 0x0D;
        Assert.True(longLines.AsSpan(0, limit).Count((byte)0x0A) + 1 < MenuLayout.TextPointerCount,
            "long lines must end the scan at the byte limit");
        var port = CompareTextTable(h, 0x0100, longLines, "long lines");
        Assert.Equal(0x20, port.Buffer[7]);
        Assert.Equal(0, port.Buffer[38]);
        Assert.Equal(0x20, port.Buffer[limit - 1]);
        Assert.Equal(0x09, port.Buffer[limit]);
        Assert.Equal(0x0D, port.Buffer[limit + 1]);

        // short lines: "a\tb\r\n" (5 bytes) reaches entry 130h after 5·12Fh < 1194h bytes
        var shortLines = new byte[0x2000];
        for (int i = 0; i < shortLines.Length; i++) shortLines[i] = (i % 5) switch { 1 => 0x09, 3 => 0x0D, 4 => 0x0A, _ => Letter() };
        port = CompareTextTable(h, 0xFF00, shortLines, "short lines");
        int end = 5 * (MenuLayout.TextPointerCount - 1);
        Assert.True(end < limit, "short lines must end the scan at the entry limit");
        Assert.Equal(0x20, port.Buffer[end - 4]);
        Assert.Equal(0x09, port.Buffer[end + 1]);
        Assert.Equal(0x0D, port.Buffer[end + 3]);
    }

    /// <summary>
    /// Runs 192D:18B6 on <paramref name="buffer"/> at DS:4884 = <paramref name="baseOffset"/> in the original and
    /// <see cref="MenuTexts.Build"/> in the port: data segment (pointer table, garbage beyond the scan kept) and the buffer
    /// through the same 16-bit addressing must agree. Returns the port's state and buffer.
    /// </summary>
    private static (Dgroup State, byte[] Buffer) CompareTextTable(MenuHarness h, ushort baseOffset, byte[] buffer, string label)
    {
        var input = h.Initial();
        input.Set(MenuLayout.TextBuffer, baseOffset);
        input.Set(MenuLayout.TextBuffer + 2, MenuHarness.TextSegment);
        // fill the pointer table with garbage: entries the scan does not reach must keep it
        for (int i = 0; i < 4 * (MenuLayout.TextPointerCount + 1); i++) input.Set(MenuLayout.TextPointers + i, (byte)(i * 7 + 3));
        var image = new byte[0x10000];
        for (int i = 0; i < buffer.Length; i++) image[(baseOffset + i) & 0xFFFF] = buffer[i];
        h.WriteTextBuffer(image);
        var original = h.RunOriginal(input, 0x18B6, _ => []);
        var originalBuffer = h.ReadTextBuffer(0x10000);

        var port = new Dgroup((byte[])input.Bytes.Clone());
        var portBuffer = (byte[])buffer.Clone();
        MenuTexts.Build(port, portBuffer);
        MenuHarness.AssertSameDs(original.DataSegment, port, label);
        // offsets wrap within the segment in the original: compare the buffer through the same 16-bit addressing
        for (int i = 0; i < buffer.Length; i++)
            Assert.True(originalBuffer[(baseOffset + i) & 0xFFFF] == portBuffer[i], $"{label}: buffer byte {i:X4}");
        return (port, portBuffer);
    }
}
