using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parsadox.Parser;

/// <summary>
/// Container for in-game dates.
/// <para/>
/// <list type="bullet">
/// <item>Game dates always have a year, month, and day, and can have an hour as well.</item>
/// <item>There is no such thing as a leap year.</item>
/// <item>The minimum year is -5000 (5000 BC).</item>
/// <item>Year 0 does not exist.</item>
/// <item>For games with binary formats, dates are stored as 32-bit integers.</item>
/// <item>This class implements all relevant comparison operators and methods, plus JSON conversion using ISO 8601.</item>
/// </list>
/// </summary>
[JsonConverter(typeof(JsonConverter))]
public struct GameDate : IComparable<GameDate>, IEquatable<GameDate>
{
    /// <remarks>1-based indexing</remarks>
    public static readonly byte[] DAYS_PER_MONTH = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    public const int MIN_YEAR = -5000;
    public static readonly GameDate MAX = FromI32(int.MaxValue, validate: false);
    public static readonly GameDate YEAR_9999 = new(9999, 1, 1);
    public static readonly GameDate YEAR_1 = new(1, 1, 1);

    public int Year { get; set; }
    public byte Month { get; set; }
    public byte Day { get; set; }
    public byte Hour { get; set; }

    public GameDate(int year, byte month, byte day) : this(year, month, day, 0, true) { }

    public GameDate(int year, byte month, byte day, byte hour) : this(year, month, day, hour, true) { }

    private GameDate(int year, byte month, byte day, byte hour, bool validate)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;

        if (validate)
            Validate();
    }

    public void Validate()
    {
        if (Year == 0 || Year < MIN_YEAR)
            throw new ArgumentOutOfRangeException(nameof(Year), $"Must be non-zero and >= {MIN_YEAR}");
        if (Month < 1 || Month > 12)
            throw new ArgumentOutOfRangeException(nameof(Month), "Must be between 1 and 12");
        if (Day < 1 || Day > DAYS_PER_MONTH[Month])
            throw new ArgumentOutOfRangeException(nameof(Day), $"Must be between 1 and {DAYS_PER_MONTH[Month]} for month {Month}");
        if (Hour < 0 || Hour > 23)
            throw new ArgumentOutOfRangeException(nameof(Hour), "Must be between 0 and 23");
        if (this > MAX)
            throw new ArgumentOutOfRangeException(nameof(Year), $"Must be <= {MAX}");
    }

    public override string ToString() => Hour == 0 ? $"{Year}.{Month}.{Day}" : $"{Year}.{Month}.{Day}.{Hour}";

    public static bool TryParse(string raw, out GameDate date)
    {
        string[] parts = raw.Split('.');
        byte hour = 0;
        if (parts.Length < 3 || parts.Length > 4
            || !int.TryParse(parts[0], out var year)
            || year == 0 || year < MIN_YEAR
            || !byte.TryParse(parts[1], out var month)
            || month < 1 || month > 12
            || !byte.TryParse(parts[2], out var day)
            || day < 1 || day > DAYS_PER_MONTH[month]
            || (parts.Length == 4 && !byte.TryParse(parts[3], out hour))
            || hour < 0 || hour > 23)
        {
            date = default;
            return false;
        }

        date = new(year, month, day, hour, validate: false);
        if (date > MAX)
        {
            date = default;
            return false;
        }
        date.Validate();
        return true;
    }

    public static GameDate Parse(string raw)
    {
        if (TryParse(raw, out GameDate date))
            return date;

        // Re-parse to get a more specific error message.
        string[] parts = raw.Split('.');
        byte hour = 0;
        if (!(parts.Length < 3 || parts.Length > 4
            || !int.TryParse(parts[0], out var year)
            || !byte.TryParse(parts[1], out var month)
            || !byte.TryParse(parts[2], out var day)
            || (parts.Length == 4 && !byte.TryParse(parts[3], out hour))))
        {
            // Validate will fail.
            _ = new GameDate(year, month, day, hour);
        }
        throw new ArgumentException($"Expecting dot-separated year/month/day like \"1234.5.6\", got {Strings.EscapeAndQuote(raw)}");
    }

    /// <summary>
    /// Convert from the binary 32-bit integer format.
    /// </summary>
    public static GameDate FromI32(int raw) => FromI32(raw, true);

    private static GameDate FromI32(int raw, bool validate)
    {
        if (raw < 0)
            throw new ArgumentOutOfRangeException(nameof(raw), "Must be non-negative");

        int year = raw / 24 / 365 + MIN_YEAR;
        int hour = raw % 24;
        // Convert days since January 1st to month and day.
        // No such thing as leap years for this data type.
        int days = raw / 24 % 365;
        (int month, int day) = days switch
        {
            >= 0 and <= 30 => (1, days + 1),
            >= 31 and <= 58 => (2, days - 30),
            >= 59 and <= 89 => (3, days - 58),
            >= 90 and <= 119 => (4, days - 89),
            >= 120 and <= 150 => (5, days - 119),
            >= 151 and <= 180 => (6, days - 150),
            >= 181 and <= 211 => (7, days - 180),
            >= 212 and <= 242 => (8, days - 211),
            >= 243 and <= 272 => (9, days - 242),
            >= 273 and <= 303 => (10, days - 272),
            >= 304 and <= 333 => (11, days - 303),
            >= 334 and <= 364 => (12, days - 333),
            _ => throw new ArgumentOutOfRangeException(nameof(raw), $"{days} not in range")
        };
        return new(year, (byte)month, (byte)day, (byte)hour, validate);
    }

    /// <summary>
    /// Convert to the binary 32-bit integer format.
    /// </summary>
    public int ToI32()
    {
        Validate();

        // Convert month and day to days since January 1st.
        // No such thing as leap years for this data type.
        int days = Month switch
        {
            1 => Day - 1,
            2 => Day + 30,
            3 => Day + 58,
            4 => Day + 89,
            5 => Day + 119,
            6 => Day + 150,
            7 => Day + 180,
            8 => Day + 211,
            9 => Day + 242,
            10 => Day + 272,
            11 => Day + 303,
            12 => Day + 333,
            _ => throw new ArgumentOutOfRangeException(nameof(Month))
        };
        return ((Year - MIN_YEAR) * 365 + days) * 24 + Hour;
    }

    public int CompareTo(GameDate other)
    {
        int result = Year.CompareTo(other.Year);
        if (result == 0)
        {
            result = Month.CompareTo(other.Month);
            if (result == 0)
            {
                result = Day.CompareTo(other.Day);
                if (result == 0)
                    result = Hour.CompareTo(other.Hour);
            }
        }
        return result;
    }

    public bool Equals(GameDate other) => CompareTo(other) == 0;

    public override bool Equals(object? obj) => obj switch
    {
        GameDate other => Equals(other),
        _ => base.Equals(obj)
    };

    public override int GetHashCode() => ToString().GetHashCode();

    public static bool operator ==(GameDate a, GameDate b) => a.Equals(b);
    public static bool operator !=(GameDate a, GameDate b) => !a.Equals(b);
    public static bool operator <(GameDate a, GameDate b) => a.CompareTo(b) < 0;
    public static bool operator >(GameDate a, GameDate b) => a.CompareTo(b) > 0;
    public static bool operator <=(GameDate a, GameDate b) => a.CompareTo(b) <= 0;
    public static bool operator >=(GameDate a, GameDate b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Convert to and from ISO 8601.
    /// <para/>
    /// In ISO 8601, year 0 is 1 BC, -1 is 2 BC, etc.
    /// </summary>
    private class JsonConverter : JsonConverter<GameDate>
    {
        private static readonly Regex RE_DATE_AND_HOUR_PART = new(@"^([+-]?\d+)([-./]\d+[-./]\d+(T\d{1,2})?)");

        public override GameDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var match = RE_DATE_AND_HOUR_PART.Match(reader.GetString() ?? string.Empty);

            int year = int.Parse(match.Groups[1].Value);
            if (year <= 0)
                year--;

            string etc = match.Groups[2].Value.Replace('-', '.').Replace('/', '.').Replace('T', '.');

            return Parse($"{year}{etc}");
        }

        public override void Write(Utf8JsonWriter writer, GameDate value, JsonSerializerOptions options)
        {
            int year = value.Year;
            if (year < 0)
                year++;

            if (value.Hour == 0)
                writer.WriteStringValue($"{year:0000}-{value.Month:00}-{value.Day:00}");
            else
                writer.WriteStringValue($"{year:0000}-{value.Month:00}-{value.Day:00}T{value.Hour:00}:00:00Z");
        }
    }
}
