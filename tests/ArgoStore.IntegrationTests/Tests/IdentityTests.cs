using ArgoStore.TestsCommon.Entities.Person;

namespace ArgoStore.IntegrationTests.Tests;

public class IdentityTests : IntegrationTestBase
{
    [Fact]
    public void PK_NotStringNotGuid_RegisterThrowsException()
    {
        Action a = () => Store.RegisterDocument<PersonPkInt32>();

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void PK_String_Set_SetsValueInDb()
    {
        Store.RegisterDocument<PersonPkString>();

        using IArgoDocumentSession s = Store.OpenSession();
        const string key = "a";

        s.Insert(new PersonPkString
        {
            Name = "ab",
            Id = key
        });

        s.SaveChanges();

        PersonPkString person = s.GetById<PersonPkString>(key);
        person.Should().NotBeNull();
        person!.Id.Should().Be(key);
    }

    [Fact]
    public void PK_String_NotSet_ThrowsExceptionOnInsert()
    {
        Store.RegisterDocument<PersonPkString>();

        using IArgoDocumentSession s = Store.OpenSession();
        // ReSharper disable once AccessToDisposedClosure
        Action a = () => s.Insert(new PersonPkString
        {
            Name = "a",
            Id = null
        });

        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Pk_Guid_NotSet_SetsInModelAndDb()
    {
        Store.RegisterDocument<PersonPkGuid>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkGuid p = new PersonPkGuid
        {
            Name = "Name12"
        };

        s.Insert(p);
        s.SaveChanges();

        p.Id.Should().NotBeEmpty();

        PersonPkGuid p2 = s.GetById<PersonPkGuid>(p.Id);
        p2.Should().BeEquivalentTo(p);
    }

    [Fact]
    public void Pk_Guid_Set_SetsInModelAndDb()
    {
        Store.RegisterDocument<PersonPkGuid>();

        using IArgoDocumentSession s = Store.OpenSession();
        Guid id = Guid.NewGuid();

        PersonPkGuid p = new PersonPkGuid
        {
            Name = "Name12",
            Id = id
        };

        s.Insert(p);
        s.SaveChanges();

        p.Id.Should().NotBeEmpty();

        PersonPkGuid p2 = s.GetById<PersonPkGuid>(id);
        p2.Should().BeEquivalentTo(p);
    }

    [Fact]
    public void Pk_OverrideFromConfig_UsesDifferentPropertyForKey()
    {
        Store.RegisterDocument<PersonPkString>(c =>
        {
            c.PrimaryKey(p => p.Name);
        });

        PersonPkString p1 = new PersonPkString
        {
            Id = "a",
            Name = "b"
        };

        using IArgoDocumentSession s = Store.OpenSession("myTenant");

        s.Insert(p1);
        s.SaveChanges();

        PersonPkString p2 = s.GetById<PersonPkString>(p1.Name);
        p2.Should().BeEquivalentTo(p1);
    }
}