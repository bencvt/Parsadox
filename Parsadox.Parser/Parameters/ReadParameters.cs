namespace Parsadox.Parser.Parameters;

/// <summary>
/// Parameters used when reading save games.
/// </summary>
public class ReadParameters : MainParameters
{
    public Game Game { get; set; }

    /// <summary>
    /// Token code mapping, required to load Ironman save games correctly.
    /// <para/>
    /// Not required for non-Ironman save games.
    /// </summary>
    public ITokenMap? TokenMap { get; set; }

    /// <summary>
    /// If true and <see cref="TokenMap"/> is null, attempt to load it
    /// using the default environment variable.
    /// </summary>
    public bool AutoLoadTokenMap { get; set; } = DefaultAutoLoadTokenMap;
    public const bool DefaultAutoLoadTokenMap = true;

    /// <summary>
    /// Whether to abort if an unmapped code is encountered when reading
    /// binary (Ironman) save games.
    /// </summary>
    public bool AbortIfUnmapped { get; set; } = DefaultAbortIfUnmapped;
    public const bool DefaultAbortIfUnmapped = true;

    /// <summary>
    /// Whether to avoid parsing when possible.
    /// <para/>
    /// For use cases that don't involve accessing every section of the
    /// save game, enabling this setting is a significant performance boost.
    /// <para/>
    /// Note that <see cref="ISaveGame.WriteFile"/>, etc. do access every section.
    /// <para/>
    /// Also note that if the save game is malformed, no <see cref="ParseException"/>
    /// will be thrown unless a malformed section is accessed.
    /// </summary>
    public bool UseLazyParsing { get; set; } = DefaultUseLazyParsing;
    public const bool DefaultUseLazyParsing = true;

    /// <summary>
    /// When populated, filter out unwanted sections when tokenizing the
    /// the save game file.
    /// <para/>
    /// This is a significant performance boost when there are large
    /// sections of unwanted data.
    /// <para/>
    /// Whether this filter is an allowlist or a blocklist is controlled by
    /// the <see cref="IsSectionFilterBlocklist"/> property.
    /// <para/>
    /// To filter nameless sections, use the empty string.
    /// <para/>
    /// Comments are always removed when using a filter.
    /// </summary>
    public HashSet<string>? SectionFilter { get; set; }

    /// <summary>
    /// Helper set-only property to populate <see cref="SectionFilter"/>.
    /// </summary>
    public string? SectionFilterCommaDelimited
    {
        set
        {
            if (value is null)
                SectionFilter = null;
            else
                SectionFilter = value.Split(',').Select(x => x.Trim()).ToHashSet();
        }
    }

    /// <summary>
    /// Controls how <see cref="SectionFilter"/> works, if set.
    /// <para/>
    /// If false then NO sections are allowed unless they are in the list.
    /// <para/>
    /// If true then ALL sections are allowed unless they are in the list.
    /// </summary>
    public bool IsSectionFilterBlocklist { get; set; }
    public const bool DefaultIsSectionFilterBlocklist = false;

    /// <summary>
    /// If there are concerns that the input file may be locked or
    /// overwritten while being read, set this to true.
    /// <para/>
    /// Ignored if reading directly from stream.
    /// </summary>
    public bool MakeTemporaryCopyOfInputFile { get; set; } = DefaultMakeTemporaryCopyOfInputFile;
    public const bool DefaultMakeTemporaryCopyOfInputFile = false;

    public override MainParameters DeepCopyAndInitialize()
    {
        var copy = (ReadParameters)base.DeepCopyAndInitialize();
        copy.SectionFilter = SectionFilter?.ToHashSet();
        copy.TokenMap = TokenMap?.DeepCopy();

        if (copy.TokenMap is null && AutoLoadTokenMap)
            copy.TokenMap = TokenMapFactory.AutoLoad(Game, Log);

        return copy;
    }

    internal static ReadParameters Get(ReadParameters? parameters, Game game)
    {
        if (parameters is null)
            return new() { Game = game };
        if (parameters.Game == Game.Unknown)
        {
            parameters = (ReadParameters)parameters.MemberwiseClone();
            parameters.Game = game;
        }
        return parameters;
    }
}
