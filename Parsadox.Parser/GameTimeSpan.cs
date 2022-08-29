namespace Parsadox.Parser;

/// <summary>
/// GameTimeSpan : GameDate :: System.TimeSpan : System.DateTime
/// </summary>
public struct GameTimeSpan : IComparable<GameTimeSpan>, IEquatable<GameTimeSpan>
{
    public int TotalHours { get; set; }

    public GameTimeSpan(int years, int days, int hours = 0) : this((years * 365L + days) * 24L + hours) { }

    internal GameTimeSpan(long totalHours)
    {
        if (totalHours < int.MinValue || totalHours > int.MaxValue)
            throw new OverflowException($"{totalHours:n0}");
        TotalHours = (int)totalHours;
    }

    public int Years => TotalHours / 24 / 365;
    public int Days => TotalHours / 24 % 365;
    public int TotalDays => TotalHours / 24;
    public int Hours => TotalHours % 24;
    private long TotalHoursLong => TotalHours;

    public int CompareTo(GameTimeSpan other) => TotalHours.CompareTo(other.TotalHours);

    public override int GetHashCode() => TotalHours;

    public bool Equals(GameTimeSpan other) => CompareTo(other) == 0;

    public override bool Equals(object? obj) => obj switch
    {
        GameTimeSpan other => Equals(other),
        _ => base.Equals(obj)
    };

    public override string ToString()
    {
        if (Years == 0)
        {
            if (Hours == 0)
                return Strings.CountItem(Days, "day");
            if (Days == 0)
                return Strings.CountItem(Hours, "hour");
            return Strings.CountItem(Days, "day") + " and " + Strings.CountItem(Hours, "hour");
        }
        if (Hours == 0)
        {
            if (Days == 0)
                return Strings.CountItem(Years, "year");
            return Strings.CountItem(Years, "year") + " and " + Strings.CountItem(Days, "day");
        }
        return Strings.CountItem(Years, "year") + ", " + Strings.CountItem(Days, "day") + ", and " + Strings.CountItem(Hours, "hour");
    }

    public static bool operator ==(GameTimeSpan a, GameTimeSpan b) => a.Equals(b);
    public static bool operator !=(GameTimeSpan a, GameTimeSpan b) => !a.Equals(b);
    public static bool operator <(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) < 0;
    public static bool operator >(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) > 0;
    public static bool operator <=(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) <= 0;
    public static bool operator >=(GameTimeSpan a, GameTimeSpan b) => a.CompareTo(b) >= 0;
    public static GameTimeSpan operator +(GameTimeSpan a) => a;
    public static GameTimeSpan operator -(GameTimeSpan a) => new(-a.TotalHoursLong);
    public static GameTimeSpan operator +(GameTimeSpan a, GameTimeSpan b) => new(a.TotalHoursLong + b.TotalHoursLong);
    public static GameTimeSpan operator -(GameTimeSpan a, GameTimeSpan b) => new(a.TotalHoursLong - b.TotalHoursLong);
}
