namespace ArgoStore.IntegrationTests;

public class WhereTests : IntegrationTestsBase
{
    private readonly TestData _td;

    public WhereTests(ITestOutputHelper output) : base(output)
    {
        using IDocumentSession session = GetNewDocumentSession();

        _td = new TestData(TestDbConnectionString);
        _td.InsertTestPersons();
    }

    [SkippableFact]
    public void WhereWithNotEqualNull_ReturnsCorrectEntities()
    {
        using IDocumentSession session = GetNewDocumentSession();

        List<Person> persons = session.Query<Person>()
            .Where(x => x.Name != null)
            .ToList();

        int count = persons.Count;
        count.Should().BeGreaterThan(1);
    }

    [SkippableFact]
    public void WhereWithEqualFromEntityType_ReturnsCorrectEntities()
    {
        using IDocumentSession session = GetNewDocumentSession();

        Person p = _td.Persons[0];

        List<Person> persons = session.Query<Person>()
            .Where(x => x.Name == p.Name)
            .ToList();

        int count = persons.Count;
        count.Should().Be(1);
    }

    [SkippableFact]
    public void WhereWithNullCheckParameters()
    {
        using IDocumentSession session = GetNewDocumentSession();

        string name = null;

        var persons = session.Query<Person>()
            .Where(x => (name == null || x.Name.Contains(name)))
            .ToList();

        int count = session.Query<Person>().Count();

        persons.Should().HaveCount(count);
    }
}