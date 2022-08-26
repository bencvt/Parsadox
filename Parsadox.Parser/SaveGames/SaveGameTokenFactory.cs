namespace Parsadox.Parser.SaveGames;

/// <summary>
/// Load a save game but do not fully parse it.
/// <para/>
/// Return its unstructured tokens instead.
/// </summary>
public static class SaveGameTokenFactory
{
    public static TokenContainer LoadFile(this Game game, string path, ReadParameters? parameters = null) =>
        LoadFileAsync(game, path, parameters, progress: null, CancellationToken.None).Result;

    public static async Task<TokenContainer> LoadFileAsync(this Game game, string path, ReadParameters? parameters, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        SaveGameTokenReader reader = new(ReadParameters.Get(parameters, game));
        return await reader.LoadFileAsync(path, progress, cancellationToken);
    }

    public static TokenContainer LoadStream(this Game game, Stream input, ReadParameters? parameters = null) =>
        LoadStreamAsync(game, input, parameters, progress: null, CancellationToken.None).Result;

    public static async Task<TokenContainer> LoadStreamAsync(this Game game, Stream input, ReadParameters? parameters, IProgress<double>? progress, CancellationToken cancellationToken)
    {
        SaveGameTokenReader reader = new(ReadParameters.Get(parameters, game));
        return await reader.LoadStreamAsync(input, progress, cancellationToken);
    }
}
