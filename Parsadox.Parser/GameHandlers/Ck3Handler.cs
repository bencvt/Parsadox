namespace Parsadox.Parser.GameHandlers;

internal class Ck3Handler : IGameHandler
{
    public string Name => "Crusader Kings III";

    public string SaveGameExtension => ".ck3";

    public Encoding TextEncoding => Encoding.UTF8;

    public SaveGameCompression Compression => SaveGameCompression.Optional;

    public bool HasBinaryFormat => true;

    public ITokenTypeMap TokenTypeMap => Ck3TokenTypeMap.Instance;

    public virtual IFloatConverter FloatConverter => Ck3AndImperatorFloatConverter.Instance;

    public bool HasHeader(Stream input) => input.PeekString(3) == "SAV";

    public ISaveGameHeader ReadHeader(Stream input, bool isMain) => new Ck3SaveGameHeader(input);

    public void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        var header = (saveGame.Header as Ck3SaveGameHeader) ?? new(saveGame.Header.Text);
        header.Write(this, saveGame.Root, output, parameters);
    }

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.Parse(saveGame, "gamestate", "meta_data", "version");

    public void DisableIronman(ISaveGame saveGame)
    {
        saveGame.Root.GetDescendantOrNull("gamestate", "meta_data", "ironman")?.SetValue(false);
        saveGame.Root.GetDescendantOrNull("gamestate", "meta_data", "can_get_achievements")?.SetValue(false);
        saveGame.Root.GetDescendantOrNull("gamestate", "ironman_manager", "ironman")?.SetValue(false);
        saveGame.Root.GetDescendantOrNull("gamestate", "ironman_manager", "save_game")?.SetValue(string.Empty);
    }
}
