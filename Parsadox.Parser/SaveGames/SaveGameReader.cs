namespace Parsadox.Parser.SaveGames;

internal class SaveGameReader : SaveGameReaderBase<INode, ISaveGame>
{
    internal SaveGameReader(ReadParameters parameters) : base(parameters) { }

    protected override INode ProcessTokens(ISaveGameHeader entryHeader, IEnumerable<IToken> tokens)
    {
        string key = entryHeader.FileName ?? SaveGame.GAMESTATE;
        List<INode> children;

        using (Timed timed = new(_log, $"Built nodes for {key} in"))
        {
            var builder = _parameters.UseLazyParsing
                ? new LazyNodeBuilder(tokens, _gameHandler.TokenTypeMap)
                : new NodeBuilder(tokens, _gameHandler.TokenTypeMap);

            children = builder.Build().ToList();
        }

        return NodeFactory.Create(key).SetChildren(children);
    }

    protected override ISaveGame Merge(ISaveGameHeader mainHeader, IEnumerable<INode> items)
    {
        var nodes = _gameHandler.EntryOrder is null
            ? items.OrderBy(node => node.Content.Text)
            : items.OrderBy(node => _gameHandler.EntryOrder.FindIndex(x => x == node.Content.Text));

        var saveGame = new SaveGame
        {
            Game = _parameters.Game,
            Header = mainHeader,
            Root = NodeFactory.CreateAnonymousArray(nodes.ToList()),
        };

        _gameHandler.Normalize(saveGame);

        if (saveGame.Header.Version.IsUnknown)
            saveGame.Header.Version = _gameHandler.GetVersion(saveGame);

        return saveGame;
    }
}
