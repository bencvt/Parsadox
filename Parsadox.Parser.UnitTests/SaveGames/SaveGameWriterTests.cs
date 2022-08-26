namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGameWriter))]
public class SaveGameWriterTests : TestsBase
{
    private const string TEST_DATA = "key1={ a { b { c } } }\nkey2=value";

    private static readonly TextWriter MockStreamWriter = new StreamWriter(Stream.Null);

    [Test]
    public void WriteFile_InvalidFormat_Throws()
    {
        var saveGame = CreateSaveGame(Game.Unknown, TEST_DATA);

        Assert.That(() => saveGame.WriteFile("some_file.txt", new() { SaveGameFormat = SaveGameFormat.Unknown }),
            Throws.TypeOf<AggregateException>()
            .And.InnerException.TypeOf<ArgumentException>()
            .And.Message.Contains("Invalid format"));
    }

    [TestCaseSource(nameof(GetAllWriteParameters))]
    public void WriteFile_Valid_IsWritten((WriteParameters? writeParameters, long expectedResult) testCase)
    {
        var saveGame = CreateSaveGame(Game.Unknown, TEST_DATA);
        using MemoryStream stream = new();
        _ = new MockFileSystem
        {
            OpenCreateReturns = stream,
        };

        long result = saveGame.WriteFile("some_file.txt", testCase.writeParameters);

        Assert.That(result, Is.EqualTo(testCase.expectedResult));
    }

    private static IEnumerable<(WriteParameters?, long)> GetAllWriteParameters()
    {
        const long UNCOMPRESSED_TEXT_MINIMAL_LENGTH = 24;
        const long UNCOMPRESSED_TEXT_FULL_LENGTH = 41;
        const long COMPRESSED_TEXT_MINIMAL_LENGTH = 158;
        const long COMPRESSED_TEXT_FULL_LENGTH = 172;

        yield return (null, COMPRESSED_TEXT_FULL_LENGTH);

        // UncompressedText
        yield return (new WriteParameters
        {
            SaveGameFormat = SaveGameFormat.UncompressedText,
            NodeOutputFormat = NodeOutputFormat.MinimalWithNewLines,
            Log = MockStreamWriter,
        }, UNCOMPRESSED_TEXT_MINIMAL_LENGTH);
        yield return (new WriteParameters
        {
            SaveGameFormat = SaveGameFormat.UncompressedText,
            NodeOutputFormat = NodeOutputFormat.Full,
            Log = MockStreamWriter,
        }, UNCOMPRESSED_TEXT_FULL_LENGTH);

        // CompressedText
        yield return (new WriteParameters
        {
            SaveGameFormat = SaveGameFormat.CompressedText,
            NodeOutputFormat = NodeOutputFormat.MinimalWithNewLines,
            Log = MockStreamWriter,
        }, COMPRESSED_TEXT_MINIMAL_LENGTH);
        yield return (new WriteParameters
        {
            SaveGameFormat = SaveGameFormat.CompressedText,
            NodeOutputFormat = NodeOutputFormat.Full,
            Log = MockStreamWriter,
        }, COMPRESSED_TEXT_FULL_LENGTH);
    }

    [Test]
    public void WriteFile_MainHeaderAndUncompressedText_IsWritten()
    {
        var saveGame = CreateSaveGame(Game.Ck3, TEST_DATA);
        using MemoryStream stream = new();
        _ = new MockFileSystem
        {
            OpenCreateReturns = stream,
        };

        long result = saveGame.WriteFile("some_file.txt", new() { SaveGameFormat = SaveGameFormat.UncompressedText });
        byte[] bytes = stream.ToArray();
        string content = Ck3Handler.TextEncoding.GetString(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(bytes.Length));
            Assert.That(content, Is.EqualTo("SAV01000000000000000000\nkey1={\n\ta\n\t{\n\t\tb\n\t\t{ c }\n\t}\n}\nkey2=value\n"));
        });
    }

    [Test]
    public void WriteFile_MainHeaderAndCompressedText_IsWritten()
    {
        var saveGame = CreateSaveGame(Game.Ck3, TEST_DATA);
        using MemoryStream stream = new();
        _ = new MockFileSystem
        {
            OpenCreateReturns = stream,
        };

        long result = saveGame.WriteFile("some_file.txt", new() { SaveGameFormat = SaveGameFormat.CompressedText });
        byte[] bytes = stream.ToArray();
        string content = Ck3Handler.TextEncoding.GetString(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(bytes.Length));
            Assert.That(content, Does.StartWith("SAV01020000000000000000\nPK"));
        });
    }

    [Test]
    public void WriteFile_MainHeaderAndMetaDataAndCompressedText_IsWritten()
    {
        var saveGame = CreateSaveGame(Game.Ck3, "meta_data={x y z}" + TEST_DATA);
        using MemoryStream stream = new();
        _ = new MockFileSystem
        {
            OpenCreateReturns = stream,
        };

        long result = saveGame.WriteFile("some_file.txt", new() { SaveGameFormat = SaveGameFormat.CompressedText });
        byte[] bytes = stream.ToArray();
        string content = Ck3Handler.TextEncoding.GetString(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(bytes.Length));
            Assert.That(content, Does.StartWith("SAV01020000000000000014\nmeta_data={ x y z }\nPK"));
        });
    }

    [Test]
    public void WriteStream_MultipleEntriesInRoot_IsWritten()
    {
        var saveGame = CreateSaveGame(Game.Ck3, TEST_DATA);
        saveGame.Root.Children.Add(NodeFactory.Create("extra_data").SetChildren(new()
        {
            NodeFactory.Create("extra_key").SetValue("extra_value"),
        }));
        using MemoryStream stream = new();

        long result = saveGame.WriteStream(stream, new() { SaveGameFormat = SaveGameFormat.UncompressedText });
        byte[] bytes = stream.ToArray();
        string content = Ck3Handler.TextEncoding.GetString(bytes);

        Assert.That(content, Is.EqualTo("SAV01000000000000000000\n" +
            "key1={\n\ta\n\t{\n\t\tb\n\t\t{ c }\n\t}\n}\nkey2=value\nextra_key=\"extra_value\"\n"));
    }

    private static ISaveGame CreateSaveGame(Game game, string content)
    {
        using var stream = CreateStream(content);
        return SaveGameFactory.LoadStream(game, stream);
    }
}
