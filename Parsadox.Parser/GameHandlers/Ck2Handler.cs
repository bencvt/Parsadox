namespace Parsadox.Parser.GameHandlers;

internal class Ck2Handler : IGameHandler
{
    public string Name => "Crusader Kings II";

    public string SaveGameExtension => ".ck2";

    public Encoding TextEncoding => Strings.Windows1252;

    public SaveGameCompression Compression => SaveGameCompression.Optional;

    // Compressed save games have a redundant meta entry.
    public string? DoNotLoadEntry => "meta";

    // Early versions of CK2 used binary, but it's all text now.
    public bool HasBinaryFormat => false;

    public bool HasHeader(Stream input) => input.PeekString(3) == "CK2";

    public ISaveGameHeader ReadHeader(Stream input, bool isMain)
    {
        string text = input.ReadString(6);
        if (text != "CK2txt")
            throw new ParseException($"Unsupported format: {text}");
        return new SaveGameHeader
        {
            Text = text,
            Format = SaveGameFormat.UncompressedText,
        };
    }

    public void WriteMainHeader(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        if (!parameters.SaveGameFormat.IsCompressed())
            output.WriteString("CK2txt\n", TextEncoding);
    }

    public void WriteEntryHeader(ISaveGame saveGame, INode entryNode, Stream output, WriteParameters parameters)
    {
        output.WriteString("CK2txt\n", TextEncoding);
    }

    private static readonly string[] META_SECTIONS = new[]
    {
        "version",
        "date",
        "player",
        "ironman",
        "seed",
        "count",
        "player_shield",
        "player_realm",
        "player_name",
        "player_age",
        "player_portrait",
        "game_rules",
        "game_speed",
        "mapmode",
        "playthrough_id",
        "checksum",
    };
    public IEnumerable<INode> GetEntryNodesToWrite(ISaveGame saveGame)
    {
        // [Re]create the meta entry.
        yield return NodeFactory.Create("meta").SetChildren(META_SECTIONS
            .SelectMany(x => saveGame.Root.GetDescendants("gamestate", x))
            .ToList());
        yield return saveGame.Root["gamestate"];
    }

    public string AdjustEntryName(string entryName, string? outputPath)
    {
        if (entryName != "gamestate")
            return entryName;
        var fileName = Path.GetFileName(outputPath);
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "game" + SaveGameExtension;
        return fileName;
    }

    // The "CK2txt" header is essentially a {, containing everything but the checksum.
    public int WriteIndentLevel => 1;

    public bool ShouldWriteSection(bool isCompressed, INode entryNode, INode section) => section.Content.Text != "checksum";

    public void WriteEntryFooter(ISaveGame saveGame, Stream output, WriteParameters parameters)
    {
        output.WriteString("}\n", TextEncoding);
        foreach (var node in saveGame.Root.GetDescendants("gamestate", "checksum"))
        {
            string text = NodeExporter.Export(node, parameters.NodeOutputFormat);
            output.WriteString(text, TextEncoding);
        }
    }

    // Compressed games have two files: meta (ignored) and the actual file,
    // which gets renamed to the standard "gamestate" here.
    public void Normalize(ISaveGame saveGame)
    {
        int count = saveGame.Root.Count();
        if (count != 1)
            throw new ParseException($"Expecting exactly one entry, found {count}: {{ {Strings.EscapeAndQuote(saveGame.Root.ChildrenOrEmpty)} }}");
        saveGame.Root.Single().Content.Text = "gamestate";
    }

    public IGameVersion GetVersion(ISaveGame saveGame) => GameVersion.Parse(saveGame, "gamestate", "version");

    public void DisableIronman(ISaveGame saveGame)
    {
        saveGame.Root["gamestate"].RemoveAllChildren("ironman");
        saveGame.Root["gamestate"].RemoveAllChildren("seed");
        saveGame.Root["gamestate"].RemoveAllChildren("count");
    }
}
