using System.IO.Compression;

namespace Parsadox.Parser.SaveGames;

internal class SaveGameWriter
{
    private readonly ISaveGame _saveGame;
    private readonly IGameHandler _gameHandler;
    private readonly WriteParameters _parameters;
    private readonly TextWriter _log;

    internal SaveGameWriter(ISaveGame saveGame, WriteParameters parameters)
    {
        _saveGame = saveGame;
        _gameHandler = _saveGame.Game.GetHandler(_saveGame.Header.Version);
        _parameters = (WriteParameters)parameters.DeepCopyAndInitialize();
        ResolveFormat();
        _log = _parameters.Log;
    }

    private void ResolveFormat()
    {
        if (_parameters.SaveGameFormat != SaveGameFormat.CompressedAuto)
            return;
        if (_gameHandler.Compression == SaveGameCompression.Never)
            _parameters.SaveGameFormat = SaveGameFormat.UncompressedText;
        else
            _parameters.SaveGameFormat = SaveGameFormat.CompressedText;
    }

    internal async Task<long> WriteFileAsync(string outputPath, CancellationToken cancellationToken)
    {
        using var outputStream = FileSystem.Instance.OpenCreate(outputPath);
        var func = () =>
        {
            _log.WriteLine($"Writing to {Path.GetFullPath(outputPath)}");
            return outputStream;
        };
        return await Task.FromResult(ValidateAndWriteStream(func, outputPath, cancellationToken));
    }

    internal async Task<long> WriteStreamAsync(Stream outputStream, CancellationToken cancellationToken)
    {
        return await Task.FromResult(ValidateAndWriteStream(() => outputStream, null, cancellationToken));
    }

    private long ValidateAndWriteStream(Func<Stream> getOutputStream, string? outputPath, CancellationToken cancellationToken)
    {
        LogStart();
        _parameters.SaveGameFormat.ValidateCanWrite(_saveGame, x => x != SaveGameFormat.Unknown && !x.IsBinary());
        using var timed = new Timed(_log, "Wrote save game in");

        Stream outputStream = getOutputStream();
        long startPosition = outputStream.Position;

        WriteStream(outputStream, outputPath, cancellationToken);

        long bytesWritten = outputStream.Position - startPosition;
        _log.WriteLineCount("Wrote", bytesWritten, "byte");
        return bytesWritten;
    }

    private void LogStart()
    {
        if (!_parameters.IsLogging)
            return;
        _log.WriteLine("--");
        _log.WriteLine($"[{DateTime.Now:o}]");
        _log.WriteLine($"Assembly: {GetType().Assembly}");
        _log.WriteLine($"Writing with {GetType().Name}: {_saveGame}");
        _log.WriteLine(_parameters);
        _log.Flush();
    }

    private void WriteStream(Stream outputStream, string? outputPath, CancellationToken cancellationToken)
    {
        _gameHandler.WriteMainHeader(_saveGame, outputStream, _parameters);
        cancellationToken.ThrowIfCancellationRequested();

        if (_parameters.SaveGameFormat.IsCompressed())
        {
            WriteCompressed(outputStream, outputPath, cancellationToken);
        }
        else
        {
            // Flatten entries.
            foreach (var entryNode in _saveGame.Root)
                WriteUncompressedText(false, entryNode, outputStream, cancellationToken);
        }
    }

    private void WriteCompressed(Stream outputStream, string? outputPath, CancellationToken cancellationToken)
    {
        using NoSeekStream wrapper = new(outputStream);
        using ZipArchive archive = new(wrapper, ZipArchiveMode.Create);
        foreach (var entryNode in _gameHandler.GetEntryNodesToWrite(_saveGame))
        {
            string entryName = _gameHandler.AdjustEntryNameForWrite(entryNode.Content.Text, outputPath);
            using var entryStream = archive.CreateEntry(entryName).Open();

            _gameHandler.WriteEntryHeader(_saveGame, entryNode, entryStream, _parameters);
            cancellationToken.ThrowIfCancellationRequested();

            WriteUncompressedText(true, entryNode, entryStream, cancellationToken);
        }
    }

    private void WriteUncompressedText(bool isCompressed, INode entryNode, Stream output, CancellationToken cancellationToken)
    {
        foreach (var section in entryNode)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_gameHandler.ShouldWriteSection(isCompressed, entryNode, section))
            {
                string text = NodeExporter.Export(section, _parameters.NodeOutputFormat, _gameHandler.WriteIndentLevel);
                output.WriteString(text, _gameHandler.TextEncoding);
            }
            else
            {
                _log.WriteLine($"Skipped {entryNode.Content.Text}.{section.Content.Text}");
            }
        }
        _gameHandler.WriteEntryFooter(_saveGame, output, _parameters);
    }
}
