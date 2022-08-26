using System.Reflection;

namespace Parsadox.Parser.Utility;

/// <summary>
/// Map <see cref="Game"/> enum values to <see cref="IGameHandler"/> instances.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal class GameHandlerAttribute : Attribute
{
    internal IGameVersion BeforeVersion { get; }

    internal IGameHandler Handler { get; }

    internal GameHandlerAttribute(Type handlerType, string? beforeVersion = null)
    {
        if (beforeVersion is null)
            BeforeVersion = GameVersion.UNKNOWN;
        else
            BeforeVersion = new GameVersion(beforeVersion);

        Handler = (Activator.CreateInstance(handlerType) as IGameHandler)
            ?? throw new InvalidOperationException($"Cannot create handler of type {handlerType.Name}");
    }

    internal static IReadOnlyDictionary<Game, Func<IGameVersion, IGameHandler>> BuildMap()
    {
        return typeof(Game).GetFields().Where(x => x.IsStatic).ToDictionary(
            field => (Game)(field.GetValue(null) ?? throw new InvalidOperationException($"{field} is null")),
            GetFunc);

        static Func<IGameVersion, IGameHandler> GetFunc(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes<GameHandlerAttribute>().ToList();

            // Quick sanity checks
            var unversioned = attributes.FirstOrDefault(x => x.BeforeVersion.IsUnknown);
            if (unversioned is null)
                throw new InvalidOperationException($"{field} missing unversioned {nameof(GameHandlerAttribute)}");
            if (attributes.IndexOf(unversioned) < attributes.Count - 1)
                throw new InvalidOperationException($"{field} has an unreachable {nameof(GameHandlerAttribute)}");

            return version => attributes.First(x => version.IsLessThan(x.BeforeVersion)).Handler;
        }
    }
}
