![icon](images/parsadox-128.png) ![ci](https://github.com/bencvt/Parsadox/workflows/ci/badge.svg)

Parsadox is an open-source .NET library to parse game files from Paradox Interactive grand strategy games.

Supported games:
 * Crusader Kings II
 * Crusader Kings III
 * Europa Universalis IV
 * Imperator: Rome
 * Hearts of Iron IV
 * Stellaris
 * Victoria II

# Basic usage
1. Install the NuGet package.
2. Use `SaveGameFactory` to load a save game. The data is parsed into a tree structure.
3. View or modify the data using the `ISaveGame` instance.
4. Optionally, write a copy of the (potentially modified) save game.
