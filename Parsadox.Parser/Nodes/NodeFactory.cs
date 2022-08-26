namespace Parsadox.Parser.Nodes;

/// <summary>
/// Create <see cref="INode"/> instances.
/// </summary>
public static class NodeFactory
{
    /// <summary>
    /// Create a textual key with no value or children.
    /// </summary>
    public static INode Create(string key) =>
        new Node(NodeContentFactory.CreateQuotedString(key));

    /// <summary>
    /// Create an empty textual key with no value or children.
    /// </summary>
    public static INode Create() =>
        new Node(NodeContentFactory.CreateQuotedString(string.Empty));

    /// <summary>
    /// Create an unkeyed associative array to store child nodes.
    /// <para/>
    /// For a named array, use <see cref="Create(string)"/>, then <see cref="INode.SetChildren"/>.
    /// </summary>
    public static INode CreateAnonymousArray(List<INode> children) =>
        new Node(NodeContentFactory.CreateAnonymousArrayKey()).SetChildren(children);

    /// <summary>
    /// Create a comment that will be prefixed with # when output.
    /// </summary>
    public static INode CreateComment(string comment) =>
        new Node(NodeContentFactory.CreateComment(comment));

    /// <summary>
    /// Load nodes directly from a string.
    /// </summary>
    public static INode LoadString(string text)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return LoadStream(stream);
    }

    /// <summary>
    /// Load nodes directly from a stream of text.
    /// </summary>
    public static INode LoadStream(Stream stream)
    {
        // Text token parsing doesn't have any game-specific logic.
        var handler = Game.Unknown.GetDefaultVersionHandler();

        using var tokenReader = new TextTokenReader(stream, handler, progress: null, CancellationToken.None);
        var tokens = tokenReader.ReadAll(shouldIncludeComments: false);

        var nodes = new NodeBuilder(tokens, handler.TokenTypeMap).Build();
        return CreateAnonymousArray(nodes.ToList());
    }

    internal static INode FromToken(IToken keyToken) =>
        new Node(keyToken.AsNodeContent);

    internal static INode FromTokenPair(IToken keyToken, IToken valueToken) =>
        new Node(keyToken.AsNodeContent).SetValue(valueToken.AsNodeContent);
}
