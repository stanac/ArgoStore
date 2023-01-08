using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class NumberComparisonTests : IntegrationTestBase
{
    public NumberComparisonTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void GreaterThan()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.Points > 3)
            .ToList();

        persons.All(x => x.Points > 3).Should().BeTrue();
    }

    [Fact]
    public void GreaterOrEqualThan()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.Points >= 3)
            .ToList();

        persons.All(x => x.Points >= 3).Should().BeTrue();
    }

    [Fact]
    public void LessThan()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.Points < 3)
            .ToList();

        persons.All(x => x.Points < 3).Should().BeTrue();
    }

    [Fact]
    public void LessThanOrEqual()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.Points <= 3)
            .ToList();

        persons.All(x => x.Points <= 3).Should().BeTrue();
    }

}