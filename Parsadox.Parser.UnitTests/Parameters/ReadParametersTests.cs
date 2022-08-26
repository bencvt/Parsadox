namespace Parsadox.Parser.UnitTests.Parameters;

[TestCovers(typeof(ReadParameters))]
[TestCovers(typeof(MainParameters))]
public class ReadParametersTests : TestsBase
{
    [Test]
    public void DeepCopyAndInitialize_Valid_IsCopied()
    {
        var original = new ReadParameters
        {
            SectionFilterCommaDelimited = "key1,key2",
            TokenMap = TokenMapFactory.Create(),
        };

        var clone = (ReadParameters)original.DeepCopyAndInitialize();

        Assert.Multiple(() =>
        {
            Assert.That(original.SectionFilter, Is.Not.SameAs(clone.SectionFilter));
            Assert.That(original.TokenMap, Is.Not.SameAs(clone.TokenMap));
            Assert.That(original, Is.Not.SameAs(clone));
        });
    }

    [Test]
    public void SectionFilterCommaDelimited_Null_IsSet()
    {
        var parameters = new ReadParameters
        {
            SectionFilterCommaDelimited = null,
        };

        Assert.That(parameters.SectionFilter, Is.Null);
    }

    [Test]
    public void SectionFilterCommaDelimited_Empty_IsSet()
    {
        var parameters = new ReadParameters
        {
            SectionFilterCommaDelimited = string.Empty,
        };

        Assert.That(parameters.SectionFilter, Is.EquivalentTo(new[] { string.Empty }));
    }

    [Test]
    public void SectionFilterCommaDelimited_Values_IsSet()
    {
        var parameters = new ReadParameters
        {
            SectionFilterCommaDelimited = "key1, key1 , key2, key1",
        };

        Assert.That(parameters.SectionFilter, Is.EquivalentTo(new[] { "key1", "key2" }));
    }

    [TestCase("key1,")]
    [TestCase(",key1")]
    [TestCase(",,, key1 ,,,")]
    public void SectionFilterCommaDelimited_ValuesIncludingEmpty_IsSet(string raw)
    {
        var parameters = new ReadParameters
        {
            SectionFilterCommaDelimited = raw,
        };

        Assert.That(parameters.SectionFilter, Is.EquivalentTo(new[] { "key1", string.Empty }));
    }

    [Test]
    public void ToString_Default_IsCorrect()
    {
        string result = new ReadParameters().ToString().NoCr();

        Assert.That(result, Is.EqualTo(
            "ReadParameters:\n  Game=Unknown\n  " +
            "TokenMap=null\n  AutoLoadTokenMap=True\n  AbortIfUnmapped=True\n  " +
            "UseLazyParsing=True\n  SectionFilter=null\n  IsSectionFilterBlocklist=False\n  " +
            "MakeTemporaryCopyOfInputFile=False\n"));
    }
}
