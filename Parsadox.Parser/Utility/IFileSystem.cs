namespace Parsadox.Parser.Utility;

/// <summary>
/// Encapsulate all file system i/o for unit testability.
/// </summary>
internal interface IFileSystem
{
    bool FileExists(string path);

    bool DirectoryExists(string path);

    IEnumerable<string> EnumerateFiles(string path, string searchPattern);

    string ReadAllText(string path, Encoding encoding);

    void Delete(string path);

    void Copy(string fromPath, string toPath, bool overwrite);

    string CreateTemporaryFile();

    string GetAbsolutePath(string path);

    long GetFileSize(string path);

    Stream OpenRead(string path);

    Stream OpenCreate(string path);

    string? GetEnvironmentVariable(string name);
}
