using Parsadox.Parser.Tokens;

namespace Parsadox.Parser.UnitTests.Tokens;

public class BinaryTokenReaderReadAllTests : BinaryTokenReaderTestsBase
{
    private static IEnumerable<BytesTest> GetTestCases()
    {
        yield return new BytesTest(new ByteBuilder(), "");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111),
            "token1");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x2222),
            "token2");
        yield return new BytesTest(new ByteBuilder().AppendBool(true),
            "yes");
        yield return new BytesTest(new ByteBuilder().AppendQuotedString("value"),
            "\"value\"");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendCode(0x2222).AppendCode(0x1111).AppendCode(0x2222),
            "token1 token2 token1 token2");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendQuotedString("value"),
            "token1 = \"value\"");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x2222).AppendEquals().AppendQuotedString("value"),
            "token2 = \"value\"");
        yield return new BytesTest(new ByteBuilder().AppendOpen().AppendClose(),
            "{ }");
        yield return new BytesTest(new ByteBuilder().AppendU64(1234).AppendEquals().AppendRgb().AppendOpen().AppendI32(255).AppendI32(0).AppendI32(0).AppendClose(),
            "1234 = rgb { 255 0 0 }");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendU32(1234),
            "token1 = 1234");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendF32(12.34f),
            "token1 = 12.340000");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendF64(12.34),
            "token1 = 12.34");
        yield return new BytesTest(new ByteBuilder().AppendOpen().AppendClose().AppendCode(0x1111).AppendOpen().AppendClose(),
            "{ } token1 { }");
        yield return new BytesTest(new ByteBuilder()
            .AppendCode(0x2222).AppendEquals().AppendOpen().AppendCode(0x3333).AppendBool(true).AppendF32(1.1f).AppendF64(2.2)
            .AppendI32(-333).AppendU32(444).AppendU64(555).AppendQuotedString("value").AppendClose(),
            "token2 = { token3 yes 1.100000 2.2 -333 444 555 \"value\" }");
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void ReadAll_Valid_IsCorrect(BytesTest arg)
    {
        string result = ReadAll(arg.Bytes);

        Assert.That(result, Is.EqualTo(arg.ExpectedResult));
    }
}
