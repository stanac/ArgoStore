using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;
using Microsoft.Data.Sqlite;
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests;

public class InsertTests : IntegrationTestBase
{
    public InsertTests()
    {
        Store.RegisterDocumentType<PersonPkGuid>();
        Store.RegisterDocumentType<PersonPkString>();
        Store.RegisterDocumentType<PersonPkInt32>();
        Store.RegisterDocumentType<PersonPkUInt32>();
        Store.RegisterDocumentType<PersonPkInt64>();
        Store.RegisterDocumentType<PersonPkUInt64>();
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
    public void InsertWithInt32PkSet_Inserts_DoesNotChangeId()
    {
        PersonPkInt32 toInsert = PersonPkInt32.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkInt32 person = s.GetById<PersonPkInt32>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithInt32PkNotSet_Inserts_ChangesId()
    {
        PersonPkInt32 toInsert = PersonPkInt32.Create();
        toInsert.Id = 0;

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkInt32 person = s.GetById<PersonPkInt32>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithInt64PkSet_Inserts_DoesNotChangeId()
    {
        PersonPkInt64 toInsert = PersonPkInt64.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkInt64 person = s.GetById<PersonPkInt64>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithInt64PkNotSet_Inserts_ChangesId()
    {
        PersonPkInt64 toInsert = PersonPkInt64.Create();
        toInsert.Id = 0;

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkInt64 person = s.GetById<PersonPkInt64>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithUInt32PkSet_Inserts_DoesNotChangeId()
    {
        PersonPkUInt32 toInsert = PersonPkUInt32.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkUInt32 person = s.GetById<PersonPkUInt32>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithUInt32PkNotSet_Inserts_ChangesId()
    {
        PersonPkUInt32 toInsert = PersonPkUInt32.Create();
        toInsert.Id = 0;

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkUInt32 person = s.GetById<PersonPkUInt32>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithUInt64PkSet_Inserts_DoesNotChangeId()
    {
        PersonPkUInt64 toInsert = PersonPkUInt64.Create();

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkUInt64 person = s.GetById<PersonPkUInt64>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
    }

    [Fact]
    public void InsertWithUInt64PkNotSet_Inserts_ChangesId()
    {
        PersonPkUInt64 toInsert = PersonPkUInt64.Create();
        toInsert.Id = 0;

        using IArgoDocumentSession s = Store.OpenSession();

        s.Insert(toInsert);
        s.SaveChanges();

        PersonPkUInt64 person = s.GetById<PersonPkUInt64>(toInsert.Id);

        person.Should().BeEquivalentTo(toInsert);
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