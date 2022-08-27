namespace Parsadox.Parser.SaveGames;

/// <summary>
/// When loading an uncompressed save game, every section will be in gamestate.
/// <para/>
/// This would cause issues when writing the game using a compressed format,
/// which requires that the sections are split into meta, gamestate, and ai.
/// <para/>
/// To prevent this, do the splitting at load time.
/// </summary>
internal static class Eu4SaveGameNormalizer
{
    private static readonly Dictionary<string, HashSet<string>> ENTRY_KEYS = new()
    {
        ["ai"] = new() { "ai" },
        ["meta"] = new()
        {
            "date",
            "save_game",
            "player",
            "displayed_country_name",
            "savegame_version",
            "savegame_versions",
            "is_ironman",
            "multi_player",
            "not_observer",
            "campaign_id",
            "campaign_length",
            "campaign_stats",
        },
    };

    internal static void Normalize(ISaveGame saveGame)
    {
        // Create missing entries and move nodes to the correct entry.
        foreach (var (entryName, entryKeys) in ENTRY_KEYS)
        {
            if (!saveGame.Root.HasChild(entryName))
            {
                List<INode> nodes = new();
                foreach (var entry in saveGame.Root)
                    nodes.AddRange(entry.RemoveAllChildren(x => entryKeys.Contains(x.Content.Text)));

                saveGame.Root.Children.Add(NodeFactory.Create(entryName).SetChildren(nodes));
            }
        }

        // Ensure each entry has a checksum node.
        string checksum = saveGame.State.GetDescendants("checksum").FirstOrDefault()?.ValueOrNull?.Text
            ?? "00000000000000000000000000000000";
        foreach (var entryNode in saveGame.Root)
        {
            if (!entryNode.HasChild("checksum"))
                entryNode.Children.Add(NodeFactory.Create("checksum").SetValue(checksum));
        }
    }
}
