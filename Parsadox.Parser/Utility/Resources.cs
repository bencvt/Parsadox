namespace Parsadox.Parser.Utility;

internal static class Resources
{
    internal static string GetStringOrThrow(string name)
    {
        using var stream = typeof(Resources).Assembly.GetManifestResourceStream(name);
        if (stream is null)
            throw new InvalidOperationException($"Assembly missing resource {name}");
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
