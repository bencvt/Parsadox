namespace Parsadox.Parser.Utility;

/// <summary>
/// Encapsulate all file system i/o for unit testability.
/// </summary>
internal class FileSystem : IFileSystem
{
    internal static IFileSystem Instance { get; set; } = new FileSystem();

    internal static void AssertFileExists(string path, string messagePrefix = "File")
    {
        if (Instance.FileExists(path))
            return;
        if (Instance.DirectoryExists(path))
            throw new ArgumentException($"{messagePrefix} is actually a directory: {path}");
        throw new ArgumentException($"{messagePrefix} does not exist: {path}");
    }
}
