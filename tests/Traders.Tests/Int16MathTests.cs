using Traders.Core;

namespace Traders.Tests;

public class Int16MathTests
{
    [Theory]
    [InlineData(32767, 1, -32768)]
    [InlineData(-32768, -1, 32767)]
    public void Addition_wraps_like_8086(int a, int b, int expected) =>
        Assert.Equal((short)expected, Int16Math.S(a + b));

    [Theory]
    [InlineData(-7, 2, -3, -1)]
    [InlineData(7, -2, -3, 1)]
    public void Division_truncates_toward_zero(int a, int b, int q, int r)
    {
        Assert.Equal((short)q, Int16Math.Div((short)a, (short)b));
        Assert.Equal((short)r, Int16Math.Mod((short)a, (short)b));
    }
    [Fact]
    public void Division_faults_before_returning_quotient_or_remainder()
    {
        Assert.Throws<OverflowException>(() => Int16Math.Div(short.MinValue, -1));
        Assert.Throws<OverflowException>(() => Int16Math.Mod(short.MinValue, -1));
        Assert.Throws<DivideByZeroException>(() => Int16Math.Div(1, 0));
        Assert.Throws<DivideByZeroException>(() => Int16Math.Mod(1, 0));
    }

    [Theory]
    [InlineData(-32768, -1)]
    [InlineData(1, 0)]
    public void Refemu_idiv_raises_divide_error_for_invalid_quotient(int dividend, int divisor)
    {
        var memory = new Traders.RefEmu.Cpu.Memory();
        memory.Write16(0, 0, 0x0010); // vector 0 handler offset
        memory.Write16(0, 2, 0x0300); // vector 0 handler segment
        memory.WriteBlock(0x1000, new byte[] { 0xF7, 0xFB }); // IDIV BX
        var cpu = new Traders.RefEmu.Cpu.Cpu286(memory)
        {
            Cs = 0x0100, IP = 0, Ss = 0x0200, Sp = 0x0100,
            Ax = unchecked((ushort)dividend), Dx = dividend < 0 ? (ushort)0xFFFF : (ushort)0,
            Bx = unchecked((ushort)divisor),
        };
        cpu.Step();
        Assert.Equal((ushort)0x0300, cpu.Cs);
        Assert.Equal((ushort)0x0010, cpu.IP);
    }
}
