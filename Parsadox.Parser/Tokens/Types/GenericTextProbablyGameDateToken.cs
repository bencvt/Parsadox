namespace Parsadox.Parser.Tokens.Types;

/// <summary>
/// Separate class from <see cref="GenericTextToken"/> to avoid unnecessary parsing.
/// </summary>
public class GenericTextProbablyGameDateToken : IToken
{
    public GenericTextProbablyGameDateToken(string text) { Text = text; }

    public ushort Code => default;

    public string Text { get; set; }

    public INodeContent AsNodeContent
    {
        get
        {
            if (GameDate.TryParse(Text, out GameDate date))
                return NodeContentFactory.CreateGameDate(date);
            return NodeContentFactory.CreateQuotedString(Text);
        }
    }

    public override string ToString() => GameDate.TryParse(Text, out GameDate date) ? date.ToString() : Strings.Quote(Text);
}
