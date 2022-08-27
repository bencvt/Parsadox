namespace Parsadox.Parser.GameHandlers;

internal class Vic2Handler : IGameHandler
{
    public string Name => "Victoria II";

    public string SaveGameExtension => ".v2";

    public Encoding TextEncoding => Strings.Windows1252;

    public SaveGameCompression Compression => SaveGameCompression.Never;

    public bool HasBinaryFormat => false;

    // No headers, Ironman, or version stored in the save game.

    public void DisableIronman(ISaveGame saveGame) { }

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.UNKNOWN;
}
