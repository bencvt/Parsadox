namespace Parsadox.Parser.Headers;

internal class Ck3SaveGameHeader : Ck3AndImperatorSaveGameHeaderBase
{
    internal Ck3SaveGameHeader(Stream input) : base(input) { }

    internal Ck3SaveGameHeader(string text) : base(text) { }

    protected override string ExportMetaData(INode state, WriteParameters parameters)
    {
        var node = state.FirstOrDefault();
        if (node is null || node.Content.Text != "meta_data")
            return string.Empty;
        return NodeExporter.Export(node, parameters.NodeOutputFormat);
    }
}
