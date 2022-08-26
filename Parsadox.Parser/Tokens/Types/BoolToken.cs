namespace Parsadox.Parser.Tokens.Types;

public class BoolToken : IToken, INodeContent
{
    public BoolToken(bool value) { AsBool = value; }

    public ushort Code => BinaryTokenCodes.BOOL;

    public string Text
    {
        get => Strings.BoolToString(AsBool);
        set => AsBool = Strings.StringToBool(value);
    }

    public bool AsBool { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
