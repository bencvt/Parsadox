namespace Parsadox.Parser.Maps;

/// <summary>
/// Create <see cref="ITokenMap"/> instances.
/// </summary>
public static class TokenMapFactory
{
    /// <summary>
    /// Create an empty token map.
    /// <para/>
    /// Call one of the load methods (e.g., <see cref="ITokenMap.LoadFile"/>)
    /// to populate it.
    /// </summary>
    public static ITokenMap Create() => new TokenMap();

    public static string GetDefaultTokenMapEnvironmentVariableName(Game game) =>
        $"{game.ToString().ToUpperInvariant()}_IRONMAN_TOKENS";

    public static void CreateTemplateFile(string outputPath)
    {
        string content = Resources.GetStringOrThrow($"{typeof(TokenMapFactory).Namespace}.token_map_template.txt");
        using var stream = FileSystem.Instance.OpenCreate(outputPath);
        stream.WriteString(content, Encoding.UTF8);
    }

    private static readonly Dictionary<Game, ITokenMap?> _autoLoadedCache = new();
    internal static bool DisableAutoLoad { get; set; } // for unit testing

    /// <summary>
    /// Automatically called when loading a save game with
    /// <see cref="ReadParameters.AutoLoadTokenMap"/> set.
    /// </summary>
    public static ITokenMap? AutoLoad(Game game, TextWriter? log = null)
    {
        lock (_autoLoadedCache)
        {
            if (DisableAutoLoad)
                return null;
            if (!_autoLoadedCache.ContainsKey(game))
            {
                if (!game.GetDefaultVersionHandler().HasBinaryFormat)
                {
                    _autoLoadedCache[game] = null;
                    return null;
                }

                log?.WriteLine(
                    $"Attempting to auto-load {game} token map from file specified by environment variable " +
                    Strings.EscapeAndQuote(GetDefaultTokenMapEnvironmentVariableName(game)));
                try
                {
                    var map = Create();
                    string path = map.LoadEnvironment(game);
                    log?.WriteLine($"Loaded {map} from {path}");
                    _autoLoadedCache[game] = map;
                }
                catch (Exception e)
                {
                    log?.WriteLine($"Error: {e.Message}");
                    _autoLoadedCache[game] = null;
                }
            }
            return _autoLoadedCache[game];
        }
    }
}
