using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class SubqueryTests : IntegrationTestBase
{
    public SubqueryTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void SimpleFromSubQuery_ToAnonymousObject_ToList_ReturnsExpected()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var result = s.Query<Person>()
            .Select(x => new { Birth = x.BirthYear, OldPoints = x.Points })
            .Select(x => new { NewPoints = x.OldPoints, x.Birth })
            .ToList();

        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        foreach (var r in result)
        {
            Person p = persons.SingleOrDefault(x => x.Points == r.NewPoints);
            p.Should().NotBeNull();

            r.Birth.Should().Be(p.BirthYear);
        }
    }
}