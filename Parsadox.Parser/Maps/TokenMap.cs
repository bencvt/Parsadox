namespace Parsadox.Parser.Maps;

internal class TokenMap : ITokenMap
{
    private static readonly Regex RE_USHORT_HEX = new(@"^0x[0-9A-Fa-f]{4}$");

    public Dictionary<ushort, string> CodeMap { get; private init; } = new();

    public void Clear() => CodeMap.Clear();

    public ITokenMap DeepCopy() => new TokenMap
    {
        CodeMap = CodeMap.ToDictionary(x => x.Key, x => x.Value),
    };

    public string LoadEnvironment(Game game) =>
        LoadEnvironment(TokenMapFactory.GetDefaultTokenMapEnvironmentVariableName(game));

    public string LoadEnvironment(string customVariableName)
    {
        string? path = FileSystem.Instance.GetEnvironmentVariable(customVariableName);
        if (path is null)
            throw new ArgumentException($"No environment variable named {Strings.EscapeAndQuote(customVariableName)} exists");
        FileSystem.AssertFileExists(path, $"File specified by {Strings.EscapeAndQuote(customVariableName)}");
        LoadFile(path);
        return path;
    }

    public ITokenMap LoadFile(string path) => LoadString(FileSystem.Instance.ReadAllText(path, Encoding.UTF8));

    public ITokenMap LoadString(string text)
    {
        foreach (var (line, key, value) in GetLines(text))
        {
            ushort code = Convert.ToUInt16(key, 16);

            // CodeMap.Add would throw anyway, but use a custom exception for clarity.
            if (CodeMap.ContainsKey(code))
                throw new ParseException($"{Strings.EscapeAndQuote(key)} is mapped to both " +
                    $"{Strings.EscapeAndQuote(CodeMap[code])} and {Strings.EscapeAndQuote(value)}");

            CodeMap.Add(code, value);
        }
        return this;
    }

    public override string ToString() => new StringBuilder()
        .AppendCount(CodeMap.Count, "code")
        .ToString();

    private static IEnumerable<(string line, string key, string value)> GetLines(string text)
    {
        foreach (string untrimmed in text.Split('\n'))
        {
            string line = untrimmed.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            string[] parts = line.Split();
            if (parts.Length != 2 || !RE_USHORT_HEX.IsMatch(parts[0]))
                throw new ParseException($"Invalid line: {line}");

            yield return (line, parts[0], parts[1]);
        }
    }
}
