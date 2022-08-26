namespace Parsadox.Parser.UnitTests.Tokens;

public class BinaryTokenReaderAbortIfUnmappedTests : BinaryTokenReaderTestsBase
{
    [Test]
    public void ReadAll_HasUnmappedAndAbortIfUnmappedTrue_Throws()
    {
        Assert.That(() => ReadAll(new byte[] { 0xad, 0xde }), Throws.TypeOf<ParseException>());
    }

    [Test]
    public void ReadAll_HasUnmappedAndAbortIfUnmappedFalse_DoesNotThrow()
    {
        _abortIfUnmapped = false;

        string result = ReadAll(new byte[] { 0xad, 0xde });

        Assert.That(result, Is.EqualTo("0xdead"));
    }

    [Test]
    public void ReadOnlySections_UnmappedInReadSection_Throws()
    {
        var bytes = new ByteBuilder()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendCode(0xdead).AppendClose()
            .AppendCode(0x2222).AppendEquals().AppendOpen().AppendCode(0x3333).AppendClose()
            .ToArrayAndDispose();

        Assert.That(() => ReadOnlySections(bytes, isBlocklist: true, "token2"), Throws.TypeOf<ParseException>());
    }

    [Test]
    public void ReadOnlySections_UnmappedInSkippedSection_DoesNotThrow()
    {
        var bytes = new ByteBuilder()
            .AppendCode(0x1111).AppendEquals().AppendOpen().AppendCode(0xdead).AppendClose()
            .AppendCode(0x2222).AppendEquals().AppendOpen().AppendCode(0x3333).AppendClose()
            .ToArrayAndDispose();

        string result = ReadOnlySections(bytes, isBlocklist: true, "token1");

        Assert.That(result, Is.EqualTo("token2 = { token3 }"));
    }
}
