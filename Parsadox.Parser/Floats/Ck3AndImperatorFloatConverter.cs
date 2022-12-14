namespace Parsadox.Parser.Floats;

internal class Ck3AndImperatorFloatConverter : DefaultFloatConverter
{
    internal static readonly new Ck3AndImperatorFloatConverter Instance = new();

    private Ck3AndImperatorFloatConverter() { }

    public override double ReadBinaryF64(BinaryReader reader) => reader.ReadInt64() / 100_000.0;

    public override void WriteBinaryF64(BinaryWriter writer, double value) => writer.Write((long)(value * 100_000.0));
}
