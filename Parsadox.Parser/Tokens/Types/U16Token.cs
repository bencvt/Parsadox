namespace Parsadox.Parser.Tokens.Types;

public class U16Token : IToken, INodeContent
{
    public U16Token(ushort code) { Code = code; }

    public ushort Code { get; set; }

    public string Text
    {
        get => $"0x{Code:x4}";
        set => Code = ushort.Parse(value);
    }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
