namespace Parsadox.Parser.Utility;

internal static class StreamExtensions
{
    internal static string ReadString(this Stream stream, int length, Encoding? encoding = null)
    {
        var bytes = stream.ReadBytes(length);
        return (encoding ?? Encoding.UTF8).GetString(bytes);
    }

    internal static string PeekString(this Stream stream, int length, Encoding? encoding = null)
    {
        var bytes = stream.PeekBytes(length);
        return (encoding ?? Encoding.UTF8).GetString(bytes);
    }

    internal static byte[] ReadBytes(this Stream stream, int length)
    {
        var bytes = new byte[length];
        if (stream.Read(bytes) != length)
            throw new ParseException($"Invalid input: fewer than {length} bytes available in stream at position {stream.Position}");
        return bytes;
    }

    internal static byte[] PeekBytes(this Stream stream, int length)
    {
        var bytes = stream.ReadBytes(length);
        stream.Seek(-length, SeekOrigin.Current);
        return bytes;
    }

    internal static void SkipByte(this Stream stream, int expected)
    {
        var actual = stream.ReadByte();
        if (actual >= 0 && actual != expected)
            stream.Seek(-1L, SeekOrigin.Current);
    }

    internal static void WriteString(this Stream stream, string text, Encoding encoding)
    {
        if (string.IsNullOrEmpty(text))
            return;
        byte[] bytes = encoding.GetBytes(text);
        stream.Write(bytes);
    }
}
