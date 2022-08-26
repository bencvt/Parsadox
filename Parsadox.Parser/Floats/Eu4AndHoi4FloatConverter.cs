namespace Parsadox.Parser.Floats;

internal class Eu4AndHoi4FloatConverter : IFloatConverter
{
    public bool IsBig => true;

    public float ReadBinaryF32(BinaryReader reader) => reader.ReadInt32() / 1000.0f;

    public void WriteBinaryF32(BinaryWriter writer, float value) => writer.Write((int)(value * 1000.0f));

    public double ReadBinaryF64(BinaryReader reader)
    {
        double value = reader.ReadInt64() / (double)(1 << 15);
        // TODO verify rounding logic
        return Math.Floor(value * 100_000.0) / 100_000.0;
    }

    public void WriteBinaryF64(BinaryWriter writer, double value) => writer.Write((long)(value * (1 << 15)));
}
