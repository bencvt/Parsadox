namespace Parsadox.Parser.UnitTests.Utility;

/// <summary>
/// Same idea as NUnit.Framework.TestOfAttribute,
/// but allows multiple types to be specified.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class TestCoversAttribute : Attribute
{
    internal Type CoversType { get; }

    internal TestCoversAttribute(Type coversType)
    {
        CoversType = coversType;
    }

    internal static IEnumerable<Type> Find(Type type) =>
        type.GetCustomAttributes(typeof(TestCoversAttribute), inherit: true)
            .OfType<TestCoversAttribute>()
            .Select(x => x.CoversType);
}
