namespace Parsadox.Parser.SaveGames;

public enum SaveGameFormat
{
    Unknown = 0,

    UncompressedText,

    UncompressedBinary,

    /// <summary>
    /// Write as <see cref="CompressedText"/> if the game supports it,
    /// otherwise use <see cref="UncompressedText"/>.
    /// <para/>
    /// This is the default value for <see cref="WriteParameters.SaveGameFormat"/>.
    /// </summary>
    CompressedAuto,

    CompressedText,

    CompressedBinary,
}
