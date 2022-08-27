namespace Parsadox.Parser.Tokens.Types;

/// <summary>
/// Won't be created by a <see cref="ITokenReader"/>, but can be created manually.
/// <para/>
/// Decimal is a decent default numeric type.
/// </summary>
public class DecimalToken : IToken, INodeContent
{
    public DecimalToken(decimal number) { AsDecimal = number; }

    public ushort Code => BinaryTokenCodes.F64;

    public string Text
    {
        get => AsDecimal.ToString();
        set => AsDecimal = decimal.Parse(value);
    }

    public decimal AsDecimal { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
