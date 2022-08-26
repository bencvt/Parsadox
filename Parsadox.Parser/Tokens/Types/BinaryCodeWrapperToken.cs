namespace Parsadox.Parser.Tokens.Types;

public record BinaryCodeWrapperToken(ushort Code, IToken Inner) : IToken
{
    public string Text => Inner.Text;

    public INodeContent AsNodeContent => Inner.AsNodeContent;

    public override string ToString() => Inner.ToString()!;
}
