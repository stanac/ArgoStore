using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class OrderByTests : IntegrationTestBase
{
    [Fact]
    public void OrderByNickName_GivesCorrectOrder()
    {
        InsertTestPersons();
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        
        List<Person> fromDb = s.Query<Person>()
            .OrderBy(x => x.NickName)
            .ToList();
        
        string names = string.Join("", fromDb.Select(x => x.NickName));
        string expected = string.Join("", PersonTestData.GetPersonTestData().OrderBy(x => x.NickName).Select(x => x.NickName));

        names.Should().Be(expected);
    }

    // TODO: order by anonymous object
    //
}