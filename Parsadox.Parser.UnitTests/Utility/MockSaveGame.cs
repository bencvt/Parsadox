namespace Parsadox.Parser.UnitTests.Utility;

internal class MockSaveGame : ISaveGame
{
    public Game Game { get; set; }

    public ISaveGameHeader Header { get; set; } = new SaveGameHeader();

    public INode Root { get; set; }

    public INode State => Root[SaveGame.GAMESTATE];

    public MockSaveGame(string raw)
    {
        Root = NodeFactory.LoadString(raw);
    }

    public long WriteFile(string outputPath, WriteParameters? parameters = null) => 0L;

    public async Task<long> WriteFileAsync(string outputPath, WriteParameters? parameters, CancellationToken cancellationToken) =>
        await Task.FromResult(0L);

    public long WriteStream(Stream outputStream, WriteParameters? parameters = null) => 0L;

    public async Task<long> WriteStreamAsync(Stream outputStream, WriteParameters? parameters, CancellationToken cancellationToken) =>
        await Task.FromResult(0L);

    public ISaveGame DisableIronman() => this;

    public ISaveGame SortNodes() => this;
}
