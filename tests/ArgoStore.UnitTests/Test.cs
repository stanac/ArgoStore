using ArgoStore.IntegrationTests;
using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

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
        AddTestPersons();
    }

    [Fact]
    public void Test1()
    {
        IArgoQueryDocumentSession session = _store.CreateQuerySession();

        List<Person> res = session.Query<Person>().Where(x => x.BirthYear.HasValue).ToList();
    }

    private void AddTestPersons()
    {
        IArgoDocumentSession session = _store.CreateSession();

        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        foreach (Person person in persons)
        {
            session.Insert(person);
        }

        session.SaveChanges();
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}