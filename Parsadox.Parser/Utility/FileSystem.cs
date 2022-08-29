namespace Parsadox.Parser.Utility;

/// <summary>
/// Encapsulate all file system i/o for unit testability.
/// </summary>
internal class FileSystem : IFileSystem
{
    internal static IFileSystem Instance { get; set; } = new FileSystem();

    public bool FileExists(string path) => File.Exists(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) =>
        new DirectoryInfo(path).EnumerateFiles(searchPattern).Select(x => x.FullName);

    public string ReadAllText(string path, Encoding encoding) => File.ReadAllText(path, encoding);

    public void Delete(string path) => File.Delete(path);

    public void Copy(string fromPath, string toPath, bool overwrite) => File.Copy(fromPath, toPath, overwrite);

    public string CreateTemporaryFile() => Path.GetTempFileName();

    public string GetAbsolutePath(string path) => new FileInfo(path).FullName;

    public long GetFileSize(string path) => new FileInfo(path).Length;

    public Stream OpenRead(string path) => new FileInfo(path).OpenRead();

    public Stream OpenCreate(string path) => File.Create(path);

    public string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    internal static void AssertFileExists(string path, string messagePrefix = "File")
    {
        if (Instance.FileExists(path))
            return;
        if (Instance.DirectoryExists(path))
            throw new ArgumentException($"{messagePrefix} is actually a directory: {path}");
        throw new ArgumentException($"{messagePrefix} does not exist: {path}");
    }
}
