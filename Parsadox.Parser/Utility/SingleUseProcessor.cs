namespace Parsadox.Parser.Utility;

/// <summary>
/// For single-use classes that consume an object passed in to the ctor,
/// ensure that the object isn't consumed multiple times.
/// </summary>
internal abstract class SingleUseProcessor
{
    private readonly object _lock = new();
    private bool _hasProcessed;

    protected void StartProcessingOrThrow()
    {
        lock (_lock)
        {
            if (_hasProcessed)
                throw new InvalidOperationException($"Attempting to process the same {GetType().Name} twice");
            _hasProcessed = true;
        }
    }
}
