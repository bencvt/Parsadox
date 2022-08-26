namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(SaveGameFactory))]
public class SaveGameFactoryTests : TestsBase
{
    [TestCase("")]
    [TestCase("none")]
    [TestCase("bad.txt")]
    [TestCase("bad.")]
    [TestCase("bad.bad")]
    public void LoadFile_UnknownExtension_Throws(string path)
    {
        Assert.That(() => SaveGameFactory.LoadFile(path), Throws.ArgumentException);
    }
}
