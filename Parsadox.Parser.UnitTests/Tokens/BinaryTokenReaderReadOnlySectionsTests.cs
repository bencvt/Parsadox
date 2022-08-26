namespace Parsadox.Parser.UnitTests.Tokens;

public class BinaryTokenReaderReadOnlySectionsTests : BinaryTokenReaderTestsBase
{
    private static IEnumerable<BytesTest> GetTestCases()
    {
        yield return new BytesTest(new ByteBuilder(), "");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111),
            "token1");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x2222),
            "");
        yield return new BytesTest(new ByteBuilder().AppendBool(true),
            "");
        yield return new BytesTest(new ByteBuilder().AppendQuotedString("value"),
            "");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendCode(0x2222).AppendCode(0x1111).AppendCode(0x2222),
            "token1 token1");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendQuotedString("value"),
            "token1 = \"value\"");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x2222).AppendEquals().AppendQuotedString("value"),
            "");
        yield return new BytesTest(new ByteBuilder().AppendOpen().AppendClose(),
            "");
        yield return new BytesTest(new ByteBuilder().AppendU64(1234).AppendEquals().AppendRgb().AppendOpen().AppendI32(255).AppendI32(0).AppendI32(0).AppendClose(),
            "");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendU32(1234),
            "token1 = 1234");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendF32(12.34f),
            "token1 = 12.340000");
        yield return new BytesTest(new ByteBuilder().AppendCode(0x1111).AppendEquals().AppendF64(12.34),
            "token1 = 12.34");
        yield return new BytesTest(new ByteBuilder().AppendOpen().AppendClose().AppendCode(0x1111).AppendOpen().AppendClose(),
            "token1");
        yield return new BytesTest(new ByteBuilder()
            .AppendCode(0x2222).AppendEquals().AppendOpen().AppendCode(0x3333).AppendBool(true).AppendF32(1.1f).AppendF64(2.2)
            .AppendI32(-333).AppendU32(444).AppendU64(555).AppendQuotedString("value").AppendClose(),
            "");
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void ReadOnlySections_Allowlist_IsCorrect(BytesTest arg)
    {
        string result = ReadOnlySections(arg.Bytes, isBlocklist: false, "token1", "token3");

        Assert.That(result, Is.EqualTo(arg.ExpectedResult));
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void ReadOnlySections_Blocklist_IsCorrect(BytesTest arg)
    {
        string result = ReadOnlySections(arg.Bytes, isBlocklist: true, "token2", "1234", "yes", "value", "");

        Assert.That(result, Is.EqualTo(arg.ExpectedResult));
    }

    [Test]
    public void ReadOnlySections_AllowlistWithEmptyValue_IsCorrect()
    {
        var bytes = new ByteBuilder().AppendOpen().AppendClose()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendBool(true).AppendClose()
            .AppendOpen().AppendClose().AppendCode(0x1111);

        string result = ReadOnlySections(bytes.ToArray(), isBlocklist: false, "token1", "");

        Assert.That(result, Is.EqualTo("{ } token1 = { yes } { } token1"));
    }

    [Test]
    public void ReadOnlySections_AllowlistNoEmptyValue_IsCorrect()
    {
        var bytes = new ByteBuilder().AppendOpen().AppendClose()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendBool(true).AppendClose()
            .AppendOpen().AppendClose().AppendCode(0x1111);

        string result = ReadOnlySections(bytes.ToArray(), isBlocklist: false, "token1");

        Assert.That(result, Is.EqualTo("token1 = { yes } token1"));
    }

    [Test]
    public void ReadOnlySections_BlocklistWithEmptyValue_IsCorrect()
    {
        var bytes = new ByteBuilder().AppendOpen().AppendClose()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendBool(true).AppendClose()
            .AppendOpen().AppendClose().AppendCode(0x1111);

        string result = ReadOnlySections(bytes.ToArray(), isBlocklist: true, "");

        Assert.That(result, Is.EqualTo("token1 = { yes } token1"));
    }

    [Test]
    public void ReadOnlySections_BlocklistNoEmptyValue_IsCorrect()
    {
        var bytes = new ByteBuilder().AppendOpen().AppendClose()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendBool(true).AppendClose()
            .AppendOpen().AppendClose().AppendCode(0x1111);

        string result = ReadOnlySections(bytes.ToArray(), isBlocklist: true);

        Assert.That(result, Is.EqualTo("{ } token1 = { yes } { } token1"));
    }
}
