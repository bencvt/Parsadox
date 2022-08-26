namespace Parsadox.Parser.Nodes;

/// <summary>
/// See <see cref="ReadParameters.UseLazyParsing"/>.
/// </summary>
internal class LazyNodeBuilder : NodeBuilder
{
    internal LazyNodeBuilder(IEnumerable<IToken> tokens, ITokenTypeMap map) : base(tokens, map) { }

    protected override INode GetArray(IToken? key, IToken? parent)
    {
        List<IToken> buffer = new();
        int depth = 1;
        while (true)
        {
            if (_tokens.Current.IsClose)
            {
                depth--;
                if (depth == 0)
                    break;
            }
            else if (_tokens.Current.IsOpen)
                depth++;

            buffer.Add(_tokens.Current);

            if (!_tokens.MoveNext())
                throw new ParseException($"Unexpected end of token stream in {parent}.{key}");
        }
        var content = key?.AsNodeContent ?? NodeContentFactory.CreateAnonymousArrayKey();
        var getChildrenFunc = () => new LazyNodeBuilder(buffer, _map).BuildStart(key, parent).ToList();
        return new LazyNode(content, getChildrenFunc);
    }
}
