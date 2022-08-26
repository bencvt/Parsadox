namespace Parsadox.Parser.UnitTests;

[TestCovers(typeof(Game))]
[TestCovers(typeof(GameExtensions))]
[TestCovers(typeof(IGameHandler))]
public class GameExtensionsTests : TestsBase
{
    [Test]
    public void GetLocalSaveGameDirectory_DoesNotExist_IsSearched([Values] Game game)
    {
        var mockFileSystem = new MockFileSystem();

        string? dir = game.GetLocalSaveGameDirectory();

        Assert.Multiple(() =>
        {
            Assert.That(dir, Is.Null);
            Assert.That(mockFileSystem.DirectoryExistsCalls, Has.Count.EqualTo(4));
        });
    }

    [Test]
    public void GetInstallDirectory_DoesNotExist_IsSearched([Values] Game game)
    {
        var mockFileSystem = new MockFileSystem();

        string? dir = game.GetInstallDirectory();

        Assert.Multiple(() =>
        {
            Assert.That(dir, Is.Null);
            Assert.That(mockFileSystem.DirectoryExistsCalls, Has.Count.EqualTo(7));
        });
    }
}
