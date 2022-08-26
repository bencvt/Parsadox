namespace Parsadox.Parser.Parameters;

/// <summary>
/// Parameters used when writing save games.
/// </summary>
public class WriteParameters : MainParameters
{
    /// <summary>
    /// Whether to compress the save game.
    /// </summary>
    public SaveGameFormat SaveGameFormat { get; set; } = DefaultSaveGameFormat;
    public const SaveGameFormat DefaultSaveGameFormat = SaveGameFormat.CompressedAuto;

    /// <summary>
    /// Whether to minimize the size of the save game or keep it human-readable.
    /// </summary>
    public NodeOutputFormat NodeOutputFormat { get; set; } = DefaultNodeOutputFormat;
    public const NodeOutputFormat DefaultNodeOutputFormat = NodeOutputFormat.Full;
}
