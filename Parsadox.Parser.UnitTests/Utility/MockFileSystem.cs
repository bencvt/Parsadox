using Parsadox.Parser.Utility;

namespace Parsadox.Parser.UnitTests.Utility;

internal class MockFileSystem : IFileSystem
{
    static MockFileSystem() => _ = new MockFileSystem();

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

    internal List<string> ReadAllTextCalls = new();
    internal string ReadAllTextReturns { get; set; } = string.Empty;
    public string ReadAllText(string path)
    {
        ReadAllTextCalls.Add(path);
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
