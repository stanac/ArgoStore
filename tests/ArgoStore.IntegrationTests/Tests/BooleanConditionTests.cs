using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests.Tests;

public class BooleanConditionTests : IntegrationTestBase
{
    public BooleanConditionTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void WhereBooleanIsTrue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.EmailConfirmed)
            .ToList();

        persons.Should().NotBeEmpty();
        persons.All(x => x.EmailConfirmed).Should().BeTrue();
    }

    [Fact]
    public void WhereBooleanIsTrueEqualTrue()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.EmailConfirmed == true)
            .ToList();

        persons.Should().NotBeEmpty();
        persons.All(x => x.EmailConfirmed).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WhereBooleanIsTrueEqualParameterTrue(bool b)
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => x.EmailConfirmed == b)
            .ToList();

        persons.Should().NotBeEmpty();
        persons.All(x => x.EmailConfirmed == b).Should().BeTrue();
    }

    [Fact]
    public void WhereBooleanIsFalse()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => !x.EmailConfirmed)
            .ToList();

        persons.Should().NotBeEmpty();
        persons.All(x => !x.EmailConfirmed).Should().BeTrue();
    }

}