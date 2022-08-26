using System.Text.RegularExpressions;

namespace Parsadox.Parser.UnitTests.Floats;

[TestCovers(typeof(IFloatConverter))]
public abstract class FloatConverterTestsBase : TestsBase
{
    internal abstract IFloatConverter Converter { get; }

    protected void TestReadBinaryF32(string hex, float expectedValue)
    {
        using MemoryStream buffer = new(FromHexDump(hex));
        using BinaryReader reader = new(buffer);
        float actual = Converter.ReadBinaryF32(reader);

        Assert.That(actual, Is.EqualTo(expectedValue));
    }

    protected void TestWriteBinaryF32(float value, string expectedHex)
    {
        using MemoryStream buffer = new();
        using BinaryWriter writer = new(buffer);
        Converter.WriteBinaryF32(writer, value);
        string actual = HexDump(buffer.ToArray());

        Assert.That(actual, Is.EqualTo(expectedHex));
    }

    protected void TestReadBinaryF64(string hex, double expectedValue)
    {
        using MemoryStream buffer = new(FromHexDump(hex));
        using BinaryReader reader = new(buffer);
        double actual = Converter.ReadBinaryF64(reader);

        Assert.That(actual, Is.EqualTo(expectedValue));
    }

    protected void TestWriteBinaryF64(double value, string expectedHex)
    {
        using MemoryStream buffer = new();
        using BinaryWriter writer = new(buffer);
        Converter.WriteBinaryF64(writer, value);
        string actual = HexDump(buffer.ToArray());

        Assert.That(actual, Is.EqualTo(expectedHex));
    }

    /// <summary>
    /// Group pairs of bytes separated by spaces.
    /// </summary>
    private static string HexDump(byte[] bytes)
    {
        string s = BitConverter.ToString(bytes).ToLowerInvariant();
        return RE_HEX_DUMP.Replace(s, "$1$2 ").TrimEnd();
    }
    private static readonly Regex RE_HEX_DUMP = new("([0-9a-f]{2})-([0-9a-f]{2})-?");

    private static byte[] FromHexDump(string raw)
    {
        using MemoryStream buffer = new();
        foreach (string pair in raw.Split())
        {
            buffer.WriteByte(Convert.ToByte(pair[..2], 16));
            if (pair.Length > 2)
                buffer.WriteByte(Convert.ToByte(pair[2..], 16));
        }
        return buffer.ToArray();
    }
}
