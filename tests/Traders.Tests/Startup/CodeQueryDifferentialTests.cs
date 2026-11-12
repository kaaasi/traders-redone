using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Phases;

namespace Traders.Tests.Startup;

/// <summary>
/// Parts of the code request 21FD:0687 called directly in the original (LoadOnly) and compared with the core: the digit
/// formula 21FD:08D3 and the decoding of the texts 21FD:081D; plus the RefEmu probe for the keypad table behind DS:10A9.
/// </summary>
public class CodeQueryDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    private static ushort Lo(int v) => unchecked((ushort)v);

    private static ushort Hi(int v) => unchecked((ushort)(v >> 16));

    /// <summary>
    /// 21FD:08D3(a, b) as long arguments over all rows and columns the request can draw (a = row + i up to 31, b up to 31)
    /// and over edge values (negative, 16-bit limits, large longs): the core's digit equals AX of the original.
    /// </summary>
    // [DIFFERENTIAL] 21FD:08D3 – digit formula of the code request
    [GameDataFact]
    public void Digit_formula_equals_the_original()
    {
        var p = OriginalProgram.LoadOnly(Dir);
        var values = Enumerable.Range(0, 35).Concat([-1, -32, -32768, 32767, 65535, 0x12345, int.MaxValue, int.MinValue]).ToArray();
        int checkedCount = 0;
        foreach (int a in values)
        foreach (int b in values.Where(v => v < 32 || v > 34))
        {
            var (ax, _) = p.Call(0x21FD, 0x08D3, Lo(a), Hi(a), Lo(b), Hi(b));
            Assert.True(CodeQuery.Digit(a, b) == (short)ax, $"21FD:08D3({a}, {b}): core {CodeQuery.Digit(a, b)} original {(short)ax}");
            checkedCount++;
        }
        Assert.True(checkedCount > 1000);
    }

    /// <summary>21FD:081D on the initial data segment: the core's decoding gives the same data segment, twice in a row.</summary>
    // [DIFFERENTIAL] 21FD:081D – decoding of the four request texts
    [GameDataFact]
    public void Text_decoding_equals_the_original()
    {
        var p = OriginalProgram.LoadOnly(Dir);
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        for (int round = 0; round < 2; round++)
        {
            p.Call(0x21FD, 0x081D);
            CodeQuery.DecodeTexts(core);
            var emu = p.DataSegment();
            for (int i = 0; i < DgroupLayout.Size; i++)
                Assert.True(core.Bytes[i] == emu[i], $"round {round}: DS:{i:X4} core {core.Bytes[i]:X2} original {emu[i]:X2}");
        }
        // after one round the first text reads "INSERT COIN!"
        var once = new Dgroup(OriginalProgram.LoadOnly(Dir).DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        CodeQuery.DecodeTexts(once);
        int t = once.U16(CodeQuery.Texts);
        Assert.Equal("INSERT COIN!", new string(Enumerable.Range(0, 12).Select(i => (char)once.U8(t + i)).ToArray()));
    }

    /// <summary>
    /// DS:10A9 in the finding of the code request is not a table at 10A9: 21FD:07CD reads <c>[SI + 10A9h]</c> only for
    /// SI = 47h..51h (keypad scancodes), i.e. the bytes DS:10F0..10FA. The word DS:10A9 itself is the current piece of the
    /// music (218B:0094 writes it: FFFF from the executable, 0 after 218B:0094(0, 0) at 0DFD:02FE). Probe: boot with the
    /// keypad keys 7 8 9 4 at the request and record the addresses read and the digits taken.
    /// </summary>
    [GameDataFact]
    public void Keypad_table_lies_behind_DS_10A9_and_does_not_touch_the_current_piece_word()
    {
        var p = OriginalProgram.Start(Dir);
        ProgramStartDifferentialTests.KeysAtRequest(0x47, 0x48, 0x49, 0x4B)(p);
        ushort query = p.Segment(0x21FD), start = p.Segment(0x0DFD);
        var tableReads = new List<int>();
        var digits = new List<int>();
        ushort? pieceBefore = null, pieceAfter = null;
        p.Machine.AddCodeHook(query, 0x07CD, m => tableReads.Add(m.Cpu.Si + 0x10A9));
        p.Machine.AddCodeHook(query, 0x07E1, m => digits.Add(m.Cpu.Si));
        p.Machine.AddCodeHook(start, 0x02FE, _ => pieceBefore = p.ReadWord(0x10A9));
        p.Machine.AddCodeHook(start, 0x0303, _ => pieceAfter = p.ReadWord(0x10A9));
        var exe = OriginalProgram.LoadOnly(Dir);
        byte[] table = Enumerable.Range(0x10F0, 11).Select(a => exe.ReadByte((ushort)a)).ToArray();

        var oracle = new RefEmuProgramOracle(p);
        oracle.RunToCallSite(ProgramLoop.Step(1));
        oracle.RunToCallSite(ProgramLoop.Step(2));
        oracle.RunToCallSite(ProgramLoop.Step(3));

        Assert.Equal(0xFFFF, exe.ReadWord(0x10A9));
        Assert.Equal((ushort)0xFFFF, pieceBefore);
        Assert.Equal((ushort)0, pieceAfter);
        Assert.Equal(4, tableReads.Count);
        Assert.All(tableReads, a => Assert.InRange(a, 0x10F0, 0x10FA));
        Assert.Equal([0x10F0, 0x10F1, 0x10F2, 0x10F4], tableReads);
        Assert.Equal([7, 8, 9, 4], digits);
        // keypad layout 7 8 9 - 4 5 6 + 1 2 3 (scancodes 47h..51h); '-' and '+' map outside 1..9
        Assert.Equal([7, 8, 9, 4, 5, 6, 1, 2, 3], new[] { 0, 1, 2, 4, 5, 6, 8, 9, 10 }.Select(i => (int)table[i]));
        Assert.DoesNotContain(table[3], Enumerable.Range(1, 9).Select(v => (byte)v));
        Assert.DoesNotContain(table[7], Enumerable.Range(1, 9).Select(v => (byte)v));
    }
}
