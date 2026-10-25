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
}
