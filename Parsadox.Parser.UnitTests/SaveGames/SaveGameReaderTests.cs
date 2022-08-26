namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGame))]
[TestCovers(typeof(ISaveGame))]
[TestCovers(typeof(SaveGameReader))]
public class SaveGameReaderTests : TestsBase
{
    private const string TEST_DATA = "key1={ a { b { c } } }\nkey2=value";

    private static readonly TextWriter MockStreamWriter = new StreamWriter(Stream.Null);

    [TestCaseSource(nameof(GetAllReadParameters))]
    public void LoadSaveGameFile_ValidText_IsLoaded((int, ReadParameters? readParameters) testCase)
    {
        _ = new MockFileSystem
        {
            FileExistsReturns = true,
            GetAbsolutePathReturns = "/path/to/some_file.txt",
            OpenReadReturns = CreateStream(TEST_DATA),
        };

        var saveGame = SaveGameFactory.LoadFile(Game.Unknown, "some_file.txt", testCase.readParameters);

        Assert.Multiple(() =>
        {
            Assert.That(saveGame.Game, Is.EqualTo(Game.Unknown));
            Assert.That(saveGame.Header.FileName, Is.EqualTo("/path/to/some_file.txt"));
            Assert.That(saveGame.Header.Text, Is.Empty);
            Assert.That(saveGame.Header.Format, Is.EqualTo(SaveGameFormat.UncompressedText));
            Assert.That(saveGame.Root.Dump(), Is.EqualTo("{gamestate={key1={a{b{c}}}key2=value}}"));
            Assert.That(saveGame.ToString, Is.EqualTo("Unknown save game loaded as UncompressedText from /path/to/some_file.txt"));
        });
    }

    private static IEnumerable<(int, ReadParameters?)> GetAllReadParameters()
    {
        // count is necessary to ensure each test case has a unique name.
        int count = 0;
        yield return (count++, null);

        var either = new[] { true, false };
        foreach (bool abortIfUnmapped in either)
        foreach (bool makeTemporaryCopyOfInputFile in either)
        foreach (bool shouldFilter in either)
        foreach (bool isSectionFilterBlocklist in either)
        foreach (bool useLazyParsing in either)
        foreach (bool shouldLog in either)
        {
            HashSet<string>? filter = null;
            if (shouldFilter)
            {
                filter = new() { "key3" };
                if (!isSectionFilterBlocklist)
                {
                    filter.Add("key1");
                    filter.Add("key2");
                }
            }

            yield return (count++, new ReadParameters
            {
                Log = shouldLog ? MockStreamWriter : StreamWriter.Null,
                AbortIfUnmapped = abortIfUnmapped,
                MakeTemporaryCopyOfInputFile = makeTemporaryCopyOfInputFile,
                SectionFilter = filter,
                IsSectionFilterBlocklist = isSectionFilterBlocklist,
                UseLazyParsing = useLazyParsing,
            });
        }
    }

    [Test]
    public void LoadSaveGameStream_UncompressedText_IsLoaded()
    {
        using var input = CreateStream(TEST_DATA);

        var saveGame = SaveGameFactory.LoadStream(Game.Unknown, input);

        Assert.Multiple(() =>
        {
            Assert.That(saveGame.Game, Is.EqualTo(Game.Unknown));
            Assert.That(saveGame.Header.FileName, Is.Null);
            Assert.That(saveGame.Header.Text, Is.Empty);
            Assert.That(saveGame.Header.Format, Is.EqualTo(SaveGameFormat.UncompressedText));
            Assert.That(saveGame.Root.Dump(), Is.EqualTo("{gamestate={key1={a{b{c}}}key2=value}}"));
            Assert.That(saveGame.ToString, Is.EqualTo("Unknown save game loaded as UncompressedText"));
        });
    }

    [Test]
    public void LoadSaveGameStream_CompressedTextSingle_IsLoaded()
    {
        using MemoryStream stream = new();
        stream.Write(Ck3Handler.TextEncoding.GetBytes("SAV0102ffffffff00000000\n"));
        stream.Write(TestData.CompressedTextSingle);
        stream.Position = 0L;

        var saveGame = SaveGameFactory.LoadStream(Game.Ck3, stream);

        Assert.Multiple(() =>
        {
            Assert.That(saveGame.Game, Is.EqualTo(Game.Ck3));
            Assert.That(saveGame.Header.FileName, Is.Null);
            Assert.That(saveGame.Header.Text, Is.EqualTo("SAV0102ffffffff00000000\n"));
            Assert.That(saveGame.Header.Format, Is.EqualTo(SaveGameFormat.CompressedText));
            Assert.That(saveGame.Root.Dump(), Is.EqualTo("{gamestate={key1={a{b{c}}}key2=value}}"));
            Assert.That(saveGame.ToString, Is.EqualTo("Ck3 save game loaded as CompressedText"));
        });
    }

    [Test]
    public void LoadSaveGameStream_CompressedTextMultiple_IsLoaded()
    {
        using MemoryStream stream = new();
        stream.Write(Ck3Handler.TextEncoding.GetBytes("SAV0102ffffffff00000000\n"));
        stream.Write(TestData.CompressedTextMultiple);
        stream.Position = 0L;

        var saveGame = SaveGameFactory.LoadStream(Game.Ck3, stream);

        Assert.Multiple(() =>
        {
            Assert.That(saveGame.Game, Is.EqualTo(Game.Ck3));
            Assert.That(saveGame.Header.FileName, Is.Null);
            Assert.That(saveGame.Header.Text, Is.EqualTo("SAV0102ffffffff00000000\n"));
            Assert.That(saveGame.Header.Format, Is.EqualTo(SaveGameFormat.CompressedText));
            Assert.That(saveGame.Root.Dump(), Is.EqualTo(
                "{ai={key1=foo key2=bar key3=baz}" +
                "gamestate={key1={a{b{c}}}key2=value}" +
                "meta={key3={x{y{z}}}key4=value key5=value}}"));
            Assert.That(saveGame.ToString, Is.EqualTo("Ck3 save game loaded as CompressedText"));
        });
    }
}
