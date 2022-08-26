namespace Parsadox.Parser.UnitTests.Tokens;

[TestCovers(typeof(IToken))]
[TestCovers(typeof(BigF32Token))]
[TestCovers(typeof(BigF64Token))]
[TestCovers(typeof(BinaryCodeWrapperToken))]
[TestCovers(typeof(BoolToken))]
[TestCovers(typeof(ColorToken))]
[TestCovers(typeof(CommentToken))]
[TestCovers(typeof(DecimalToken))]
[TestCovers(typeof(F32Token))]
[TestCovers(typeof(F64Token))]
[TestCovers(typeof(GameDateToken))]
[TestCovers(typeof(I32Token))]
[TestCovers(typeof(QuotedStringToken))]
[TestCovers(typeof(ResolvedNameToken))]
[TestCovers(typeof(SpecialToken))]
[TestCovers(typeof(GenericTextProbablyGameDateToken))]
[TestCovers(typeof(GenericTextToken))]
[TestCovers(typeof(U16Token))]
[TestCovers(typeof(U32Token))]
[TestCovers(typeof(U64Token))]
public class TokenTests : TestsBase
{
    [TestCaseSource(nameof(TEST_CASES))]
    public void Token_All_IsCorrect(IToken token, ushort expectedCode, string expectedText, string expectedToString, bool nodeContentThrows)
    {
        Assert.Multiple(() =>
        {
            Assert.That(token.Code, Is.EqualTo(expectedCode));
            Assert.That(token.Text, Is.EqualTo(expectedText));
            Assert.That(token.ToString(), Is.EqualTo(expectedToString), "aa");
            if (nodeContentThrows)
                Assert.That(() => _ = token.AsNodeContent, Throws.TypeOf<ParseException>());
            else
                Assert.That(token.AsNodeContent.ToString(), Is.EqualTo(expectedToString));
        });
    }

    private static readonly object[] TEST_CASES =
    {
        new object[] { new BinaryCodeWrapperToken(0x1111, new GenericTextToken("foo")), (ushort)0x1111, "foo", "foo", false },
        new object[] { new BoolToken(true), BinaryTokenCodes.BOOL, "yes", "yes", false },
        new object[] { new BoolToken(false), BinaryTokenCodes.BOOL, "no", "no", false },
        new object[] { ColorToken.RGB, BinaryTokenCodes.RGB, "rgb", "rgb", false },
        new object[] { new CommentToken("foo"), (ushort)0, "foo", "#foo", false },
        new object[] { new CommentToken("foo\nbar"), (ushort)0, "foo\nbar", "#foo\n#bar", false },
        new object[] { new DecimalToken(123.45m), BinaryTokenCodes.F64, "123.45", "123.45", false },
        new object[] { new F32Token(123.45f), BinaryTokenCodes.F32, "123.450000", "123.450000", false },
        new object[] { new BigF32Token(123.45f), BinaryTokenCodes.F32, "123.450", "123.450", false },
        new object[] { new F64Token(123.45), BinaryTokenCodes.F64, "123.45", "123.45", false },
        new object[] { new BigF64Token(123.45), BinaryTokenCodes.F64, "123.45000", "123.45000", false },
        new object[] { new GameDateToken(GameDate.YEAR_9999), BinaryTokenCodes.I32, "9999.1.1", "9999.1.1", false },
        new object[] { new GenericTextProbablyGameDateToken("1066.1.1"), (ushort)0, "1066.1.1", "1066.1.1", false },
        new object[] { new GenericTextProbablyGameDateToken("not a date..."), (ushort)0, "not a date...", "\"not a date...\"", false },
        new object[] { new GenericTextToken("foo"), (ushort)0, "foo", "foo", false },
        new object[] { new I32Token(123), BinaryTokenCodes.I32, "123", "123", false },
        new object[] { new QuotedStringToken("foo"), (ushort)0, "foo", "\"foo\"", false },
        new object[] { new QuotedStringToken(" "), (ushort)0, " ", "\" \"", false },
        new object[] { new QuotedStringToken(""), (ushort)0, "", "\"\"", false },
        new object[] { new QuotedStringToken("\""), (ushort)0, "\"", "\"\\\"\"", false },
        new object[] { new QuotedStringToken("\\"), (ushort)0, "\\", "\"\\\\\"", false },
        new object[] { new QuotedStringToken("\n"), (ushort)0, "\n", "\"\n\"", false },
        new object[] { new ResolvedNameToken("foo"), (ushort)0, "foo", "foo", false },
        new object[] { SpecialToken.EMPTY, (ushort)0, "", "", true },
        new object[] { SpecialToken.OPEN, BinaryTokenCodes.OPEN, "{", "{", true },
        new object[] { SpecialToken.CLOSE, BinaryTokenCodes.CLOSE, "}", "}", true },
        new object[] { SpecialToken.EQUALS, BinaryTokenCodes.EQUALS, "=", "=", true },
        new object[] { SpecialToken.YES, BinaryTokenCodes.BOOL, "yes", "yes", false },
        new object[] { SpecialToken.NO, BinaryTokenCodes.BOOL, "no", "no", false },
        new object[] { new U16Token(0x1111), (ushort)0x1111, "0x1111", "0x1111", false },
        new object[] { new U32Token(123), BinaryTokenCodes.U32, "123", "123", false },
        new object[] { new U64Token(123), BinaryTokenCodes.U64, "123", "123", false },
    };

    [Test]
    public void ColorTokenTextSet_Any_Throws()
    {
        Assert.That(() => ColorToken.RGB.Text = "foo", Throws.TypeOf<NodeContentException>());
    }

    [Test]
    public void ColorTokenFrom_ColorToken_ReturnsToken()
    {
        Assert.That(ColorToken.From(ColorToken.RGB), Is.SameAs(ColorToken.RGB));
    }

    [Test]
    public void ColorTokenFrom_MatchingTextToken_ReturnsToken()
    {
        Assert.That(ColorToken.From(new GenericTextToken("rgb")), Is.SameAs(ColorToken.RGB));
    }

    [TestCase("foo")]
    [TestCase("")]
    [TestCase("RGB")]
    [TestCase(" rgb ")]
    public void ColorTokenFrom_NonMatchingTextToken_ReturnsNull(string text)
    {
        Assert.That(ColorToken.From(new GenericTextToken(text)), Is.Null);
    }

    [Test]
    public void ColorTokenFrom_OtherToken_ReturnsNull()
    {
        Assert.That(ColorToken.From(new QuotedStringToken("rgb")), Is.Null);
    }

    [TestCase("rgb")]
    [TestCase(" rgb ")]
    [TestCase(" RGB ")]
    public void ColorTokenParse_Matching_ReturnsToken(string text)
    {
        Assert.That(ColorToken.Parse(text), Is.SameAs(ColorToken.RGB));
    }

    [TestCase("")]
    [TestCase("foo")]
    [TestCase("rgb,")]
    public void ColorTokenParse_NonMatching_Throws(string text)
    {
        Assert.That(() => ColorToken.Parse(text), Throws.ArgumentException);
    }
}
