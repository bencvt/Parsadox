using System.Runtime.CompilerServices;
using System.Text;

namespace Parsadox.Parser.UnitTests.Utility;

public abstract class TestsBase
{
    static TestsBase()
    {
        // Ensure that no actual file i/o happens.
        RuntimeHelpers.RunClassConstructor(typeof(MockFileSystem).TypeHandle);
        TokenMapFactory.DisableAutoLoad = true;
    }

    protected static MemoryStream CreateStream(string content, Encoding? encoding = null) =>
        new((encoding ?? Encoding.UTF8).GetBytes(content));

    internal static IGameHandler Handler => Game.Unknown.GetDefaultVersionHandler();
    internal static IGameHandler Ck3Handler => Game.Ck3.GetDefaultVersionHandler();
}