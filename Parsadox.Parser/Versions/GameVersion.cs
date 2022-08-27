namespace Parsadox.Parser.Versions;

internal class GameVersion : IGameVersion
{
    internal static readonly GameVersion UNKNOWN = new("Unknown");

    internal GameVersion(string text, bool ignorePatchMinor = false)
    {
        Text = text;

        List<string> parts = new();
        StringBuilder builder = new();
        foreach (char c in text)
        {
            if (char.IsDigit(c))
            {
                builder.Append(c);
            }
            else if (builder.Length > 0)
            {
                parts.Add(builder.ToString());
                builder.Clear();
                if (parts.Count > 3)
                    break;
            }
        }
        if (builder.Length > 0)
            parts.Add(builder.ToString());
        if (parts.Count > 0)
        {
            if (int.TryParse(parts[0], out int value))
                Major = value;
            if (parts.Count > 1)
            {
                if (int.TryParse(parts[1], out value))
                    Minor = value;
                if (parts.Count > 2)
                {
                    if (int.TryParse(parts[2], out value))
                        Patch = value;
                    if (parts.Count > 3 && !ignorePatchMinor && int.TryParse(parts[3], out value))
                        PatchMinor = value;
                }
            }
        }
    }

    public string Text { get; }

    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public int PatchMinor { get; }

    public bool IsUnknown => ReferenceEquals(this, UNKNOWN);

    public bool IsLessThan(IGameVersion other)
    {
        if (IsUnknown || other.IsUnknown)
            return true;

        return Major < other.Major
            || Minor < other.Minor
            || Patch < other.Patch
            || PatchMinor < other.PatchMinor;
    }

    public override string ToString() => Text;

    internal static IGameVersion Parse(ISaveGame saveGame, params string[] path)
    {
        var text = saveGame.Root.GetDescendantOrNull(path)?.Value.Text;
        if (text is null)
            return UNKNOWN;
        return new GameVersion(text);
    }
}
