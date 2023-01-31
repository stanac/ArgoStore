using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests.WhereOnTypesTests;

public class DateTimeTests : IntegrationTestBase
{
    public DateTimeTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void GreaterThan_GivesExpectedResults()
    {
        List<DateTime> dates = PersonTestData.GetPersonTestData()
            .Where(x => x.CakeDay is not null)
            .Select(x => x.CakeDay.Value)
            .OrderBy(x => x)
            .ToList();

        DateTime dt = dates.Skip(dates.Count / 2).First();

        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.CakeDay > dt)
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.CakeDay != null && x.CakeDay > dt)
            .ToList();

        r.Should().HaveCount(expected.Count);
        r.Should().BeEquivalentTo(expected);
    }
}