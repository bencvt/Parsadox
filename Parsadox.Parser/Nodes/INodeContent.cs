namespace Parsadox.Parser.Nodes;

/// <summary>
/// String wrapper to provide formatting and conversion.
/// </summary>
public interface INodeContent
{
    string Text { get; set; }

    bool AsBool
    {
        get => Strings.StringToBool(Text);
        set => Text = Strings.BoolToString(value);
    }
    decimal AsDecimal
    {
        get => decimal.Parse(Text);
        set => Text = value.ToString();
    }
    float AsF32
    {
        get => float.Parse(Text);
        set => Text = Strings.F32ToString(value);
    }
    double AsF64
    {
        get => double.Parse(Text);
        set => Text = Strings.F64ToString(value);
    }
    GameDate AsGameDate
    {
        get => GameDate.Parse(Text);
        set => Text = value.ToString();
    }
    int AsI32
    {
        get => int.Parse(Text);
        set => Text = value.ToString();
    }
    uint AsU32
    {
        get => uint.Parse(Text);
        set => Text = value.ToString();
    }
    ulong AsU64
    {
        get => ulong.Parse(Text);
        set => Text = value.ToString();
    }

    bool IsComment => false;
    bool IsAnonymousArrayKey => false;
}
