namespace Parsadox.Parser.GameHandlers;

internal class Eu4Handler : IGameHandler
{
    public string Name => "Europa Universalis IV";

    public string SaveGameExtension => ".eu4";

    public Encoding TextEncoding => Strings.Windows1252;

    public SaveGameCompression Compression => SaveGameCompression.Optional;

    public List<string>? EntryOrder { get; } = new() { "meta", "gamestate", "ai" };

    public bool HasBinaryFormat => true;

    // TODO add Eu4TokenTypeMap

    public IFloatConverter FloatConverter => new Eu4AndHoi4FloatConverter();

    public bool HasHeader(Stream input) => input.PeekString(3) == "EU4";

    public ISaveGameHeader ReadHeader(Stream input, bool isMain)
    {
        string text = input.ReadString(6);
        return new SaveGameHeader
        {
            Text = text,
            Format = text == "EU4bin" ? SaveGameFormat.UncompressedBinary : SaveGameFormat.UncompressedText,
        };
    }

    public void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        if (!parameters.SaveGameFormat.IsCompressed())
            output.WriteString("EU4txt\n", TextEncoding);
    }

    public void WriteEntryHeader(ISaveGame saveGame, INode entryNode, Stream output, WriteParameters parameters)
    {
        output.WriteString("EU4txt\n", TextEncoding);
    }

    // When writing uncompressed, skip all but the last checksum.
    public bool ShouldWriteSection(bool isCompressed, INode entryNode, INode section) =>
        isCompressed || entryNode.Content.Text == "ai" || section.Content.Text != "checksum";

    public void Normalize(ISaveGame saveGame) => Eu4SaveGameNormalizer.Normalize(saveGame);

    public IGameVersion GetVersion(ISaveGame saveGame)
    {
        IGameVersion version = GameVersion.UNKNOWN;
        foreach (var other in saveGame.Root.GetDescendants("meta", "savegame_versions")
            .Select(x => new GameVersion(x.Content.Text)))
        {
            if (version.IsLessThan(other))
                version = other;
        }
        return version;
    }

    public void DisableIronman(ISaveGame saveGame) => saveGame.Root["meta"].RemoveChild("is_ironman");
}
