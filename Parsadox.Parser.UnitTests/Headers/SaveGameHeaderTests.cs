namespace Parsadox.Parser.UnitTests.Headers;

[TestCovers(typeof(ISaveGameHeader))]
[TestCovers(typeof(SaveGameHeader))]
public class SaveGameHeaderTests : TestsBase
{
    [Test]
    public void Ctor_Valid_IsCreated()
    {
        using var input = CreateStream(string.Empty);

        var header = Handler.ReadHeader(input, isMain: true);

        Assert.Multiple(() =>
        {
            Assert.That(header.FileName, Is.Null);
            Assert.That(header.Text, Is.Empty);
            Assert.That(header.Version.IsUnknown, Is.True);
            Assert.That(header.Format, Is.EqualTo(SaveGameFormat.Unknown));
            Assert.That(header.BytesUntilContent, Is.Zero);
            Assert.That(header.ToString()?.NoCr(), Is.EqualTo(
                "SaveGameHeader:\n  FileName=\n  Text=\"\"\n  " +
                "Version=Unknown\n  Format=Unknown\n  BytesUntilContent=0\n"));
        });
    }
}
