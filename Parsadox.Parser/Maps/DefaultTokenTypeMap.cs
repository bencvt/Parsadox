namespace Parsadox.Parser.Maps;

/// <summary>
/// Assume all I32 tokens are <see cref="GameDate"/>s unless its value is too small,
/// in which case they're ints.
/// <para/>
/// This gives false positives for large ints (e.g., random seeds).
/// </summary>
internal class DefaultTokenTypeMap : ITokenTypeMap
{
    private readonly int _assumeIntIfLessThan;

    internal DefaultTokenTypeMap(GameDate assumeIntIfBefore)
    {
        _assumeIntIfLessThan = assumeIntIfBefore.ToI32();
    }

    public bool IsKeyGameDate(I32Token key, IToken? parent, IToken? grandparent) =>
        key.AsI32 >= _assumeIntIfLessThan;

    public bool IsValueGameDate(I32Token value, IToken key) =>
        value.AsI32 >= _assumeIntIfLessThan;
}
