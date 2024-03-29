﻿using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class QueryOnNestedObjectTests : IntegrationTestBase
{
    public QueryOnNestedObjectTests()
    {
        // UseFileDb();
        InsertTestPersons();
    }

    [Fact]
    public void QueryOnSimpleNestedProp_ReturnsExpected()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.PrimaryContact.ContactType < 0)
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.PrimaryContact != null && x.PrimaryContact.ContactType < 0)
            .ToList();

        r.Should().HaveCount(expected.Count);
        r.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void QueryOnNestedObjectWithSubqueryWithBoolCondition_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.PrimaryContact.ContactInfos.Any(c => c.Active))
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.PrimaryContact != null 
                        && x.PrimaryContact.ContactInfos != null 
                        && x.PrimaryContact.ContactInfos.Any(y => y.Active))
            .ToList();

        r.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void QueryOnNestedObjectWithSubqueryContainingSubquery_GivesExpectedResults()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> r = s.Query<Person>()
            .Where(x => x.PrimaryContact.ContactInfos.Any(c => c.Details.Any(d => d == "s2")))
            .ToList();

        List<Person> expected = PersonTestData.GetPersonTestData()
            .Where(x => x.PrimaryContact != null
                        && x.PrimaryContact.ContactInfos != null
                        && x.PrimaryContact.ContactInfos.Any(
                            y => y.Details != null
                                    && y.Details.Contains("s2")))
            .ToList();

        r.Should().BeEquivalentTo(expected);
    }
}