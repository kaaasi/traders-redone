namespace Traders.Tests.Phases;

/// <summary>
/// Trait of the long RefEmu runs of this folder (several emulated weeks, minutes each). <c>tools/pruefen.sh --schnell</c>
/// leaves them out (<c>--filter "Dauer!=lang"</c>); the full run and the CI with original data run them.
/// </summary>
internal static class Long
{
    public const string Trait = "Dauer";
    public const string Value = "lang";
}
