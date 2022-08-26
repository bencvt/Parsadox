namespace Parsadox.Parser.Tokens.Types;

public class F64Token : IToken, INodeContent
{
    public F64Token(double number) { AsF64 = number; }

    public ushort Code => BinaryTokenCodes.F64;

    public string Text
    {
        get => Strings.F64ToString(AsF64);
        set => AsF64 = double.Parse(value);
    }

    public double AsF64 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
