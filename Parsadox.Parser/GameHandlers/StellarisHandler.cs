namespace Parsadox.Parser.GameHandlers;

internal class StellarisHandler : IGameHandler
{
    public string Name => "Stellaris";

    public string SaveGameExtension => ".sav";

    public Encoding TextEncoding => Encoding.UTF8;
  
    public SaveGameCompression Compression => SaveGameCompression.Mandatory;

    public bool HasBinaryFormat => false;

    // No headers.

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.Parse(saveGame, "version");

    public void DisableIronman(ISaveGame saveGame)
    {
        saveGame.Root["meta"].RemoveChild("ironman");
        saveGame.State.GetDescendants("galaxy", "ironman").FirstOrDefault()?.SetValue(false);
    }
}
