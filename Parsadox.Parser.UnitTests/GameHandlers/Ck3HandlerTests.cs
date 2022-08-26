namespace Parsadox.Parser.UnitTests.GameHandlers;

[TestCovers(typeof(Ck3Handler))]
public class Ck3HandlerTests : TestsBase
{
    [Test]
    public void HasHeader_Exists_ReturnsTrue()
    {
        using var input = CreateStream("SAV");

        bool result = Ck3Handler.HasHeader(input);

        Assert.That(result, Is.True);
    }

    [TestCase("Sav")]
    [TestCase("sav")]
    [TestCase("abc")]
    public void HasHeader_DoesNotExist_ReturnsFalse(string first3)
    {
        using var input = CreateStream(first3);

        bool result = Ck3Handler.HasHeader(input);

        Assert.That(result, Is.False);
    }

    [Test]
    public void WriteMainHeader_Create_IsCorrect()
    {
        var saveGame = CreateSaveGame();

        string result = WriteMainHeader(saveGame, new() { SaveGameFormat = SaveGameFormat.CompressedText });

        Assert.That(result, Is.EqualTo("SAV01020000000000000014\nmeta_data={ a b c }\n"));
    }

    [Test]
    public void WriteMainHeader_UpdateCompressed_IsCorrect()
    {
        var saveGame = CreateSaveGame("SAV0103fefefefe000005ed\n");

        string result = WriteMainHeader(saveGame, new() { SaveGameFormat = SaveGameFormat.CompressedText });

        Assert.That(result, Is.EqualTo("SAV0102fefefefe00000014\nmeta_data={ a b c }\n"));
    }

    [Test]
    public void WriteMainHeader_UpdateUncompressed_IsCorrect()
    {
        var saveGame = CreateSaveGame("SAV0103fefefefe000005ed\n");

        string result = WriteMainHeader(saveGame, new() { SaveGameFormat = SaveGameFormat.UncompressedText });

        Assert.That(result, Is.EqualTo("SAV0100fefefefe00000014\n"));
    }

    [Test]
    public void WriteMainHeader_UpdateMinimal_IsCorrect()
    {
        var saveGame = CreateSaveGame("SAV0103fefefefe000005ed\n");

        string header = WriteMainHeader(saveGame, new()
        {
            SaveGameFormat = SaveGameFormat.CompressedText,
            NodeOutputFormat = NodeOutputFormat.MinimalWithNewLines,
        });

        Assert.That(header, Is.EqualTo("SAV0102fefefefe00000011\nmeta_data={a b c}"));
    }

    [Test]
    public void WriteMainHeader_Invalid_Throws()
    {
        var saveGame = CreateSaveGame("SAV0103fefefefe000005ed\n");

        using MemoryStream buffer = new();

        Assert.That(() => Ck3Handler.WriteMainHeader(saveGame, buffer, new() { SaveGameFormat = SaveGameFormat.Unknown }),
            Throws.ArgumentException);
    }

    [TestCase("SAV0100fefefefe000005ed\n", SaveGameFormat.UncompressedText, 0)]
    [TestCase("SAV0101fefefefe000005ed\n", SaveGameFormat.UncompressedBinary, 0)]
    [TestCase("SAV0102fefefefe000005ed\n", SaveGameFormat.CompressedText, 0x5ed)]
    [TestCase("SAV0103fefefefe000005ed\n", SaveGameFormat.CompressedBinary, 0x5ed)]
    [TestCase("SAV0104fefefefe000005ed\n", SaveGameFormat.Unknown, 0)]
    [TestCase("SAV0105fefefefe000005ed\n", SaveGameFormat.Unknown, 0)]
    [TestCase("SAV0106fefefefe000005ed\n", SaveGameFormat.Unknown, 0)]
    [TestCase("SAV00000000000000000000\n", SaveGameFormat.UncompressedText, 0)]
    [TestCase("SAV00000000000000000000\nother content", SaveGameFormat.UncompressedText, 0)]
    [TestCase("SAV00000000000000000000\r", SaveGameFormat.UncompressedText, 0)]
    [TestCase("SAV00000000000000000000\r\nother content", SaveGameFormat.UncompressedText, 0)]
    [TestCase("SAV00000000000000000000\rother content", SaveGameFormat.UncompressedText, 0)]
    public void ReadHeader_Valid_IsCorrect(string raw, SaveGameFormat expectedFormat, long expectedBytesUntilContent)
    {
        using var input = CreateStream(raw);

        var header = Ck3Handler.ReadHeader(input, isMain: true);

        Assert.Multiple(() =>
        {
            Assert.That(header.Text, Is.EqualTo(raw[..24].Replace('\r', '\n')));
            Assert.That(header.Format, Is.EqualTo(expectedFormat));
            Assert.That(header.BytesUntilContent, Is.EqualTo(expectedBytesUntilContent));
        });
    }

    [TestCase("SAV0100fefefefe000005ed")]
    [TestCase("sav0100fefefefe000005ed\n")]
    [TestCase("SAV0100FEFEFEFE000005ED\n")]
    public void ReadHeader_Invalid_Throws(string raw)
    {
        using var input = CreateStream(raw);

        Assert.That(() => Ck3Handler.ReadHeader(input, isMain: true),
            Throws.TypeOf<ParseException>());
    }

    private static ISaveGame CreateSaveGame(string headerText = "")
    {
        var result = new MockSaveGame("gamestate={ meta_data={a b c} other_data={x y z} }");
        result.Header.Text = headerText;
        return result;
    }

    private static string WriteMainHeader(ISaveGame saveGame, WriteParameters parameters)
    {
        using MemoryStream buffer = new();
        var gameHandler = Ck3Handler;
        gameHandler.WriteMainHeader(saveGame, buffer, parameters);
        return gameHandler.TextEncoding.GetString(buffer.ToArray());
    }
}
