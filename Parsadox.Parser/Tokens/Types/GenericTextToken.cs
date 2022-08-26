namespace Parsadox.Parser.Tokens.Types;

public class GenericTextToken : IToken, INodeContent
{
    public GenericTextToken(string text) { Text = text; }

    public string Text { get; set; }

    public ushort Code => default;

    public INodeContent AsNodeContent => this;

    public override string ToString() => Text;
}
