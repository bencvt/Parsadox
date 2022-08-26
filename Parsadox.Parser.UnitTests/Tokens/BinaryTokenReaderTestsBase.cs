using System.Runtime.CompilerServices;

namespace Parsadox.Parser.UnitTests.Tokens;

[TestCovers(typeof(BinaryTokenCodes))]
public abstract class BinaryTokenReaderTestsBase : TestsBase
{
    protected readonly ITokenMap _tokenMap = TokenMapFactory.Create();
    protected bool _abortIfUnmapped;
    protected bool _shouldExtractCodes;

    [SetUp]
    public void SetUpBase()
    {
        _tokenMap.Clear();
        _tokenMap.CodeMap[0x1111] = "token1";
        _tokenMap.CodeMap[0x2222] = "token2";
        _tokenMap.CodeMap[0x3333] = "token3";

        _abortIfUnmapped = true;
        _shouldExtractCodes = false;
    }

    protected string ReadAll(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryTokenReader(stream, Handler, _tokenMap, _abortIfUnmapped, _shouldExtractCodes, progress: null, CancellationToken.None);

        var tokens = reader.ReadAll(shouldIncludeComments: false);
        return string.Join(' ', tokens.Select(x => $"{x}"));
    }

    protected string ReadOnlySections(byte[] bytes, bool isBlocklist, params string[] filter)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryTokenReader(stream, Handler, _tokenMap, _abortIfUnmapped, _shouldExtractCodes, progress: null, CancellationToken.None);

        var tokens = reader.ReadOnlySections(isBlocklist, filter.ToHashSet());
        return string.Join(' ', tokens.Select(x => $"{x}"));
    }

    public class BytesTest
    {
        private readonly int _lineNumber;

        public byte[] Bytes { get; }

        public string ExpectedResult { get; }

        internal BytesTest(ByteBuilder byteBuilder, string expectedResult, [CallerLineNumber] int lineNumber = 0)
        {
            _lineNumber = lineNumber;
            Bytes = byteBuilder.ToArray();
            byteBuilder.Dispose();
            ExpectedResult = expectedResult;
        }

        public override string ToString() => $"Line {_lineNumber}: {Bytes.Length} bytes => {ExpectedResult}";
    }
}
