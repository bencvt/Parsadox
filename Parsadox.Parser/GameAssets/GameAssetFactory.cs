namespace Parsadox.Parser.GameAssets;

/// <summary>
/// Retrieve and parse files that are part of the game itself
/// (i.e., not player save games).
/// </summary>
public class GameAssetFactory
{
    public string InstallDirectory { get; }

    private readonly Encoding _gameEncoding;

    /// <summary>
    /// If the installDirectory parameter is unspecified, the default game
    /// install directory will be used, if it exists.
    /// See <see cref="GameExtensions.GetInstallDirectory"/>.
    /// <para/>
    /// If that fallback fails, an exception will be thrown.
    /// </summary>
    public GameAssetFactory(Game game, string? installDirectory = null)
    {
        InstallDirectory = installDirectory
            ?? game.GetInstallDirectory()
            ?? throw new ArgumentException($"Default game install directory not found.");
        if (!FileSystem.Instance.DirectoryExists(InstallDirectory))
            throw new ArgumentException($"Game install directory not found: {InstallDirectory}");

        _gameEncoding = game.GetDefaultVersionHandler().TextEncoding;
    }

    /// <summary>
    /// Load a text file.
    /// </summary>
    /// <param name="filePath">The file's path relative to <see cref="InstallDirectory"/>.</param>
    /// <param name="encoding">Defaults to the game's encoding.</param>
    public string LoadText(string filePath, Encoding? encoding = null)
    {
        string fullPath = Path.Join(InstallDirectory, filePath);
        return FileSystem.Instance.ReadAllText(fullPath, encoding ?? _gameEncoding);
    }

    /// <summary>
    /// Parse a group of files in a directory.
    /// <para/>
    /// Caveat: Some game files use syntaxes not found in save games,
    /// such as operators other than =. (E.g., != and ==).
    /// These will not parse correctly.
    /// </summary>
    /// <param name="path">The directory containing the files, relative to <see cref="InstallDirectory"/>.</param>
    /// <param name="searchPattern">Which files to parse. For a single file, omit the wildcard.</param>
    /// <param name="encoding">Defaults to the game's encoding.</param>
    /// <returns>an anonymous array <see cref="INode"/> instance</returns>
    public INode LoadNodes(string path, string searchPattern = "*.txt", Encoding? encoding = null)
    {
        var allNodes = NodeFactory.CreateAnonymousArray(new());
        string fullPath = Path.Join(InstallDirectory, path);
        foreach (var filePath in FileSystem.Instance.EnumerateFiles(fullPath, searchPattern).OrderBy(x => x))
        {
            string content = FileSystem.Instance.ReadAllText(filePath, encoding ?? _gameEncoding);
            var fileNodes = NodeFactory.LoadString(content);
            allNodes.Children.AddRange(fileNodes);
        }
        return allNodes;
    }

    /// <summary>
    /// Parse a group of files in a directory and convert the data to a
    /// dictionary structure.
    /// <para/>
    /// Will throw if there are multiple nodes with the same key.
    /// </summary>
    /// <param name="path">The directory containing the files, relative to <see cref="InstallDirectory"/>.</param>
    /// <param name="searchPattern">Which files to parse. For a single file, omit the wildcard.</param>
    /// <param name="encoding">Defaults to the game's encoding.</param>
    /// <param name="includeNodesWithoutChildren">
    ///     If false (default), exclude nodes that don't have children storage.
    ///     I.e., only key={...}, not key=value.
    /// </param>
    public Dictionary<string, INode> LoadNodeMap(string path, string searchPattern = "*.txt", Encoding? encoding = null, bool includeNodesWithoutChildren = false)
    {
        return LoadNodes(path, searchPattern, encoding)
            .Where(x => includeNodesWithoutChildren || x.HasChildrenStorage)
            .ToDictionary(x => x.Content.Text, x => x);
    }

    /// <summary>
    /// Load a YAML localization file to a dictionary.
    /// </summary>
    /// <remarks>
    /// Uses a quick-and-dirty parser that assumes:
    /// <list type="bullet">
    /// <item>a single root key, followed by any number of key/value pairs on individual lines</item>
    /// <item>each key can have a ":0", ":1", etc. suffix that is removed</item>
    /// <item>each value is enclosed in quotes</item>
    /// <item>no escape sequences are needed for values</item>
    /// <item>values can have embedded quotes</item>
    /// <item>a line has either a key/value pair or a comment - never both</item>
    /// </list>
    /// </remarks>
    /// <param name="filePath">The file's path relative to <see cref="InstallDirectory"/>.</param>
    /// <param name="encoding">Defaults to UTF8.</param>
    public Dictionary<string, string> LoadYamlLocalizationMap(string filePath, Encoding? encoding = null)
    {
        var map = new Dictionary<string, string>();
        foreach (string line in LoadText(filePath, encoding ?? Encoding.UTF8).Split('\n'))
        {
            var match = RE_YAML_LINE.Match(line);
            if (match.Success)
                map[match.Groups[1].Value] = match.Groups[3].Value;
        }
        return map;
    }
    private static readonly Regex RE_YAML_LINE = new(@"^\s+(\w+)(:\d+)?\s+""(.*)""");
}
