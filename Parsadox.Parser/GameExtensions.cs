namespace Parsadox.Parser;

/// <summary>
/// Miscellaneous game-specific methods. 
/// </summary>
public static class GameExtensions
{
    private static readonly IReadOnlyDictionary<Game, Func<IGameVersion, IGameHandler>> GameHandlerFactories = GameHandlerAttribute.BuildMap();

    internal static IGameHandler GetHandler(this Game game, IGameVersion version) => GameHandlerFactories[game](version);

    internal static IGameHandler GetDefaultVersionHandler(this Game game) => GameHandlerFactories[game](GameVersion.UNKNOWN);

    /// <summary>
    /// Get the name of the game as it's used in directories.
    /// </summary>
    public static string GetFullName(this Game game) => game.GetDefaultVersionHandler().Name;

    /// <summary>
    /// Extension including the dot. Example: ".ck3"
    /// </summary>
    public static string GetSaveGameExtension(this Game game) => game.GetDefaultVersionHandler().SaveGameExtension;

    /// <summary>
    /// Either UTF8 or Windows-1252.
    /// </summary>
    public static Encoding GetEncoding(this Game game) => game.GetDefaultVersionHandler().TextEncoding;

    /// <summary>
    /// Attempt to find the save game directory that stores local saves.
    /// <para/>
    /// Cloud saves are also saved locally, but in a different platform-specific directory.
    /// </summary>
    public static string? GetLocalSaveGameDirectory(this Game game)
    {
        string gameDir = game.GetFullName();

        // Linux may use ~/.local/share, which is LocalApplicationData.
        return new[] { Environment.SpecialFolder.Personal, Environment.SpecialFolder.LocalApplicationData }
            .SelectMany(folder => new[] { gameDir, $"{gameDir} GamePass" }
                .Select(x => Path.Join(Environment.GetFolderPath(folder), "Paradox Interactive", x, "save games")))
            .Where(FileSystem.Instance.DirectoryExists)
            .FirstOrDefault();
    }

    /// <summary>
    /// Attempt to find the directory where the game is installed.
    /// <para/>
    /// Returns null if the game is installed in a non-standard location.
    /// <para/>
    /// Also returns null if the game is installed via the Windows Store
    /// platform (includes PC Game Pass).
    /// Accessing game files in this case requires a bit more work:
    /// <see href="https://github.com/Wunkolo/UWPDumper"/>.
    /// </summary>
    public static string? GetInstallDirectory(this Game game)
    {
        string gameDir = game.GetFullName();

        List<string> paths = new()
        {
            // Windows
            Path.Join(Path.GetPathRoot(Environment.SystemDirectory),
                "Program Files (x86)", "Steam", "steamapps", "common", gameDir),
            Path.Join("C:", "SteamLibrary", "steamapps", "common", gameDir),
            Path.Join("D:", "SteamLibrary", "steamapps", "common", gameDir),
        };
        AddMac("SteamApps");
        AddMac("steamapps");
        AddLinux("SteamApps");
        AddLinux("steamapps");

        return paths.Where(FileSystem.Instance.DirectoryExists).FirstOrDefault();

        void AddMac(string steamApps)
        {
            // Personal => ~
            paths.Add(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Library", "Application Support", "Steam", steamApps, "common", gameDir));
        }
        void AddLinux(string steamApps)
        {
            // LocalApplicationData => ~/.local/share
            paths.Add(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Steam", steamApps, "common", gameDir));
            // May be a symlink to the above, but it won't hurt to check.
            paths.Add(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                ".steam", "steam", steamApps, "common", gameDir));
        }
    }
}
