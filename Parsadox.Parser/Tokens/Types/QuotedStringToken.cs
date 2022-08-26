namespace Parsadox.Parser.Tokens.Types;

/// <remarks>
/// This is the only token type that can be directly created by either
/// <see cref="TextTokenReader"/> or <see cref="BinaryTokenReader"/>.
/// </remarks>
public class QuotedStringToken : IToken, INodeContent
{
    public QuotedStringToken(string text) { Text = text; }

    /// <remarks>
    /// May have been <see cref="BinaryTokenCodes.STRING"/>
    /// or <see cref="BinaryTokenCodes.STRING_ALT"/>.
    /// </remarks>
    public ushort Code => default;

    public string Text { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Strings.Quote(Text);
}
