namespace Parsadox.Parser.Tokens;

/// <summary>
/// A token is the smallest unit of data read from save game files.
/// <para/>
/// Because working with tokens is performance-critical, the classes
/// implementing this interface do not use base classes or generics.
/// <para/>
/// Where possible, the token classes also implement
/// <see cref="INodeContent"/>. This way, the node structure can
/// reference the token instances directly without having to
/// instantiate a new object or copy data.
/// </summary>
public interface IToken
{
    /// <summary>
    /// The binary value associated with this token, or 0x0000 if none.
    /// <para/>
    /// See also: <see cref="BinaryTokenCodes"/> and <see cref="ITokenMap"/>.
    /// </summary>
    ushort Code { get; }

    string Text { get; }

    INodeContent AsNodeContent { get; }

    bool IsOpen => ReferenceEquals(this, SpecialToken.OPEN);
    bool IsClose => ReferenceEquals(this, SpecialToken.CLOSE);
    bool IsEquals => ReferenceEquals(this, SpecialToken.EQUALS);
    bool IsComment => false;
}
