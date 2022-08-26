namespace Parsadox.Parser.SaveGames;

/// <summary>
/// Load save games.
/// </summary>
public static class SaveGameFactory
{
    /// <remarks>
    /// Throws if the game cannot be determined from the file extension.
    /// </remarks>
    public static ISaveGame LoadFile(string path, ReadParameters? parameters = null) =>
        LoadFile(GetGameOrThrow(path), path, parameters);

    public static ISaveGame LoadFile(Game game, string path, ReadParameters? parameters = null) =>
        LoadFileAsync(game, path, parameters, progress: null, CancellationToken.None).Result;

    /// <remarks>
    /// Throws if the game cannot be determined from the file extension.
    /// </remarks>
    public static async Task<ISaveGame> LoadFileAsync(string path, ReadParameters? parameters, IProgress<double>? progress, CancellationToken cancellationToken) =>
        await LoadFileAsync(GetGameOrThrow(path), path, parameters, progress, cancellationToken);

    public static async Task<ISaveGame> LoadFileAsync(Game game, string path, ReadParameters? parameters, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        SaveGameReader reader = new(ReadParameters.Get(parameters, game));
        return await reader.LoadFileAsync(path, progress, cancellationToken);
    }

    public static ISaveGame LoadStream(Game game, Stream input, ReadParameters? parameters = null) =>
        LoadStreamAsync(game, input, parameters, progress: null, CancellationToken.None).Result;

    public static async Task<ISaveGame> LoadStreamAsync(this Game game, Stream input, ReadParameters? parameters, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        SaveGameReader reader = new(ReadParameters.Get(parameters, game));
        return await reader.LoadStreamAsync(input, progress, cancellationToken);
    }

    private static Game GetGameOrThrow(string path)
    {
        string ext = Path.GetExtension(path);
        string extLower = ext.ToLowerInvariant().Trim();
        foreach (Game game in Enum.GetValues(typeof(Game)))
        {
            if (game.GetDefaultVersionHandler().SaveGameExtension == extLower)
                return game;
        }
        throw new ArgumentException($"Unknown extension: {ext}");
    }
}
