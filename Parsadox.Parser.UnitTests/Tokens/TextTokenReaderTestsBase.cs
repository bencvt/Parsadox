namespace Parsadox.Parser.UnitTests.Tokens;

public abstract class TextTokenReaderTestsBase : TestsBase
{
    protected virtual bool ShouldIncludeComments => false;

    protected string ReadAll(string raw, char tokenSeparator)
    {
        using var stream = CreateStream(raw);
        using var reader = new TextTokenReader(stream, Handler, progress: null, CancellationToken.None);

        var tokens = reader.ReadAll(ShouldIncludeComments);
        return string.Join(tokenSeparator, tokens.Select(x => $"{x}"));
    }

    protected static string ReadOnlySections(string raw, char tokenSeparator, bool isBlocklist, params string[] filter)
    {
        using var stream = CreateStream(raw);
        using var reader = new TextTokenReader(stream, Handler, progress: null, CancellationToken.None);

        var tokens = reader.ReadOnlySections(isBlocklist, filter.ToHashSet());
        return string.Join(tokenSeparator, tokens.Select(x => $"{x}"));
    }
}
