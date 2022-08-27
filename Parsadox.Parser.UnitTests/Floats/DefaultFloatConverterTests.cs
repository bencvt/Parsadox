namespace Parsadox.Parser.UnitTests.Floats;

[TestCovers(typeof(DefaultFloatConverter))]
public class DefaultFloatConverterTests : FloatConverterTestsBase
{
    internal override IFloatConverter Converter => DefaultFloatConverter.Instance;

    private static readonly object[] F32_CASES =
    {
        new object[] { "0000 0000", 0.0f, 0.0f },
        new object[] { "0000 803f", 1.0f, 1.0f },
        new object[] { "0000 80bf", -1.0f, - 1.0f },
        new object[] { "ffff 7f7f", float.MaxValue, float.MaxValue },
        new object[] { "ffff 7fff", float.MinValue, float.MinValue },
        new object[] { "0000 c0ff", float.NaN, float.NaN },
    };

    private static readonly object[] F64_CASES =
    {
        new object[] { "0000 0000 0000 0000", 0.0, 0.0 },
        new object[] { "0000 0000 0000 f03f", 1.0, 1.0 },
        new object[] { "0000 0000 0000 f0bf", -1.0, -1.0 },
        new object[] { "a2d5 24d3 fcb0 2840", 12.345678899999999, 12.3456789 },
        new object[] { "ffff ffff ffff ef7f", double.MaxValue, double.MaxValue },
        new object[] { "ffff ffff ffff efff", double.MinValue, double.MinValue },
        new object[] { "0000 0000 0000 f8ff", double.NaN, double.NaN },
    };

    [TestCaseSource(nameof(F32_CASES))]
    public void ReadBinaryF32_All_IsCorrect(string hex, float toValue, float _) => TestReadBinaryF32(hex, toValue);

    [TestCaseSource(nameof(F32_CASES))]
    public void WriteBinaryF32_All_IsCorrect(string hex, float _, float fromValue) => TestWriteBinaryF32(fromValue, hex);

    [TestCaseSource(nameof(F64_CASES))]
    public void ReadBinaryF64_All_IsCorrect(string hex, double toValue, double _) => TestReadBinaryF64(hex, toValue);

    [TestCaseSource(nameof(F64_CASES))]
    public void WriteBinaryF64_All_IsCorrect(string hex, double _, double fromValue) => TestWriteBinaryF64(fromValue, hex);
}
