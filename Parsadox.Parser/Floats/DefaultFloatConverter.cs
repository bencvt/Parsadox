namespace Parsadox.Parser.Floats;

internal class DefaultFloatConverter : IFloatConverter
{
    public bool IsBig => false;

    public virtual float ReadBinaryF32(BinaryReader reader) => reader.ReadSingle();

    public virtual void WriteBinaryF32(BinaryWriter writer, float value) => writer.Write(value);

    public virtual double ReadBinaryF64(BinaryReader reader) => reader.ReadDouble();

    public virtual void WriteBinaryF64(BinaryWriter writer, double value) => writer.Write(value);
}
