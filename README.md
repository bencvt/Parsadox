[![NuGet](https://img.shields.io/nuget/v/Parsadox.svg?logo=nuget)](https://www.nuget.org/packages/Parsadox) ![ci](https://img.shields.io/github/workflow/status/bencvt/Parsadox/ci?logo=github)

**Parsadox** is an open-source .NET library to work with files from Paradox Interactive grand strategy games.

Supported games:
 * Crusader Kings II
 * Crusader Kings III
 * Europa Universalis IV
 * Imperator: Rome
 * Hearts of Iron IV
 * Stellaris
 * Victoria II

# Example

After installing the [NuGet package](https://www.nuget.org/packages/Parsadox):

```cs
ISaveGame saveGame = SaveGameFactory.LoadFile("my_game.ck3");

// Inspect data
string id = saveGame.State["currently_played_characters"].First().Content.Text;
INode player = saveGame.State["living"][id];
string name = player["first_name"].Value.Text;
decimal? stress = player.GetDescendantOrNull("alive_data", "stress")?.Value.AsDecimal;
Console.WriteLine($"{name} has {stress ?? 0} stress");

// Modify data
if (stress.HasValue)
    player["alive_data"]["stress"].Value.AsDecimal -= 25;
saveGame.DisableIronman().WriteFile("my_game_modified.ck3");
```

# Legal notice

Parsadox is a community project unaffiliated with Paradox Interactive.
