namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGameTokenFactory))]
[TestCovers(typeof(SaveGameTokenReader))]
[TestCovers(typeof(TokenContainer))]
public class SaveGameTokenReaderTests : TestsBase
{
    [Test]
    public void LoadFile_Valid_IsLoaded()
    {
        _ = new MockFileSystem
        {
            FileExistsReturns = true,
            OpenReadReturns = CreateText(),
        };

        var tokens = SaveGameTokenFactory.LoadFile(Game.Unknown, "some_file.txt");

        Assert.That(tokens, Has.Count.EqualTo(10));
    }

    [Test]
    public void LoadStream_Text_IsLoaded()
    {
        using var stream = CreateText();

        var tokens = SaveGameTokenFactory.LoadStream(Game.Unknown, stream);

        Assert.That(tokens, Has.Count.EqualTo(10));
    }

    [Test]
    public void LoadStreamDump_Text_IsCorrect()
    {
        using var stream = CreateText();
        var tokens = SaveGameTokenFactory.LoadStream(Game.Unknown, stream);
        using StringWriter writer = new();

        tokens.Dump(writer);
        string result = writer.ToString().NoCr();

        Assert.That(result, Is.EqualTo(
            "# gamestate\n" +
            "0x0000\tkey1\n" +
            "0x0001\t=\n" +
            "0x0003\t{\n" +
            "0x0000\t\ta\n" +
            "0x0000\t\tb\n" +
            "0x0000\t\tc\n" +
            "0x0004\t}\n" +
            "0x0000\tkey2\n" +
            "0x0001\t=\n" +
            "0x0000\tvalue\n" +
            "\n" +
            "# Found 0 unmapped codes\n"));
    }

    [Test]
    public void LoadTokensFromSaveGameStream_BinaryUnmapped_Throws()
    {
        using var stream = CreateBinary();

        Assert.That(() => SaveGameTokenFactory.LoadStream(Game.Unknown, stream),
            Throws.TypeOf<AggregateException>().And.Message.Contains("0x1111 is not in the token map, which has 0 codes"));
    }

    [Test]
    public void LoadTokensFromSaveGameStream_Binary_IsLoaded()
    {
        using var stream = CreateBinary();

        var tokens = SaveGameTokenFactory.LoadStream(Game.Unknown, stream, new() { AbortIfUnmapped = false });

        Assert.That(tokens, Has.Count.EqualTo(8));
    }

    [Test]
    public void LoadTokensFromSaveGameStreamDump_BinaryUnmapped_IsCorrect()
    {
        using var stream = CreateBinary();
        var tokens = SaveGameTokenFactory.LoadStream(Game.Unknown, stream, new() { AbortIfUnmapped = false });
        using StringWriter writer = new();

        tokens.Dump(writer);
        string result = writer.ToString().NoCr();

        Assert.That(result, Is.EqualTo(
            "# gamestate\n" +
            "0x1111\t0x1111\n" +
            "0x0001\t=\n" +
            "0x0003\t{\n" +
            "0x000c\t\t53138160\n" +
            "0x0004\t}\n" +
            "0x2222\t0x2222\n" +
            "0x0001\t=\n" +
            "0x000f\t\"value\"\n" +
            "\n" +
            "# Found 2 unmapped codes\n" +
            "0x1111: 1 time\n" +
            "0x2222: 1 time\n"));
    }

    [Test]
    public void LoadTokensFromSaveGameStreamDump_BinaryMapped_IsCorrect()
    {
        var tokenMap = TokenMapFactory.Create().LoadString("0x1111 key1\n0x2222 key2");
        using var stream = CreateBinary();
        var tokens = SaveGameTokenFactory.LoadStream(Game.Unknown, stream, new() { TokenMap = tokenMap });
        using StringWriter writer = new();

        tokens.Dump(writer);
        string result = writer.ToString().NoCr();

        Assert.That(result, Is.EqualTo(
            "# gamestate\n" +
            "0x1111\tkey1\n" +
            "0x0001\t=\n" +
            "0x0003\t{\n" +
            "0x000c\t\t53138160\n" +
            "0x0004\t}\n" +
            "0x2222\tkey2\n" +
            "0x0001\t=\n" +
            "0x000f\t\"value\"\n" +
            "\n" +
            "# Found 0 unmapped codes\n"));
    }

    private static MemoryStream CreateText() => CreateStream("key1={ a b c }\nkey2=value");

    private static MemoryStream CreateBinary() => new ByteBuilder()
        .AppendCode(0x1111).AppendEquals().AppendOpen().AppendI32(53138160).AppendClose()
        .AppendCode(0x2222).AppendEquals().AppendQuotedString("value")
        .ResetPosition();
}
