namespace Parsadox.Parser.Tokens.Types;

/// <summary>
/// Created when <see cref="IFloatConverter.IsBig"/> is true.
/// <para/>
/// Same data as <see cref="F64Token"/> but different text conversion logic.
/// </summary>
public class BigF64Token : IToken, INodeContent
{
    public BigF64Token(double number) { AsF64 = number; }

    public ushort Code => BinaryTokenCodes.F64;

    public string Text
    {
        get => Strings.BigF64ToString(AsF64);
        set => AsF64 = double.Parse(value);
    }

    public double AsF64 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
