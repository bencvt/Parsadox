namespace Parsadox.Parser.Utility;

/// <summary>
/// Encapsulate all file system i/o for unit testability.
/// </summary>
internal interface IFileSystem
{
    bool FileExists(string path) => File.Exists(path);

    bool DirectoryExists(string path) => Directory.Exists(path);

    string ReadAllText(string path) => File.ReadAllText(path);

    void Delete(string path) => File.Delete(path);

    void Copy(string fromPath, string toPath, bool overwrite) => File.Copy(fromPath, toPath, overwrite);

    string CreateTemporaryFile() => Path.GetTempFileName();

    string GetAbsolutePath(string path) => new FileInfo(path).FullName;

    long GetFileSize(string path) => new FileInfo(path).Length;

    Stream OpenRead(string path) => new FileInfo(path).OpenRead();

    Stream OpenCreate(string path) => File.Create(path);

    string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);
}
