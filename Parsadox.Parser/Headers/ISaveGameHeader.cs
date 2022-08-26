namespace Parsadox.Parser.Headers;

/// <summary>
/// Container for a save game's header and other metadata.
/// </summary>
public interface ISaveGameHeader
{
    string? FileName { get; internal set; }

    string Text { get; internal set; }

    IGameVersion Version { get; internal set; }

    SaveGameFormat Format { get; }

    long BytesUntilContent { get; }
}
