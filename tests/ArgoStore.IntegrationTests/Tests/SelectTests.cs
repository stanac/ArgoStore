﻿using ArgoStore.TestsCommon.Entities.Person;
using ArgoStore.TestsCommon.TestData;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests.Tests;

public class SelectTests : IntegrationTestBase
{
    public SelectTests()
    {
        InsertTestPersons();
    }

    [Fact]
    public void SelectStringPropertyToList_ReturnsPropertyList()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<string> names = s.Query<Person>().Select(x => x.Name).ToList();
        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        names.Should().HaveCount(persons.Count);

        foreach (Person p in persons)
        {
            names.Should().Contain(p.Name);
        }
    }

    [Fact]
    public void SelectIntPropertyToList_ReturnsPropertyList()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        List<int> points = s.Query<Person>().Select(x => x.Points).ToList();

        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        points.Should().HaveCount(persons.Count);

        foreach (Person p in persons)
        {
            points.Should().Contain(p.Points);
        }
    }

    [Fact]
    public void SelectAnonymousObject_ReturnsCorrectValues()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var result = s.Query<Person>()
            .Select(x => new { x.Name, x.Points, x.BirthYear, Birth = x.BirthYear })
            .ToList();
        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        result.Should().HaveCount(persons.Count);

        foreach (var r in result)
        {
            Person p = persons.Single(x => x.Name == r.Name);
            r.BirthYear.Should().Be(p.BirthYear);
            r.Points.Should().Be(p.Points);
            r.Birth.Should().Be(p.BirthYear);
        }
    }

    [Fact]
    public void SelectAnonymousObjectSingleProperty_ReturnsCorrectValues()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var result = s.Query<Person>()
            .Select(x => new { x.Points })
            .ToList();
        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        result.Should().HaveCount(persons.Count);

        foreach (var r in result)
        {
            Person p = persons.SingleOrDefault(x => x.Points == r.Points);
            p.Should().NotBeNull();
        }
    }

    [Fact]
    public void SelectAnonymousObjectSinglePropertyRenamed_ReturnsCorrectValues()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        var result = s.Query<Person>()
            .Select(x => new { NewPoints = x.Points })
            .ToList();
        List<Person> persons = PersonTestData.GetPersonTestData().ToList();

        result.Should().HaveCount(persons.Count);

        foreach (var r in result)
        {
            Person p = persons.SingleOrDefault(x => x.Points == r.NewPoints);
            p.Should().NotBeNull();
        }
    }

    [Fact]
    public void SelectAnonymousEmptyObject_ThrowsException()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        Action a = () => s.Query<Person>()
            .Select(x => new { })
            .ToList();

        a.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void SelectAnonymousObjectWithConstants_ReturnsExpected()
    {
        using IArgoQueryDocumentSession s = Store.OpenQuerySession();

        // {"points":3,"isSelected":1,"value":5}

        var selected = s.Query<Person>()
            .Select(x => new { x.Points, IsSelected = true, Value = 5 })
            .ToList();

        List<Person> persons = PersonTestData.GetPersonTestData().ToList();
        selected.Should().HaveCount(persons.Count);

        foreach (var r in selected)
        {
            Person p = persons.SingleOrDefault(x => x.Points == r.Points);
            p.Should().NotBeNull();

            r.Value.Should().Be(5);
            r.IsSelected.Should().BeTrue();
        }
    }
}