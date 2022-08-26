namespace Parsadox.Parser.UnitTests.SaveGames;

[TestCovers(typeof(Eu4SaveGameNormalizer))]
public class Eu4SaveGameNormalizerTests : TestsBase
{
    [TestCase("gamestate={player={a} key1={b}}",
        "{gamestate={key1={b}checksum=00000000000000000000000000000000}" +
        "ai={checksum=00000000000000000000000000000000}" +
        "meta={player={a}checksum=00000000000000000000000000000000}}")]
    [TestCase("gamestate={player={a} key1={b} checksum=abc}",
        "{gamestate={key1={b}checksum=abc}" +
        "ai={checksum=abc}" +
        "meta={player={a}checksum=abc}}")]
    [TestCase("ai={} meta={player={a}} gamestate={key1={b}}",
        "{ai={checksum=00000000000000000000000000000000}" +
        "meta={player={a}checksum=00000000000000000000000000000000}" +
        "gamestate={key1={b}checksum=00000000000000000000000000000000}}")]
    public void Normalize_All_IsModified(string nodes, string expected)
    {
        var saveGame = new SaveGame
        {
            Game = Game.Eu4,
            Root = NodeFactory.LoadString(nodes),
        };

        Eu4SaveGameNormalizer.Normalize(saveGame);

        Assert.That(saveGame.Root.Dump(), Is.EqualTo(expected));
    }
}
