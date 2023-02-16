using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;
using Microsoft.Data.Sqlite;
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class InsertTests : IntegrationTestBase
{
    public InsertTests()
    {
        Store.RegisterDocument<PersonPkGuid>();
        Store.RegisterDocument<PersonPkString>();
    }

    [Fact]
    public void InsertWithGuidPkSet_Inserts_DoesNotChangeId()
    {
        PersonPkGuid toInsert = PersonPkGuid.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkGuid person = s.GetById<PersonPkGuid>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithGuidPkNotSet_Inserts_SetsNewId()
    {
        PersonPkGuid toInsert = PersonPkGuid.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        toInsert.Id = default;
        s.Insert(toInsert);
        s.SaveChanges();

        toInsert.Id.Should().NotBe(Guid.Empty);

        PersonPkGuid person = s.GetById<PersonPkGuid>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithStringPkSet_Inserts_DoesNotChangeId()
    {
        PersonPkString toInsert = PersonPkString.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkString person = s.GetById<PersonPkString>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithStringPkNotSet_ThrowsException()
    {
        PersonPkString toInsert = PersonPkString.Create();
        toInsert.Id = null;

        using IArgoDocumentSession s = Store.OpenSession();

        Action a = () => s.Insert(toInsert);
        a.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void InsertPkString_WithExistingId_ThrowsException()
    {
        // UseFileDb();

        Person p = PersonTestData.GetPersonTestData().First();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(p);
        s.SaveChanges();

        s.Insert(p);
        Action a = () => s.SaveChanges();

        a.Should().Throw<SqliteException>();
    }
}