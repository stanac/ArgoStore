using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class CollectionPropertySubQueryTests : IntegrationTestBase
{
    public CollectionPropertySubQueryTests()
    {
        UseFileDb();
        InsertTestPersons();
    }

    [Fact]
    public void AnyOnStringCollection_NoCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Any())
            .ToList();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles != null && x.Roles.Count > 0);
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void AnyWithConditionOnStringCollection_NoCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Any(role => role.Length == 4))
            .ToList();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles != null && x.Roles.Any(role => role.Length == 4));
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void CountOnStringCollection_NoCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Count > 0)
            .ToList();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.Roles != null && x.Roles.Count > 0);
        r.Should().HaveCount(expectedCount);
    }

    [Fact]
    public void CountEqualsOnStringCollection_NoCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.Roles.Count == 3)
            .ToList();

        var expected = PersonTestData.GetPersonTestData().Where(x => x.Roles != null && x.Roles.Count == 3).ToList();
        r.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void CountWithConditionEquals_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var expected = PersonTestData.GetPersonTestData()
            .Where(x => x.Contacts != null && x.Contacts.Count(y => y.ContactType < 0) == 1)
            .ToList();

        /*
            -- EXPECTED SQL

            SELECT t1.jsonData
            FROM Person t1
            WHERE (
			            SELECT COUNT(1)
			            FROM json_each(t1.jsonData, '$.contacts') t2
			            WHERE json_extract(t2.value, '$.contactType') < 0
	              ) = 1
         */
        
        List<Person> r1 = s.Query<Person>()
            .Where(x => x.Contacts.Count(y => y.ContactType < 0) == 1)
            .ToList();

        List<Person> r2 = s.Query<Person>()
            // ReSharper disable once ReplaceWithSingleCallToCount
            .Where(x => x.Contacts.Where(y => y.ContactType < 0).Count() == 1)
            .ToList();

        r1.Should().BeEquivalentTo(expected);
        r2.Should().BeEquivalentTo(expected);
    }

}