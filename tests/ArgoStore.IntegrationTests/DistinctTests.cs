namespace ArgoStore.IntegrationTests;

public class DistinctTests : IntegrationTestsBase
{
    public DistinctTests(ITestOutputHelper output) : base(output)
    {
        InsertTestData();
    }

    [SkippableFact]
    public void DistinctNumberOnSelect_GivesExpectedResults()
    {
        using IDocumentSession session = GetNewDocumentSession();

        List<int> result = session.Query<Person>()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Distinct()
            .ToList();
    }

    [SkippableFact]
    public void DistinctNumberOnWhere_GivesExpectedResults()
    {
        using IDocumentSession session = GetNewDocumentSession();

        List<int> result = session.Query<Person>()
            .Where(x => x.BirthYear.HasValue)
            .Select(x => x.BirthYear.Value)
            .Where(x => x > 0)
            .Distinct()
            .ToList();
    }

    private void InsertTestData()
    {
        using IDocumentSession session = GetNewDocumentSession();

        session.Insert(new Person
        {
            Name = "Petra",
            EmailAddress = "petra@example.com",
            BirthYear = 1988
        });

        session.Insert(new Person
        {
            Name = "Peter",
            EmailAddress = "peter@example.com",
            BirthYear = 1988
        });

        session.Insert(new Person
        {
            Name = "Peter",
            EmailAddress = "peter2@example.com",
            BirthYear = 1989
        });

        session.Insert(new Person
        {
            Name = "Mark",
            EmailAddress = "mark@example.com",
            BirthYear = 1988
        });

        session.SaveChanges();
    }
}