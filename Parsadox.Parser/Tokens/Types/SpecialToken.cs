namespace Parsadox.Parser.Tokens.Types;

public record SpecialToken(ushort Code, string Text) : IToken
{
    public static readonly SpecialToken EMPTY = new(default, string.Empty);

    public static readonly SpecialToken OPEN = new(BinaryTokenCodes.OPEN, "{");
    public static readonly SpecialToken CLOSE = new(BinaryTokenCodes.CLOSE, "}");
    public static readonly SpecialToken EQUALS = new(BinaryTokenCodes.EQUALS, "=");

    // Avoid creating redundant token instances
    public static readonly SpecialToken YES = new(BinaryTokenCodes.BOOL, "yes");
    public static readonly SpecialToken NO = new(BinaryTokenCodes.BOOL, "no");

    public INodeContent AsNodeContent
    {
        get
        {
            if (ReferenceEquals(this, YES))
                return NodeContentFactory.CreateBool(true);
            if (ReferenceEquals(this, NO))
                return NodeContentFactory.CreateBool(false);
            throw new ParseException($"Structural token cannot be converted to {nameof(INodeContent)}: " +
                $"{nameof(SpecialToken)}(0x{Code:x4}, {Strings.EscapeAndQuote(Text)})");
        }
    }

    public override string ToString() => Text;
}
