namespace Parsadox.Parser.Nodes;

public enum NodeOutputFormat
{
    Minimal = 0,

    /// <summary>
    /// Same size as <see cref="Minimal"/>, but with newlines instead of spaces.
    /// <para/>
    /// Useful in case the minimized content will be opened in a text editor
    /// that has trouble with extremely long lines.
    /// </summary>
    MinimalWithNewLines,

    /// <summary>
    /// Human-readable: tab-indented with newlines and comments.
    /// </summary>
    Full,
}
