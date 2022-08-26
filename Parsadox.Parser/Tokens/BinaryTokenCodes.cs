namespace Parsadox.Parser.Tokens;

/// <summary>
/// Essential binary codes.
/// <para/>
/// Thousands of other codes exist, many game-specific.
/// <see cref="ITokenMap"/> handles them.
/// </summary>
public static class BinaryTokenCodes
{
    /// <summary> = </summary>
    public const ushort EQUALS = 0x0001;

    /// <summary> { </summary>
    public const ushort OPEN = 0x0003;

    /// <summary> } </summary>
    public const ushort CLOSE = 0x0004;

    public const ushort I32 = 0x000c;
    public const ushort F32 = 0x000d;
    public const ushort BOOL = 0x000e;
    public const ushort STRING = 0x000f;
    public const ushort U32 = 0x0014;
    public const ushort STRING_ALT = 0x0017;
    public const ushort VERSION = 0x00ee;
    public const ushort F64 = 0x0167;
    public const ushort HSV = 0x0201;
    public const ushort RGB = 0x0243;
    public const ushort HEX = 0x0244;
    public const ushort U64 = 0x029c;
    public const ushort HSV360 = 0x05ff;
}
