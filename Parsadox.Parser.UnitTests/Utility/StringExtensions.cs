namespace Parsadox.Parser.UnitTests.Utility;

internal static class StringExtensions
{
    internal static string NoCr(this string text) => text.Replace("\r", string.Empty);
}
