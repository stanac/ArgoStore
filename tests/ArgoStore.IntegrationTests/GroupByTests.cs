using ArgoStore.TestsCommon.Entities;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests;

public class GroupByTests : IntegrationTestBase
{
    [Fact]
    public void GroupBy_ThrowsNotSupportedException()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        Action a = () => s.Query<Person>()
            .GroupBy(x => x.NickName)
            .ToList();

        a.Should().Throw<NotSupportedException>();
    }
}