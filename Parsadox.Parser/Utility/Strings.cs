namespace Parsadox.Parser.Utility;

internal static class Strings
{
    internal static Encoding Windows1252;
    static Strings()
    {
        // Modern .NET versions do not include the Windows-1252 encoding.
        // Instead, the System.Text.Encoding.CodePages NuGet package is required,
        // along with the following initialization:
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Windows1252 = Encoding.GetEncoding(1252);
    }

    internal static string EscapeAndQuote(string text)
    {
        var b = new StringBuilder(text.Length + 2).Append('"');
        foreach (char c in text)
        {
            b.Append(c switch
            {
                '\"' => "\\\"",
                '\\' => "\\\\",
                '\0' => @"\0",
                '\a' => @"\a",
                '\b' => @"\b",
                '\f' => @"\f",
                '\n' => @"\n",
                '\r' => @"\r",
                '\t' => @"\t",
                '\v' => @"\v",
                _ => c,
            });
        }
        return b.Append('"').ToString();
    }

    internal static string EscapeAndQuote(IEnumerable<string> values) =>
        string.Join(", ", values.Select(s => EscapeAndQuote(s)));

    internal static string EscapeAndQuote(object? obj) => obj is null ? "null" : EscapeAndQuote($"{obj}");

    internal static string Quote(string raw) => "\"" + raw.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

    private static readonly Regex RE_NEED_QUOTES = new(@"[={}""\s]");
    internal static string QuoteOnlyIfNeeded(string text)
    {
        if (RE_NEED_QUOTES.IsMatch(text) || text == string.Empty)
            return Quote(text);
        return text;
    }

    internal static string Comment(string content, int depth)
    {
        StringBuilder builder = new();
        foreach (string line in content.Replace("\r", string.Empty).Split('\n'))
            builder.Append('\t', depth).Append('#').Append(line).Append('\n');
        return builder.ToString();
    }

    internal static string BoolToString(bool value) => value ? "yes" : "no";

    internal static string F32ToString(float value) => $"{value:0.000000}";

    internal static string BigF32ToString(float value) => $"{value:0.000}";

    /// <summary>
    /// Without a decimal conversion, small values would be incorrectly
    /// output in scientific notation, e.g., 0.00001 => "1E-05".
    /// </summary>
    internal static string F64ToString(double value) => new decimal(value).ToString();

    internal static string BigF64ToString(double value) => $"{value:0.00000}";

    internal static bool StringToBool(string text)
    {
        string lower = text.Trim().ToLowerInvariant();
        if (lower == "yes")
            return true;
        if (lower == "no")
            return false;
        return bool.Parse(text);
    }

    internal static string CountItem(long count, string item) => count == 1 ? $"1 {item}" : $"{count:n0} {item}s";

    internal static void WriteLineCount(this TextWriter writer, string prefix, long count, string name) =>
        writer.WriteLine($"{prefix} {CountItem(count, name)}");

    internal static StringBuilder AppendCount(this StringBuilder builder, long count, string name) =>
        builder.Append(CountItem(count, name));
}
