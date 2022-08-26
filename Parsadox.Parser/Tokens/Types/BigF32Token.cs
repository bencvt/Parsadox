namespace Parsadox.Parser.Tokens.Types;

/// <summary>
/// Created when <see cref="IFloatConverter.IsBig"/> is true.
/// <para/>
/// Same data as <see cref="F32Token"/> but different text conversion logic.
/// </summary>
public class BigF32Token : IToken, INodeContent
{
    public BigF32Token(float number) { AsF32 = number; }

    public ushort Code => BinaryTokenCodes.F32;

    public string Text
    {
        get => Strings.BigF32ToString(AsF32);
        set => AsF32 = float.Parse(value);
    }

    public float AsF32 { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
