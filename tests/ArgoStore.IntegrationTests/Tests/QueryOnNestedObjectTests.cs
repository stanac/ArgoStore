using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class QueryOnNestedObjectTests : IntegrationTestBase
{
    public QueryOnNestedObjectTests()
    {
        UseFileDb();
        InsertTestPersons();
    }

    [Fact]
    public void Test12()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.PrimaryContact.ContactType < 0)
            .ToList();

        var expected = PersonTestData.GetPersonTestData()
            .Where(x => x.PrimaryContact != null && x.PrimaryContact.ContactType < 0)
            .ToList();

        r.Should().HaveCount(expected.Count);
    }

    //[Fact]
    //public void Test1()
    //{
    //    using IArgoQueryDocumentSession s = Store.OpenQuerySession();

    //    List<Person> r = s.Query<Person>()
    //        .Where(x => x.PrimaryContact.ContactInfos.Any(c => c.Details.Any(d => d == "s2")))
    //        .ToList();
    //}
}