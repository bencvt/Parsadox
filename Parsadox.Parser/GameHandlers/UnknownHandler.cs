namespace Parsadox.Parser.GameHandlers;

internal class UnknownHandler : IGameHandler
{
    public string Name => "Unknown";

    public string SaveGameExtension => ".*";

    public Encoding TextEncoding => Encoding.UTF8;

    public SaveGameCompression Compression => SaveGameCompression.Optional;

    public bool HasBinaryFormat => false;

    public void DisableIronman(ISaveGame saveGame) => throw new NotImplementedException();

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.UNKNOWN;
}
