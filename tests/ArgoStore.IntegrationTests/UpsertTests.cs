using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class UpsertTests : IntegrationTestBase
{
    public UpsertTests()
    {
        Store.RegisterDocument<PersonPkString>();
        Store.RegisterDocument<PersonPkGuid>();
        Store.RegisterDocument<PersonPkInt32>();
        Store.RegisterDocument<PersonPkUInt32>();
        Store.RegisterDocument<PersonPkInt64>();
        Store.RegisterDocument<PersonPkUInt64>();
    }

    [Fact]
    public void StringPk_ExistsInDb_UpdatesEntry()
    {
        PersonPkString p1 = new PersonPkString
        {
            Id = "person1",
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Insert(p1);
        s.SaveChanges();

        p1.Name = "Name 2";
        s.Upsert(p1);
        s.SaveChanges();

        PersonPkString pFromDb = s.GetById<PersonPkString>(p1.Id);
        pFromDb!.Name.Should().Be(p1.Name);
    }

    [Fact]
    public void StringPk_DoesNotExistInDb_InsertsEntry()
    {
        PersonPkString p1 = new PersonPkString
        {
            Id = "person1",
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Upsert(p1);
        s.SaveChanges();

        PersonPkString pFromDb = s.GetById<PersonPkString>(p1.Id);
        pFromDb!.Name.Should().Be(p1.Name);
    }

    [Fact]
    public void StringPk_DoesNotExistInDb_PkNotSet_InsertsEntry()
    {
        PersonPkString p1 = new PersonPkString
        {
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Query<PersonPkString>().Count().Should().Be(0);
        
        s.Upsert(p1);
        s.SaveChanges();

        PersonPkString pFromDb = s.Query<PersonPkString>().First();
        pFromDb!.Name.Should().Be(p1.Name);
    }

    [Fact]
    public void GuidPk_ExistsInDb_UpdatesEntry()
    {
        PersonPkGuid p1 = new PersonPkGuid
        {
            Id = Guid.NewGuid(),
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Insert(p1);
        s.SaveChanges();

        string newName = "Name 2";
        p1.Name = newName;

        s.Upsert(p1);
        s.SaveChanges();

        PersonPkGuid fromDb = s.Query<PersonPkGuid>().First();
        fromDb.Name.Should().Be(newName);
    }

    [Fact]
    public void GuidPk_DoesNotExistInDb_InsertsEntry()
    {
        PersonPkGuid p1 = new PersonPkGuid
        {
            Id = Guid.NewGuid(),
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Upsert(p1);
        s.SaveChanges();
        
        PersonPkGuid pFromDb = s.Query<PersonPkGuid>().Single();
        pFromDb.Should().NotBeNull();
        pFromDb!.Name.Should().Be(p1.Name);
    }

    [Fact]
    public void GuidPk_DoesNotExistInDb_IdNotSet_InsertsEntry()
    {
        PersonPkGuid p1 = new PersonPkGuid
        {
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Query<PersonPkGuid>().Count().Should().Be(0);
        
        s.Upsert(p1);
        s.SaveChanges();
        
        PersonPkGuid pFromDb = s.Query<PersonPkGuid>().Single();
        pFromDb.Should().NotBeNull();
        pFromDb!.Name.Should().Be(p1.Name);
    }

    [Fact]
    public void Int32Pk_DoesNotExistInDb_IdNotSet_Inserts()
    {
        PersonPkInt32 p1 = new PersonPkInt32
        {
            Name = "Name 1"
        };

        IArgoDocumentSession s = Store.OpenSession();
        s.Query<PersonPkInt32>().Count().Should().Be(0);

        s.Upsert(p1);
        s.SaveChanges();

        PersonPkInt32 pFromDb = s.Query<PersonPkInt32>().Single();
        pFromDb.Should().NotBeNull();
        pFromDb!.Name.Should().Be(p1.Name);
    }
}