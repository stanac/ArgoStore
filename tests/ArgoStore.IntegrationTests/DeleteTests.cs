using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

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
    public void DeleteById_DeletesDocument()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();

        s.Query<Person>().Count().Should().Be(_count);
        s.DeleteById<Person>(_person1.Id, _person2.Id);

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();
        s.Query<Person>().Count().Should().Be(_count);

        s.SaveChanges();

        s.Query<Person>().Count().Should().Be(_count - 2);

        s.GetById<Person>(_person1.Id).Should().BeNull();
        s.GetById<Person>(_person2.Id).Should().BeNull();
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