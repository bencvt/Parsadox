using System.Diagnostics;

namespace Parsadox.Parser.Utility;

internal abstract class StreamProcessor : SingleUseProcessor
{
    private const long UPDATE_INTERVAL_MS = 500;

    private readonly Stream _stream;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly IProgress<long>? _progress;
    private readonly CancellationToken _cancellationToken;

    protected StreamProcessor(Stream stream, IProgress<long>? progress, CancellationToken cancellationToken)
    {
        _stream = stream;
        _progress = progress;
        _cancellationToken = cancellationToken;
    }

    protected void AsyncUpdate()
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (_progress is not null && _stopwatch.ElapsedMilliseconds >= UPDATE_INTERVAL_MS)
        {
            _progress.Report(_stream.Position);
            _stopwatch.Restart();
        }
    }
}
