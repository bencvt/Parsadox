using System.IO.Compression;

namespace Parsadox.Parser.SaveGames;

internal abstract class SaveGameReaderBase<TItem, TResult>
{
    protected const string DEFAULT_ENTRY_NAME = "gamestate";

    protected readonly ReadParameters _parameters;
    protected readonly ITokenMap _tokenMap;
    protected IGameHandler _gameHandler;
    protected readonly TextWriter _log;

    protected SaveGameReaderBase(ReadParameters parameters)
    {
        _parameters = (ReadParameters)parameters.DeepCopyAndInitialize();
        _tokenMap = _parameters.TokenMap ?? TokenMapFactory.Create();
        _gameHandler = _parameters.Game.GetDefaultVersionHandler();
        _log = _parameters.Log;
    }

    protected virtual bool ShouldExtractCodes => false;

    protected virtual bool ShouldIncludeComments => false;

    internal async Task<TResult> LoadFileAsync(string path, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        progress?.Report(0.0);
        LogStart();
        using Timed timed = new(_log, "Total time spent reading and parsing file:");

        FileSystem.AssertFileExists(path);
        string originalFilePath = FileSystem.Instance.GetAbsolutePath(path);
        string actualFilePath = originalFilePath;

        string? tempPath = null;
        try
        {
            if (_parameters.MakeTemporaryCopyOfInputFile)
            {
                tempPath = FileSystem.Instance.CreateTemporaryFile();
                _log.WriteLine($"Creating temporary copy of {originalFilePath}");
                FileSystem.Instance.Copy(originalFilePath, tempPath, overwrite: true);
                actualFilePath = tempPath;
            }

            using var stream = FileSystem.Instance.OpenRead(actualFilePath);
            if (_parameters.IsLogging)
            {
                _log.WriteLine($"Loading save game file: {FileSystem.Instance.GetAbsolutePath(actualFilePath)}");
                _log.WriteLineCount("File is", FileSystem.Instance.GetFileSize(actualFilePath), "byte");
            }

            TResult result = await LoadInternalAsync(stream, originalFilePath, progress, cancellationToken);
            progress?.Report(100.0);
            return result;
        }
        finally
        {
            if (tempPath is not null)
            {
                _log.WriteLine("Deleting temporary file");
                FileSystem.Instance.Delete(tempPath);
            }
        }
    }

    internal async Task<TResult> LoadStreamAsync(Stream input, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        progress?.Report(0.0);
        LogStart();
        _log.WriteLine($"Loading save game from stream");
        using Timed timed = new(_log, "Total time spent reading and parsing stream:");
        TResult result = await LoadInternalAsync(input, originalFilePath: null, progress, cancellationToken);
        progress?.Report(100.0);
        return result;
    }

    private void LogStart()
    {
        if (!_parameters.IsLogging)
            return;
        _log.WriteLine("--");
        _log.WriteLine($"[{DateTime.Now:o}]");
        _log.WriteLine($"Assembly: {GetType().Assembly}");
        _log.WriteLine($"Loading save game with {GetType().Name}");
        _log.WriteLine(_parameters);
        _log.Flush();
    }

    private async Task<TResult> LoadInternalAsync(Stream input, string? originalFilePath, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        _log.WriteLine();
        _log.Flush();

        var mainHeader = new SaveGameHeaderReader(input, isMain: true, originalFilePath, _log, _gameHandler)
            .ReadAndSkipToContent();

        _gameHandler = _parameters.Game.GetHandler(mainHeader.Version);

        var entries = CreateEntries(mainHeader, input, cancellationToken).ToList();
        try
        {
            AddProgressTracker(entries, progress);

            IEnumerable<TItem> items = entries.AsParallel().WithCancellation(cancellationToken).Select(ProcessEntry);

            _log.WriteLine("Processing tokens");
            _log.WriteLine();
            _log.Flush();
            return await Task.FromResult(Merge(mainHeader, items));
        }
        finally
        {
            foreach (var entry in entries)
                entry.Input.Dispose();
        }
    }

    /// <summary>
    /// The caller is responsible for positioning the input stream to the
    /// start of the content.
    /// <para/>
    /// For non-compressed save games, this is right after the header.
    /// <para/>
    /// For compressed save games, this is past the header and any other
    /// meta data, right before "PK".
    /// </summary>
    private IEnumerable<Entry> CreateEntries(ISaveGameHeader mainHeader, Stream input, CancellationToken cancellationToken)
    {
        if (!mainHeader.Format.IsCompressed())
        {
            _log.WriteLine("Copying stream to memory");
            _log.Flush();
            using Timed timedCopy = new(_log, "Copied stream to memory in");

            MemoryStream entryStream = new();
            input.CopyTo(entryStream);
            _log.WriteLineCount("Copied", entryStream.Position, "byte");
            entryStream.Position = 0L;

            var entryHeader = new SaveGameHeaderReader(entryStream, isMain: false, DEFAULT_ENTRY_NAME, _log, _gameHandler)
                .ReadAndSkipToContent();

            yield return new Entry(entryHeader, entryStream, cancellationToken);
            yield break;
        }

        _log.WriteLine("Decompressing content");
        _log.Flush();
        using Timed timedDecompress = new(_log, "Decompressed content in");

        // See NoSeekStream class for explanation why the wrapper is needed.
        using NoSeekStream wrapper = new(input);
        using ZipArchive archive = new(wrapper);
        foreach (var entry in archive.Entries)
        {
            if (entry.Name == _gameHandler.DoNotLoadEntry)
            {
                _log.WriteLine($"Skipped redundant {entry.Name} entry");
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Copy all of the decompressed content to a memory buffer.
            // This is more performant than using archiveStream directly.
            using Stream originalEntryStream = entry.Open();
            MemoryStream entryStream = new();
            originalEntryStream.CopyTo(entryStream);
            _log.WriteLineCount("Decompressed", entryStream.Position, "byte");
            entryStream.Position = 0L;

            var entryHeader = new SaveGameHeaderReader(entryStream, isMain: false, entry.Name, _log, _gameHandler)
                .ReadAndSkipToContent();

            yield return new Entry(entryHeader, entryStream, cancellationToken);
        }
    }

    /// <summary>
    /// Entries are loaded in parallel, which complicates IProgress updates.
    /// <para/>
    /// This logic is encapsulated in the <see cref="AggregateProgress"/> class.
    /// </summary>
    private static void AddProgressTracker(List<Entry> entries, IProgress<double>? progress)
    {
        if (progress is null)
            return;

        var tracker = new AggregateProgress(progress);
        foreach (var entry in entries)
            entry.Progress = tracker.Add(entry.Input);
    }

    private TItem ProcessEntry(Entry entry)
    {
        using var tokenReader = CreateTokenReader(entry);

        var tokens = _parameters.SectionFilter is null
            ? tokenReader.ReadAll(ShouldIncludeComments)
            : tokenReader.ReadOnlySections(_parameters.IsSectionFilterBlocklist, _parameters.SectionFilter);

        return ProcessTokens(entry.Header, tokens);
    }

    private ITokenReader CreateTokenReader(Entry entry) => entry.Header.Format switch
    {
        SaveGameFormat.UncompressedText => new TextTokenReader(entry.Input, _gameHandler, entry.Progress, entry.CancellationToken),
        SaveGameFormat.UncompressedBinary => new BinaryTokenReader(entry.Input, _gameHandler, _tokenMap, _parameters.AbortIfUnmapped, ShouldExtractCodes, entry.Progress, entry.CancellationToken),
        _ => throw new InvalidOperationException($"Invalid save game format for {entry.Header}")
    };

    protected abstract TItem ProcessTokens(ISaveGameHeader entryHeader, IEnumerable<IToken> tokens);

    protected abstract TResult Merge(ISaveGameHeader mainHeader, IEnumerable<TItem> items);

    private class Entry
    {
        internal ISaveGameHeader Header { get; }
        internal Stream Input { get; }
        internal IProgress<long>? Progress { get; set; }
        internal CancellationToken CancellationToken { get; }

        internal Entry(ISaveGameHeader header, Stream input, CancellationToken cancellationToken)
        {
            Header = header;
            Input = input;
            CancellationToken = cancellationToken;
        }
    }
}
