namespace Parsadox.Parser.SaveGames;

/// <summary>
/// Container for save game header and data, loaded by <see cref="SaveGameFactory"/>.
/// </summary>
public interface ISaveGame
{
    Game Game { get; }

    ISaveGameHeader Header { get; }

    /// <summary>
    /// Contains the save game data in a tree structure.
    /// <para/>
    /// Loaded as an anonymous array with a "gamestate" node (entry).
    /// <para/>
    /// Depending on the game, additional entries may exist (e.g., "meta" and "ai").
    /// <para/>
    /// Each entry has an arbitrary number of child nodes (sections).
    /// Each section may have child nodes of its own, etc.
    /// </summary>
    INode Root { get; set; }

    /// <summary>
    /// Shorthand for Root["gamestate"].
    /// </summary>
    INode State { get; }

    /// <returns>the number of bytes written</returns>
    long WriteFile(string outputPath, WriteParameters? parameters = null);

    /// <summary>
    /// Shorthand for:
    /// <code>
    /// WriteFile(outputPath, new() { SaveGameFormat = SaveGameFormat.UncompressedText })
    /// </code>
    /// </summary>
    /// <returns>the number of bytes written</returns>
    long WriteFileUncompressed(string outputPath) =>
        WriteFile(outputPath, new() { SaveGameFormat = SaveGameFormat.UncompressedText });

    /// <returns>the number of bytes written</returns>
    Task<long> WriteFileAsync(string outputPath, WriteParameters? parameters, CancellationToken cancellationToken);

    /// <returns>the number of bytes written</returns>
    long WriteStream(Stream outputStream, WriteParameters? parameters = null);

    /// <returns>the number of bytes written</returns>
    Task<long> WriteStreamAsync(Stream outputStream, WriteParameters? parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Use game-specific logic to unset any existing Ironman flags.
    /// <para/>
    /// Useful for "melting" binary Ironman save games before saving them in text format.
    /// </summary>
    ISaveGame DisableIronman();
}
