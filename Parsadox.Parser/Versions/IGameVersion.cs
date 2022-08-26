namespace Parsadox.Parser.Versions;

/// <summary>
/// Container for the game version extracted from a save game,
/// parsed as a semantic version.
/// </summary>
public interface IGameVersion
{
    string Text { get; }

    int Major { get; }

    int Minor { get; }

    int Patch { get; }

    int PatchMinor { get; }

    bool IsUnknown { get; }

    /// <summary>
    /// Always returns true when either this or other's <see cref="IsUnknown"/> is true.
    /// </summary>
    bool IsLessThan(IGameVersion other);
}
