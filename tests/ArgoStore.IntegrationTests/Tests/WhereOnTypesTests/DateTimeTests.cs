using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests.WhereOnTypesTests;

public class DateTimeTests : IntegrationTestBase
{
    public DateTimeTests()
    {
        UseFileDb();
        InsertTestPersons();
    }

    [Fact]
    public void GreaterThanDateTime_GivesExpectedResults()
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

    [Fact]
    public void GreaterThanDateTimeOffset_GivesExpectedResults()
    {
        List<DateTimeOffset> dates = PersonTestData.GetPersonTestData()
            .Select(x => x.RegistrationTime)
            .OrderBy(x => x)
            .ToList();

        DateTimeOffset dto = dates.Skip(dates.Count / 2).First();

        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.RegistrationTime > dto)
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.RegistrationTime > dto)
            .ToList();

        r.Should().HaveCount(expected.Count);
        r.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GreaterThanDateOnly_GivesExpectedResults()
    {
        List<DateOnly> dates = PersonTestData.GetPersonTestData()
            .Select(x => x.CoronationDate)
            .OrderBy(x => x)
            .ToList();

        DateOnly dto = dates.Skip(dates.Count / 2).First();

        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.CoronationDate > dto)
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.CoronationDate > dto)
            .ToList();

        r.Should().HaveCount(expected.Count);
        r.Should().BeEquivalentTo(expected);
    }

    // TODO: add TimeOnly test
}