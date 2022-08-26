namespace Parsadox.Parser.Headers;

internal class ImperatorSaveGameHeader : Ck3AndImperatorSaveGameHeaderBase
{
    private static readonly HashSet<string> META_KEYS = new()
    {
        "save_game_version",
        "version",
        "date",
        "ironman",
        "meta_player_name",
        "enabled_dlcs",
        "started",
    };

    internal ImperatorSaveGameHeader(Stream input) : base(input) { }

    internal ImperatorSaveGameHeader(string text) : base(text) { }

    protected override string ExportMetaData(INode root, WriteParameters parameters)
    {
        StringBuilder builder = new();
        foreach (var node in root["gamestate"].TakeWhile(x => META_KEYS.Contains(x.Content.Text)))
        {
            builder.Append(NodeExporter.Export(node, parameters.NodeOutputFormat));
            if (parameters.NodeOutputFormat != NodeOutputFormat.Full)
                builder.Append('\n');
        }
        return builder.ToString();
    }
}
