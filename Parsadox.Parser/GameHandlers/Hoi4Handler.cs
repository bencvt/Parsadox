﻿namespace Parsadox.Parser.GameHandlers;

internal class Hoi4Handler : IGameHandler
{
    public string Name => "Hearts of Iron IV";

    public string SaveGameExtension => ".hoi4";

    public Encoding TextEncoding => Encoding.UTF8;

    public SaveGameCompression Compression => SaveGameCompression.Never;

    public bool HasBinaryFormat => true;

    // TODO add Hoi4TokenTypeMap

    public IFloatConverter FloatConverter => new Eu4AndHoi4FloatConverter();

    public bool HasHeader(Stream input) => input.PeekString(4) == "HOI4";

    public ISaveGameHeader ReadHeader(Stream input, bool isMain)
    {
        string text = input.ReadString(7);
        return new SaveGameHeader
        {
            Text = text,
            Format = text == "HOI4bin" ? SaveGameFormat.UncompressedBinary : SaveGameFormat.UncompressedText,
        };
    }

    public void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        output.WriteString("HOI4txt\n", TextEncoding);
    }

    public IGameVersion GetVersion(ISaveGame saveGame)
    {
        var text = saveGame.Root.GetDescendantOrNull("gamestate", "version")?.Value.Text;
        if (text is null)
            return GameVersion.UNKNOWN;
        // The 4th version component is hexadecimal.
        return new GameVersion(text, ignorePatchMinor: true);
    }

    public void DisableIronman(ISaveGame saveGame)
    {
        saveGame.Root["gamestate"].RemoveAllChildren("ironman");
        saveGame.Root["gamestate"].RemoveAllChildren("achievement");
        saveGame.Root.GetDescendantOrNull("gamestate", "gameplaysettings", "ironman")?.SetValue("0");
    }
}
