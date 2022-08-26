namespace Parsadox.Parser.Maps;

/// <summary>
/// Determine whether an I32 token contains a <see cref="GameDate"/>
/// or just a regular int.
/// </summary>
internal interface ITokenTypeMap
{
    bool IsKeyGameDate(I32Token key, IToken? parent, IToken? grandparent);

    bool IsValueGameDate(I32Token value, IToken key);
}
