namespace Parsadox.Parser.Tokens;

/// <summary>
/// Container for tokens loaded using <see cref="SaveGameTokenFactory"/>.
/// </summary>
public record TokenContainer(Dictionary<ISaveGameHeader, List<IToken>> Items)
{
    public int Count => Items.Sum(x => x.Value.Count);

    /// <summary>
    /// Output all tokens, along with its binary code (if available).
    /// <para/>
    /// Indentation is added for readability.
    /// </summary>
    public void Dump(TextWriter output)
    {
        Dictionary<ushort, int> unmappedCounts = new();

        foreach (var (header, tokens) in Items.OrderBy(kvp => kvp.Key.FileName))
        {
            output.WriteLine($"# {header.FileName}");

            long count = 0;
            int depth = 0;
            foreach (var token in tokens)
            {
                count++;

                if (token.IsClose)
                    depth--;

                output.Write($"0x{token.Code:x4}\t");
                for (int i = 0; i < depth; i++)
                    output.Write('\t');
                output.WriteLine(token);

                if (token.IsOpen)
                    depth++;
                else if (token is U16Token code)
                    unmappedCounts[code.Code] = unmappedCounts.GetValueOrDefault(code.Code) + 1;
            }
            output.WriteLine();
        }

        output.WriteLineCount("# Found", unmappedCounts.Count, "unmapped code");
        foreach (ushort code in unmappedCounts.Keys.OrderBy(x => x))
            output.WriteLineCount($"0x{code:x4}:", unmappedCounts[code], "time");
    }
}
