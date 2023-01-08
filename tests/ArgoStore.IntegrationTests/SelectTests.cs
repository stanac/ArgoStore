using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

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
        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        names.Should().HaveCount(persons.Count);

        foreach (Person p in persons)
        {
            names.Should().Contain(p.Name);
        }
    }

    [Fact]
    public void SelectIntPropertyToList_ReturnsPropertyList()
    {

    }
}