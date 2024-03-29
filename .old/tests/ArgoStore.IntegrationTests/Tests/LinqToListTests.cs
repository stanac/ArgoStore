﻿using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests.Tests;

public class LinqToListTests : IntegrationTestBase
{
    [Fact]
    public void NoDocuments_ToList_GivesEmptyList()
    {
        IArgoDocumentSession s = Store.OpenSession();

        List<Person> persons = s.Query<Person>().ToList();
        persons.Should().BeEmpty();
    }

    [Fact]
    public void SingleDocument_ToList_GivesListWithOfOne()
    {
        using IArgoDocumentSession s = Store.OpenSession();
        s.Insert(PersonTestData.GetPersonTestData().First());
        s.SaveChanges();

        List<Person> persons = s.Query<Person>().ToList();
        persons.Should().HaveCount(1);
    }

    [Fact]
    public void ToListWithCondition_GivesExpectedCount()
    {
        InsertTestPersons();
        using IArgoDocumentSession s = Store.OpenSession();

        int expectedCount = PersonTestData.GetPersonTestData().Count(x => x.BirthYear.HasValue);

        List<Person> persons = s.Query<Person>().Where(x => x.BirthYear.HasValue).ToList();
        persons.Should().HaveCount(expectedCount);
    }
}