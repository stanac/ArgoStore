using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class EnumTests : IntegrationTestBase
{
    public EnumTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void Insert_Get_ReturnsCorrectValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        foreach (Person p in persons)
        {
            Person stored = s.GetById<Person>(p.Id);

            stored.Type.Should().Be(p.Type);
        }
    }
}