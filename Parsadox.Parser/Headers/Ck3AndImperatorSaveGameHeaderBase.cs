namespace Parsadox.Parser.Headers;

internal abstract class Ck3AndImperatorSaveGameHeaderBase : SaveGameHeader
{
    private const string BLANK_HEADER = $"SAV01000000000000000000\n";
    private const int HEADER_LENGTH = 24;
    private static readonly Regex RE_HEADER = new(@"^SAV[0-9a-f]{20}[\r\n]$");
    private static readonly Dictionary<string, SaveGameFormat> FORMATS = new()
    {
        // Used for autosaves:
        ["00"] = SaveGameFormat.UncompressedText,
        ["01"] = SaveGameFormat.UncompressedBinary,

        // Used for non-Ironman manual saves:
        ["02"] = SaveGameFormat.CompressedText,

        // Used for Ironman saves:
        ["03"] = SaveGameFormat.CompressedBinary,

        // Unsupported:
        // 04: compressed text with uncompressed metadata
        // 05: compressed binary with uncompressed metadata
    };

    protected Ck3AndImperatorSaveGameHeaderBase(Stream input)
    {
        string text = input.ReadString(HEADER_LENGTH);
        if (text.EndsWith('\r'))
        {
            // Handle save games that were manually modified with Windows newlines.
            text = text.Replace('\r', '\n');
            input.SkipByte('\n');
        }
        Initialize(text);

        if (Format.IsBinary())
            Version = Ck3AndImperatorBinaryVersionReader.Read(input);
    }

    protected Ck3AndImperatorSaveGameHeaderBase(string text)
    {
        try
        {
            Initialize(text);
        }
        catch
        {
            Initialize(BLANK_HEADER);
        }
    }

    private void Initialize(string text)
    {
        Text = text;
        if (!RE_HEADER.IsMatch(Text))
            throw new ParseException($"Invalid header: {Strings.EscapeAndQuote(Text)}");
        Format = FORMATS.GetValueOrDefault(Text[5..7]);
    }

    public override long BytesUntilContent => Format.IsCompressed() ? MetaDataLength : 0L;

    internal int MetaDataLength => Convert.ToInt32(Text[15..23], 16);

    internal void SetMetaDataLength(int length)
    {
        Text = $"{Text[..15]}{length:x8}{Text[23..]}";
    }

    internal void SetFormat(SaveGameFormat format)
    {
        if (!FORMATS.ContainsValue(format))
            throw new ArgumentException($"Invalid format: {format}");
        Format = format;
        string formatRaw = FORMATS.Single(kvp => kvp.Value == format).Key;
        Text = $"{Text[..5]}{formatRaw}{Text[7..]}";
    }

    internal void Write(IGameHandler handler, INode root, Stream output, WriteParameters parameters)
    {
        SetFormat(parameters.SaveGameFormat);

        // The meta data will be serialized twice, but it's not worth caching because it's relatively small.
        byte[] metaData = handler.TextEncoding.GetBytes(ExportMetaData(root, parameters));
        if (metaData.Length == 0)
            parameters.Log.WriteLine("Missing meta_data section");
        SetMetaDataLength(metaData.Length);

        output.WriteString(Text, handler.TextEncoding);

        // Write uncompressed meta data before compressed content.
        if (parameters.SaveGameFormat.IsCompressed())
            output.Write(metaData);
    }

    protected abstract string ExportMetaData(INode root, WriteParameters parameters);
}
