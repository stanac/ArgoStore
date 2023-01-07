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

        string name = "Paul"; // should be two names containing Paul

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseSensitive_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul"; // should be two names containing Paul

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseMatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "Paul"; // should be two names containing Paul

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().HaveCount(2);
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseNotMatched_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul"; // should be two names containing Paul, but zero containing paul

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.Ordinal)).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void ContainsCaseInsensitiveCaseMismatched_ReturnsCorrectItems()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        string name = "paul"; // should be two names containing Paul

        List<Person> p = s.Query<Person>().Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        p.Should().HaveCount(2);
    }
}