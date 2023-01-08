using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

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

    [Fact]
    public void DistinctNumberOnWhere_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<int> values = s.Query<Person>()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Distinct()
            .ToList();

        values = values.OrderBy(x => x).ToList();

        List<int> expectedValues = PersonTestData.GetPersonTestData()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        values.Should().HaveCount(expectedValues.Count);

        for (int i = 0; i < values.Count; i++)
        {
            values[i].Should().Be(expectedValues[i]);
        }
    }
    
    [Fact]
    public void DistinctNumberOnWhereAfterSelect_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<int> values = s.Query<Person>()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Where(x => x > 0)
            .Distinct()
            .ToList();

        values = values.OrderBy(x => x).ToList();

        List<int> expectedValues = PersonTestData.GetPersonTestData()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        values.Should().HaveCount(expectedValues.Count);

        for (int i = 0; i < values.Count; i++)
        {
            values[i].Should().Be(expectedValues[i]);
        }
    }
}