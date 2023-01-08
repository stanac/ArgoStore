using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class DistinctTests : IntegrationTestBase
{
    public DistinctTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void SimpleDistinct_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<int> values = s.Query<Person>().Select(x => x.NumberOfPorts).Distinct().ToList();
        values.Should().HaveCount(5);

        for (int i = 0; i < 5; i++)
        {
            values.Should().Contain(i);
        }

        values.Should().NotContain(5);
    }

    [Fact]
    public void DistinctAnonymousObject_GivesExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var values = s.Query<Person>().Select(x => new { x.NumberOfPorts, x.OddNumberOfPorts }).Distinct().ToList();
        values.Should().HaveCountGreaterThan(5);
        values.Should().HaveCountLessThan(51);

        string[] sValues = values.Select(x => $"{x.NumberOfPorts} {x.OddNumberOfPorts}").ToArray();

        sValues.Distinct().Count().Should().Be(sValues.Length);
    }
}