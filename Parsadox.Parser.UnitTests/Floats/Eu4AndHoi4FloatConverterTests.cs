namespace Parsadox.Parser.UnitTests.Floats;

[TestCovers(typeof(Eu4AndHoi4FloatConverter))]
public class Eu4AndHoi4FloatConverterTests : FloatConverterTestsBase
{
    internal override IFloatConverter Converter => Eu4AndHoi4FloatConverter.Instance;

    private static readonly object[] F32_CASES =
    {
        new object[] { "0000 0000", 0.0f, 0.0f },
        new object[] { "e803 0000", 1.0f, 1.0f },
        new object[] { "18fc ffff", -1.0f, -1.0f },
        new object[] { "0000 0080", -2147483.75f, float.MaxValue },
        new object[] { "0000 0080", -2147483.75f, float.MinValue },
        new object[] { "0000 0080", -2147483.75f, float.NaN },
    };

    private static readonly object[] F64_CASES =
    {
        new object[] { "0000 0000 0000 0000", 0.0, 0.0 },
        new object[] { "0080 0000 0000 0000", 1.0, 1.0 },
        new object[] { "0080 ffff ffff ffff", -1.0, -1.0 },
        new object[] { "3f2c 0600 0000 0000", 12.34567, 12.3456789 },
        new object[] { "0000 0000 0000 0080", -281474976710656.0, double.MaxValue },
        new object[] { "0000 0000 0000 0080", -281474976710656.0, double.MinValue },
        new object[] { "0000 0000 0000 0080", -281474976710656.0, double.NaN },
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
