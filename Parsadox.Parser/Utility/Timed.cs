using System.Diagnostics;

namespace Parsadox.Parser.Utility;

internal class Timed : IDisposable
{
    private readonly TextWriter _log;
    private readonly string _messagePrefix;
    private readonly Stopwatch _stopwatch;

    internal Timed(TextWriter log, string messagePrefix)
    {
        _log = log;

        if (string.IsNullOrEmpty(messagePrefix))
            _messagePrefix = string.Empty;
        else
            _messagePrefix = $"{messagePrefix} ";

        _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _log.WriteLine($"{_messagePrefix}{_stopwatch.ElapsedMilliseconds:n0}ms");
        _log.WriteLine();
        _log.Flush();
    }
}
