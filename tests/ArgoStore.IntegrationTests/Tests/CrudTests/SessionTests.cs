using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class SessionTests : IntegrationTestBase
{
    [Fact]
    public void DiscardChanges_DiscardsChanges()
    {
        Person[] persons = PersonTestData.GetPersonTestData().ToArray();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(persons);
        s.DiscardChanges();

        s.SaveChanges();

        int count = s.Query<Person>().Count();

        count.Should().Be(0);
    }
}