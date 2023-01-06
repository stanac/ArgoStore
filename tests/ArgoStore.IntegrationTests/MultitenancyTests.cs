using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class MultitenancyTests : IntegrationTestBase
{
    private readonly Person _tenant1Person;
    private readonly Person _tenant2Person;

    private const string Tenant1 = "t1";
    private const string Tenant2 = "t2";

    public MultitenancyTests()
    {
        _tenant1Person = PersonTestData.GetPersonTestData().First();
        _tenant1Person.Name = "Tenant 1";

        _tenant2Person = PersonTestData.GetPersonTestData().First();
        _tenant2Person.Id = _tenant1Person.Id;
        _tenant2Person.Name = "Tenant 2";
    }

    [Fact]
    public void InsertPersonFromT1_CanInsertPersonFromT2()
    {
        InsertTenant1Person();
        Action a = InsertTenant2Person;

        a.Should().NotThrow();
    }

    [Fact]
    public void GetPersonFromT2_ReturnsCorrectPerson()
    {
        InsertTenant1Person();
        InsertTenant2Person();

        using IArgoDocumentSession s = Store.OpenSession(Tenant2);
        Person p = s.Query<Person>().First();

        p.Name.Should().NotBe(_tenant1Person.Name);
        p.Name.Should().Be(_tenant2Person.Name);
    }

    private void InsertTenant1Person()
    {
        using IArgoDocumentSession s = Store.OpenSession(Tenant1);
        s.Insert(_tenant1Person);
        s.SaveChanges();
    }

    private void InsertTenant2Person()
    {
        using IArgoDocumentSession s = Store.OpenSession(Tenant2);
        s.Insert(_tenant2Person);
        s.SaveChanges();
    }
}