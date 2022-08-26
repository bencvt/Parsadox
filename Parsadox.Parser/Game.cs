namespace Parsadox.Parser;

/// <summary>
/// Paradox grand strategy games supported by this library.
/// </summary>
public enum Game
{
    [GameHandler(typeof(UnknownHandler))]
    Unknown = 0,

    [GameHandler(typeof(Ck2Handler))]
    Ck2,

    [GameHandler(typeof(Ck3Pre1_5Handler), "1.5")]
    [GameHandler(typeof(Ck3Handler))]
    Ck3,

    [GameHandler(typeof(Eu4Handler))]
    Eu4,

    [GameHandler(typeof(Hoi4Handler))]
    Hoi4,

    [GameHandler(typeof(ImperatorHandler))]
    Imperator,

    [GameHandler(typeof(StellarisHandler))]
    Stellaris,

    [GameHandler(typeof(Vic2Handler))]
    Vic2,
}
