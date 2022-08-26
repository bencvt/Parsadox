namespace Parsadox.Parser.Maps;

/// <summary>
/// Assume all I32 tokens are ints unless they match one of the conditions,
/// in which case they're <see cref="GameDate"/>s.
/// </summary>
internal class Ck3TokenTypeMap : ITokenTypeMap
{
    // key=1000.1.1
    private static readonly Regex RE_KEYS_WITH_DATE_VALUE = new(@"^\w+_(date|decision|interaction)$");
    private static readonly HashSet<string> KEYS_WITH_DATE_VALUE = new()
    {
        "befriend",
        "birth",
        "blocked",
        "claim_throne",
        "convert_to_witchcraft",
        "cooldown",
        "courting",
        "created",
        "date",
        "date_defeated_last_ai_raider",
        "elope",
        "fabricate_hook",
        "force",
        "hired_until",
        "last_action",
        "last_councillor_change",
        "last_counter_raid",
        "last_raid",
        "learn_language",
        "murder",
        "next",
        "pool_history",
        "raid_0",
        "raid_1",
        "raid_immunity_0",
        "raid_immunity_1",
        "reign_opinion_held_since",
        "sponsored",
        "start_time",
        "steal_back_artifact",
    };

    // key={ 1000.1.1 }
    private static readonly HashSet<string> KEYS_WITH_DATE_CHILDREN = new()
    {
        "changes",
        "dates",
        "history",
    };

    // key={ 1={ 1000.1.1 } }
    private const string KEY_WITH_DATE_GRANDCHILDREN = "acceptance_changes";

    public bool IsKeyGameDate(I32Token key, IToken? parent, IToken? grandparent)
    {
        if (grandparent?.Text == KEY_WITH_DATE_GRANDCHILDREN)
            return true;
        return parent is not null && KEYS_WITH_DATE_CHILDREN.Contains(parent.Text);
    }

    public bool IsValueGameDate(I32Token value, IToken key) =>
        RE_KEYS_WITH_DATE_VALUE.IsMatch(key.Text)
        || KEYS_WITH_DATE_VALUE.Contains(key.Text);
}
