using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests.Tests;

public class FirstSingleTests : IntegrationTestBase
{
    [Fact]
    public void EmptyTable_FirstOrDefault_ReturnsNull()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person p = s.Query<Person>().FirstOrDefault();
        p.Should().BeNull();
    }

    [Fact]
    public void NonEmptyTable_FirstOrDefault_ReturnsDocument()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertSingleTestPerson();

        Person p = s.Query<Person>().FirstOrDefault();
        p.Should().NotBeNull();
    }

    [Fact]
    public void EmptyTable_First_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Action a = () => s.Query<Person>().First();
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NonEmptyTable_First_ReturnsDocument()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertSingleTestPerson();

        Person p = s.Query<Person>().First();
        p.Should().NotBeNull();
    }

    [Fact]
    public void EmptyTable_SingleOrDefault_ReturnsNull()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person p = s.Query<Person>().SingleOrDefault();
        p.Should().BeNull();
    }

    [Fact]
    public void TableWithSingleItem_SingleOrDefault_ReturnsItem()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertSingleTestPerson();

        Person p = s.Query<Person>().SingleOrDefault();
        p.Should().NotBeNull();
    }

    [Fact]
    public void TableWithMultipleItems_SingleOrDefault_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertTestPersons();

        Action a = () => s.Query<Person>().SingleOrDefault();
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EmptyTable_Single_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Action a = () => s.Query<Person>().Single();
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void TableWithSingleItem_Single_ReturnsItem()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertSingleTestPerson();

        Person p = s.Query<Person>().Single();
        p.Should().NotBeNull();
    }

    [Fact]
    public void TableWithMultipleItems_Single_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        InsertTestPersons();

        Action a = () => s.Query<Person>().Single();
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EmptyTable_FirstWithCondition_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Action a = () => s.Query<Person>().First(x => x.BirthYear.HasValue);
        a.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NonEmptyTable_FirstWithCondition_ReturnsSingleItem()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        persons[0].BirthYear = null;
        persons[1].BirthYear = 9000;

        s.Insert(persons);
        s.SaveChanges();

        Person p = s.Query<Person>().First(x => x.BirthYear.HasValue);
        p.BirthYear.Should().HaveValue();
    }

    [Fact]
    public void TableSingleItemMet_FirstOrDefaultWithCondition_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        persons[0].BirthYear = null;
        persons[1].BirthYear = 9000;

        s.Insert(persons);
        s.SaveChanges();

        Person p = s.Query<Person>().FirstOrDefault(x => x.BirthYear.HasValue);
        p.BirthYear.Should().HaveValue();
    }

    [Fact]
    public void NonEmptyTable_SingleWithCondition_ReturnsSingleItem()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        persons[0].BirthYear = null;
        persons[1].BirthYear = 9000;

        s.Insert(persons);
        s.SaveChanges();

        Person p = s.Query<Person>().Single(x => x.BirthYear.HasValue);
        p.BirthYear.Should().HaveValue();
    }

    [Fact]
    public void TableSingleItemMet_SingleOrDefaultWithCondition_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        persons[0].BirthYear = null;
        persons[1].BirthYear = 9000;

        s.Insert(persons);
        s.SaveChanges();

        Person p = s.Query<Person>().SingleOrDefault(x => x.BirthYear.HasValue);
        p.BirthYear.Should().HaveValue();
    }
}