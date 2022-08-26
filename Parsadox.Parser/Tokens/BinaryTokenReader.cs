namespace Parsadox.Parser.Tokens;

internal class BinaryTokenReader : StreamProcessor, ITokenReader
{
    private readonly IGameHandler _gameHandler;
    private readonly BinaryReader _reader;
    private readonly ITokenMap _tokenMap;
    private readonly bool _abortIfUnmapped;
    private readonly Func<ushort, IToken, IToken> _wrapTokenFunc;

    internal BinaryTokenReader(Stream input, IGameHandler gameHandler, ITokenMap tokenMap, bool abortIfUnmapped, bool shouldExtractCodes, IProgress<long>? progress, CancellationToken cancellationToken)
        : base(input, progress, cancellationToken)
    {
        _gameHandler = gameHandler;

        _reader = new BinaryReader(input, gameHandler.TextEncoding, leaveOpen: true);

        _tokenMap = tokenMap;

        _abortIfUnmapped = abortIfUnmapped;

        // Don't waste time and memory storing codes when not needed.
        _wrapTokenFunc = shouldExtractCodes
            ? (code, token) => new BinaryCodeWrapperToken(code, token)
            : (code, token) => token;
    }

    public void Dispose() => _reader.Dispose();

    // Binary data never contains comments, so ignore the parameter.
    public IEnumerable<IToken> ReadAll(bool shouldIncludeComments)
    {
        StartProcessingOrThrow();
        while (true)
        {
            AsyncUpdate();

            IToken token;
            // Inner try block is necessary because CS1626 (Cannot yield a value in the body of a try block with a catch clause).
            // This isn't overly expensive in practice, as long as the try block stays in this method.
            try
            {
                token = ReadNext(isIgnoringData: false);
            }
            catch (EndOfStreamException)
            {
                break;
            }
            yield return token;
        }
    }

    public IEnumerable<IToken> ReadOnlySections(bool isBlocklist, IReadOnlySet<string> filter)
    {
        StartProcessingOrThrow();
        int depth = 0;
        bool isAllowed = false;
        bool isExpectingEquals = false;
        bool isExpectingValueOrOpen = false;

        while (true)
        {
            AsyncUpdate();

            IToken token;
            try
            {
                token = ReadNext(!isAllowed && depth > 0);
            }
            catch (EndOfStreamException)
            {
                break;
            }

            if (token.IsOpen)
            {
                // Check for anonymous array at the root level.
                if (depth == 0 && !isExpectingValueOrOpen)
                    isAllowed = isBlocklist ? !filter.Contains(string.Empty) : filter.Contains(string.Empty);

                depth++;
                isExpectingValueOrOpen = false;
            }
            else if (token.IsClose)
            {
                depth--;
            }
            else if (depth == 0)
            {
                if (isExpectingEquals)
                {
                    if (token.IsEquals)
                    {
                        isExpectingEquals = false;
                        isExpectingValueOrOpen = true;
                    }
                    else
                    {
                        // Key had no value; starting on another key
                        isAllowed = isBlocklist ? !filter.Contains(token.Text) : filter.Contains(token.Text);
                    }
                }
                else if (isExpectingValueOrOpen)
                {
                    // Simple key=value in root (e.g. random_seed)
                    isExpectingValueOrOpen = false;
                }
                else if (isBlocklist ? !filter.Contains(token.Text) : filter.Contains(token.Text))
                {
                    isAllowed = true;
                    isExpectingEquals = true;
                }
                else
                {
                    isAllowed = false;
                    isExpectingEquals = true;
                }
            }

            if (isAllowed)
                yield return token;
        }
    }

    private IToken ReadNext(bool isIgnoringData)
    {
        ushort code = _reader.ReadUInt16();

        if (code == BinaryTokenCodes.OPEN)
            return SpecialToken.OPEN;

        if (code == BinaryTokenCodes.CLOSE)
            return SpecialToken.CLOSE;

        if (isIgnoringData)
        {
            ushort skipBytes = code switch
            {
                BinaryTokenCodes.BOOL => 1,
                BinaryTokenCodes.I32 or BinaryTokenCodes.U32 or BinaryTokenCodes.F32 => 4,
                BinaryTokenCodes.U64 or BinaryTokenCodes.F64 => 8,
                BinaryTokenCodes.STRING or BinaryTokenCodes.STRING_ALT => _reader.ReadUInt16(),
                _ => 0,
            };
            if (skipBytes > 0)
                _reader.BaseStream.Seek(skipBytes, SeekOrigin.Current);
            return SpecialToken.EMPTY;
        }

        switch (code)
        {
            case BinaryTokenCodes.EQUALS:
                return SpecialToken.EQUALS;

            case BinaryTokenCodes.BOOL:
                if (_reader.ReadByte() == 0)
                    return SpecialToken.NO;
                return SpecialToken.YES;

            case BinaryTokenCodes.I32:
                return new I32Token(_reader.ReadInt32());

            case BinaryTokenCodes.U32:
                return new U32Token(_reader.ReadUInt32());

            case BinaryTokenCodes.U64:
                return new U64Token(_reader.ReadUInt64());

            case BinaryTokenCodes.F32:
                if (_gameHandler.FloatConverter.IsBig)
                    return new BigF32Token(_gameHandler.FloatConverter.ReadBinaryF32(_reader));
                else
                    return new F32Token(_gameHandler.FloatConverter.ReadBinaryF32(_reader));

            case BinaryTokenCodes.F64:
                if (_gameHandler.FloatConverter.IsBig)
                    return new BigF64Token(_gameHandler.FloatConverter.ReadBinaryF64(_reader));
                else
                    return new F64Token(_gameHandler.FloatConverter.ReadBinaryF64(_reader));

            case BinaryTokenCodes.STRING:
            case BinaryTokenCodes.STRING_ALT:
                byte[] raw = _reader.ReadBytes(_reader.ReadUInt16());
                return _wrapTokenFunc(code, new QuotedStringToken(_gameHandler.TextEncoding.GetString(raw)));

            case BinaryTokenCodes.RGB:
                return ColorToken.RGB;

            case BinaryTokenCodes.HEX:
                return ColorToken.HEX;

            case BinaryTokenCodes.HSV:
                return ColorToken.HSV;

            case BinaryTokenCodes.HSV360:
                return ColorToken.HSV360;

            default:
                string? name = _tokenMap.CodeMap.GetValueOrDefault(code);
                if (name is null)
                {
                    if (_abortIfUnmapped)
                        throw new ParseException($"0x{code:x4} is not in the mapping");
                    return new U16Token(code);
                }
                return _wrapTokenFunc(code, new ResolvedNameToken(name));
        };
    }
}
