using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class SkipTakeTests : IntegrationTestBase
{
    public SkipTakeTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void Take_LimitsTakenCount()
    {
        IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> result = s.Query<Person>()
            .OrderBy(x => x.NumberOfPorts)
            .Take(3)
            .ToList();

        result.Should().HaveCount(3);
        result.ForEach(x => x.NumberOfPorts.Should().Be(0));
    }

    [Fact]
    public void SkipTask_GivesExpectedResult()
    {
        IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> result = s.Query<Person>()
            .OrderBy(x => x.NumberOfPorts)
            .Skip(50)
            .Take(3)
            .ToList();

        result.Should().HaveCount(3);
        result.ForEach(x => x.NumberOfPorts.Should().BeGreaterThan(0));
    }
}