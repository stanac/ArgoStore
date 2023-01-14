using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class UpsertTests : IntegrationTestBase
{
    public UpsertTests()
    {
        Store.RegisterDocument<PersonPkString>();
        Store.RegisterDocument<PersonPkGuid>();
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
    public void GuidPk_ExistsInDb_UpdatesEntry()
    {
        PersonPkGuid p1 = new PersonPkGuid
        {
            Id = Guid.NewGuid(),
            Name = "Name 1"
        };

        return;

        throw new NotImplementedException();
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

        var list = s.Query<PersonPkGuid>().ToList();

        PersonPkGuid pFromDb = s.GetById<PersonPkGuid>(p1.Id);
        pFromDb.Should().NotBeNull();
        pFromDb!.Name.Should().Be(p1.Name);
    }
}