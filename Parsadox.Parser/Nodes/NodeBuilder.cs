namespace Parsadox.Parser.Nodes;

/// <summary>
/// Convert tokens to a tree of nodes.
/// </summary>
internal class NodeBuilder : SingleUseProcessor
{
    protected readonly IEnumerator<IToken> _tokens;
    protected readonly ITokenTypeMap _map;

    internal NodeBuilder(IEnumerable<IToken> tokens, ITokenTypeMap map)
    {
        // Simply filter out comments for now.
        // Parsing them is non-trivial, since they can occur anywhere,
        // e.g., "key=#comment\nvalue".
        _tokens = tokens.Where(x => !x.IsComment).GetEnumerator();
        _map = map;
    }

    internal IEnumerable<INode> Build() => BuildStart(null, null);

    protected IEnumerable<INode> BuildStart(IToken? parent, IToken? grandparent)
    {
        StartProcessingOrThrow();

        // Prime the pump so _tokens.Current is valid for the recursive method.
        if (!_tokens.MoveNext())
            return Enumerable.Empty<INode>();

        return BuildRecurse(parent, grandparent);
    }

    protected virtual INode GetArray(IToken? key, IToken? parent)
    {
        if (key is null)
            return NodeFactory.CreateAnonymousArray(BuildRecurse(SpecialToken.EMPTY, parent).ToList());

        return NodeFactory.FromToken(key).SetChildren(BuildRecurse(key, parent).ToList());
    }

    private IEnumerable<INode> BuildRecurse(IToken? parent, IToken? grandparent)
    {
        while (true)
        {
            if (_tokens.Current.IsOpen)
            {
                if (!_tokens.MoveNext())
                {
                    // {<eof>: Empty anonymous array
                    yield return NodeFactory.CreateAnonymousArray(new());
                    yield break;
                }
                else if (_tokens.Current.IsClose)
                {
                    // {}: Empty anonymous array
                    yield return NodeFactory.CreateAnonymousArray(new());
                }
                else
                {
                    // {...: Non-empty anonymous array - recurse
                    yield return GetArray(null, parent);
                }
            }
            else if (_tokens.Current.IsClose)
            {
                if (parent is not null)
                {
                    // ...}: Stop recursion
                    yield break;
                }
                // Else ignore extra }s at the root level
            }
            else if (_tokens.Current.IsEquals)
            {
                throw new ParseException($"Unexpected token: {_tokens.Current}");
            }
            else
            {
                IToken key = _tokens.Current;
                if (key is I32Token keyI32 && _map.IsKeyGameDate(keyI32, parent, grandparent))
                    key = new GameDateToken(keyI32.AsGameDate);

                if (!_tokens.MoveNext())
                {
                    // key<eof>: Key with no value
                    yield return NodeFactory.FromToken(key);
                    yield break;
                }
                if (!_tokens.Current.IsEquals)
                {
                    // key<x>: Key with no value, followed by unrelated node
                    yield return NodeFactory.FromToken(key);
                    continue;
                }
                // key=...
                if (!_tokens.MoveNext())
                {
                    // key=<eof>
                    throw new ParseException($"Unexpected end of token stream after {key}=");
                }
                if (_tokens.Current.IsOpen)
                {
                    // key={...
                    if (!_tokens.MoveNext())
                    {
                        // key={<eof>: Key with empty child array
                        yield return NodeFactory.FromToken(key).SetChildren(new());
                        yield break;
                    }
                    else if (_tokens.Current.IsClose)
                    {
                        // key={}: Key with empty child array
                        yield return NodeFactory.FromToken(key).SetChildren(new());
                    }
                    else
                    {
                        // key={...: Key with non-empty child array - recurse
                        yield return GetArray(key, parent);
                    }
                }
                else if (_tokens.Current.IsClose || _tokens.Current.IsEquals)
                {
                    throw new ParseException($"Unexpected token after {key}=: {_tokens.Current}");
                }
                else if (ColorToken.From(_tokens.Current) is not null)
                {
                    // key=rgb...
                    var node = NodeFactory.FromTokenPair(key, ColorToken.From(_tokens.Current)!);
                    if (!_tokens.MoveNext())
                    {
                        // key=rgb<eof>: Key with rgb value but missing components
                        yield return node;
                        yield break;
                    }
                    if (!_tokens.Current.IsOpen)
                    {
                        // key=rgb<x>: Key with rgb value but missing components, followed by unrelated node
                        yield return node;
                        continue;
                    }
                    // key=rgb{...: Key with rgb value and components
                    if (!_tokens.MoveNext())
                    {
                        // key=rgb{<eof>: Key with rgb value and empty components
                        node.Children = new();
                        yield return node;
                        yield break;
                    }
                    if (_tokens.Current.IsClose)
                    {
                        // key=rgb{}: Key with rgb value and empty components
                        node.Children = new();
                        yield return node;
                    }
                    else
                    {
                        // key=rgb{...: Key with rgb value and components - recurse, non-lazily
                        node.Children = BuildRecurse(key, parent).ToList();
                        yield return node;
                    }
                }
                else
                {
                    // key=value: Key with simple value
                    IToken value = _tokens.Current;
                    if (value is I32Token valueI32 && _map.IsValueGameDate(valueI32, key))
                        value = new GameDateToken(valueI32.AsGameDate);
                    yield return NodeFactory.FromTokenPair(key, value);
                }
            }

            if (!_tokens.MoveNext())
                yield break;
        }
    }
}
