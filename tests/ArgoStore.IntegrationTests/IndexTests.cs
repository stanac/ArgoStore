using ArgoStore.TestsCommon.Entities;
using Microsoft.Data.Sqlite;

namespace ArgoStore.IntegrationTests;

public class IndexTests : IntegrationTestBase
{
    [Fact]
    public void PersonWithUniqueKeyOn_Email()
    {
        Store.RegisterDocument<Person>(p =>
        {
            p.UniqueIndex(x => x.EmailAddress);
        });

        using IArgoDocumentSession s = Store.OpenSession();

        Person p1 = SingleTestPerson();

        s.Insert(p1);
        s.SaveChanges();

        p1.Id = Guid.NewGuid();
        s.Insert(p1);

        // ReSharper disable once AccessToDisposedClosure
        Action a = () => s.SaveChanges();
        a.Should().Throw<SqliteException>();
    }

    [Fact]
    public void PersonWithUniqueKeyOn_EmailAndEmailConfirmed()
    {
        Store.RegisterDocument<Person>(p =>
        {
            p.UniqueIndex(x => new {x.EmailAddress, x.EmailConfirmed});
        });

        using IArgoDocumentSession s = Store.OpenSession();

        Person p1 = SingleTestPerson();

        s.Insert(p1);
        s.SaveChanges();

        p1.Id = Guid.NewGuid();
        p1.EmailConfirmed = !p1.EmailConfirmed;
        s.Insert(p1);

        // ReSharper disable once AccessToDisposedClosure
        Action a = () => s.SaveChanges();
        a.Should().NotThrow(); // not throwing because EmailConfirmed is different

        p1.Id = Guid.NewGuid();
        s.Insert(p1);
        // ReSharper disable once AccessToDisposedClosure
        a = () => s.SaveChanges();
        a.Should().Throw<SqliteException>();
    }

    [Fact]
    public void CreateIndexOnSingleProperty_CreatesIndex()
    {
        Store.RegisterDocument<Person>(p =>
        {
            p.NonUniqueIndex(x => new { x.EmailAddress });
            p.NonUniqueIndex(x => x.EmailConfirmed);
        });

        using SqliteConnection c = CurrentTestDb.GetAndOpenConnection();
        List<SqliteHelpers.IndexInfo> indexes = SqliteHelpers.ListIndexes(c);

        indexes.Should().Contain(i => i.Sql.Contains("emailAddress"));
        indexes.Should().Contain(i => i.Sql.Contains("emailConfirmed"));
    }

    [Fact]
    public void CreateIndexOnMultipleProperties_CreatesIndex()
    {
        Store.RegisterDocument<Person>(p =>
        {
            p.NonUniqueIndex(x => new { x.EmailAddress, x.EmailConfirmed });
        });

        using SqliteConnection c = CurrentTestDb.GetAndOpenConnection();
        List<SqliteHelpers.IndexInfo> indexes = SqliteHelpers.ListIndexes(c);

        indexes.Should().Contain(i => i.Sql.Contains("emailAddress") && i.Sql.Contains("emailConfirmed"));
    }
}