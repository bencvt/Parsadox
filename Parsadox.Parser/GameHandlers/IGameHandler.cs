namespace Parsadox.Parser.GameHandlers;

/// <summary>
/// Encapsulate all game-specific logic.
/// </summary>
internal interface IGameHandler
{
    string Name { get; }

    string SaveGameExtension { get; }

    Encoding TextEncoding { get; }

    SaveGameCompression Compression { get; }

    /// <summary>
    /// If this game's compressed format uses multiple entries
    /// (e.g., meta/gamestate/ai), ensure they are ordered correctly for
    /// writing uncompressed, which necessarily flattens the entries.
    /// </summary>
    List<string>? EntryOrder => null;

    /// <summary>
    /// If this game's compressed format has an entry that only contains a
    /// redundant copy of data, don't load it.
    /// </summary>
    string? DoNotLoadEntry => null;

    bool HasBinaryFormat { get; }

    /// <summary>
    /// Determine whether an I32 token contains a <see cref="GameDate"/>
    /// or just a regular int.
    /// <para/>
    /// Only relevant when loading binary save games.
    /// </summary>
    ITokenTypeMap TokenTypeMap => DefaultTokenTypeMap.Instance;

    /// <summary>
    /// Only relevant when loading binary save games.
    /// </summary>
    IFloatConverter FloatConverter => DefaultFloatConverter.Instance;

    /// <summary>
    /// Detect whether a save game file has a header.
    /// </summary>
    bool HasHeader(Stream input) => false;

    /// <summary>
    /// Extract the header when reading a save game file.
    /// <para/>
    /// Only called when <see cref="HasHeader"/> returns true.
    /// <para/>
    /// Can be called multiple times when reading compressed save games:
    /// Once with isMain=true, then once per entry in the archive with isMain=false.
    /// </summary>
    ISaveGameHeader ReadHeader(Stream input, bool isMain) => new SaveGameHeader();

    void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters) { }

    void WriteEntryHeader(ISaveGame saveGame, INode entryNode, Stream output, WriteParameters parameters) { }

    IEnumerable<INode> GetEntryNodesToWrite(ISaveGame saveGame) => saveGame.Root;

    string AdjustEntryNameForWrite(string entryName, string? outputPath) => entryName;

    int WriteIndentLevel => 0;

    bool ShouldWriteSection(bool isCompressed, INode entryNode, INode section) => true;

    void WriteEntryFooter(ISaveGame saveGame, Stream output, WriteParameters parameters) { }

    void DisableIronman(ISaveGame saveGame);

    /// <summary>
    /// Move or add nodes to account for differences when loading compressed vs uncompressed.
    /// <para/>
    /// Called when loading the save game.
    /// </summary>
    void Normalize(ISaveGame saveGame) { }

    /// <summary>
    /// Extract the game version from the save game content.
    /// <para/>
    /// Called when loading the save game.
    /// </summary>
    IGameVersion GetVersion(ISaveGame saveGame);
}
