namespace Parsadox.Parser.Tokens.Types;

public class U32Token : IToken, INodeContent
{
    public U32Token(uint number) { AsU32 = number; }

    public ushort Code => BinaryTokenCodes.U32;

    public string Text
    {
        get => AsU32.ToString();
        set => AsU32 = uint.Parse(value);
    }

    public uint AsU32 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
