namespace Parsadox.Parser.GameHandlers;

internal class Ck3Pre1_5Handler : Ck3Handler
{
    public override IFloatConverter FloatConverter => new Ck3Pre1_5FloatConverter();
}
