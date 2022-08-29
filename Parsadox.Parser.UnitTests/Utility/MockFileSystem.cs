using Parsadox.Parser.Utility;
using System.Text;

namespace Parsadox.Parser.UnitTests.Utility;

internal class MockFileSystem : IFileSystem
{
    internal MockFileSystem()
    {
        FileSystem.Instance = this;
    }

    internal List<string> FileExistsCalls = new();
    internal bool FileExistsReturns { get; set; }
    public bool FileExists(string path)
    {
        FileExistsCalls.Add(path);
        return FileExistsReturns;
    }

    internal List<string> DirectoryExistsCalls = new();
    internal bool DirectoryExistsReturns { get; set; }
    public bool DirectoryExists(string path)
    {
        DirectoryExistsCalls.Add(path);
        return DirectoryExistsReturns;
    }

    internal List<(string path, string searchPattern)> EnumerateFilesCalls = new();
    internal IEnumerable<string> EnumerateFilesReturns { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        EnumerateFilesCalls.Add((path, searchPattern));
        return EnumerateFilesReturns;
    }

    internal List<(string path, Encoding encoding)> ReadAllTextCalls = new();
    internal string ReadAllTextReturns { get; set; } = string.Empty;
    public string ReadAllText(string path, Encoding encoding)
    {
        ReadAllTextCalls.Add((path, encoding));
        return ReadAllTextReturns;
    }

    internal List<string> DeleteCalls = new();
    public void Delete(string path)
    {
        DeleteCalls.Add(path);
    }

    internal List<(string fromPath, string toPath, bool overwrite)> CopyCalls = new();
    public void Copy(string fromPath, string toPath, bool overwrite)
    {
        CopyCalls.Add((fromPath, toPath, overwrite));
    }

    internal long CreateTemporaryFileCalls;
    internal string CreateTemporaryFileReturns { get; set; } = string.Empty;
    public string CreateTemporaryFile()
    {
        CreateTemporaryFileCalls++;
        return CreateTemporaryFileReturns;
    }

    internal List<string> GetAbsolutePathCalls = new();
    internal string GetAbsolutePathReturns { get; set; } = string.Empty;
    public string GetAbsolutePath(string path)
    {
        GetAbsolutePathCalls.Add(path);
        return GetAbsolutePathReturns;
    }

    internal List<string> GetFileSizeCalls = new();
    internal long GetFileSizeReturns { get; set; }
    public long GetFileSize(string path)
    {
        GetFileSizeCalls.Add(path);
        return GetFileSizeReturns;
    }

    internal List<string> OpenReadCalls = new();
    internal Stream? OpenReadReturns { get; set; }
    public Stream OpenRead(string path)
    {
        OpenReadCalls.Add(path);
        return OpenReadReturns ?? new MemoryStream();
    }

    internal List<string> OpenCreateCalls = new();
    internal Stream? OpenCreateReturns { get; set; }
    public Stream OpenCreate(string path)
    {
        OpenCreateCalls.Add(path);
        return OpenCreateReturns ?? new MemoryStream();
    }

    internal List<string> GetEnvironmentVariableCalls = new();
    internal string? GetEnvironmentVariableReturns { get; set; }
    public string? GetEnvironmentVariable(string name)
    {
        GetEnvironmentVariableCalls.Add(name);
        return GetEnvironmentVariableReturns;
    }
}
