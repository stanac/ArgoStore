using ArgoStore.TestsCommon.Entities;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests;

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
    public void NonEmptyTable_FirstOrDefault_ReturnsEntity()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        AddTestPerson();

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
    public void NonEmptyTable_First_ReturnsEntity()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        AddTestPerson();

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

        AddTestPerson();

        Person p = s.Query<Person>().SingleOrDefault();
        p.Should().NotBeNull();
    }

    [Fact]
    public void TableWithMultipleItems_SingleOrDefault_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        AddTestPersons();

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

        AddTestPerson();

        Person p = s.Query<Person>().Single();
        p.Should().NotBeNull();
    }

    [Fact]
    public void TableWithMultipleItems_Single_ThrowsException()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        AddTestPersons();

        Action a = () => s.Query<Person>().Single();
        a.Should().Throw<InvalidOperationException>();
    }
}