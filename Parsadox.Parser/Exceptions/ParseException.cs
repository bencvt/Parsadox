namespace Parsadox.Parser.Exceptions;

/// <summary>
/// Thrown when invalid save game data is encountered.
/// </summary>
public class ParseException : Exception
{
    public ParseException(string message) : base(message) { }
}
