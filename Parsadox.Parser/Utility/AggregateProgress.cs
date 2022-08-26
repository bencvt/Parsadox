namespace Parsadox.Parser.Utility;

/// <summary>
/// Track progress for multiple streams being processed in parallel.
/// </summary>
internal class AggregateProgress
{
    private readonly IProgress<double> _progress;
    private readonly List<long> _amounts = new();
    private long _total;

    internal AggregateProgress(IProgress<double> progress)
    {
        _progress = progress;
    }

    internal IProgress<long> Add(Stream stream)
    {
        _total += stream.Length;

        int index = _amounts.Count;
        _amounts.Add(stream.Position);

        return new Progress<long>(value =>
        {
            _amounts[index] = value;
            Report();
        });
    }

    private void Report() => _progress.Report(100.0 * _amounts.Sum() / _total);
}
