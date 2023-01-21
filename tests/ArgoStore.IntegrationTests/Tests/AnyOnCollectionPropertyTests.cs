using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class AnyOnCollectionPropertyTests : IntegrationTestBase
{
    public AnyOnCollectionPropertyTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void AnyOnStringCollection_NoCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Any())
            .ToList();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles == null || x.Roles.Count == 0);
        r.Should().HaveCount(expectedCount);
    }
}