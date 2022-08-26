using static Parsadox.Parser.SaveGames.SaveGameTokenReader;

namespace Parsadox.Parser.SaveGames;

internal class SaveGameTokenReader : SaveGameReaderBase<Item, TokenContainer>
{
    internal record Item(ISaveGameHeader EntryHeader, List<IToken> Tokens);

    internal SaveGameTokenReader(ReadParameters parameters) : base(parameters) { }

    protected override bool ShouldExtractCodes => true;

    protected override bool ShouldIncludeComments => true;

    protected override Item ProcessTokens(ISaveGameHeader entryHeader, IEnumerable<IToken> tokens) =>
        new(entryHeader, tokens.ToList());

    protected override TokenContainer Merge(ISaveGameHeader mainHeader, IEnumerable<Item> items) =>
        new(items.ToDictionary(x => x.EntryHeader, x => x.Tokens));
}
