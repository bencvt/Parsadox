namespace Parsadox.Parser.Utility;

/// <summary>
/// Wrapper around Stream that always returns false for CanSeek.
/// <para/>
/// This is necessary because System.IO.Compression seeks to the beginning of the stream if it can.
/// When it finds non-zip-related content, bad things happen.
/// <para/>
/// For example, when reading, it throws:
/// <code>
///   Number of entries expected in End Of Central Directory does not correspond to number of entries in Central Directory.
///      at System.IO.Compression.ZipArchive.ReadCentralDirectory()
///      at System.IO.Compression.ZipArchive.get_Entries()
/// </code>
/// Compressed Ck3 and Imperator save game files always have header data before the compressed data.
/// A workaround is necessary, either:
/// <list type="number">
/// <item>Use a temporary file;</item>
/// <item>Use a MemoryStream buffer; or</item>
/// <item>Use a lightweight wrapper around the stream that always returns false for CanSeek.</item>
/// </list>
/// Option #3 is the most performant.
/// </summary>
internal class NoSeekStream : Stream
{
    private readonly Stream _inner;
    public NoSeekStream(Stream inner) { _inner = inner; }
    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => _inner.CanWrite;
    public override long Length => _inner.Length;
    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }
    public override void Flush() => _inner.Flush();
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
    public override void SetLength(long value) => _inner.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
}
