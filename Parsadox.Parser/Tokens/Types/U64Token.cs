namespace Parsadox.Parser.Tokens.Types;

public class U64Token : IToken, INodeContent
{
    public U64Token(ulong number) { AsU64 = number; }

    public ushort Code => BinaryTokenCodes.U64;

    public string Text
    {
        get => AsU64.ToString();
        set => AsU64 = ulong.Parse(value);
    }

    public ulong AsU64 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
