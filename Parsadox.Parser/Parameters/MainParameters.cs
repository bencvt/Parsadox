namespace Parsadox.Parser.Parameters;

public abstract class MainParameters
{
    /// <summary>
    /// When left unset, do not log.
    /// <para/>
    /// To log to stdout, set to <see cref="Console.Out"/>.
    /// <para/>
    /// To log to a file, use a <see cref="StreamWriter"/>.
    /// </summary>
    public TextWriter Log { get; set; } = StreamWriter.Null;
    public bool IsLogging => Log != StreamWriter.Null;

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.AppendLine($"{GetType().Name}:");
        foreach (var property in GetType().GetProperties())
        {
            if (!(property.CanWrite && property.CanRead) || property.PropertyType == typeof(TextWriter))
                continue;

            builder.Append($"  {property.Name}=");

            object? value = property.GetValue(this);
            if (value is null)
                builder.Append("null");
            else if (value is string)
                builder.Append(Strings.EscapeAndQuote(value));
            else if (value is IEnumerable<string> e)
                builder.Append($"{{ {Strings.EscapeAndQuote(e.OrderBy(x => x))} }}");
            else
                builder.Append(value);

            builder.AppendLine();
        }
        return builder.ToString();
    }

    /// <summary>
    /// Make a copy of the parameters for thread safety.
    /// </summary>
    public virtual MainParameters DeepCopyAndInitialize() => (MainParameters)MemberwiseClone();
}
