namespace Parsadox.Parser.UnitTests.Nodes;

[TestCovers(typeof(INode))]
[TestCovers(typeof(NodeOutputFormat))]
public class NodeComplexStructureTests : TestsBase
{
    [Test]
    public void Dump_Comment_IsCorrect()
    {
        var node = NodeFactory.CreateAnonymousArray(new()
        {
            NodeFactory.Create("key").SetChildren(new()
            {
                NodeFactory.CreateComment("hello"),
            }),
        });

        AssertDumpsAre(node,
            "{key={}}",
            "{\n\tkey={\n\t\t#hello\n\t}\n}\n");
    }

    [Test]
    public void Dump_CommentMultiline_IsCorrect()
    {
        var node = NodeFactory.CreateAnonymousArray(new()
        {
            NodeFactory.Create("key").SetChildren(new()
            {
                NodeFactory.CreateComment("hello\n world #1 \r\n\r\nb\r\rye"),
            }),
        });

        AssertDumpsAre(node,
            "{key={}}",
            "{\n\tkey={\n\t\t#hello\n\t\t# world #1 \n\t\t#\n\t\t#bye\n\t}\n}\n");
    }

    [Test]
    public void Dump_TwoKeys_IsCorrect()
    {
        var node = NodeFactory.CreateAnonymousArray(new()
        {
            NodeFactory.Create("key1"),
            NodeFactory.Create("key2"),
        });

        AssertDumpsAre(node,
            "{key1 key2}",
            "{ key1 key2 }\n");
    }

    [Test]
    public void Dump_TwoKeyValues_IsCorrect()
    {
        var node = NodeFactory.CreateAnonymousArray(new()
        {
            NodeFactory.Create("key1").SetValue(111),
            NodeFactory.Create("key2").SetValue(222),
        });

        AssertDumpsAre(node,
            "{key1=111 key2=222}",
            "{\n\tkey1=111\n\tkey2=222\n}\n");
    }

    [Test]
    public void Dump_TwoKeysWithChildren_IsCorrect()
    {
        var node = NodeFactory.CreateAnonymousArray(new()
        {
            NodeFactory.Create("key1").SetChildren(new()),
            NodeFactory.Create("key2").SetChildren(new()),
        });

        AssertDumpsAre(node,
            "{key1={}key2={}}",
            "{\n\tkey1={}\n\tkey2={}\n}\n");
    }

    [Test]
    public void Dump_RgbFromString_IsCorrect()
    {
        var node = NodeFactory.LoadString("key1=rgb { 255 140 0 }");

        Assert.Multiple(() =>
        {
            Assert.That(node["key1"].HasValue, Is.True);
            Assert.That(node["key1"].ValueOrNull?.Text, Is.EqualTo("rgb"));
            Assert.That(node["key1"].HasChildrenStorage, Is.True);
            Assert.That(node["key1"].ChildrenOrNull?.Count, Is.EqualTo(3));
            AssertDumpsAre(node,
                "{key1=rgb{255 140 0}}",
                "{\n\tkey1=rgb { 255 140 0 }\n}\n");
        });
    }

    [Test]
    public void Dump_NotRgbFromString_IsCorrect()
    {
        var node = NodeFactory.LoadString("key1=notrgb { 255 140 0 }");

        Assert.Multiple(() =>
        {
            Assert.That(node["key1"].HasValue, Is.True);
            Assert.That(node["key1"].ValueOrNull?.Text, Is.EqualTo("notrgb"));
            Assert.That(node["key1"].HasChildrenStorage, Is.False);
            AssertDumpsAre(node,
                "{key1=notrgb{255 140 0}}",
                "{\n\tkey1=notrgb\n\t{ 255 140 0 }\n}\n");
        });
    }

    [Test]
    public void Dump_RgbEmptyFromString_IsCorrect()
    {
        var node = NodeFactory.LoadString("key1=rgb{}key2");

        Assert.Multiple(() =>
        {
            Assert.That(node["key1"].HasValue, Is.True);
            Assert.That(node["key1"].ValueOrNull?.Text, Is.EqualTo("rgb"));
            Assert.That(node["key1"].HasChildrenStorage, Is.True);
            Assert.That(node["key1"].ChildrenOrNull?.Count, Is.EqualTo(0));
            AssertDumpsAre(node,
                "{key1=rgb{}key2}",
                "{\n\tkey1=rgb {}\n\tkey2\n}\n");
        });
    }

    [Test]
    public void Dump_RgbIncompleteFromString_IsCorrect()
    {
        var node = NodeFactory.LoadString("key1=rgb key2");

        Assert.Multiple(() =>
        {
            Assert.That(node["key1"].HasValue, Is.True);
            Assert.That(node["key1"].ValueOrNull?.Text, Is.EqualTo("rgb"));
            Assert.That(node["key1"].HasChildrenStorage, Is.False);
            AssertDumpsAre(node,
                "{key1=rgb key2}",
                "{\n\tkey1=rgb\n\tkey2\n}\n");
        });
    }

    [TestCase(null, "", "root")]
    [TestCase(null, "a", "root")]
    [TestCase("", "a", "")]
    [TestCase("a", "a", "a")]
    [TestCase("a", "a=1", "a")]
    [TestCase("a", "a a a", "a,a,a")]
    [TestCase("a", "a b", "a")]
    [TestCase("a,a", "a a a", "")]
    [TestCase("a,b", "a=b", "")]
    [TestCase("a,b", "a={b}", "b")]
    [TestCase("a,b", "a={b b}", "b,b")]
    [TestCase("a,b", "a={b} a={b}", "b,b")]
    [TestCase("a,b", "a={b b} a={b}", "b,b,b")]
    [TestCase("a,b", "b=a b={a} a={b a={b} a=b b} a={b} a={}", "b,b,b")]
    [TestCase("a,b,c", "a={b={c=d}}", "c")]
    [TestCase("character,family_data,child", "character={ id=111 family_data={ spouse=222 child=333 child=444 } } }", "child,child")]
    [TestCase("character,family_data", "character={ id=111 family_data={ spouse=222 child=333 child=444 } } }", "family_data")]
    [TestCase("character", "character={ id=111 family_data={ spouse=222 child=333 child=444 } } }", "character")]
    [TestCase(null, "character={ id=111 family_data={ spouse=222 child=333 child=444 } } }", "root")]
    [TestCase("character,id,child", "character={ id=111 family_data={ spouse=222 child=333 child=444 } } }", "")]
    public void GetDescendants_Valid_IsCorrect(string? path, string nodes, string expectedResult)
    {
        string[] pathArray = path?.Split(',') ?? Array.Empty<string>();
        var node = NodeFactory.LoadString(nodes).SetContent("root");

        string result = string.Join(',', node.GetDescendants(pathArray).Select(x => x.Content.Text));

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    private static void AssertDumpsAre(INode actual, string expectedMinimal, string expectedFull)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Dump(), Is.EqualTo(expectedMinimal));
            Assert.That(actual.Dump(NodeOutputFormat.Full), Is.EqualTo(expectedFull));
        });
    }
}
