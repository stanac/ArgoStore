using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class IntegrationTestBase : IDisposable
{
    protected TestDb TestDb { get; } = TestDb.CreateNew();
    protected ArgoDocumentStore Store { get; }

    public IntegrationTestBase()
    {
        Store = new ArgoDocumentStore(TestDb.ConnectionString);
        Store.RegisterDocumentType<Person>();
    }

    protected void AddTestPerson()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();
    }

    protected void AddTestPersons()
    {
        using IArgoDocumentSession s = Store.OpenSession();
        
        s.Insert(PersonTestData.GetPersonTestData());
        s.SaveChanges();
    }

    public void Dispose()
    {
        TestDb.Dispose();
    }
}