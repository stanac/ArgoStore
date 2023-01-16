using ArgoStore.TestsCommon.Entities;

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

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains("sales"))
            .ToList();
    }
}