namespace Parsadox.Parser.Tokens.Types;

public class CommentToken : IToken, INodeContent
{
    public CommentToken(string text) { Text = text; }

    public ushort Code => default;

    public string Text { get; set; }

    public INodeContent AsNodeContent => this;

    public bool IsComment => true;

    public override string ToString() => Strings.Comment(Text, 0).TrimEnd('\n');
}
