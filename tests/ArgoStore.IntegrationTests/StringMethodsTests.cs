using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class StringMethodsTests : IntegrationTestBase
{
    private readonly IReadOnlyList<Person> _persons = PersonTestData.GetPersonTestData().ToList();

    public StringMethodsTests()
    {
        AddTestPersons();
        using IArgoDocumentSession s = Store.OpenSession();

        Person p = _persons[0].Copy();
        p.Id = Guid.NewGuid();
        p.Name = p.Name.ToLower();
        
        s.Insert(p);
        s.SaveChanges();
    }

    [Fact]
    public void Equals_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name.Equals(_persons[0].Name)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void EqualsCaseSensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name.Equals(_persons[0].Name, StringComparison.Ordinal)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void EqualsCaseInsensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name.Equals(_persons[0].Name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void Contains_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseSensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseMatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseInsensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul1";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void ContainsCaseSensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul1";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseNotMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseMismatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().HaveCount(2);
    }
    
    [Fact]
    public void StartsWith_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void StartsWithSensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void StartsWithInsensitiveCaseMatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void StartsWithInsensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul1";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void StartsWithCaseSensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul1";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void StartsWithCaseInsensitiveCaseNotMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void StartsWithCaseInsensitiveCaseMismatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul";

        List<Person> p = s.Query<Person>().Where(x => x.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().HaveCount(1);
    }

    [Fact]
    public void EndsWith_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Diaz";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void EndsWithSensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Diaz";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void EndsWithInsensitiveCaseMatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Diaz";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void EndsWithInsensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Dia1z";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void EndsWithCaseSensitiveNoValueMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Diaz1";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void EndsWithCaseInsensitiveCaseNotMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "diaz";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name, StringComparison.Ordinal)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void EndsWithCaseInsensitiveCaseMismatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "diaz";

        List<Person> p = s.Query<Person>().Where(x => x.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().HaveCount(1);
    }
}