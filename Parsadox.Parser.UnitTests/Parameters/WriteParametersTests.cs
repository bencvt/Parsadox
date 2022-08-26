namespace Parsadox.Parser.UnitTests.Parameters;

[TestCovers(typeof(WriteParameters))]
public class WriteParametersTests : TestsBase
{
    [Test]
    public void DeepCopyAndInitialize_Valid_IsCopied()
    {
        var original = new WriteParameters();

        var clone = original.DeepCopyAndInitialize();

        Assert.That(original, Is.Not.SameAs(clone));
    }

    [Test]
    public void ToString_Default_IsCorrect()
    {
        string result = new WriteParameters().ToString().NoCr();

        Assert.That(result, Is.EqualTo("WriteParameters:\n  " +
            "SaveGameFormat=CompressedAuto\n  NodeOutputFormat=Full\n"));
    }
}
