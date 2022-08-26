using System.Collections;

namespace Parsadox.Parser.Nodes;

internal class Node : INode
{
    internal Node(INodeContent content)
    {
        Content = content;
    }

    public INodeContent Content { get; set; }

    public INode SetContent(INodeContent content)
    {
        Content = content;
        return this;
    }
    public INode SetContent(string text) => SetContent(NodeContentFactory.CreateQuotedString(text));
    public INode SetContent(decimal value) => SetContent(NodeContentFactory.CreateDecimal(value));
    public INode SetContent(bool value) => SetContent(NodeContentFactory.CreateBool(value));
    public INode SetContent(GameDate date) => SetContent(NodeContentFactory.CreateGameDate(date));
    public INode SetContentI32(int value) => SetContent(NodeContentFactory.CreateI32(value));
    public INode SetContentU32(uint value) => SetContent(NodeContentFactory.CreateU32(value));
    public INode SetContentU64(ulong value) => SetContent(NodeContentFactory.CreateU64(value));
    public INode SetContentF32(float value, Game? game = null) => SetContent(NodeContentFactory.CreateF32(value, game));
    public INode SetContentF64(double value, Game? game = null) => SetContent(NodeContentFactory.CreateF64(value, game));

    public INodeContent Value
    {
        get => ValueOrNull ?? throw new NodeContentException(this, "does not have a value");
        set => ValueOrNull = value;
    }

    public INodeContent? ValueOrNull { get; set; }

    public bool HasValue => ValueOrNull is not null;

    public INode SetValue(INodeContent value)
    {
        ValueOrNull = value;
        return this;
    }
    public INode SetValue(string text) => SetValue(NodeContentFactory.CreateQuotedString(text));
    public INode SetValue(decimal value) => SetValue(NodeContentFactory.CreateDecimal(value));
    public INode SetValue(bool value) => SetValue(NodeContentFactory.CreateBool(value));
    public INode SetValue(GameDate date) => SetValue(NodeContentFactory.CreateGameDate(date));
    public INode SetValueI32(int value) => SetValue(NodeContentFactory.CreateI32(value));
    public INode SetValueU32(uint value) => SetValue(NodeContentFactory.CreateU32(value));
    public INode SetValueU64(ulong value) => SetValue(NodeContentFactory.CreateU64(value));
    public INode SetValueF32(float value, Game? game = null) => SetValue(NodeContentFactory.CreateF32(value, game));
    public INode SetValueF64(double value, Game? game = null) => SetValue(NodeContentFactory.CreateF64(value, game));

    public INode SetColorHex(string value) => SetValue(ColorToken.HEX)
        .SetChildren(new()
        {
            NodeFactory.Create(value)
        });
    public INode SetColorHsv(double hue, double saturation, double value) => SetValue(ColorToken.HSV)
        .SetChildren(new()
        {
            NodeFactory.Create().SetContentF64(hue),
            NodeFactory.Create().SetContentF64(saturation),
            NodeFactory.Create().SetContentF64(value),
        });
    public INode SetColorHsv360(uint hue, uint saturation, uint value) => SetValue(ColorToken.HSV360)
        .SetChildren(new()
        {
            NodeFactory.Create().SetContentU32(hue),
            NodeFactory.Create().SetContentU32(saturation),
            NodeFactory.Create().SetContentU32(value),
        });
    public INode SetColorRgb(uint red, uint green, uint blue) => SetValue(ColorToken.RGB)
        .SetChildren(new()
        {
            NodeFactory.Create().SetContentU32(red),
            NodeFactory.Create().SetContentU32(green),
            NodeFactory.Create().SetContentU32(blue),
        });

    public List<INode> Children
    {
        get => ChildrenOrNull ?? throw new NodeContentException(this, "does not have storage for children nodes");
        set => ChildrenOrNull = value;
    }

    public virtual List<INode>? ChildrenOrNull { get; set; }

    public IEnumerable<INode> ChildrenOrEmpty => ChildrenOrNull ?? Enumerable.Empty<INode>();

    public INode SetChildren(List<INode> children)
    {
        Children = children;
        return this;
    }

    public bool HasChildrenStorage => ChildrenOrNull is not null;

    public bool HasChild(string key) => ChildrenOrEmpty.Any(x => x.Content.Text == key);

    public INode? GetChildOrNull(params string[] keys) => keys
        .Select(key => ChildrenOrEmpty.FirstOrDefault(x => x.Content.Text == key))
        .FirstOrDefault(x => x is not null);

    public INode GetChildOrThrow(params string[] keys) => keys
        .Select(key => Children.FirstOrDefault(x => x.Content.Text == key))
        .FirstOrDefault(x => x is not null)
        ?? throw new NodeContentException(this, $"does not contain any {Strings.EscapeAndQuote(keys)}");

    public INode GetOrCreateChild(string key)
    {
        var child = GetChildOrNull(key);
        if (child is null)
        {
            if (!HasChildrenStorage)
                Children = new();
            child = NodeFactory.Create(key);
            Children.Add(child);
        }
        return child;
    }

    public IEnumerator<INode> GetEnumerator() => ChildrenOrEmpty.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ChildrenOrEmpty.GetEnumerator();

    public bool RemoveChild(string key) 
    {
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i].Content.Text == key)
            {
                Children.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public List<INode> RemoveAllChildren(Predicate<INode> match)
    {
        var removed = new List<INode>();
        for (int i = 0; i < Children.Count; i++)
        {
            if (match(Children[i]))
            {
                removed.Add(Children[i]);
                Children.RemoveAt(i);
                i--;
            }
        }
        return removed;
    }

    public List<INode> RemoveAllChildren(string key) => RemoveAllChildren(x => x.Content.Text == key);

    public IEnumerable<INode> GetDescendants(params string[] path)
    {
        if (path.Length == 0)
            return Enumerable.Repeat(this, 1);

        return ChildrenOrEmpty
            .Where(x => x.Content.Text == path[0])
            .SelectMany(x => x.GetDescendants(path[1..]));
    }

    public INode? GetDescendantOrNull(params string[] path) => GetDescendants(path).FirstOrDefault();

    public INode this[string key] => GetChildOrThrow(key);

    public long GetSize() => 1L + (ChildrenOrNull?.Sum(x => x.GetSize()) ?? 0L);

    public string Dump(NodeOutputFormat format = NodeOutputFormat.Minimal) => NodeExporter.Export(this, format);

    public override string ToString() => NodeExporter.Export(this, NodeOutputFormat.Minimal, maxDepth: 0).Trim();
}
