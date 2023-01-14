﻿using ArgoStore.TestsCommon.Entities;
using ArgoStore.TestsCommon.TestData;

namespace ArgoStore.IntegrationTests;

public class DeleteTests : IntegrationTestBase
{
    private readonly Person _person1;
    private readonly Person _person2;
    private readonly int _count;

    public DeleteTests()
    {
        InsertTestPersons();

        Person[] persons = PersonTestData.GetPersonTestData().Take(2).ToArray();
        _person1 = persons[0];
        _person2 = persons[1];
        _count = PersonTestData.GetPersonTestData().Count();
    }

    [Fact]
    public void DeleteDocument_DeletesDocument()
    {
        using IArgoDocumentSession s = Store.OpenSession();
        
        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();

        s.Query<Person>().Count().Should().Be(_count);
        s.Delete(_person1, _person2);

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();
        s.Query<Person>().Count().Should().Be(_count);

        s.SaveChanges();

        s.Query<Person>().Count().Should().Be(_count - 2);

        s.GetById<Person>(_person1.Id).Should().BeNull();
        s.GetById<Person>(_person2.Id).Should().BeNull();
    }

    [Fact]
    public void DeletePkGuidById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkGuid>();
        
        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkGuid p1 = PersonPkGuid.TestPerson1;
        PersonPkGuid p2 = PersonPkGuid.TestPerson2;
        Guid id1 = p1.Id;
        Guid id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkGuid>(id1).Should().NotBeNull();
        s.GetById<PersonPkGuid>(id2).Should().NotBeNull();

        s.Query<PersonPkGuid>().Count().Should().Be(2);
        s.DeleteById<PersonPkGuid>(id1, id2);

        s.GetById<PersonPkGuid>(id1).Should().NotBeNull();
        s.GetById<PersonPkGuid>(id2).Should().NotBeNull();
        s.Query<PersonPkGuid>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkGuid>().Count().Should().Be(0);

        s.GetById<PersonPkGuid>(id1).Should().BeNull();
        s.GetById<PersonPkGuid>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkStringById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkString>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkString p1 = PersonPkString.TestPerson1;
        PersonPkString p2 = PersonPkString.TestPerson2;
        string id1 = p1.Id;
        string id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkString>(id1).Should().NotBeNull();
        s.GetById<PersonPkString>(id2).Should().NotBeNull();

        s.Query<PersonPkString>().Count().Should().Be(2);
        s.DeleteById<PersonPkString>(id1, id2);

        s.GetById<PersonPkString>(id1).Should().NotBeNull();
        s.GetById<PersonPkString>(id2).Should().NotBeNull();
        s.Query<PersonPkString>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkString>().Count().Should().Be(0);

        s.GetById<PersonPkString>(id1).Should().BeNull();
        s.GetById<PersonPkString>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkInt32ById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkInt32>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkInt32 p1 = PersonPkInt32.TestPerson1;
        PersonPkInt32 p2 = PersonPkInt32.TestPerson2;
        int id1 = p1.Id;
        int id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkInt32>(id1).Should().NotBeNull();
        s.GetById<PersonPkInt32>(id2).Should().NotBeNull();

        s.Query<PersonPkInt32>().Count().Should().Be(2);
        s.DeleteById<PersonPkInt32>(id1, id2);

        s.GetById<PersonPkInt32>(id1).Should().NotBeNull();
        s.GetById<PersonPkInt32>(id2).Should().NotBeNull();
        s.Query<PersonPkInt32>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkInt32>().Count().Should().Be(0);

        s.GetById<PersonPkInt32>(id1).Should().BeNull();
        s.GetById<PersonPkInt32>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkInt64ById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkInt64>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkInt64 p1 = PersonPkInt64.TestPerson1;
        PersonPkInt64 p2 = PersonPkInt64.TestPerson2;
        long id1 = p1.Id;
        long id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkInt64>(id1).Should().NotBeNull();
        s.GetById<PersonPkInt64>(id2).Should().NotBeNull();

        s.Query<PersonPkInt64>().Count().Should().Be(2);
        s.DeleteById<PersonPkInt64>(id1, id2);

        s.GetById<PersonPkInt64>(id1).Should().NotBeNull();
        s.GetById<PersonPkInt64>(id2).Should().NotBeNull();
        s.Query<PersonPkInt64>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkInt64>().Count().Should().Be(0);

        s.GetById<PersonPkInt64>(id1).Should().BeNull();
        s.GetById<PersonPkInt64>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkUInt32ById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkUInt32>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkUInt32 p1 = PersonPkUInt32.TestPerson1;
        PersonPkUInt32 p2 = PersonPkUInt32.TestPerson2;
        uint id1 = p1.Id;
        uint id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkUInt32>(id1).Should().NotBeNull();
        s.GetById<PersonPkUInt32>(id2).Should().NotBeNull();

        s.Query<PersonPkUInt32>().Count().Should().Be(2);
        s.DeleteById<PersonPkUInt32>(id1, id2);

        s.GetById<PersonPkUInt32>(id1).Should().NotBeNull();
        s.GetById<PersonPkUInt32>(id2).Should().NotBeNull();
        s.Query<PersonPkUInt32>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkUInt32>().Count().Should().Be(0);

        s.GetById<PersonPkUInt32>(id1).Should().BeNull();
        s.GetById<PersonPkUInt32>(id2).Should().BeNull();
    }

    [Fact]
    public void DeletePkUInt64ById_DeletesDocument()
    {
        Store.RegisterDocument<PersonPkUInt64>();

        using IArgoDocumentSession s = Store.OpenSession();

        PersonPkUInt64 p1 = PersonPkUInt64.TestPerson1;
        PersonPkUInt64 p2 = PersonPkUInt64.TestPerson2;
        ulong id1 = p1.Id;
        ulong id2 = p2.Id;

        s.Insert(p1, p2);
        s.SaveChanges();

        s.GetById<PersonPkUInt64>(id1).Should().NotBeNull();
        s.GetById<PersonPkUInt64>(id2).Should().NotBeNull();

        s.Query<PersonPkUInt64>().Count().Should().Be(2);
        s.DeleteById<PersonPkUInt64>(id1, id2);

        s.GetById<PersonPkUInt64>(id1).Should().NotBeNull();
        s.GetById<PersonPkUInt64>(id2).Should().NotBeNull();
        s.Query<PersonPkUInt64>().Count().Should().Be(2);

        s.SaveChanges();

        s.Query<PersonPkUInt64>().Count().Should().Be(0);

        s.GetById<PersonPkUInt64>(id1).Should().BeNull();
        s.GetById<PersonPkUInt64>(id2).Should().BeNull();
    }

    [Fact]
    public void DeleteWhere_DeletesDocuments()
    {
        using IArgoDocumentSession s = Store.OpenSession();

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();

        s.Query<Person>().Count().Should().Be(_count);
        s.DeleteWhere<Person>(x => x.Id == _person1.Id || x.Id == _person2.Id);

        s.GetById<Person>(_person1.Id).Should().NotBeNull();
        s.GetById<Person>(_person2.Id).Should().NotBeNull();
        s.Query<Person>().Count().Should().Be(_count);

        s.SaveChanges();

        s.Query<Person>().Count().Should().Be(_count - 2);

        s.GetById<Person>(_person1.Id).Should().BeNull();
        s.GetById<Person>(_person2.Id).Should().BeNull();
    }
}