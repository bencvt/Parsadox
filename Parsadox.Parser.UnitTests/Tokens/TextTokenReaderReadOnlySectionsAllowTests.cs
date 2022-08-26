namespace Parsadox.Parser.UnitTests.Tokens;

public class TextTokenReaderReadOnlySectionsAllowTests : TextTokenReaderTestsBase
{
    [TestCase("", "")]
    [TestCase(" ", "")]
    [TestCase("key1", "key1")]
    [TestCase("key2", "")]
    [TestCase("key3", "key3")]
    [TestCase("123", "")]
    [TestCase("\"value\"", "")]
    [TestCase("key1 key1", "key1 key1")]
    [TestCase("key1 key2 key3", "key1 key3")]
    [TestCase("key1=value", "key1 = value")]
    [TestCase("key2=value", "")]
    [TestCase("key1 = value", "key1 = value")]
    [TestCase("key2 = value", "")]
    [TestCase("key1=\"value\"", "key1 = \"value\"")]
    [TestCase("key1=value key2=value key3=value", "key1 = value key3 = value")]
    [TestCase("key1=value key2=value key1=value key2=value", "key1 = value key1 = value")]
    [TestCase("key1=\"value\"key3", "key1 = \"value\" key3")]
    [TestCase("key1={}key3", "key1 = { } key3")]
    [TestCase("key1\"value1\"\"value2\"key2", "key1")]
    [TestCase("# comment", "")]
    [TestCase("\"# not a comment\"", "")]
    [TestCase("\"eof in string", "")]
    [TestCase("\"eof in string\\", "")]
    [TestCase("#comment\nkey1=value", "key1 = value")]
    [TestCase("key1#comment\n=value", "key1 = value")]
    [TestCase("key1=#comment\nvalue", "key1 = value")]
    [TestCase("key1=value#comment", "key1 = value")]
    [TestCase("key1={key2={key3=value}} key2={key1={key3=value}}", "key1 = { key2 = { key3 = value } }")]
    [TestCase("#\nkey1#\n=#\n{#\nkey2#\n=#\n{#\nkey3#\n=#\nvalue#\n}#\n}#\n", "key1 = { key2 = { key3 = value } }")]
    [TestCase("#\nkey2#\n=#\n{#\nkey1#\n=#\n{#\nkey3#\n=#\nvalue#\n}#\n}#\n", "")]
    [TestCase("key2=value\"value\"key1", "key1")]
    [TestCase("\"key1\"=value", "\"key1\" = value")]
    [TestCase("\"key2\"=value", "")]
    [TestCase("\"key1\"={value}", "\"key1\" = { value }")]
    [TestCase("\"key2\"={value}", "")]
    [TestCase("{}key1{}", "key1")]
    public void ReadOnlySections_Allowlist_IsCorrect(string source, string expectedResult)
    {
        string result = ReadOnlySections(source, ' ', isBlocklist: false, "key1", "key3");

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ReadOnlySections_AllowlistWithEmptyValue_IsCorrect()
    {
        string result = ReadOnlySections("{}token1={value}{}", ' ', isBlocklist: false, "token1", "");

        Assert.That(result, Is.EqualTo("{ } token1 = { value } { }"));
    }

    [Test]
    public void ReadOnlySections_AllowlistNoEmptyValue_IsCorrect()
    {
        string result = ReadOnlySections("{}token1={value}{}", ' ', isBlocklist: false, "token1");

        Assert.That(result, Is.EqualTo("token1 = { value }"));
    }

    [Test]
    public void ReadOnlySections_ComplexFile_IsCorrect()
    {
        string result = ReadOnlySections(TestData.SaveGame, '\n', isBlocklist: false, "data1", "played_character");

        Assert.That(result, Is.EqualTo(TestData.SaveGame_TokensFiltered.NoCr()));
    }
}
