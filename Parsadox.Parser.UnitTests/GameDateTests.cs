using System.Text.Json;

namespace Parsadox.Parser.UnitTests;

[TestCovers(typeof(GameDate))]
public class GameDateTests : TestsBase
{
    [Test]
    public void Value_Default_IsAllZeroes()
    {
        GameDate date = default;

        AssertValueIs(date, 0, 0, 0, 0, false);
    }

    [Test]
    public void Ctor_Default_IsAllZeroes()
    {
        var date = new GameDate();

        AssertValueIs(date, 0, 0, 0, 0, false);
    }

    [TestCase(1, 1, 1)]
    [TestCase(-1, 1, 1)]
    [TestCase(2022, 7, 4)]
    [TestCase(-4000, 11, 17)]
    [TestCase(1066, 1, 31)]
    [TestCase(1066, 2, 28)]
    [TestCase(1066, 3, 31)]
    [TestCase(1066, 4, 30)]
    [TestCase(1066, 5, 31)]
    [TestCase(1066, 6, 30)]
    [TestCase(1066, 7, 31)]
    [TestCase(1066, 8, 31)]
    [TestCase(1066, 9, 30)]
    [TestCase(1066, 10, 31)]
    [TestCase(1066, 11, 30)]
    [TestCase(1066, 12, 31)]
    [TestCase(99999, 12, 31)]
    public void Ctor_ValidValues_IsValid(int year, byte month, byte day)
    {
        var date = new GameDate(year, month, day);

        AssertValueIs(date, year, month, day, 0, true);
    }

    [TestCase(int.MinValue, 1, 1)]
    [TestCase(0, 1, 1)]
    [TestCase(1066, 0, 1)]
    [TestCase(1066, 13, 1)]
    [TestCase(1066, 1, 0)]
    [TestCase(1066, 1, 32)]
    [TestCase(1066, 2, 29)]
    [TestCase(1066, 3, 32)]
    [TestCase(1066, 4, 31)]
    [TestCase(1066, 5, 32)]
    [TestCase(1066, 6, 31)]
    [TestCase(1066, 7, 32)]
    [TestCase(1066, 8, 32)]
    [TestCase(1066, 9, 31)]
    [TestCase(1066, 10, 32)]
    [TestCase(1066, 11, 31)]
    [TestCase(1066, 12, 32)]
    [TestCase(2020, 2, 29)] // leap year but GameDate don't care
    public void Ctor_InvalidValues_Throws(int year, byte month, byte day)
    {
        Assert.That(() => new GameDate(year, month, day), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Set_Invalid_DoesNotThrow()
    {
        GameDate date = new(1066, 12, 25)
        {
            Month = 13
        };

        AssertValueIs(date, 1066, 13, 25, 0, false);
    }

    [TestCase("-5000.1.1")]
    [TestCase("1066.1.1")]
    [TestCase("1066.1.1.0")]
    [TestCase("1066.1.1.1")]
    [TestCase("1066.1.1.23")]
    [TestCase(" 1066.1.1 ")]
    [TestCase(" 1066 . 1 . 1 ")]
    [TestCase("-4000.12.25")]
    [TestCase("1.1.1")]
    [TestCase("-1.1.1")]
    [TestCase("0867.1.1")]
    [TestCase("00001.00001.00001")]
    [TestCase("00001.00001.00001.00001")]
    [TestCase("240146.7.15.7")]
    public void Parse_Valid_IsValid(string raw)
    {
        var date = GameDate.Parse(raw);

        Assert.That(() => date.Validate(), Throws.Nothing);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("1066")]
    [TestCase("1066.1")]
    [TestCase("1066/1/1")]
    [TestCase("$1066.1.1")]
    [TestCase("1066.1.1$")]
    [TestCase("- 4000.12.25")]
    [TestCase("1.1.1066")]
    [TestCase("1066.1.1.-1")]
    public void Parse_InvalidFormat_Throws(string raw)
    {
        Assert.That(() => GameDate.Parse(raw), Throws.ArgumentException);
    }

    [TestCase("-9999999.1.1")]
    [TestCase("-5001.1.1")]
    [TestCase("0.1.1")]
    [TestCase("-0.1.1")]
    [TestCase("1066.0.1")]
    [TestCase("1066.13.1")]
    [TestCase("1066.13.1")]
    [TestCase("1066.1.0")]
    [TestCase("1066.1.32")]
    [TestCase("2020.2.29")]
    [TestCase("2020.2.29")]
    [TestCase("1066.1.1.24")]
    [TestCase("240146.7.15.8")]
    [TestCase("9999999.1.1")]
    public void Parse_InvalidNumbers_Throws(string raw)
    {
        Assert.That(() => GameDate.Parse(raw), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    private static readonly object[] I32_CASES =
    {
        new object[] { 0, "-5000.1.1" },
        new object[] { 1, "-5000.1.1.1" },
        new object[] { 23, "-5000.1.1.23" },
        new object[] { 24, "-5000.1.2" },
        new object[] { 25, "-5000.1.2.1" },
        new object[] { 5313816, "-4394.8.8" },
        new object[] { 53138159, "1065.12.31.23" },
        new object[] { 53138160, "1066.1.1" },
        new object[] { 53138161, "1066.1.1.1" },
        new object[] { 131391240, "9999.1.1" },
        new object[] { 531381600, "55660.1.1" },
        new object[] { int.MaxValue, "240146.7.15.7" },
    };

    [TestCaseSource(nameof(I32_CASES))]
    public void FromI32_Valid_IsValid(int raw, string expectedToString)
    {
        var date = GameDate.FromI32(raw);

        AssertValueIs(date, expectedToString);
    }

    [TestCase(int.MinValue)]
    [TestCase(-531381700)]
    [TestCase(-53138170)]
    [TestCase(-43791240)] // -9999.1.1
    [TestCase(-1)]
    public void FromI32_Invalid_Throws(int raw)
    {
        Assert.That(() => GameDate.FromI32(raw), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCaseSource(nameof(I32_CASES))]
    public void ToI32_Valid_IsValid(int expectedValue, string raw)
    {
        int value = GameDate.Parse(raw).ToI32();

        Assert.That(value, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ToI32_Invalid_Throws()
    {
        var bad = new GameDate
        {
            Month = 13,
        };

        Assert.That(() => bad.ToI32(), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [TestCase(1066, 1, 25)]
    [TestCase(1066, 13, 0)]
    public void CompareTo_TwoEqualDates_AreCompared(int year, byte month, byte day)
    {
        GameDate date1 = new() { Year = year, Month = month, Day = day };
        GameDate date2 = new() { Year = year, Month = month, Day = day };

        Assert.That(date1, Is.EqualTo(date2));
    }

    [TestCase("1066.1.1", "1066.1.2")]
    [TestCase("1066.12.25", "1067.1.1")]
    [TestCase("-1066.1.1", "1066.1.1")]
    [TestCase("-1066.1.1", "1.1.1")]
    public void CompareTo_TwoDifferentDates_AreCompared(string smaller, string larger)
    {
        var smallerDate = GameDate.Parse(smaller);
        var largerDate = GameDate.Parse(larger);

        Assert.That(smallerDate, Is.LessThan(largerDate));
    }

    [TestCase("867.1.1", "867.1.1", true)]
    [TestCase("1066.1.1", "867.1.1", false)]
    [TestCase("867.1.1", "1066.1.1", false)]
    public void Operator_Eq_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) == GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("867.1.1", "867.1.1", false)]
    [TestCase("1066.1.1", "867.1.1", true)]
    [TestCase("867.1.1", "1066.1.1", true)]
    public void Operator_Neq_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) != GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("867.1.1", "867.1.1", false)]
    [TestCase("1066.1.1", "867.1.1", false)]
    [TestCase("867.1.1", "1066.1.1", true)]
    public void Operator_Lt_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) < GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("867.1.1", "867.1.1", false)]
    [TestCase("1066.1.1", "867.1.1", true)]
    [TestCase("867.1.1", "1066.1.1", false)]
    public void Operator_Gt_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) > GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("867.1.1", "867.1.1", true)]
    [TestCase("1066.1.1", "867.1.1", false)]
    [TestCase("867.1.1", "1066.1.1", true)]
    public void Operator_Lte_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) <= GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("867.1.1", "867.1.1", true)]
    [TestCase("1066.1.1", "867.1.1", true)]
    [TestCase("867.1.1", "1066.1.1", false)]
    public void Operator_Gte_IsCorrect(string first, string second, bool expectedValue)
    {
        bool result = GameDate.Parse(first) >= GameDate.Parse(second);

        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [TestCase("-5000.1.1", "\"-4999-01-01\"")]
    [TestCase("-2.1.1", "\"-0001-01-01\"")]
    [TestCase("-1.1.1", "\"0000-01-01\"")]
    [TestCase("1.1.1", "\"0001-01-01\"")]
    [TestCase("867.12.25", "\"0867-12-25\"")]
    [TestCase("867.12.25.14", "\"0867-12-25T14:00:00Z\"")]
    [TestCase("2000.2.28", "\"2000-02-28\"")]
    public void JsonSerialize_Valid_IsCorrect(string raw, string expectedValue)
    {
        string json = JsonSerializer.Serialize(GameDate.Parse(raw));

        Assert.That(json, Is.EqualTo(expectedValue));
    }

    [TestCase("\"-4999-01-01\"", "-5000.1.1")]
    [TestCase("\"-0001-01-01\"", "-2.1.1")]
    [TestCase("\"-0000-01-01\"", "-1.1.1")]
    [TestCase("\"0000-01-01\"", "-1.1.1")]
    [TestCase("\"+0000-01-01\"", "-1.1.1")]
    [TestCase("\"0001-01-01\"", "1.1.1")]
    [TestCase("\"+0001-01-01\"", "1.1.1")]
    [TestCase("\"0867-12-25\"", "867.12.25")]
    [TestCase("\"867.12.25\"", "867.12.25")]
    [TestCase("\"867/12/25\"", "867.12.25")]
    [TestCase("\"0867-12-25T14:04:52.179Z\"", "867.12.25.14")]
    [TestCase("\"123456-12-25\"", "123456.12.25")]
    [TestCase("\"2000-02-28\"", "2000.2.28")]
    public void JsonDeserialize_Valid_IsCorrect(string json, string expectedToString)
    {
        var date = JsonSerializer.Deserialize<GameDate>(json);

        AssertValueIs(date, expectedToString);
    }

    [TestCase("\"867-13-33\"")]
    [TestCase("\"867-12\"")]
    [TestCase("12345")]
    [TestCase("null")]
    [TestCase("")]
    [TestCase("\"2000-02-29\"")]
    public void JsonDeserialize_Invalid_Throws(string json)
    {
        Assert.That(() => JsonSerializer.Deserialize<GameDate>(json), Throws.Exception);
    }

    private static void AssertValueIs(GameDate actual, int expectedYear, int expectedMonth, int expectedDay, int expectedHour, bool expectedIsValid)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Year, Is.EqualTo(expectedYear));
            Assert.That(actual.Month, Is.EqualTo(expectedMonth));
            Assert.That(actual.Day, Is.EqualTo(expectedDay));
            Assert.That(actual.Hour, Is.EqualTo(expectedHour));
            if (expectedIsValid)
                Assert.That(() => actual.Validate(), Throws.Nothing);
            else
                Assert.That(() => actual.Validate(), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(actual.ToString(), Is.EqualTo($"{expectedYear}.{expectedMonth}.{expectedDay}"));
        });
    }

    private static void AssertValueIs(GameDate actual, string expectedToString)
    {
        Assert.Multiple(() =>
        {
            Assert.That(() => actual.Validate(), Throws.Nothing);
            Assert.That(actual.ToString(), Is.EqualTo(expectedToString));
        });
    }
}