namespace Parsadox.Parser.UnitTests.Headers;

[TestCovers(typeof(Ck3SaveGameHeader))]
public class Ck3SaveGameHeaderTests : TestsBase
{
    [TestCase("01")]
    [TestCase("03")]
    public void Ctor_BinaryFormat_VersionIsSet(string format)
    {
        using var input = CreateInput(format);

        Ck3SaveGameHeader header = new(input);

        Assert.That(header.Version.Text, Is.EqualTo("4.5.6"));
    }

    [TestCase("00")]
    [TestCase("02")]
    [TestCase("04")]
    [TestCase("05")]
    [TestCase("06")]
    public void Ctor_NonBinaryFormat_VersionIsNotSet(string format)
    {
        using var input = CreateInput(format);

        Ck3SaveGameHeader header = new(input);

        Assert.That(header.Version.IsUnknown, Is.True);
    }

    private static Stream CreateInput(string format) => new ByteBuilder()
        .AppendUtf8($"SAV01{format}0000000000000000\n")
        .AppendCode(0x1111).AppendOpen()
        .AppendCode(0x2222).AppendEquals().AppendI32(123)
        .AppendCode(BinaryTokenCodes.VERSION).AppendEquals().AppendQuotedString("4.5.6")
        .AppendClose().ResetPosition();
}
