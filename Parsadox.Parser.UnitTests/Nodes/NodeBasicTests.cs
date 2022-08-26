using System.Collections;

namespace Parsadox.Parser.UnitTests.Nodes;

[TestCovers(typeof(INode))]
[TestCovers(typeof(NodeContentException))]
public class NodeBasicTests : TestsBase
{
    [Test]
    public void ValueGet_HasValue_Returns()
    {
        var root = NodeFactory.Create("key").SetValue("value");

        Assert.That(root.Value.Text, Is.EqualTo("value"));
    }

    [Test]
    public void ValueGet_NoValue_Throws()
    {
        var root = NodeFactory.Create("key");

        Assert.That(() => _ = root.Value, Throws.TypeOf<NodeContentException>());
    }

    [Test]
    public void ValueSet_HasValue_IsSet()
    {
        var root = NodeFactory.Create("key").SetValue("value");

        root.Value = NodeFactory.Create("hello").Content;

        Assert.That(root.Value.Text, Is.EqualTo("hello"));
    }

    [Test]
    public void ValueSet_NoValue_IsSet()
    {
        var root = NodeFactory.Create("key");

        root.Value = NodeFactory.Create("hello").Content;

        Assert.That(root.Value.Text, Is.EqualTo("hello"));
    }

    [Test]
    public void ValueOrNullSet_ToNull_IsNull()
    {
        var root = NodeFactory.Create("key").SetValue("value");

        root.ValueOrNull = null;

        Assert.That(root.HasValue, Is.False);
    }

    [Test]
    public void ChildrenGet_HasChildrenStorage_Returns()
    {
        var root = NodeFactory.CreateAnonymousArray(new());

        Assert.That(root.Children, Is.Empty);
    }

    [Test]
    public void ChildrenGet_NoChildrenStorage_Throws()
    {
        var root = NodeFactory.Create("key");

        Assert.That(() => _ = root.Children, Throws.TypeOf<NodeContentException>());
    }

    [Test]
    public void ChildrenSet_HasChildren_IsSet()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key") });

        root.Children = new() { NodeFactory.Create("new") };

        Assert.That(root.Children.Single().Content.Text, Is.EqualTo("new"));
    }

    [Test]
    public void ChildrenSet_NoChildren_IsSet()
    {
        var root = NodeFactory.Create("key");

        root.Children = new() { NodeFactory.Create("new") };

        Assert.That(root.Children.Single().Content.Text, Is.EqualTo("new"));
    }

    [Test]
    public void ChildrenOrNullSet_ToNull_IsNull()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key") });

        root.ChildrenOrNull = null;

        Assert.That(root.HasChildrenStorage, Is.False);
    }

    [Test]
    public void IndexerGetSingle_Exists_Returns()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        Assert.That(root["key2"].Content.Text, Is.EqualTo("key2"));
    }

    [Test]
    public void IndexerGetSingle_DoesNotExist_Throws()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        Assert.That(() => _ = root["key3"], Throws.TypeOf<NodeContentException>());
    }

    [TestCase("key1", true)]
    [TestCase("key2", true)]
    [TestCase("key3", false)]
    [TestCase(" key1 ", false)]
    [TestCase("Key1", false)]
    [TestCase("", false)]
    public void HasChild_HasChildrenStorage_IsCorrect(string key, bool expectedResult)
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        Assert.That(root.HasChild(key), Is.EqualTo(expectedResult));
    }

    [Test]
    public void HasChild_NoChildrenStorage_IsFalse()
    {
        var root = NodeFactory.Create("key");

        Assert.That(root.HasChild("key"), Is.False);
    }

    [TestCase("key1", "key1")]
    [TestCase("key2", "key2")]
    [TestCase("key1,key1", "key1")]
    [TestCase("key3,key1", "key1")]
    [TestCase("key3,key2,key1", "key2")]
    public void GetChildOrThrow_HasChildrenStorageAndKeyExists_IsCorrect(string keys, string expectedResult)
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        Assert.That(root.GetChildOrThrow(keys.Split(',')).Content.Text, Is.EqualTo(expectedResult));
    }

    [TestCase("key3")]
    [TestCase("key3,key4,key5")]
    [TestCase(" key1 ")]
    [TestCase("Key1")]
    public void GetChildOrThrow_HasChildrenStorageAndKeyDoesNotExist_Throws(string keys)
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        Assert.That(() => root.GetChildOrThrow(keys.Split(',')), Throws.TypeOf<NodeContentException>());
    }

    [Test]
    public void GetOrCreateChild_Exists_Returns()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1") });

        var node = root.GetOrCreateChild("key1");

        Assert.Multiple(() =>
        {
            Assert.That(node, Is.SameAs(root["key1"]));
            Assert.That(root.Dump(), Is.EqualTo("{key1}"));
        });
    }

    [Test]
    public void GetOrCreateChild_DoesNotExist_IsCreated()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1") });

        var node = root.GetOrCreateChild("key2");

        Assert.Multiple(() =>
        {
            Assert.That(node, Is.SameAs(root["key2"]));
            Assert.That(root.Dump(), Is.EqualTo("{key1 key2}"));
        });
    }

    [Test]
    public void GetOrCreateChild_RootDoesNotHaveChildrenStorage_IsCreated()
    {
        var root = NodeFactory.Create("key1");

        var node = root.GetOrCreateChild("key2");

        Assert.Multiple(() =>
        {
            Assert.That(node, Is.SameAs(root["key2"]));
            Assert.That(root.Dump(), Is.EqualTo("key1={key2}"));
        });
    }

    [Test]
    public void GetEnumerator_HasChildrenStorage_IsCorrect()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        int count = (root).Count();

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void GetEnumeratorNonGeneric_HasChildrenStorage_IsCorrect()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        var e = (root as IEnumerable).GetEnumerator();
        int count = 0;
        while (e.MoveNext())
            count++;

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void RemoveChild_Exists_IsRemoved()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        bool result = root.RemoveChild("key1");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(root.Dump(), Is.EqualTo("{key2}"));
        });
    }

    [Test]
    public void RemoveChild_DoesNotExist_IsNotRemoved()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key2") });

        bool result = root.RemoveChild("key3");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(root.Dump(), Is.EqualTo("{key1 key2}"));
        });
    }

    [Test]
    public void RemoveChild_ExistMultiple_FirstIsRemoved()
    {
        var root = NodeFactory.CreateAnonymousArray(new() { NodeFactory.Create("key1"), NodeFactory.Create("key1"), NodeFactory.Create("key1") });

        bool result = root.RemoveChild("key1");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(root.Dump(), Is.EqualTo("{key1 key1}"));
        });
    }

    [Test]
    public void RemoveAllChildren_Valid_IsRemoved()
    {
        var root = NodeFactory.LoadString("key1 key1=value key2 key3");

        var result = root.RemoveAllChildren(x => x.Content.Text == "key1" || x.Content.Text == "key3");

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(root.Dump(), Is.EqualTo("{key2}"));
        });
    }

    [TestCase("", 1)]
    [TestCase(" ", 1)]
    [TestCase("{}", 2)]
    [TestCase("key", 2)]
    [TestCase("key={}", 2)]
    [TestCase("key=value", 2)]
    [TestCase("key={ data }", 3)]
    [TestCase("key={ a=b }", 3)]
    [TestCase(" key = { a = b } ", 3)]
    [TestCase("key1 key2", 3)]
    [TestCase("key1 #comment\nkey2", 3)]
    public void GetSize_Valid_IsCorrect(string text, long expectedSize)
    {
        var root = NodeFactory.LoadString(text);

        Assert.That(root.GetSize(), Is.EqualTo(expectedSize));
    }

    [TestCase("{}", "{}")]
    [TestCase("{ data }", "{data}")]
    [TestCase("key", "key")]
    [TestCase("key={}", "key={}")]
    [TestCase("key=value", "key=value")]
    [TestCase("key={ data }", "key={data}")]
    [TestCase("key={ a=b }", "key={a=b}")]
    [TestCase(" key = { a = b } ", "key={a=b}")]
    [TestCase("key={ a={b} {{}} }", "key={a={b}{{}}}")]
    [TestCase("key={ value #comment\n}", "key={value}")]
    [TestCase("\\=value", "\\=value")]
    [TestCase("\"\"=value", "\"\"=value")]
    [TestCase("\"\\\"\"=value", "\"\\\"\"=value")]
    [TestCase("\"\\\\\"=value", "\\=value")]
    [TestCase("\"key\"=value", "key=value")]
    [TestCase("\"key \"=value", "\"key \"=value")]
    [TestCase("\"\"=value", "\"\"=value")]
    [TestCase("\" \"=value", "\" \"=value")]
    [TestCase("\"=\"=value", "\"=\"=value")]
    [TestCase("\"{\"=value", "\"{\"=value")]
    [TestCase("\"}\"=value", "\"}\"=value")]
    [TestCase("\"\n\"=value", "\"\n\"=value")]
    public void Dump_Minimal_IsCorrect(string text, string expected)
    {
        var root = NodeFactory.LoadString(text).Single();

        Assert.That(root.Dump(), Is.EqualTo(expected));
    }

    [TestCase("{}", "{}\n")]
    [TestCase("{ data }", "{ data }\n")]
    [TestCase("key", "key\n")]
    [TestCase("key={}", "key={}\n")]
    [TestCase("key=value", "key=value\n")]
    [TestCase("key={ data }", "key={ data }\n")]
    [TestCase("key={ a=b }", "key={\n\ta=b\n}\n")]
    [TestCase(" key = { a = b } ", "key={\n\ta=b\n}\n")]
    [TestCase("key={ a={b} {} }", "key={\n\ta={ b }\n\t{}\n}\n")]
    [TestCase("key={ a={b} {{}} }", "key={\n\ta={ b }\n\t{\n\t\t{}\n\t}\n}\n")]
    [TestCase("key={ value #comment\n}", "key={ value }\n")]
    [TestCase("\\=value", "\\=value\n")]
    [TestCase("\"\"=value", "\"\"=value\n")]
    [TestCase("\"\\\"\"=value", "\"\\\"\"=value\n")]
    [TestCase("\"\\\\\"=value", "\\=value\n")]
    [TestCase("\"key\"=value", "key=value\n")]
    [TestCase("\"key \"=value", "\"key \"=value\n")]
    [TestCase("\"\"=value", "\"\"=value\n")]
    [TestCase("\" \"=value", "\" \"=value\n")]
    [TestCase("\"=\"=value", "\"=\"=value\n")]
    [TestCase("\"{\"=value", "\"{\"=value\n")]
    [TestCase("\"}\"=value", "\"}\"=value\n")]
    [TestCase("\"\n\"=value", "\"\n\"=value\n")]
    public void Dump_Full_IsCorrect(string text, string expected)
    {
        var root = NodeFactory.LoadString(text).Single();

        Assert.That(root.Dump(NodeOutputFormat.Full), Is.EqualTo(expected));
    }

    [Test]
    public void Dump_CommentMinimal_IsEmpty()
    {
        var root = NodeFactory.CreateComment("hello");

        Assert.That(root.Dump(), Is.Empty);
    }

    [Test]
    public void Dump_CommentFull_IsCorrect()
    {
        var root = NodeFactory.CreateComment("hello");

        Assert.That(root.Dump(NodeOutputFormat.Full), Is.EqualTo("#hello\n"));
    }

    [TestCase("{}", "{}")]
    [TestCase("{ data }", "{...}")]
    [TestCase("key", "key")]
    [TestCase("key={}", "key={}")]
    [TestCase("key=value", "key=value")]
    [TestCase("key={ data }", "key={...}")]
    [TestCase("key={ a=b }", "key={...}")]
    [TestCase(" key = { a = b } ", "key={...}")]
    [TestCase("key={ a={b} {{}} }", "key={...}")]
    [TestCase("key={ value #comment\n}", "key={...}")]
    public void ToString_Valid_IsCorrect(string text, string expected)
    {
        var root = NodeFactory.LoadString(text).Single();

        Assert.That(root.ToString(), Is.EqualTo(expected));
    }
}
