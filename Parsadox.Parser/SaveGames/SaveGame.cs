namespace Parsadox.Parser.SaveGames;

internal class SaveGame : ISaveGame
{
    public Game Game { get; init; }

    public ISaveGameHeader Header { get; init; } = new SaveGameHeader();

    public INode Root { get; set; } = NodeFactory.CreateAnonymousArray(new());

    public long WriteFile(string outputPath, WriteParameters? parameters = null) =>
        WriteFileAsync(outputPath, parameters, CancellationToken.None).Result;

    public async Task<long> WriteFileAsync(string outputPath, WriteParameters? parameters, CancellationToken cancellationToken) =>
        await new SaveGameWriter(this, parameters ?? new()).WriteFileAsync(outputPath, cancellationToken);

    public long WriteStream(Stream outputStream, WriteParameters? parameters = null) =>
        WriteStreamAsync(outputStream, parameters, CancellationToken.None).Result;

    public async Task<long> WriteStreamAsync(Stream outputStream, WriteParameters? parameters, CancellationToken cancellationToken) =>
        await new SaveGameWriter(this, parameters ?? new()).WriteStreamAsync(outputStream, cancellationToken);

    public ISaveGame DisableIronman()
    {
        Game.GetHandler(Header.Version).DisableIronman(this);
        return this;
    }

    public override string ToString()
    {
        StringBuilder builder = new($"{Game} save game loaded as {Header.Format}");
        if (!string.IsNullOrWhiteSpace(Header.FileName))
            builder.Append(" from ").Append(Header.FileName);
        return builder.ToString();
    }
}
