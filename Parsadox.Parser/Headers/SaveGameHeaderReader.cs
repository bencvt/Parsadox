namespace Parsadox.Parser.Headers;

internal class SaveGameHeaderReader : SingleUseProcessor
{
    private const char ZIP_START_0 = 'P';
    private const char ZIP_START_1 = 'K';
    private const char ZIP_START_2 = '\x03';
    private const char ZIP_START_3 = '\x04';

    private readonly Stream _input;
    private readonly bool _isMain;
    private readonly string? _fileName;
    private readonly TextWriter _log;
    private readonly IGameHandler _gameHandler;

    internal SaveGameHeaderReader(Stream input, bool isMain, string? fileName, TextWriter log, IGameHandler gameHandler)
    {
        _input = input;
        _isMain = isMain;
        _fileName = fileName;
        _log = log;
        _gameHandler = gameHandler;
    }

    internal ISaveGameHeader ReadAndSkipToContent()
    {
        StartProcessingOrThrow();

        if (_gameHandler.HasHeader(_input))
            return ReadAndSkipToContentInternal();

        _log.WriteLine(_isMain ? "No main header" : "No entry header");
        _log.WriteLine();
        return new SaveGameHeader
        {
            FileName = _fileName,
            Format = IsCompressedContent()
                ? SaveGameFormat.CompressedAuto
                : IsBinaryContent() ? SaveGameFormat.UncompressedBinary : SaveGameFormat.UncompressedText,
        };
    }

    private bool IsCompressedContent()
    {
        var bytes = _input.PeekBytes(4);
        return bytes[0] == ZIP_START_0 && bytes[1] == ZIP_START_1 && bytes[2] == ZIP_START_2 && bytes[3] == ZIP_START_3;
    }

    private bool IsBinaryContent()
    {
        // Peek the first four bytes and look for an equals token in the latter two bytes.
        var bytes = _input.PeekBytes(4);
        bool isBinary = bytes[2] == 1 && bytes[3] == 0;

        _log.WriteLine("Format appears to be " + (isBinary ? "binary" : "text"));
        return isBinary;
    }

    private ISaveGameHeader ReadAndSkipToContentInternal()
    {
        var header = _gameHandler.ReadHeader(_input, _isMain);
        header.FileName = _fileName;

        _log.WriteLine(header);

        if (header.BytesUntilContent > 0)
            _input.Seek(header.BytesUntilContent, SeekOrigin.Current);

        if (header.Format.IsCompressed() && !IsCompressedContent())
        {
            _log.WriteLine($"Content starting at 0x{_input.Position:x8} is not compressed");
            _log.WriteLine("Attempting to find compressed content start exhaustively in case the header is bad but the data is good");

            _input.Seek(-header.BytesUntilContent, SeekOrigin.Current);
            SkipToCompressedContentStart();
        }

        return header;
    }

    private void SkipToCompressedContentStart()
    {
        int cur = 0;
        while (true)
        {
            if (cur == ZIP_START_0)
            {
                if (Next() == ZIP_START_1 && Next() == ZIP_START_2 && Next() == ZIP_START_3)
                {
                    _input.Seek(-4, SeekOrigin.Current);
                    _log.WriteLine($"Found compressed content start at 0x{_input.Position:x8}");
                    return;
                }
            }
            else
                Next();
        }
        int Next()
        {
            cur = _input.ReadByte();
            if (cur < 0)
                throw new ParseException("Compressed format missing content");
            return cur;
        }
    }
}
