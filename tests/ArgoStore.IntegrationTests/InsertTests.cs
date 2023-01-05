using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class InsertTests : IDisposable
{
    private readonly TestDb _testDb = TestDb.CreateNew();
    private readonly ArgoDocumentStore _store;
    private readonly Person _testPersonSetId = PersonTestData.GetPersonTestData().First();
    private readonly Person _testPersonNotSetId = PersonTestData.GetPersonTestData().Skip(1).First();

    public InsertTests()
    {
        _store = new ArgoDocumentStore(_testDb.ConnectionString);
        _store.RegisterDocumentType<Person>();
        _testPersonNotSetId.Id = default;
    }

    [Fact]
    public void InsertWithGuidPkSet_Inserts()
    {
        using IArgoDocumentSession s = _store.CreateSession();

        s.Insert(_testPersonSetId);
        s.SaveChanges();

        Person person = s.GetById<Person>(_testPersonSetId.Id);

        person.Should().BeEquivalentTo(_testPersonSetId);
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}