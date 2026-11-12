using Traders.Assets.Loading;
using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Tests.Startup;

/// <summary>Program start and code request without original data: branches as read from the disassembly (finding 0DFD_000D).</summary>
public class StartupTests
{
    private static ImageFileLengths Lengths(int n) => new((uint)(0x1000 + n), (uint)(0x20000 + n));

    [Fact]
    public void First_load_stores_both_lengths_and_later_loads_keep_them()
    {
        var d = new Dgroup();
        d.Set(ImageTable.CurrentImage, (short)-1);
        int calls = 0;
        ImageTable.RecordLoad(d, 3, n => { calls++; return Lengths(n); });
        int e = ImageTable.Entry(3);
        Assert.Equal(0x09CE + 3 * 0x13, e);
        Assert.Equal(0x1003u, d.U32(e + 0x0B));
        Assert.Equal(0x20003u, d.U32(e + 0x0F));
        Assert.Equal(3, d.S16(ImageTable.CurrentImage));

        ImageTable.RecordLoad(d, 3, _ => { calls++; return new ImageFileLengths(1, 1); });
        Assert.Equal(1, calls);
        Assert.Equal(0x20003u, d.U32(e + 0x0F));
        Assert.Throws<ArgumentOutOfRangeException>(() => ImageTable.RecordLoad(d, 10, Lengths));
    }

    [Fact]
    public void Trailer_gives_the_unpacked_length_big_endian()
    {
        byte[] file = [0x50, 0x50, 0x32, 0x30, 0xAA, 0x01, 0x43, 0x04, 0x08];
        Assert.Equal((9u, 0x014304u), ImageFiles.Lengths(file));
        Assert.Throws<InvalidDataException>(() => ImageFiles.Lengths(new byte[3]));
    }

    /// <summary>Data segment with the four coded texts at 0100h.. and the code text as the third.</summary>
    private static Dgroup WithTexts()
    {
        var d = new Dgroup();
        int[] at = [0x100, 0x120, 0x140, 0x160];
        string[] plain = ["INSERT COIN!", "@", "CODE: X-nn", "X"];
        for (int i = 0; i < 4; i++)
        {
            d.Set(CodeQuery.Texts + 4 * i, (ushort)at[i]);
            for (int k = 0; k < plain[i].Length; k++) d.Set(at[i] + k, unchecked((byte)(plain[i][k] - 0xFD)));
        }
        d.Set(ImageTable.CodeRequestNumber, (short)6);
        return d;
    }

    private sealed class CountingPlaque : ITextPlaque
    {
        public List<(short, short, int, short)> Calls { get; } = [];

        public void Show(Dgroup d, OriginalRandom random, short height, short step, int texts, short extra) =>
            Calls.Add((height, step, texts, extra));
    }

    private static string Text(Dgroup d, int at, int length) => new(Enumerable.Range(0, length).Select(i => (char)d.U8(at + i)).ToArray());

    [Theory]
    [InlineData(0x2847C48A, "CODE: C-12")]   // two-digit column
    [InlineData(0x29743763, "CODE: D-5 ")]   // one digit, then blank
    public void Code_request_reseeds_draws_once_and_writes_the_code(int clock, string expected)
    {
        var d = WithTexts();
        var random = new OriginalRandom(new RecordedClock([clock]));
        var plaque = new CountingPlaque();

        var code = CodeQuery.Run(d, random, Lengths, plaque);

        Assert.Equal(2, random.Calls);
        Assert.Equal("INSERT COIN!", Text(d, 0x100, 12));
        Assert.Equal(expected, Text(d, 0x140, 10));
        Assert.Equal(expected[6..].TrimEnd(), code.Code);
        Assert.Equal(0, code.Row % 4);
        Assert.All(code.Digits, x => Assert.InRange(x, 1, 9));
        Assert.Equal([(0x50, 0x1E, 0x10E0, -3)], plaque.Calls.Select(c => ((int)c.Item1, (int)c.Item2, c.Item3, (int)c.Item4)));
        Assert.Equal(6, d.S16(ImageTable.CurrentImage));
    }

    [Fact]
    public void Program_start_loads_the_six_pictures_in_the_order_of_0DFD_0239_then_the_request_picture()
    {
        var d = WithTexts();
        short[] numbers = [4, 5, 0, 1, 3, 2]; // DS:08CE..08D8
        for (int i = 0; i < 6; i++) d.Set(0x08CE + 2 * i, numbers[i]);
        var order = new List<int>();
        var start = new ProgramStart(n => { order.Add(n); return Lengths(n); }, new CountingPlaque());
        var machine = new ProgramMachine(d, new OriginalRandom(new RecordedClock([1])),
            new Dictionary<int, IProgramStepHandler> { [1] = new ProgramInitialisation(1), [2] = start }, new Dictionary<int, IWeekStepHandler>());

        machine.Advance(new PhaseTick(0));

        // 08CE, 08D0, 08D6, 08D8, 08D4, 08D2
        Assert.Equal([4, 5, 3, 2, 1, 0, 6], order);
        Assert.NotNull(start.Code);
        Assert.Equal(3, machine.NextStep);
        Assert.Equal(ProgramStatus.Blocked, machine.Status);
    }
}
