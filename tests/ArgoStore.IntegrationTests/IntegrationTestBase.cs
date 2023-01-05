using ArgoStore.TestsCommon.Entities;

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

    public void Dispose()
    {
        TestDb.Dispose();
    }
}