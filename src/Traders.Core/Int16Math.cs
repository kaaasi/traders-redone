namespace Traders.Core;

/// <summary>
/// Helpers that reproduce 16-bit 8086/Turbo C semantics. Original intermediate results
/// are often 16 bit wide and wrap; porting them with 32/64-bit maths changes behaviour.
/// </summary>
public static class Int16Math
{
    /// <summary>Wraps to a signed 16-bit value (C <c>int</c> in Turbo C).</summary>
    public static short S(int value) => unchecked((short)value);

    /// <summary>Wraps to an unsigned 16-bit value (C <c>unsigned</c> in Turbo C).</summary>
    public static ushort U(int value) => unchecked((ushort)value);

    /// <summary>Signed 16-bit division as IDIV: truncates toward zero.</summary>
    public static short Div(short a, short b)
    {
        ValidateDivision(a, b);
        return S(a / b);
    }

    /// <summary>Signed 16-bit remainder as IDIV: sign follows the dividend.</summary>
    public static short Mod(short a, short b)
    {
        ValidateDivision(a, b);
        return S(a % b);
    }

    // IDIV faults before producing either quotient or remainder.
    private static void ValidateDivision(short a, short b)
    {
        if (b == 0) throw new DivideByZeroException();
        if (a == short.MinValue && b == -1)
            throw new OverflowException("16-bit IDIV quotient overflow.");
    }

    /// <summary>Signed 32-bit wrap (C <c>long</c> in Turbo C).</summary>
    public static int L(long value) => unchecked((int)value);
}
