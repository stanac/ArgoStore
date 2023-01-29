using ArgoStore.TestsCommon.Entities.Person;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace ArgoStore.IntegrationTests.Tests.CrudTests;

public class LastTests : IntegrationTestBase
{
    [Fact]
    public void Last_ThrowsNotSupportedException()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        Action a = () => s.Query<Person>().Last();

        a.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void LastOrDefault_ThrowsNotSupportedException()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();
        Action a = () => s.Query<Person>().LastOrDefault();

        a.Should().Throw<NotSupportedException>();
    }
}