using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class CountTests : IntegrationTestBase
{
    [Fact]
    public void Insert1_Count_Returns1()
    {
        IArgoDocumentSession s = Store.OpenSession();

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();

        int count = s.Query<Person>().Count();
    }
}