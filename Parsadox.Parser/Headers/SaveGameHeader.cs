namespace Parsadox.Parser.Headers;

/// <summary>
/// Generic empty header class for games that don't use headers.
/// </summary>
internal class SaveGameHeader : ISaveGameHeader
{
    public string? FileName { get; set; }

    public string Text { get; set; } = string.Empty;

    public IGameVersion Version { get; set; } = GameVersion.UNKNOWN;

    public SaveGameFormat Format { get; internal set; }

    public virtual long BytesUntilContent => 0L;

    public override string ToString() => new StringBuilder()
        .AppendLine($"{GetType().Name}:")
        .AppendLine($"  {nameof(FileName)}={FileName}")
        .AppendLine($"  {nameof(Text)}={Strings.EscapeAndQuote(Text)}")
        .AppendLine($"  {nameof(Version)}={Version}")
        .AppendLine($"  {nameof(Format)}={Format}")
        .AppendLine($"  {nameof(BytesUntilContent)}={BytesUntilContent:n0}")
        .ToString();
}
