namespace Parsadox.Parser.GameHandlers;

internal class ImperatorHandler : IGameHandler
{
    public string Name => "Imperator";

    public string SaveGameExtension => ".rome";

    public Encoding TextEncoding => Encoding.UTF8;

    public SaveGameCompression Compression => SaveGameCompression.Optional;

    public bool HasBinaryFormat => true;

    // TODO add ImperatorTokenTypeMap

    public IFloatConverter FloatConverter => Ck3AndImperatorFloatConverter.Instance;

    public bool HasHeader(Stream input) => input.PeekString(3) == "SAV";

    public ISaveGameHeader ReadHeader(Stream input, bool isMain) => new ImperatorSaveGameHeader(input);

    public void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        var header = (saveGame.Header as ImperatorSaveGameHeader) ?? new(saveGame.Header.Text);
        header.Write(this, saveGame.Root, output, parameters);
    }

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.Parse(saveGame, "gamestate", "version");

    public void DisableIronman(ISaveGame saveGame)
    {
        saveGame.Root["gamestate"].RemoveAllChildren("ironman");
        saveGame.Root["gamestate"].RemoveAllChildren("iron");
        saveGame.Root.GetDescendantOrNull("gamestate", "game_configuration")?.RemoveAllChildren("ironman");
        saveGame.Root.GetDescendantOrNull("gamestate", "game_configuration", "ironman_cloud")?.SetValue(false);
        saveGame.Root.GetDescendantOrNull("gamestate", "game_configuration", "ironman_save_name")?.SetValue(false);
    }
}
