﻿namespace ArgoStore.IntegrationTests;

public class SelectStringMethodTests : IntegrationTestsBase
{
    private const string TestName = " SoMEonE ";

    public SelectStringMethodTests(ITestOutputHelper output) : base(output)
    {
    }

    [SkippableFact]
    public void SelectFirst_GivesExpectedResult()
    {
        InsertTestData();

        using IQueryDocumentSession s = GetNewDocumentSession();

        string name = s.Query<Person>().Select(x => x.Name).First();

        name.Should().Be(TestName);
    }

    //[SkippableFact]
    //public void SelectToUpper_GivesExpectedResult()
    //{
    //    InsertTestData();
            
    //    using IQueryDocumentSession s = GetNewDocumentSession();

    //    string name = s.Query<Person>().Select(x => x.Name.ToUpper()).First();

    //    name.Should().Be(TestName.ToUpper());
    //}
        
    private void InsertTestData()
    {
        using IDocumentSession s = GetNewDocumentSession();

        s.Insert(new Person
        {
            Name = TestName,
            EmailAddress = "someone@example.com"
        });

        s.SaveChanges();
    }
}