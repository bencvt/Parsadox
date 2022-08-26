namespace Parsadox.Parser.Nodes;

/// <summary>
/// See <see cref="ReadParameters.UseLazyParsing"/>.
/// </summary>
internal class LazyNode : Node
{
    private Func<List<INode>>? _getChildrenFunc;
    private List<INode>? _children;

    internal LazyNode(INodeContent content, Func<List<INode>> getChildrenFunc)
        : base(content)
    {
        _getChildrenFunc = getChildrenFunc;
    }

    public override List<INode>? ChildrenOrNull
    {
        get
        {
            if (_getChildrenFunc is not null)
            {
                _children = _getChildrenFunc();
                _getChildrenFunc = null;
            }
            return _children;
        }
        set
        {
            _getChildrenFunc = null;
            _children = value;
        }
    }
}
