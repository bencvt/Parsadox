namespace Parsadox.Parser.Versions;

/// <summary>
/// Prior to loading save game data, sniff out the version with a minimal
/// amount of reading.
/// <para/>
/// Useful when there is version-specific parsing logic.
/// </summary>
internal static class Ck3AndImperatorBinaryVersionReader
{
    /// <summary>
    /// Assumption: input stream is positioned immediately after the header.
    /// </summary>
    internal static IGameVersion Read(Stream input)
    {
        long originalPosition = input.Position;

        using BinaryTokenReader reader = new(input, Game.Ck3.GetDefaultVersionHandler(), TokenMapFactory.Create(),
            abortIfUnmapped: false, shouldExtractCodes: false, progress: null, CancellationToken.None);

        // Ck3 token 9: metadata={ save_game_version=123 version="4.5.6"
        // Imperator token 6: save_game_version=123 version="4.5.6"
        int expectVersionValueAt = -1;
        int count = 0;
        foreach (var token in reader.ReadAll(shouldIncludeComments: false))
        {
            count++;
            if (expectVersionValueAt == count && token is QuotedStringToken q)
            {
                input.Position = originalPosition;
                return new GameVersion(q.Text);
            }
            if (count > 8)
                break;
            if (token is U16Token u16 && u16.Code == BinaryTokenCodes.VERSION)
                expectVersionValueAt = count + 2;
        }

        input.Position = originalPosition;
        return GameVersion.UNKNOWN;
    }
}
