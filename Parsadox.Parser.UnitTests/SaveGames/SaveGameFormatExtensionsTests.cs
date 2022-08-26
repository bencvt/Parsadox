namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGameFormat))]
[TestCovers(typeof(SaveGameCompression))]
[TestCovers(typeof(SaveGameFormatExtensions))]
public class SaveGameFormatExtensionsTests : TestsBase
{
    [TestCase(SaveGameFormat.Unknown, ExpectedResult = false)]
    [TestCase(SaveGameFormat.UncompressedText, ExpectedResult = false)]
    [TestCase(SaveGameFormat.UncompressedBinary, ExpectedResult = true)]
    [TestCase(SaveGameFormat.CompressedAuto, ExpectedResult = false)]
    [TestCase(SaveGameFormat.CompressedText, ExpectedResult = false)]
    [TestCase(SaveGameFormat.CompressedBinary, ExpectedResult = true)]
    public bool IsBinary_Format_IsCorrect(SaveGameFormat format) => format.IsBinary();

    [TestCase(SaveGameFormat.Unknown, ExpectedResult = false)]
    [TestCase(SaveGameFormat.UncompressedText, ExpectedResult = false)]
    [TestCase(SaveGameFormat.UncompressedBinary, ExpectedResult = false)]
    [TestCase(SaveGameFormat.CompressedAuto, ExpectedResult = true)]
    [TestCase(SaveGameFormat.CompressedText, ExpectedResult = true)]
    [TestCase(SaveGameFormat.CompressedBinary, ExpectedResult = true)]
    public bool IsCompressed_Format_IsCorrect(SaveGameFormat format) => format.IsCompressed();

    [TestCase("gamestate={}")]
    [TestCase("gamestate={} meta={}")]
    public void ValidateCanWrite_Valid_DoesNotThrow(string raw)
    {
        Assert.That(() => SaveGameFormat.Unknown.ValidateCanWrite(new MockSaveGame(raw), _ => true),
            Throws.Nothing);
    }

    [Test]
    public void ValidateCanWrite_InvalidFormat_Throws()
    {
        Assert.That(() => SaveGameFormat.Unknown.ValidateCanWrite(new MockSaveGame("gamestate={}"), _ => false),
            Throws.ArgumentException);
    }

    [TestCase("")]
    [TestCase("gamestate={} gamestate={}")]
    [TestCase("gamestate={} GameState={}")]
    public void ValidateCanWrite_BadData_Throws(string raw)
    {
        Assert.That(() => SaveGameFormat.Unknown.ValidateCanWrite(new MockSaveGame(raw), _ => true),
            Throws.ArgumentException);
    }

    [Test]
    public void ValidateCanWrite_UncompressedButGameIsAlwaysCompressed_Throws()
    {
        Assert.That(() => SaveGameFormat.UncompressedText.ValidateCanWrite(new MockSaveGame("gamestate={}") { Game = Game.Stellaris }, _ => true),
            Throws.ArgumentException);
    }

    [Test]
    public void ValidateCanWrite_CompressedButGameIsNeverCompressed_Throws()
    {
        Assert.That(() => SaveGameFormat.CompressedText.ValidateCanWrite(new MockSaveGame("gamestate={}") { Game = Game.Vic2 }, _ => true),
            Throws.ArgumentException);
    }
}
