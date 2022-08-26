using System.Text;

namespace Parsadox.Parser.UnitTests.Maps;

[TestCovers(typeof(TokenMapFactory))]
[TestCovers(typeof(ITokenMap))]
public class TokenMapTests : TestsBase
{
    private ITokenMap _tokenMap = TokenMapFactory.Create();
    private MockFileSystem _mockFileSystem = new();

    [SetUp]
    public void SetUp()
    {
        _mockFileSystem = new MockFileSystem();
        _tokenMap = TokenMapFactory.Create();
    }

    [Test]
    public void Ctor_Default_IsEmpty()
    {
        AssertIsEmpty();
    }

    [Test]
    public void DeepCopy_Valid_IsCopied()
    {
        _tokenMap.CodeMap[0x7777] = "token7";

        var clone = _tokenMap.DeepCopy();

        Assert.That(_tokenMap.CodeMap, Is.Not.SameAs(clone.CodeMap));
    }

    [Test]
    public void LoadEnvironment_NoVariable_Throws()
    {
        Assert.Multiple(() =>
        {
            Assert.That(() => _tokenMap.LoadEnvironment(Game.Ck3), Throws.ArgumentException);
            Assert.That(_mockFileSystem.GetEnvironmentVariableCalls, Has.Count.EqualTo(1));
            Assert.That(_mockFileSystem.FileExistsCalls, Has.Count.EqualTo(0));
            Assert.That(_mockFileSystem.ReadAllTextCalls, Has.Count.EqualTo(0));
            AssertIsEmpty();
        });
    }

    [Test]
    public void LoadEnvironment_NoFile_Throws()
    {
        _mockFileSystem.GetEnvironmentVariableReturns = "some_file.txt";

        Assert.Multiple(() =>
        {
            Assert.That(() => _tokenMap.LoadEnvironment(Game.Ck3), Throws.ArgumentException);
            Assert.That(_mockFileSystem.GetEnvironmentVariableCalls, Has.Count.EqualTo(1));
            Assert.That(_mockFileSystem.FileExistsCalls.Single(), Is.EqualTo("some_file.txt"));
            Assert.That(_mockFileSystem.ReadAllTextCalls, Has.Count.EqualTo(0));
            AssertIsEmpty();
        });
    }

    [Test]
    public void LoadEnvironment_InvalidFile_Throws()
    {
        _mockFileSystem.GetEnvironmentVariableReturns = "some_file.txt";
        _mockFileSystem.FileExistsReturns = true;
        _mockFileSystem.ReadAllTextReturns = "invalid";

        Assert.Multiple(() =>
        {
            Assert.That(() => _tokenMap.LoadEnvironment(Game.Ck3), Throws.TypeOf<ParseException>());
            Assert.That(_mockFileSystem.GetEnvironmentVariableCalls, Has.Count.EqualTo(1));
            Assert.That(_mockFileSystem.FileExistsCalls.Single(), Is.EqualTo("some_file.txt"));
            Assert.That(_mockFileSystem.ReadAllTextCalls.Single(), Is.EqualTo("some_file.txt"));
            AssertIsEmpty();
        });
    }

    [Test]
    public void LoadEnvironment_ValidFile_IsLoaded()
    {
        _mockFileSystem.GetEnvironmentVariableReturns = "some_file.txt";
        _mockFileSystem.FileExistsReturns = true;
        _mockFileSystem.ReadAllTextReturns = "0x7777 token7";

        _tokenMap.LoadEnvironment(Game.Ck3);

        Assert.Multiple(() =>
        {
            Assert.That(_mockFileSystem.GetEnvironmentVariableCalls, Has.Count.EqualTo(1));
            Assert.That(_mockFileSystem.FileExistsCalls.Single(), Is.EqualTo("some_file.txt"));
            Assert.That(_mockFileSystem.ReadAllTextCalls.Single(), Is.EqualTo("some_file.txt"));
            Assert.That(_tokenMap.CodeMap, Is.Not.Empty);
            Assert.That(_tokenMap.CodeMap, Does.ContainKey(0x7777).WithValue("token7"));
        });
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("\n\n\n")]
    [TestCase("# nothing but a comment")]
    public void LoadString_Empty_IsLoaded(string raw)
    {
        _tokenMap.LoadString(raw);

        AssertIsEmpty();
    }

    [TestCase("0x7777")]
    [TestCase("7777 token7")]
    [TestCase("0x777 token7")]
    [TestCase("0x77777 token7")]
    [TestCase("0x7777 token7 0x8888 token8")]
    [TestCase("0x7777 token7 # inline comment")]
    [TestCase("0x7777 token7\n0x7777 token7duplicate")]
    [TestCase("token7 0x7777")]
    public void LoadString_Invalid_Throws(string raw)
    {
        Assert.That(() => _tokenMap.LoadString(raw), Throws.TypeOf<ParseException>());
    }

    [Test]
    public void LoadString_Valid_IsLoaded()
    {
        _tokenMap.LoadString("# comment\n # another comment \n\n0x7777 token7\n0x7070 token7\n 0x8888 token8 \n");

        Assert.Multiple(() =>
        {
            Assert.That(_tokenMap.CodeMap, Is.EqualTo(new Dictionary<ushort, string>
            {
                [0x7070] = "token7",
                [0x7777] = "token7",
                [0x8888] = "token8",
            }));
            Assert.That(_tokenMap.ToString(), Is.EqualTo("3 codes"));
        });
    }

    [Test]
    public void LoadString_Multiple_IsMerged()
    {
        _tokenMap.LoadString("0x7777 token7");
        _tokenMap.LoadString("0x8888 token8");

        Assert.Multiple(() =>
        {
            Assert.That(_tokenMap.CodeMap, Is.EqualTo(new Dictionary<ushort, string>
            {
                [0x7777] = "token7",
                [0x8888] = "token8",
            }));
            Assert.That(_tokenMap.ToString(), Is.EqualTo("2 codes"));
        });
    }

    [TestCase(Game.Unknown, "UNKNOWN_IRONMAN_TOKENS")]
    [TestCase(Game.Ck3, "CK3_IRONMAN_TOKENS")]
    public void GetDefaultTokenMapEnvironmentVariableName_Valid_IsCorrect(Game game, string expectedResult)
    {
        string result = TokenMapFactory.GetDefaultTokenMapEnvironmentVariableName(game);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void CreateTemplateFile_Valid_IsCreated()
    {
        using MemoryStream buffer = new();
        _mockFileSystem.OpenCreateReturns = buffer;

        TokenMapFactory.CreateTemplateFile("some_file.txt");
        string result = Encoding.UTF8.GetString(buffer.ToArray());

        Assert.Multiple(() =>
        {
            Assert.That(_mockFileSystem.OpenCreateCalls.Single(), Is.EqualTo("some_file.txt"));
            Assert.That(result, Does.StartWith("# DO NOT DISTRIBUTE THIS FILE WITH CONTENT."));
            Assert.That(result.Contains('\r'), Is.False);
        });
    }

    private void AssertIsEmpty()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_tokenMap.CodeMap, Is.Empty);
            Assert.That(_tokenMap.ToString(), Is.EqualTo("0 codes"));
        });
    }
}
