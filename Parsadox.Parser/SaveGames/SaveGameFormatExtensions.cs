namespace Parsadox.Parser.SaveGames;

public static class SaveGameFormatExtensions
{
    public static bool IsBinary(this SaveGameFormat format) =>
        format == SaveGameFormat.UncompressedBinary
        || format == SaveGameFormat.CompressedBinary;

    public static bool IsCompressed(this SaveGameFormat format) =>
        format == SaveGameFormat.CompressedAuto
        || format == SaveGameFormat.CompressedText
        || format == SaveGameFormat.CompressedBinary;

    public static void ValidateCanWrite(this SaveGameFormat format, ISaveGame saveGame, Func<SaveGameFormat, bool> formatIsValidFunc)
    {
        if (!formatIsValidFunc(format))
            Fail("Invalid format");

        if (!saveGame.Root.HasChildrenStorage || !saveGame.Root.Children.Any())
            Fail("No data in root");

        if (saveGame.Root.Children.Select(x => x.Content.Text.ToLowerInvariant().Trim()).Distinct().Count() < saveGame.Root.Children.Count)
            Fail($"Duplicate entries in root: {Strings.EscapeAndQuote(saveGame.Root.Select(x => x.Content.Text))}");

        var gameCompression = saveGame.Game.GetHandler(saveGame.Header.Version).Compression;

        if (format.IsCompressed() && gameCompression == SaveGameCompression.Never)
            Fail("This game does not compress saves");

        if (!format.IsCompressed() && gameCompression == SaveGameCompression.Mandatory)
            Fail("This game always compresses saves");

        void Fail(string message) => throw new ArgumentException(
            $"Cannot write {saveGame.Game} save game as {format}: {message}");
    }
}
