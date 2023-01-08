﻿using ArgoStore.TestsCommon.Entities;

namespace ArgoStore.IntegrationTests;

public class NegationTests : IntegrationTestBase
{
    public NegationTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void NegateIntegerComparison_ReturnsExpectedResult()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<Person> persons = s.Query<Person>()
            .Where(x => !(x.Points > 3))
            .ToList();

        persons.All(x => x.Points <= 3).Should().BeTrue();
    }
}