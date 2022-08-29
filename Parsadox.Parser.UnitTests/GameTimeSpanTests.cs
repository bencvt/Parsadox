namespace Parsadox.Parser.UnitTests;

[TestCovers(typeof(GameTimeSpan))]
public class GameTimeSpanTests : TestsBase
{
    [TestCase(0, 0, 0, "0 days")]
    [TestCase(0, 1, 0, "1 day")]
    [TestCase(0, 0, 1, "1 hour")]
    [TestCase(0, 1, 1, "1 day and 1 hour")]
    [TestCase(1, 0, 0, "1 year")]
    [TestCase(1, 0, 1, "1 year, 0 days, and 1 hour")]
    [TestCase(1, 1, 0, "1 year and 1 day")]
    [TestCase(1, 1, 1, "1 year, 1 day, and 1 hour")]
    [TestCase(0, 0, 48, "2 days")]
    [TestCase(0, -10, 48, "-8 days")]
    [TestCase(0, 0, 10_000, "1 year, 51 days, and 16 hours")]
    public void Ctor_Valid_IsCreated(int years, int days, int hours, string expected)
    {
        var actual = new GameTimeSpan(years, days, hours);

        Assert.That(actual.ToString(), Is.EqualTo(expected));
    }
}
