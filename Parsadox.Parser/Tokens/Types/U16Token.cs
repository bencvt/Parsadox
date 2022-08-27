namespace Parsadox.Parser.Tokens.Types;

public class U16Token : IToken, INodeContent
{
    public U16Token(ushort code) { Code = code; }

    public ushort Code { get; set; }

    public string Text
    {
        get => $"0x{Code:x4}";
        set
        {
            if (!value.StartsWith("0x"))
                throw new FormatException("Value must start with 0x");
            Code = Convert.ToUInt16(value, 16);
        }
    }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
