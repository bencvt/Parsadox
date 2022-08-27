namespace Parsadox.Parser.UnitTests.Floats;

[TestCovers(typeof(Ck3AndImperatorFloatConverter))]
public class Ck3AndImperatorFloatConverterTests : FloatConverterTestsBase
{
    internal override IFloatConverter Converter => Ck3AndImperatorFloatConverter.Instance;

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
        new object[] { "a086 0100 0000 0000", 1.0, 1.0 },
        new object[] { "6079 feff ffff ffff", -1.0, -1.0 },
        new object[] { "87d6 1200 0000 0000", 12.34567, 12.3456789 },
        new object[] { "0000 0000 0000 0080", -92233720368547.766, double.MaxValue },
        new object[] { "0000 0000 0000 0080", -92233720368547.766, double.MinValue },
        new object[] { "0000 0000 0000 0080", -92233720368547.766, double.NaN },
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
