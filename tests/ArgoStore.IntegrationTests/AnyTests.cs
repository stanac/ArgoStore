using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class AnyTests : IntegrationTestBase
{
    [Fact]
    public void EmptyTable_AnyReturnsFalse()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        bool any = s.Query<Person>().Any();
        any.Should().BeFalse();
    }

    [Fact]
    public void NotEmptyTable_AnyReturnsTrue()
    {
        InsertSingleTestPerson();
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        bool any = s.Query<Person>().Any();
        any.Should().BeTrue();
    }

    [Fact]
    public void NonEmptyTable_AnyWithPredicate_ConditionNotMet_ReturnsFalse()
    {
        InsertSingleTestPerson();
        Person person = SingleTestPerson();
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        bool any = s.Query<Person>().Any(x => x.Name != person.Name);
        any.Should().BeFalse();
    }

    [Fact]
    public void NonEmptyTable_AnyWithPredicate_ConditionMet_ReturnsTrue()
    {
        InsertSingleTestPerson();
        Person person = SingleTestPerson();
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        bool any = s.Query<Person>().Any(x => x.Name == person.Name);
        any.Should().BeTrue();
    }
}