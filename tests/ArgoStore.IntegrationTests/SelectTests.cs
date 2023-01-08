using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class SelectTests : IntegrationTestBase
{
    public SelectTests()
    {
        AddTestPersons();
    }

    [Fact]
    public void SelectStringPropertyToList_ReturnsPropertyList()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<string> names = s.Query<Person>().Select(x => x.Name).ToList();

        names.Should().HaveCountGreaterThan(90);
    }

    [Fact]
    public void SelectIntPropertyToList_ReturnsPropertyList()
    {

    }
}