namespace Parsadox.Parser.Tokens;

internal class TextTokenReader : StreamProcessor, ITokenReader
{
    private readonly IGameHandler _gameHandler;
    private readonly TextReader _reader;
    private readonly StringBuilder _buffer = new();
    private bool _isQuotedString;
    private bool _isComment;
    private int _dotCount;

    internal TextTokenReader(Stream input, IGameHandler gameHandler, IProgress<long>? progress, CancellationToken cancellationToken)
        : base(input, progress, cancellationToken)
    {
        _gameHandler = gameHandler;

        _reader = new StreamReader(input, _gameHandler.TextEncoding);
    }

    public void Dispose() => _reader.Dispose();

    public IEnumerable<IToken> ReadAll(bool shouldIncludeComments)
    {
        StartProcessingOrThrow();

        while (true)
        {
            int cur = _reader.Read();
            if (cur < 0)
            {
                // End of stream
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                yield break;
            }
            char c = (char)cur;
            if (c == '=')
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                yield return SpecialToken.EQUALS;
            }
            else if (c == '{')
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                yield return SpecialToken.OPEN;
            }
            else if (c == '}')
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                yield return SpecialToken.CLOSE;
            }
            else if (c == '"')
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                ReadOrSkipRestOfQuotedString(isReading: true);
                yield return CreateTokenAndClearBuffer();
            }
            else if (c == '#')
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
                ReadOrSkipRestOfComment(isReading: shouldIncludeComments);
                if (shouldIncludeComments)
                    yield return CreateTokenAndClearBuffer();
            }
            else if (char.IsWhiteSpace(c))
            {
                if (_buffer.Length > 0)
                    yield return CreateTokenAndClearBuffer();
            }
            else
            {
                _buffer.Append(c);
                if (c == '.')
                    _dotCount++;
            }
        }
    }

    public IEnumerable<IToken> ReadOnlySections(bool isBlocklist, IReadOnlySet<string> filter)
    {
        StartProcessingOrThrow();
        int depth = 0;
        char toProcess = default;
        bool isTokenEnd = false;
        bool isExpectingEquals = false;
        bool isNextTokenAllowed = false;
        IToken token;

        while (true)
        {
            char c;
            if (toProcess == default)
            {
                int cur = _reader.Read();
                if (cur < 0)
                {
                    // End of stream
                    if (_buffer.Length > 0 && GetTokenAndIsAllowed())
                        yield return token;
                    yield break;
                }
                c = (char)cur;
            }
            else
            {
                c = toProcess;
                toProcess = default;
            }

            if (c == '#')
            {
                // Always skip comments when using a section whitelist.
                ReadOrSkipRestOfComment(isReading: false);
                isTokenEnd = true;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                isTokenEnd = true;
                continue;
            }

            if (isTokenEnd || c == '=' || c == '{' || c == '}' || c == '"')
            {
                isTokenEnd = false;
                if (_buffer.Length > 0)
                {
                    if (GetTokenAndIsAllowed())
                    {
                        yield return token;
                        isExpectingEquals = true;
                    }
                    else if (c == '=')
                    {
                        toProcess = SkipValueAndChildren();
                        continue;
                    }
                }
            }

            if (c == '=')
            {
                yield return SpecialToken.EQUALS;
                if (isExpectingEquals)
                {
                    isExpectingEquals = false;
                    isNextTokenAllowed = true;
                }
                continue;
            }
            isExpectingEquals = false;

            if (c == '{')
            {
                // Check for anonymous array at the root level.
                if (depth == 0 && !isNextTokenAllowed && !IsAnonymousArrayAllowed())
                {
                    SkipRestOfArray();
                    continue;
                }

                depth++;
                yield return SpecialToken.OPEN;
                continue;
            }

            if (c == '}')
            {
                depth--;
                yield return SpecialToken.CLOSE;
                continue;
            }

            if (c == '"')
            {
                ReadOrSkipRestOfQuotedString(isReading: true);
                if (depth > 0 || isNextTokenAllowed)
                {
                    if (GetTokenAndIsAllowed())
                        yield return token;
                }
                else
                    isTokenEnd = true;
                continue;
            }

            _buffer.Append(c);
            if (c == '.')
                _dotCount++;
        }

        bool GetTokenAndIsAllowed()
        {
            token = CreateTokenAndClearBuffer();
            if (isNextTokenAllowed)
            {
                isNextTokenAllowed = false;
                return true;
            }
            if (depth > 0)
                return true;
            if (isBlocklist)
                return !filter.Contains(token.Text);
            return filter.Contains(token.Text);
        }

        bool IsAnonymousArrayAllowed()
        {
            if (isBlocklist)
                return !filter.Contains(string.Empty);
            return filter.Contains(string.Empty);
        }
    }

    private char SkipValueAndChildren()
    {
        // Assumption: reader is just past key=
        //
        // Even though the content is being ignored, quoted strings and comments
        // still need to be processed because they may contain unbalanced {}'s.
        while (true)
        {
            int cur = _reader.Read();

            if (cur < 0)
                return default;
            if (cur == '{')
                break;
            if (char.IsWhiteSpace((char)cur))
                continue;
            if (cur == '"')
            {
                // Handle key="value"
                ReadOrSkipRestOfQuotedString(isReading: false);
                return default;
            }
            if (cur == '#')
            {
                ReadOrSkipRestOfComment(isReading: false);
                continue;
            }
            // Handle key=value
            while (true)
            {
                cur = _reader.Read();
                if (cur < 0 || char.IsWhiteSpace((char)cur))
                    return default;
                if (cur == '=' || cur == '{' || cur == '}' || cur == '#' || cur == '"')
                {
                    // Whoops, hit the next token.
                    return (char)cur;
                }
            }
        }
        // Handle key={value}
        SkipRestOfArray();
        return default;
    }

    private void SkipRestOfArray()
    {
        // Assumption: reader is just past {
        //
        // Even though the content is being ignored, quoted strings and comments
        // still need to be processed because they may contain unbalanced {}'s.
        int depth = 1;
        while (true)
        {
            int cur = _reader.Read();
            if (cur < 0)
                return;
            if (cur == '{')
                depth++;
            else if (cur == '}')
            {
                depth--;
                AsyncUpdate();
                if (depth == 0)
                    return;
            }
            else if (cur == '"')
                ReadOrSkipRestOfQuotedString(isReading: false);
            else if (cur == '#')
                ReadOrSkipRestOfComment(isReading: false);
        }
    }

    private void ReadOrSkipRestOfQuotedString(bool isReading)
    {
        if (isReading)
            _isQuotedString = true;

        // Assumption: reader is just past "
        bool isEscaping = false;
        while (true)
        {
            int cur = _reader.Read();
            if (cur < 0)
            {
                // End of stream inside quoted string
                if (isReading && isEscaping)
                    _buffer.Append('\\');
                return;
            }
            if (isEscaping)
            {
                if (isReading)
                    _buffer.Append((char)cur);
                isEscaping = false;
            }
            else if (cur == '\\')
            {
                isEscaping = true;
            }
            else if (cur == '"')
            {
                return;
            }
            else if (isReading)
            {
                _buffer.Append((char)cur);
            }
        }
    }

    private void ReadOrSkipRestOfComment(bool isReading)
    {
        if (isReading)
            _isComment = true;

        // Assumption: reader is just past #
        while (true)
        {
            int cur = _reader.Read();
            if (cur == '\n' || cur < 0)
                return;
            if (isReading && cur != '\r')
                _buffer.Append((char)cur);
        }
    }

    private IToken CreateTokenAndClearBuffer()
    {
        AsyncUpdate();

        string value = _buffer.ToString();
        _buffer.Clear();

        IToken result;
        if (_isQuotedString)
            result = new QuotedStringToken(value);
        else if (_isComment)
            result = new CommentToken(value);
        else if (_dotCount >= 2)
            result = new GenericTextProbablyGameDateToken(value);
        else
            result = new GenericTextToken(value);

        _isQuotedString = false;
        _isComment = false;
        _dotCount = 0;

        return result;
    }
}
