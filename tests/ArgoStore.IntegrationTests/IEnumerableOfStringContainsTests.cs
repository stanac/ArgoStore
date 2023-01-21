using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class IEnumerableOfStringContainsTests : IntegrationTestBase
{
    public IEnumerableOfStringContainsTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void WhereWithContains_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles != null && x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }
}