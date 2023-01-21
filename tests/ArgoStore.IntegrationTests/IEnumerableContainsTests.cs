using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class IEnumerableContainsTests : IntegrationTestBase
{
    public IEnumerableContainsTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void ContainsStringInList_GivesExpectedResult()
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

    [Fact]
    public void ContainsStringInArray_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesArray != null && x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void ContainsStringInIEnumerable_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesIEnumerable != null && x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void ContainsStringInIList_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesIList != null && x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void ContainsStringInIReadOnlyList_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesIReadOnlyList != null && x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ContainsInt_GivesExpectedResult(bool checkForNull)
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const int port = 2;

        List<Person> r;

        if (checkForNull)
        {
            r = s.Query<Person>()
                .Where(x => x.PortList != null && x.PortList.Contains(port))
                .ToList();
        }
        else
        {
            r = s.Query<Person>()
                .Where(x => x.PortList.Contains(port))
                .ToList();
        }

        r.All(x => x.PortList.Contains(port)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesIReadOnlyList != null && x.PortList.Contains(port));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void NotContainsInt_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const int port = 2;

        List<Person> r = s.Query<Person>()
            .Where(x => !x.PortList.Contains(port))
            .ToList();

        r.All(x => x.PortList == null || !x.PortList.Contains(port)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.RolesIReadOnlyList == null || !x.PortList.Contains(port));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void NotContainsString_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        const string role = "sales";

        List<Person> r = s.Query<Person>()
            .Where(x => !x.Roles.Contains(role))
            .ToList();

        r.All(x => x.Roles == null || !x.Roles.Contains(role)).Should().BeTrue();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles == null || !x.Roles.Contains(role));
        r.Should().HaveCount(expectedCount);
    }
}