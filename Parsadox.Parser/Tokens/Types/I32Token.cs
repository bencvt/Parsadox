namespace Parsadox.Parser.Tokens.Types;

public class I32Token : IToken, INodeContent
{
    public I32Token(int number) { AsI32 = number; }

    public ushort Code => BinaryTokenCodes.I32;

    public string Text
    {
        get => AsI32.ToString();
        set => AsI32 = int.Parse(value);
    }

    public int AsI32 { get; set; }

    public GameDate AsGameDate => GameDate.FromI32(AsI32);

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
