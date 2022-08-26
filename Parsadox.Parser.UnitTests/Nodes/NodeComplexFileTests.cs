namespace Parsadox.Parser.UnitTests.Nodes;

[TestCovers(typeof(INode))]
[TestCovers(typeof(NodeOutputFormat))]
public class NodeComplexFileTests : TestsBase
{
    private INode _root;

    [SetUp]
    public void SetUp()
    {
        _root = NodeFactory.LoadString(TestData.SaveGame);
    }

    [Test]
    public void Dump_Minimal_IsCorrect()
    {
        string dump = _root.Dump(NodeOutputFormat.MinimalWithNewLines);

        Assert.That(dump, Is.EqualTo(TestData.SaveGame_NodesMinimal.NoCr()));
    }

    [Test]
    public void Dump_Full_IsCorrect()
    {
        string dump = _root.Dump(NodeOutputFormat.Full);

        Assert.That(dump, Is.EqualTo(TestData.SaveGame_NodesFull.NoCr()));
    }

    [Test]
    public void Dump_FromMinimalToMinimal_IsCorrect()
    {
        _root = NodeFactory.LoadString(TestData.SaveGame_NodesFilteredMinimal);

        string dump = _root.Dump(NodeOutputFormat.MinimalWithNewLines);

        Assert.That(dump, Is.EqualTo("{" + TestData.SaveGame_NodesFilteredMinimal.NoCr() + "}"));
    }

    [Test]
    public void GetChildOrNull_KeyExists_Return()
    {
        var data = _root.GetChildOrNull("data1")?.Value.Text;

        Assert.That(data, Is.EqualTo("1234"));
    }

    [Test]
    public void GetChildOrNull_KeysOneExists_Return()
    {
        var data = _root.GetChildOrNull("foo", "bar", "data1", "baz", "data2")?.Value.Text;

        Assert.That(data, Is.EqualTo("1234"));
    }
}
