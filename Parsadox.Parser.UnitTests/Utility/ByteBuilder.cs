using System.Text;

namespace Parsadox.Parser.UnitTests.Utility;

/// <summary>
/// Helper class to build byte arrays with parseable data.
/// </summary>
internal class ByteBuilder : MemoryStream
{
    private readonly IFloatConverter _converter;
    private readonly Encoding _encoding;

    public ByteBuilder(IFloatConverter? converter = null, Encoding? encoding = null)
    {
        _converter = converter ?? DefaultFloatConverter.Instance;
        _encoding = encoding ?? Encoding.UTF8;
    }

    public ByteBuilder Append(byte[] bytes)
    {
        Write(bytes);
        return this;
    }

    public ByteBuilder AppendCode(ushort code) => Append(BitConverter.GetBytes(code));

    public ByteBuilder AppendEquals() => AppendCode(BinaryTokenCodes.EQUALS);
    public ByteBuilder AppendOpen() => AppendCode(BinaryTokenCodes.OPEN);
    public ByteBuilder AppendClose() => AppendCode(BinaryTokenCodes.CLOSE);
    public ByteBuilder AppendRgb() => AppendCode(BinaryTokenCodes.RGB);

    public ByteBuilder AppendQuotedString(string value, ushort typeCode = BinaryTokenCodes.STRING)
    {
        AppendCode(typeCode);
        var bytes = _encoding.GetBytes(value);
        Write(BitConverter.GetBytes((ushort)bytes.Length));
        Write(bytes);
        return this;
    }

    public ByteBuilder AppendBool(bool value) => AppendCode(BinaryTokenCodes.BOOL).Append(BitConverter.GetBytes(value));
    public ByteBuilder AppendI32(int value) => AppendCode(BinaryTokenCodes.I32).Append(BitConverter.GetBytes(value));
    public ByteBuilder AppendU32(uint value) => AppendCode(BinaryTokenCodes.U32).Append(BitConverter.GetBytes(value));
    public ByteBuilder AppendU64(ulong value) => AppendCode(BinaryTokenCodes.U64).Append(BitConverter.GetBytes(value));

    public ByteBuilder AppendF32(float value)
    {
        AppendCode(BinaryTokenCodes.F32);
        using BinaryWriter writer = new(this, _encoding, leaveOpen: true);
        _converter.WriteBinaryF32(writer, value);
        return this;
    }

    public ByteBuilder AppendF64(double value)
    {
        AppendCode(BinaryTokenCodes.F64);
        using BinaryWriter writer = new(this, _encoding, leaveOpen: true);
        _converter.WriteBinaryF64(writer, value);
        return this;
    }

    public ByteBuilder AppendUtf8(string text)
    {
        Write(Encoding.UTF8.GetBytes(text));
        return this;
    }

    public ByteBuilder ResetPosition()
    {
        Position = 0L;
        return this;
    }

    public byte[] ToArrayAndDispose()
    {
        Dispose();
        return ToArray();
    }
}
