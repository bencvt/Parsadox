namespace Parsadox.Parser.Nodes;

internal static class NodeContentFactory
{
    internal static INodeContent CreateAnonymousArrayKey() => AnonymousArrayKey.Instance;
    internal static INodeContent CreateQuotedString(string text) => new QuotedStringToken(text);
    internal static INodeContent CreateComment(string content) => new CommentToken(content);
    internal static INodeContent CreateBool(bool value) => new BoolToken(value);
    internal static INodeContent CreateF32(float value, Game? game) => IsBig(game) ? new BigF32Token(value) : new F32Token(value);
    internal static INodeContent CreateF64(double value, Game? game) => IsBig(game) ? new BigF64Token(value) : new F64Token(value);
    internal static INodeContent CreateI32(int value) => new I32Token(value);
    internal static INodeContent CreateU32(uint value) => new U32Token(value);
    internal static INodeContent CreateU64(ulong value) => new U64Token(value);
    internal static INodeContent CreateDecimal(decimal value) => new DecimalToken(value);
    internal static INodeContent CreateGameDate(GameDate date) => new GameDateToken(date);

    private static bool IsBig(Game? game) => game?.GetDefaultVersionHandler().FloatConverter.IsBig ?? false;

    private class AnonymousArrayKey : INodeContent
    {
        internal static readonly AnonymousArrayKey Instance = new();

        public string Text
        {
            get => string.Empty;
            set => throw NodeContentException.CreateReadOnly(this);
        }
        public bool IsAnonymousArrayKey => true;
    }
}
