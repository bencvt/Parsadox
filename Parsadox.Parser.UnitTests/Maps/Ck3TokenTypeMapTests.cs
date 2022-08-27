namespace Parsadox.Parser.UnitTests.Maps;

[TestCovers(typeof(Ck3TokenTypeMap))]
[TestCovers(typeof(ITokenTypeMap))]
[TestCovers(typeof(NodeBuilder))]
[TestCovers(typeof(LazyNodeBuilder))]
public class Ck3TokenTypeMapTests : TestsBase
{
    [TestCaseSource(nameof(VALUE_CASES))]
    public void IsValueGameDate_Value_IsCorrect(string key, bool expected)
    {
        bool actual = Ck3TokenTypeMap.Instance.IsValueGameDate(new I32Token(123), new GenericTextToken(key));

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(VALUE_CASES))]
    public void NodeBuilder_IsValueGameDate_IsCorrect(string key, bool expectedIsGameDate)
    {
        var tokens = new IToken[]
        {
            new ResolvedNameToken(key),
            SpecialToken.EQUALS,
            new I32Token(new GameDate(1066, 12, 25).ToI32()),
        };

        var node = new NodeBuilder(tokens, Ck3TokenTypeMap.Instance).Build().Single();

        if (expectedIsGameDate)
            Assert.That(node.Value, Is.TypeOf<GameDateToken>());
        else
            Assert.That(node.Value, Is.TypeOf<I32Token>());
    }

    private static readonly object[] VALUE_CASES =
    {
        new object[] { "birth", true },
        new object[] { " birth ", false },
        new object[] { "Birth", false },
        new object[] { "token1", false },
        new object[] { "some_date", true },
        new object[] { "some_decision", true },
        new object[] { "some_interaction", true },
        new object[] { "some_Date", false },
        new object[] { "some_Decision", false },
        new object[] { "some_Interaction", false },
        new object[] { "X_date", true },
        new object[] { "X_decision", true },
        new object[] { "X_interaction", true },
        new object[] { "1_date", true },
        new object[] { "1_decision", true },
        new object[] { "1_interaction", true },
        new object[] { "__date", true },
        new object[] { "__decision", true },
        new object[] { "__interaction", true },
        new object[] { "_date", false },
        new object[] { "_decision", false },
        new object[] { "_interaction", false },
    };

    [TestCaseSource(nameof(KEY_CASES))]
    public void IsKeyGameDate_Value_IsCorrect(string? grandparent, string? parent, bool expected)
    {
        bool actual = Ck3TokenTypeMap.Instance.IsKeyGameDate(
            new I32Token(123),
            parent is null ? null : new GenericTextToken(parent),
            grandparent is null ? null : new GenericTextToken(grandparent));

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(KEY_CASES))]
    public void NodeBuilder_IsKeyGameDate_IsCorrect(string? grandparent, string? parent, bool expectedIsGameDate) =>
        TestNodeBuilderIsKeyGameDate(grandparent, parent, expectedIsGameDate, isLazy: false);

    [TestCaseSource(nameof(KEY_CASES))]
    public void LazyNodeBuilder_IsKeyGameDate_IsCorrect(string? grandparent, string? parent, bool expectedIsGameDate) =>
        TestNodeBuilderIsKeyGameDate(grandparent, parent, expectedIsGameDate, isLazy: true);

    private static void TestNodeBuilderIsKeyGameDate(string? grandparent, string? parent, bool expectedIsGameDate, bool isLazy)
    {
        var tokens = new IToken[]
        {
            new ResolvedNameToken(grandparent ?? string.Empty),
            SpecialToken.EQUALS,
            SpecialToken.OPEN,
            new ResolvedNameToken(parent ?? string.Empty),
            SpecialToken.EQUALS,
            SpecialToken.OPEN,
            new I32Token(new GameDate(1066, 12, 25).ToI32()),
            SpecialToken.CLOSE,
            SpecialToken.CLOSE,
        };
        var builder = isLazy
            ? new LazyNodeBuilder(tokens, Ck3TokenTypeMap.Instance)
            : new NodeBuilder(tokens, Ck3TokenTypeMap.Instance);

        var node = builder.Build().Single().Single().Single();

        if (expectedIsGameDate)
            Assert.That(node.Content, Is.TypeOf<GameDateToken>());
        else
            Assert.That(node.Content, Is.TypeOf<I32Token>());
    }

    private static readonly object[] KEY_CASES =
    {
        new object?[] { "any", "changes", true },
        new object?[] { "any", "dates", true },
        new object?[] { "any", "history", true },
        new object?[] { "acceptance_changes", "any", true },
        new object?[] { "any", "any", false },
        new object?[] { null, null, false },
        new object?[] { null, "history", true },
        new object?[] { "acceptance_changes", null, true },
    };
}
