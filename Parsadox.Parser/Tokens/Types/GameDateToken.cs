namespace Parsadox.Parser.Tokens.Types;

public class GameDateToken : IToken, INodeContent
{
    public GameDateToken(GameDate date) { AsGameDate = date; }

    public ushort Code => BinaryTokenCodes.I32;

    public string Text
    {
        get => AsGameDate.ToString();
        set => AsGameDate = GameDate.Parse(value);
    }

    public GameDate AsGameDate { get; set; }

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
