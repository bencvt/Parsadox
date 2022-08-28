namespace Parsadox.Parser.Nodes;

/// <summary>
/// Nodes are mutable structured data extracted from save games.
/// Each node has three elements:
/// <list type="number">
/// <item>Content: scalar, always present</item>
/// <item>Value: scalar, optional</item>
/// <item>Children: list of nodes, optional</item>
/// </list>
/// <para/>
/// A tree of nodes is created when loading a save game file using
/// <see cref="SaveGameFactory"/>.
/// <para/>
/// Nodes can be modified, deleted, or added as needed.
/// To create new nodes, use <see cref="GetOrCreateChild"/>.
/// For more complex structures, use <see cref="NodeFactory"/>.
/// </summary>
public interface INode : IEnumerable<INode>
{
    INodeContent Content { get; set; }

    INode SetContent(INodeContent content);
    INode SetContent(string text);
    INode SetContent(decimal value);
    INode SetContent(bool value);
    INode SetContent(GameDate date);
    INode SetContentI32(int value);
    INode SetContentU32(uint value);
    INode SetContentU64(ulong value);

    /// <summary>
    /// Use the optional game parameter to ensure the number output uses the
    /// game-standard number of decimal digits.
    /// </summary>
    INode SetContentF32(float value, Game? game = null);

    /// <summary>
    /// The optional game parameter ensures the number output uses the
    /// game-standard number of decimal digits.
    /// </summary>
    INode SetContentF64(double value, Game? game = null);

    /// <summary>
    /// Accessing Value when HasValue is false will throw an exception.
    /// </summary>
    INodeContent Value { get; set; }

    INodeContent? ValueOrNull { get; set; }

    /// <summary>
    /// Shorthand for <code>ValueOrNull is not null</code>
    /// </summary>
    bool HasValue => ValueOrNull is not null;

    INode SetValue(INodeContent value);
    INode SetValue(string text);
    INode SetValue(decimal value);
    INode SetValue(bool value);
    INode SetValue(GameDate date);
    INode SetValueI32(int value);
    INode SetValueU32(uint value);
    INode SetValueU64(ulong value);

    /// <summary>
    /// The optional game parameter ensures the number output uses the
    /// game-standard number of decimal digits.
    /// </summary>
    INode SetValueF32(float value, Game? game = null);

    /// <summary>
    /// The optional game parameter ensures the number output uses the
    /// game-standard number of decimal digits.
    /// </summary>
    INode SetValueF64(double value, Game? game = null);

    /// <summary>
    /// Set <see cref="Value"/> to "hex" and <see cref="Children"/> to {value}.
    /// </summary>
    INode SetColorHex(string value);

    /// <summary>
    /// Set <see cref="Value"/> to "hsv" and <see cref="Children"/> to {h, s, v}.
    /// </summary>
    INode SetColorHsv(double hue, double saturation, double value);

    /// <summary>
    /// Set <see cref="Value"/> to "hsv360" and <see cref="Children"/> to {h, s, v}.
    /// </summary>
    INode SetColorHsv360(uint hue, uint saturation, uint value);

    /// <summary>
    /// Set <see cref="Value"/> to "rgb" and <see cref="Children"/> to {r, g, b}.
    /// </summary>
    INode SetColorRgb(uint red, uint green, uint blue);

    /// <summary>
    /// Accessing Children when HasChildrenStorage is false will throw an exception.
    /// </summary>
    List<INode> Children { get; set; }

    List<INode>? ChildrenOrNull { get; set; }

    /// <remarks>
    /// Get an enumerable over this node's children, if there are any.
    /// <para/>
    /// No exception is thrown even if HasChildrenStorage is false.
    /// <para/>
    /// Note that <see cref="INode"/> implements <see cref="IEnumerable{INode}"/>,
    /// so the following are equivalent:
    /// <code>foreach (var x in node) DoSomething(x);</code>
    /// <code>foreach (var x in node.ChildrenOrEmpty) DoSomething(x);</code>
    /// </remarks>
    IEnumerable<INode> ChildrenOrEmpty { get; }

    INode SetChildren(List<INode> children);

    /// <summary>
    /// Shorthand for <code>ChildrenOrNull is not null</code>
    /// </summary>
    bool HasChildrenStorage => ChildrenOrNull is not null;

    /// <summary>
    /// Whether any of this node's children has a child node with the specified content.
    /// </summary>
    bool HasChild(string key);

    /// <summary>
    /// Get the first child matching one of the specified keys, or null if there is no such child.
    /// </summary>
    INode? GetChildOrNull(params string[] keys);

    /// <summary>
    /// Get the first child matching one of the specified keys, or throw an exception if there is no such child.
    /// </summary>
    INode GetChildOrThrow(params string[] keys);

    /// <summary>
    /// Get the first child matching the specified key, or create one if there is no such child.
    /// </summary>
    INode GetOrCreateChild(string key);

    /// <summary>
    /// Remove the first child node matching the specified key, if there is such a child.
    /// </summary>
    /// <returns>true if a node was removed</returns>
    bool RemoveChild(string key);

    /// <summary>
    /// Remove all child nodes matching the predicate.
    /// </summary>
    /// <returns>the removed nodes, if any</returns>
    List<INode> RemoveAllChildren(Predicate<INode> match);

    /// <summary>
    /// Shorthand for RemoveAllChildren(x => x.Content.Text == key).
    /// </summary>
    /// <returns>the removed nodes, if any</returns>
    List<INode> RemoveAllChildren(string key) => RemoveAllChildren(x => x.Content.Text == key);

    /// <summary>
    /// Recursively get all descendants matching the path. Example:
    /// <code>
    /// NodeFactory.LoadString("character={ id=111 family_data={ spouse=222 child=333 child=444 } } }")
    ///   .GetDescendants("character", "family_data", "child")
    ///   .Count() == 2
    /// </code>
    /// </summary>
    IEnumerable<INode> GetDescendants(params string[] path);

    /// <summary>
    /// Shorthand for GetDescendants(path).FirstOrDefault().
    /// </summary>
    INode? GetDescendantOrNull(params string[] path) => GetDescendants(path).FirstOrDefault();

    /// <summary>
    /// Get the first child matching the key, or throw an exception if there is no such child.
    /// </summary>
    INode this[string key] { get; }

    /// <summary>
    /// Recursively calculate how many nodes are in the tree.
    /// </summary>
    long GetSize();

    /// <summary>
    /// Recursively convert the node tree to text.
    /// </summary>
    string Dump(NodeOutputFormat format = NodeOutputFormat.Minimal);

    /// <summary>
    /// Shorthand for Dump(NodeOutputFormat.Full).
    /// </summary>
    string DumpIndented() => Dump(NodeOutputFormat.Full);
}
