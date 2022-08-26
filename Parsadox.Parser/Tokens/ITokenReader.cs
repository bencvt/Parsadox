namespace Parsadox.Parser.Tokens;

/// <summary>
/// Token readers are responsible for converting a stream of text or binary data
/// to <see cref="IToken"/> instances.
/// <para/>
/// The tokens are unstructured. They can be converted to a tree structure by
/// <see cref="NodeBuilder"/>.
/// </summary>
internal interface ITokenReader : IDisposable
{
    IEnumerable<IToken> ReadAll(bool shouldIncludeComments);

    /// <summary>
    /// Instantiating all tokens, even those that won't ever be referenced, is expensive.
    /// This method filters unneeded tokens while reading from file.
    /// </summary>
    IEnumerable<IToken> ReadOnlySections(bool isBlocklist, IReadOnlySet<string> filter);
}
