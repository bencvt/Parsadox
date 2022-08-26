namespace Parsadox.Parser.Nodes;

/// <summary>
/// Convert a node tree to text.
/// </summary>
internal static class NodeExporter
{
    internal static string Export(INode node, NodeOutputFormat format, int startDepth = 0, int? maxDepth = null)
    {
        StringBuilder builder = new();
        ExportRecurse(builder, node, format, startDepth, maxDepth);
        return builder.ToString();
    }

    private static void ExportRecurse(StringBuilder builder, INode node, NodeOutputFormat format, int depth, int? maxDepth)
    {
        bool isFull = format == NodeOutputFormat.Full;

        if (node.Content.IsComment)
        {
            if (isFull)
                builder.Append(Strings.Comment(node.Content.Text, depth));
            // Comment nodes should never have a value or children.
            return;
        }

        if (isFull)
            builder.Append('\t', depth);

        if (!node.Content.IsAnonymousArrayKey)
        {
            builder.Append(Strings.QuoteOnlyIfNeeded(node.Content.Text));
            if (node.HasValue || node.HasChildrenStorage)
                builder.Append('=');
        }
        // Assumption: AnonymousArrayKey nodes will always have HasValue=false
        // and HasChildrenStorage=true.
        //
        // Even if the node has been externally modified to make this incorrect,
        // the result will still be syntactically valid. It just won't parse
        // back to the same (invalid) structure.

        if (isFull)
            builder.Append(node.ValueOrNull);
        else if (node.HasValue)
            builder.Append(Strings.QuoteOnlyIfNeeded(node.Value.Text));

        if (node.HasChildrenStorage)
        {
            // An rgb node will have both a value and children.
            if (node.HasValue && isFull)
                builder.Append(' ');

            builder.Append('{');

            if (node.Any())
            {
                if (maxDepth.HasValue && depth >= maxDepth.Value)
                {
                    builder.Append("...");
                }
                else if (node.All(x => !x.HasChildrenStorage && !x.HasValue && !x.Content.IsComment))
                {
                    // Keep arrays of simple data on a single line.
                    if (isFull)
                        builder.Append(' ');
                    builder.Append(string.Join(' ', node.Select(x => x.ToString())));
                    if (isFull)
                        builder.Append(' ');
                }
                else if (isFull)
                {
                    // Recurse.
                    builder.Append('\n');
                    foreach (var child in node)
                        ExportRecurse(builder, child, format, depth + 1, maxDepth);
                    builder.Append('\t', depth);
                }
                else
                {
                    // Recurse with minimal output.
                    bool needSpace = false;
                    foreach (var child in node.Where(x => !x.Content.IsComment))
                    {
                        StringBuilder childBuilder = new();
                        ExportRecurse(childBuilder, child, format, depth + 1, maxDepth);
                        string text = childBuilder.ToString();
                        if (needSpace && !text.StartsWith('{') && !text.StartsWith('"'))
                            builder.Append(format == NodeOutputFormat.MinimalWithNewLines ? '\n' : ' ');
                        builder.Append(text);
                        needSpace = !text.EndsWith('}') && !text.EndsWith('"');
                    }
                }
            }

            builder.Append('}');
        }

        if (isFull)
            builder.Append('\n');
    }
}
