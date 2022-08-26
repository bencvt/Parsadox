namespace Parsadox.Parser.Floats;

internal class Ck3Pre1_5FloatConverter : DefaultFloatConverter
{
    public override double ReadBinaryF64(BinaryReader reader) => reader.ReadInt64() / 1000.0;

    public override void WriteBinaryF64(BinaryWriter writer, double value) => writer.Write((long)(value * 1000.0));
}
