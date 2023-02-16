using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class DeleteTests : IntegrationTestBase
{
    private readonly Person _person1;
    private readonly Person _person2;
    private readonly int _count;

    public DeleteTests()
    {
        InsertTestPersons();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        _person1 = persons[0];
        _person2 = persons[1];
        _count = PersonTestData.GetPersonTestData().Count();
    }

    [Fact]
    public void DeleteDocument_DeletesDocument()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();

        s.Query<Person>().Count().Should().Be(_count);
        s.Delete(_person1, _person2);

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();
        s.Query<Person>().Count().Should().Be(_count);

        s.SaveChanges();

        s.Query<Person>().Count().Should().Be(_count - 2);

        s.GetById<Person>(_person1.Id).Should().BeNull();
        s.GetById<Person>(_person2.Id).Should().BeNull();
    }

    [Fact]
    public void DeletePkGuidById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkGuid>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkGuid p1 = PersonPkGuid.TestPerson1;
        PersonPkGuid p2 = PersonPkGuid.TestPerson2;
        Guid id1 = p1.Id;
        Guid id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkGuid>(id1).Should().NotBeNull();
        s.GetById<PersonPkGuid>(id2).Should().NotBeNull();

        s.Query<PersonPkGuid>().Count().Should().Be(2);
        s.DeleteById<PersonPkGuid>(id1, id2);

        s.GetById<PersonPkGuid>(id1).Should().NotBeNull();
        s.GetById<PersonPkGuid>(id2).Should().NotBeNull();
        s.Query<PersonPkGuid>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkGuid>().Count().Should().Be(0);

        s.GetById<PersonPkGuid>(id1).Should().BeNull();
        s.GetById<PersonPkGuid>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkStringById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkString>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkString p1 = PersonPkString.TestPerson1;
        PersonPkString p2 = PersonPkString.TestPerson2;
        string id1 = p1.Id;
        string id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkString>(id1).Should().NotBeNull();
        s.GetById<PersonPkString>(id2).Should().NotBeNull();

        s.Query<PersonPkString>().Count().Should().Be(2);
        s.DeleteById<PersonPkString>(id1, id2);

        s.GetById<PersonPkString>(id1).Should().NotBeNull();
        s.GetById<PersonPkString>(id2).Should().NotBeNull();
        s.Query<PersonPkString>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkString>().Count().Should().Be(0);

        s.GetById<PersonPkString>(id1).Should().BeNull();
        s.GetById<PersonPkString>(id2).Should().BeNull();
    }
    
    [Fact]
    public void DeleteWhere_DeletesDocuments()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();

        s.Query<Person>().Count().Should().Be(_count);
        s.DeleteWhere<Person>(x => x.Id == _person1.Id || x.Id == _person2.Id);

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();
        s.Query<Person>().Count().Should().Be(_count);

        s.SaveChanges();

        s.Query<Person>().Count().Should().Be(_count - 2);

        s.GetById<Person>(_person1.Id).Should().BeNull();
        s.GetById<Person>(_person2.Id).Should().BeNull();
    }
}