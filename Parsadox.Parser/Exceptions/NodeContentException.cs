using System.Runtime.CompilerServices;

namespace Parsadox.Parser.Exceptions;

/// <summary>
/// Thrown when attempting to access save game data that does not exist.
/// </summary>
public class NodeContentException : Exception
{
    public NodeContentException(string message) : base(message) { }

    public NodeContentException(INode node, string message)
        : this($"Node {Strings.EscapeAndQuote(node.Content)} {message}") { }

    public static NodeContentException CreateReadOnly(INodeContent content, [CallerMemberName] string memberName = "") =>
        new($"{content.GetType().Name}.{memberName} is read-only");
}
