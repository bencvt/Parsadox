namespace Parsadox.Parser.UnitTests;

public class AssemblyTests : TestsBase
{
    [Test]
    public void MainAssembly_ExportedTypes_TestSuiteExistsForAll()
    {
        var exportedTypes = typeof(Game).Assembly.GetExportedTypes()
            .Where(t => !t.IsGenericType)
            .ToList();

        var coveredTypes = GetType().Assembly.GetExportedTypes()
            .SelectMany(t => TestCoversAttribute.Find(t))
            .ToHashSet();

        Assert.Multiple(() =>
        {
            foreach (Type type in exportedTypes)
            {
                if (!coveredTypes.Contains(type))
                    Assert.Fail($"Missing test suite for {type}");
            }
        });
    }

    [Test]
    public void TestAssembly_TestSuites_InheritFromTestsBase()
    {
        var testSuiteTypes = GetType().Assembly.GetExportedTypes()
            .Where(t => !t.IsNested)
            .ToList();

        Assert.Multiple(() =>
        {
            foreach (Type type in testSuiteTypes)
            {
                if (!type.IsSubclassOf(typeof(TestsBase)) && type != typeof(TestsBase))
                    Assert.Fail($"Test suite {type} does not inherit from {nameof(TestsBase)}");
            }
        });
    }
}
