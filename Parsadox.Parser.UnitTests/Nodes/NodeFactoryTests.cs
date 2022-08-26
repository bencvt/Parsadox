namespace Parsadox.Parser.UnitTests.Nodes;

[TestCovers(typeof(NodeFactory))]
[TestCovers(typeof(INodeContent))]
[TestCovers(typeof(ParseException))]
public class NodeFactoryTests : TestsBase
{
    [TestCase("", "{}")]
    [TestCase(" ", "{}")]
    [TestCase("{}", "{{}}")]
    [TestCase("key", "{key}")]
    [TestCase("key={}", "{key={}}")]
    [TestCase("key=value", "{key=value}")]
    [TestCase("key={ data }", "{key={data}}")]
    [TestCase("key={ a=b }", "{key={a=b}}")]
    [TestCase(" key = { a = b } ", "{key={a=b}}")]
    [TestCase("key1 key2", "{key1 key2}")]
    [TestCase("key1=rgb{0 0 0}", "{key1=rgb{0 0 0}}")]
    [TestCase("key1=rgb{}", "{key1=rgb{}}")]
    [TestCase("key1=rgb{{}}", "{key1=rgb{{}}}")]
    [TestCase("key1=rgb{0 0 0}{0 0 0}", "{key1=rgb{0 0 0}{0 0 0}}")]
    [TestCase("key1=rgb{0 0 0}key2", "{key1=rgb{0 0 0}key2}")]
    [TestCase("key1=rgb", "{key1=rgb}")]
    [TestCase("key1=rgb}", "{key1=rgb}")]
    [TestCase("key1=rgb{", "{key1=rgb{}}")]
    [TestCase("key1=rgb{0", "{key1=rgb{0}}")]
    [TestCase("key1={", "{key1={}}")]
    [TestCase("key1{", "{key1{}}")]
    [TestCase("key1{key2", "{key1{key2}}")]
    [TestCase("key1={key2", "{key1={key2}}")]
    [TestCase("key1{key2{", "{key1{key2{}}}")]
    [TestCase("key1}key2", "{key1 key2}")]
    [TestCase("key1}}key2", "{key1 key2}")]
    [TestCase("key1}}}key2", "{key1 key2}")]
    [TestCase("{", "{{}}")]
    [TestCase("{{", "{{{}}}")]
    [TestCase("{{{", "{{{{}}}}")]
    [TestCase("}", "{}")]
    [TestCase("}}", "{}")]
    [TestCase("}}}", "{}")]
    public void LoadString_Valid_DoesNotThrow(string text, string expectedDump)
    {
        var node = NodeFactory.LoadString(text);

        Assert.That(node.Dump(), Is.EqualTo(expectedDump));
    }

    [TestCase("=")]
    [TestCase("{=")]
    [TestCase("}=")]
    [TestCase("{}=")]
    [TestCase("{}=value")]
    [TestCase("{key}=value")]
    [TestCase("key=")]
    [TestCase("key==")]
    [TestCase("key=}")]
    [TestCase("key=value1=value2")]
    public void LoadString_Invalid_Throws(string text)
    {
        Assert.That(() => NodeFactory.LoadString(text), Throws.TypeOf<ParseException>());
    }

    [Test]
    public void CreateKey_Valid_IsCreated()
    {
        var node = NodeFactory.Create("key");

        AssertCreated(node, "key\n", false, false, false);
    }

    [Test]
    public void CreateKeyAndArray_Valid_IsCreated()
    {
        var node = NodeFactory.Create("key").SetChildren(new());

        AssertCreated(node, "key={}\n", false, false, true);
    }

    [Test]
    public void CreateAnonymousArray_Valid_IsCreated()
    {
        var node = NodeFactory.CreateAnonymousArray(new());

        AssertCreated(node, "{}\n", true, false, true);
    }

    [Test]
    public void CreateComment_Empty_IsCreated()
    {
        var node = NodeFactory.CreateComment(string.Empty);

        AssertCreatedComment(node, "#\n");
    }

    [Test]
    public void CreateComment_Simple_IsCreated()
    {
        var node = NodeFactory.CreateComment("comment");

        AssertCreatedComment(node, "#comment\n");
    }

    [Test]
    public void CreateComment_Padded_IsCreated()
    {
        var node = NodeFactory.CreateComment(" comment  ");

        AssertCreatedComment(node, "# comment  \n");
    }

    [Test]
    public void CreateComment_Multiline_IsCreated()
    {
        var node = NodeFactory.CreateComment("line 1\nline 2\n\nline 4");

        AssertCreatedComment(node, "#line 1\n#line 2\n#\n#line 4\n");
    }

    [Test]
    public void CreateKeyAndTextValue_Valid_IsCreated()
    {
        var node = NodeFactory.Create("key").SetValue("value");

        AssertCreated(node, "key=\"value\"\n", false, true, false);
    }

    [Test]
    public void CreateKeyAndBoolValue_True_IsCreated()
    {
        var node = NodeFactory.Create("key").SetValue(true);

        AssertCreated(node, "key=yes\n", false, true, false);
    }

    [Test]
    public void CreateKeyAndBoolValue_False_IsCreated()
    {
        var node = NodeFactory.Create("key").SetValue(false);

        AssertCreated(node, "key=no\n", false, true, false);
    }

    [Test]
    public void CreateKeyAndDecimalValue_Valid_IsCreated()
    {
        var node = NodeFactory.Create("key").SetValue(123.45m);

        AssertCreated(node, "key=123.45\n", false, true, false);
    }

    [Test]
    public void CreateKeyAndGameDateValue_Valid_IsCreated()
    {
        var node = NodeFactory.Create("key").SetValue(new GameDate(1066, 12, 25));

        AssertCreated(node, "key=1066.12.25\n", false, true, false);
    }

    [Test]
    public void CreateKeyAndColorComponents_Rgb_IsCreated()
    {
        var node = NodeFactory.Create("key").SetColorRgb(255, 140, 0);

        AssertCreated(node, "key=rgb { 255 140 0 }\n", false, true, true);
    }

    [Test]
    public void CreateKeyAndColorComponents_Hex_IsCreated()
    {
        var node = NodeFactory.Create("key").SetColorHex("ff8c00");

        AssertCreated(node, "key=hex { ff8c00 }\n", false, true, true);
    }

    private static void AssertCreated(INode actual, string expectedFullDump, bool expectedContentIsEmpty, bool expectedHasValue, bool expectedHasChildrenStorage)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Dump(NodeOutputFormat.Full), Is.EqualTo(expectedFullDump));
            Assert.That(actual.Content.IsAnonymousArrayKey, Is.EqualTo(expectedContentIsEmpty));
            Assert.That(actual.Content.IsComment, Is.False);
            Assert.That(actual.HasValue, Is.EqualTo(expectedHasValue));
            Assert.That(actual.HasChildrenStorage, Is.EqualTo(expectedHasChildrenStorage));
        });
    }

    private static void AssertCreatedComment(INode actual, string expectedFullDump)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Content.IsAnonymousArrayKey, Is.False);
            Assert.That(actual.Content.IsComment, Is.True);
            Assert.That(actual.HasValue, Is.False);
            Assert.That(actual.HasChildrenStorage, Is.False);
            Assert.That(actual.Dump(NodeOutputFormat.Full), Is.EqualTo(expectedFullDump));
        });
    }
}
