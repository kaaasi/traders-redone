namespace Traders.Core.Rules;

/// <summary>
/// 32-bit arithmetic of the Turbo C runtime used by the weekly rules: LXMUL (0000:4F4E, low 32 bits of the product)
/// and LDIV/LMOD (0000:4F89 / 0000:4F98, common body 0000:4FA3). The division works on magnitudes and fixes the sign
/// afterwards, so it truncates toward zero, the remainder takes the dividend's sign and MinValue / -1 wraps to MinValue
/// instead of faulting. A zero divisor always ends in a 16-bit DIV (0000:5027) and raises the divide error.
/// </summary>
// No routine of its own (runtime helpers, listed with docs/routines/21FD_01BA_random.md); checked by the differential
// tests of the rules that use them (tests/Traders.Tests/Rules/).
internal static class LongMath
{
    public static int Mul(int a, int b) => unchecked(a * b);

    public static int Div(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException();
        return Int16Math.L((long)a / b);
    }

    public static int Mod(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException();
        return Int16Math.L((long)a % b);
    }
}
