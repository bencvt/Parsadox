namespace Parsadox.Parser.Floats;

/// <summary>
/// Binary floating point conversions.
/// </summary>
internal interface IFloatConverter
{
    /// <summary>
    /// Whether to output fewer decimal digits.
    /// </summary>
    bool IsBig { get; }

    float ReadBinaryF32(BinaryReader reader);

    void WriteBinaryF32(BinaryWriter writer, float value);

    double ReadBinaryF64(BinaryReader reader);

    void WriteBinaryF64(BinaryWriter writer, double value);
}
