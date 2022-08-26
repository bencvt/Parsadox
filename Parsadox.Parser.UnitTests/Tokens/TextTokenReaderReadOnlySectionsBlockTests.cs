namespace Parsadox.Parser.UnitTests.Tokens;

public class TextTokenReaderReadOnlySectionsBlockTests : TextTokenReaderTestsBase
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
    public void ReadOnlySections_Blocklist_IsCorrect(string source, string expectedResult)
    {
        string result = ReadOnlySections(source, ' ', isBlocklist: true,
            "key2", "value", "value1", "value2", "123", "# not a comment", "eof in string", "eof in string\\", "");

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ReadOnlySections_BlocklistWithEmptyValue_IsCorrect()
    {
        string result = ReadOnlySections("{}token1={value}{}", ' ', isBlocklist: true, "");

        Assert.That(result, Is.EqualTo("token1 = { value }"));
    }

    [Test]
    public void ReadOnlySections_BlocklistNoEmptyValue_IsCorrect()
    {
        string result = ReadOnlySections("{}token1={value}{}", ' ', isBlocklist: true);

        Assert.That(result, Is.EqualTo("{ } token1 = { value } { }"));
    }

    [Test]
    public void ReadOnlySections_ComplexFile_IsCorrect()
    {
        string result = ReadOnlySections(TestData.SaveGame, '\n', isBlocklist: true,
            "data2", "data3", "data4", "data5", "currently_played_characters");

        Assert.That(result, Is.EqualTo(TestData.SaveGame_TokensFiltered.NoCr()));
    }
}
