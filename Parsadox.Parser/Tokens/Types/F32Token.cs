namespace Parsadox.Parser.Tokens.Types;

public class F32Token : IToken, INodeContent
{
    public F32Token(float number) { AsF32 = number; }

    public ushort Code => BinaryTokenCodes.F32;

    public string Text
    {
        get => Strings.F32ToString(AsF32);
        set => AsF32 = float.Parse(value);
    }

    public float AsF32 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
