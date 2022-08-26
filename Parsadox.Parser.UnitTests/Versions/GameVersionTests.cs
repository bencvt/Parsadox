namespace Parsadox.Parser.UnitTests.Versions;

[TestCovers(typeof(GameVersion))]
[TestCovers(typeof(IGameVersion))]
public class GameVersionTests : TestsBase
{
    [TestCase("", 0, 0, 0, 0)]
    [TestCase("no numbers", 0, 0, 0, 0)]
    [TestCase("1", 1, 0, 0, 0)]
    [TestCase("1.6", 1, 6, 0, 0)]
    [TestCase("1.6.1", 1, 6, 1, 0)]
    [TestCase("1.6.1.2", 1, 6, 1, 2)]
    [TestCase("1.6.1.2.3", 1, 6, 1, 2)]
    [TestCase("Cepheus v3.4.3", 3, 4, 3, 0)]
    [TestCase(" Cepheus v 3 . 4 . 3 ", 3, 4, 3, 0)]
    [TestCase("1.15.2", 1, 15, 2, 0)]
    [TestCase("1.2147483647.2", 1, int.MaxValue, 2, 0)]
    [TestCase("1.2147483648.2", 1, 0, 2, 0)]
    [TestCase("1.9999999999999.2", 1, 0, 2, 0)]
    [TestCase("1.-42.2", 1, 42, 2, 0)]
    [TestCase("1.0x32.2", 1, 0, 32, 2)]
    public void Ctor_Valid_IsCorrect(string text, int expectedMajor, int expectedMinor, int expectedPatch, int expectedPatchMinor)
    {
        GameVersion version = new(text);

        Assert.Multiple(() =>
        {
            Assert.That(version.Text, Is.EqualTo(text));
            Assert.That(version.Major, Is.EqualTo(expectedMajor));
            Assert.That(version.Minor, Is.EqualTo(expectedMinor));
            Assert.That(version.Patch, Is.EqualTo(expectedPatch));
            Assert.That(version.PatchMinor, Is.EqualTo(expectedPatchMinor));
            Assert.That(version.IsLessThan(version), Is.False);
        });
    }

    [TestCase("1.2.3", "1.2.3", false, false)]
    [TestCase("1.2", "1.2.3", true, false)]
    [TestCase("1.2", "1.2.0", false, false)]
    [TestCase("1.2", "1.123", true, false)]
    [TestCase("foo", "bar", false, false)]
    [TestCase("1.2", "Neat v1.2", false, false)]
    public void IsLessThan_Valid_IsCorrect(string a, string b, bool expectedALessThanB, bool expectedBLessThanA)
    {
        GameVersion versionA = new(a);
        GameVersion versionB = new(b);

        Assert.Multiple(() =>
        {
            Assert.That(versionA.IsLessThan(versionB), Is.EqualTo(expectedALessThanB));
            Assert.That(versionB.IsLessThan(versionA), Is.EqualTo(expectedBLessThanA));
        });
    }

    [Test]
    public void IsLessThan_VersionVsUnknown_IsTrue()
    {
        Assert.That(new GameVersion("1.2.3").IsLessThan(GameVersion.UNKNOWN), Is.True);
    }

    [Test]
    public void IsLessThan_UnknownVsUnknown_IsTrue()
    {
        Assert.That(GameVersion.UNKNOWN.IsLessThan(GameVersion.UNKNOWN), Is.True);
    }
}
