using ArgoStore.IntegrationTests.TestInfra;
using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class IntegrationTestBase : IDisposable
{
    protected TestDb CurrentTestDb { get; private set; } = TestDb.CreateNew();
    protected ArgoDocumentStore Store { get; private set; }
    protected virtual bool InitializeUser => true;

    public IntegrationTestBase()
    {
        Initialize();
    }

    protected void InsertSingleTestPerson()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();
    }

    protected Person SingleTestPerson()
    {
        return PersonTestData.GetPersonTestData().First();
    }

    protected void InsertTestPersons()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(PersonTestData.GetPersonTestData().ToArray());
        s.SaveChanges();
    }

    protected void UseFileDb()
    {
        Dispose();

        CurrentTestDb = new OnDiskTestDb();
        Initialize();
    }

    public void Dispose()
    {
        CurrentTestDb.Dispose();
    }

    private void Initialize()
    {
        if (InitializeUser)
        {
            Store = new ArgoDocumentStore(CurrentTestDb.ConnectionString);
            Store.RegisterDocument<Person>();
        }
    }
}