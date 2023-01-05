using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

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
}