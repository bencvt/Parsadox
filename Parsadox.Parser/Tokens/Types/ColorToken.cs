namespace Parsadox.Parser.Tokens.Types;

public class ColorToken : IToken, INodeContent
{
    public static readonly ColorToken RGB = new(BinaryTokenCodes.RGB, "rgb");
    public static readonly ColorToken HEX = new(BinaryTokenCodes.HEX, "hex");
    public static readonly ColorToken HSV = new(BinaryTokenCodes.HSV, "hsv");
    public static readonly ColorToken HSV360 = new(BinaryTokenCodes.HSV360, "hsv360");

    public static readonly ColorToken[] ALL = new[] { RGB, HEX, HSV, HSV360 };

    public ushort Code { get; }

    private readonly string _text;
    public string Text
    {
        get => _text;
        set => throw NodeContentException.CreateReadOnly(this);
    }

    private ColorToken(ushort code, string text)
    {
        Code = code;
        _text = text;
    }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;

    public static ColorToken? From(IToken token)
    {
        if (token is ColorToken)
            return token as ColorToken;
        if (token is GenericTextToken)
            return ALL.FirstOrDefault(x => x.Text == token.Text);
        return null;
    }

    public static ColorToken Parse(string text)
    {
        string lower = text.Trim().ToLowerInvariant();
        return ALL.SingleOrDefault(x => x.Text == lower)
            ?? throw new ArgumentException($"Expecting valid color token value like \"rgb\", got {Strings.EscapeAndQuote(text)}");
    }
}
