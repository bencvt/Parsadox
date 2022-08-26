namespace Parsadox.Parser.Maps;

/// <summary>
/// When reading binary save games, this map is used to deobfuscate binary
/// codes to their text format equivalents.
/// </summary>
public interface ITokenMap
{
    Dictionary<ushort, string> CodeMap { get; }

    void Clear();

    ITokenMap DeepCopy();

    /// <summary>
    /// Populate the maps from a file specified in an environment variable.
    /// <para/>
    /// Throws if the environment variable doesn't exist, if the file doesn't exist,
    /// or if the file contents are invalid.
    /// </summary>
    /// <returns>The path of the file that was succesfully loaded.</returns>
    string LoadEnvironment(Game game);

    /// <summary>
    /// Populate the maps from a file specified in an environment variable.
    /// <para/>
    /// Throws if the environment variable doesn't exist, if the file doesn't exist,
    /// or if the file contents are invalid.
    /// </summary>
    /// <returns>The path of the file that was succesfully loaded.</returns>
    string LoadEnvironment(string customVariableName);

    /// <summary>
    /// Populate the maps from a file.
    /// </summary>
    ITokenMap LoadFile(string path);

    /// <summary>
    /// Populate the maps from a string.
    /// </summary>
    ITokenMap LoadString(string text);
}
