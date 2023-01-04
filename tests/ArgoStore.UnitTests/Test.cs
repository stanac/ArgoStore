using ArgoStore.IntegrationTests;
using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.UnitTests;

public class Test : IDisposable
{
    private TestDb _testDb;
    private readonly ArgoDocumentStore _store;

    public Test()
    {
        _testDb = TestDb.CreateNew();
        _store = new ArgoDocumentStore(_testDb.ConnectionString);
        _store.RegisterDocumentType<Person>();
    }

    [Fact]
    public void Test1()
    {
        IArgoQueryDocumentSession session = _store.CreateQuerySession();

        List<Person> res = session.Query<Person>().Where(x => x.BirthYear.HasValue).ToList();
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}