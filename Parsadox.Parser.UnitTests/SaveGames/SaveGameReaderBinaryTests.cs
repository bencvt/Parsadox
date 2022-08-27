namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGameReader))]
public class SaveGameReaderBinaryTests : TestsBase
{
    [Test]
    public void LoadSaveGameFile_ValidBinary_IsLoaded()
    {
        _ = new MockFileSystem
        {
            FileExistsReturns = true,
            GetAbsolutePathReturns = "/path/to/some_file.txt",
            OpenReadReturns = new ByteBuilder()
                .AppendCode(0x1111).AppendEquals().AppendOpen().AppendI32(53138160).AppendClose()
                .AppendCode(0x2222).AppendEquals().AppendQuotedString("some value")
                .ResetPosition(),
        };

        var saveGame = SaveGameFactory.LoadFile(Game.Unknown, "some_file.txt", new()
        {
            TokenMap = TokenMapFactory.Create().LoadString("0x1111 birth\n0x2222 key2"),
        });

        Assert.Multiple(() =>
        {
            Assert.That(saveGame.Game, Is.EqualTo(Game.Unknown));
            Assert.That(saveGame.Header.FileName, Is.EqualTo("/path/to/some_file.txt"));
            Assert.That(saveGame.Header.Text, Is.Empty);
            Assert.That(saveGame.Header.Format, Is.EqualTo(SaveGameFormat.UncompressedBinary));
            Assert.That(saveGame.Root.Dump(), Is.EqualTo("{gamestate={birth={1066.1.1}key2=\"some value\"}}"));
            Assert.That(saveGame.ToString, Is.EqualTo("Unknown save game loaded as UncompressedBinary from /path/to/some_file.txt"));
        });
    }

    [Test]
    public void LoadSaveGameFile_UnmappedBinary_Throws()
    {
        _ = new MockFileSystem
        {
            FileExistsReturns = true,
            GetAbsolutePathReturns = "/path/to/some_file.txt",
            OpenReadReturns = new ByteBuilder()
                .AppendCode(0x1111).AppendEquals().AppendOpen().AppendI32(53138160).AppendClose()
                .AppendCode(0x2222).AppendEquals().AppendQuotedString("value")
                .ResetPosition(),
        };

        Assert.That(() => SaveGameFactory.LoadFile(Game.Unknown, "some_file.txt"),
            Throws.TypeOf<AggregateException>().And.Message.Contains("0x1111 is not in the token map, which has 0 codes"));
    }
}
