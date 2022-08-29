using System.Text;

namespace Parsadox.Parser.UnitTests.Utility;

public abstract class TestsBase
{
    static TestsBase()
    {
        // Ensure that no actual file i/o happens.
        ResetMockFileSystem();
        TokenMapFactory.DisableAutoLoad = true;
    }

    [TearDown]
    public static void ResetMockFileSystem() => _ = new MockFileSystem();

    protected static MemoryStream CreateStream(string content, Encoding? encoding = null) =>
        new((encoding ?? Encoding.UTF8).GetBytes(content));

    internal static IGameHandler Handler => Game.Unknown.GetDefaultVersionHandler();
    internal static IGameHandler Ck3Handler => Game.Ck3.GetDefaultVersionHandler();
}
