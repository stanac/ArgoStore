using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class WhereStringTransformTests : IntegrationTestBase
{
    private const string LookupTrim = "Shelly Hunt";
    private const string LookupTrimStart = "Rosalie Norman";
    private const string LookupTrimEnd = "Katie Banks";
    private const string LookupUpperCase = "Ramona Taylor";
    private const string LookupLowerCase = "Ken Sutton";
    
    public WhereStringTransformTests()
    {
        AddTestPersons();
    }

    [Fact]
    public void NoMatchedWithoutTrim_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name == LookupTrim).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void NoMatchedWithoutTrim_Trim_ReturnsExpectedValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        List<Person> p = s.Query<Person>().Where(x => x.Name.Trim() == LookupTrim).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void NoMatchedWithoutTrimStart_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name == LookupTrimStart).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void NoMatchedWithoutTrimStart_TrimStart_ReturnsExpectedValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        List<Person> p = s.Query<Person>().Where(x => x.Name.TrimStart() == LookupTrimStart).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void NoMatchedWithoutTrimEnd_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name == LookupTrimEnd).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void NoMatchedWithoutTrimEnd_TrimEnd_ReturnsExpectedValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        List<Person> p = s.Query<Person>().Where(x => x.Name.TrimEnd() == LookupTrimEnd).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void NoMatchedWithoutToUpper_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        
        List<Person> p = s.Query<Person>().Where(x => x.Name == LookupUpperCase).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void NoMatchedWithoutToUpper_ToUpper_ReturnsExpectedValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        List<Person> p = s.Query<Person>().Where(x => x.Name.ToUpper() == LookupUpperCase.ToUpper()).ToList();
        p.Should().ContainSingle();
    }

    [Fact]
    public void NoMatchedWithoutToLower_ReturnsEmptyCollection()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> p = s.Query<Person>().Where(x => x.Name == LookupLowerCase).ToList();
        p.Should().BeEmpty();
    }

    [Fact]
    public void NoMatchedWithoutToLower_ToLower_ReturnsExpectedValue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        List<Person> p = s.Query<Person>().Where(x => x.Name.ToLower() == LookupLowerCase.ToLower()).ToList();
        p.Should().ContainSingle();
    }
}