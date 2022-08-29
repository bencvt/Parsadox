namespace Parsadox.Parser.UnitTests.GameAssets;

[TestCovers(typeof(GameAssetFactory))]
public class GameAssetFactoryTests : TestsBase
{
    private MockFileSystem mockFileSystem = new();

    [SetUp]
    public void SetUp()
    {
        mockFileSystem = new()
        {
            FileExistsReturns = true,
            DirectoryExistsReturns = true,
        };
    }

    [TestCase(null)]
    [TestCase("path")]
    public void Ctor_PathDoesNotExist_Throws(string? path)
    {
        mockFileSystem = new();

        Assert.That(() => new GameAssetFactory(Game.Unknown, path), Throws.ArgumentException);
    }

    [Test]
    public void Ctor_ExplicitPathExists_IsCreated()
    {
        GameAssetFactory factory = new(Game.Unknown, "path");

        Assert.That(factory.InstallDirectory, Is.EqualTo("path"));
    }

    [Test]
    public void Ctor_AutoPathExists_IsCreated()
    {
        GameAssetFactory factory = new(Game.Unknown);

        Assert.That(factory.InstallDirectory, Is.EqualTo(mockFileSystem.DirectoryExistsCalls.First()));
    }

    [Test]
    public void LoadText_Exists_IsRead()
    {
        mockFileSystem.ReadAllTextReturns = "content";

        string actual = new GameAssetFactory(Game.Unknown).LoadText("file.txt");

        Assert.That(actual, Is.EqualTo("content"));
    }

    [Test]
    public void LoadNodes_Exists_IsRead()
    {
        mockFileSystem.EnumerateFilesReturns = new[] { "file1.txt", "file2.txt" };
        mockFileSystem.ReadAllTextReturns = "key={a=1 b=2}";

        var node = new GameAssetFactory(Game.Unknown).LoadNodes("path");

        Assert.That(node.Dump(), Is.EqualTo("{key={a=1 b=2}key={a=1 b=2}}"));
    }

    [Test]
    public void LoadNodeMap_Exists_IsRead()
    {
        mockFileSystem.EnumerateFilesReturns = new[] { "file.txt" };
        mockFileSystem.ReadAllTextReturns = "key={a=1 b=2}";

        var map = new GameAssetFactory(Game.Unknown).LoadNodeMap("path");

        Assert.Multiple(() =>
        {
            Assert.That(map, Has.Count.EqualTo(1));
            Assert.That(map.Keys.Single(), Is.EqualTo("key"));
            Assert.That(map.Values.Single().Dump(), Is.EqualTo("key={a=1 b=2}"));
        });
    }

    [Test]
    public void LoadYamlLocalizationMap_Exists_IsRead()
    {
        mockFileSystem.ReadAllTextReturns = "root:\n # comment\n key1:0 \"value1\"\n\n\n key2:1 \"value with \"quotes\"\"";

        var map = new GameAssetFactory(Game.Unknown).LoadYamlLocalizationMap("path");

        Assert.That(map, Is.EqualTo(new Dictionary<string, string>
        {
            ["key1"] = "value1",
            ["key2"] = "value with \"quotes\"",
        }));
    }
}
