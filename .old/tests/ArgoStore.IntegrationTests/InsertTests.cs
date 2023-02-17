using ArgoStore.IntegrationTests.Entities;

// ReSharper disable AccessToDisposedClosure

namespace ArgoStore.IntegrationTests;

public class InsertTests : IntegrationTestsBase
{
    public InsertTests(ITestOutputHelper output) : base(output)
    {
            
    }

    [SkippableFact]
    public void InsertEntity_SaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        Person toInsert = new Person
        {
            Name = "Person Name",
            BirthYear = 1990,
            CakeDay = DateTime.UtcNow,
            EmailAddress = "person@example.com",
            EmailConfirmed = true,
            RegistrationTime = DateTimeOffset.UtcNow,
            Roles = new List<string>
            {
                "1", "2", "Role 3"
            }
        };

        s.Insert(toInsert);

        s.SaveChanges();

        toInsert.Id.Should().NotBeEmpty();

        Person gotPerson = s.Query<Person>().First(x => x.Id == toInsert.Id);

        gotPerson.Should().BeEquivalentTo(toInsert);
    }

    [SkippableFact]
    public void InsertEntityStringPk_SaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonStringPk toInsert = new PersonStringPk
        {
            EmailAddress = "asd123@example.com",
            BirthYear = 1954,
            Name = "Some Person"
        };

        s.Insert(toInsert);
        s.SaveChanges();

        toInsert.Id.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(toInsert.Id, out Guid createdGuid).Should().BeTrue();
        createdGuid.Should().NotBeEmpty();

        PersonStringPk retrievedPerson = s.Query<PersonStringPk>().FirstOrDefault(x => x.Id == toInsert.Id);
        retrievedPerson.Should().NotBeNull();
        retrievedPerson.Should().BeEquivalentTo(toInsert);
    }

    [SkippableFact]
    public void InsertEntityIntPk_SaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonIntPk toInsert = new PersonIntPk
        {
            EmailAddress = "asd123@example.com",
            BirthYear = 1954,
            Name = "Some Person"
        };

        s.Insert(toInsert);
        s.SaveChanges();

        toInsert.Id.Should().NotBe(0);

        PersonIntPk retrievedPerson = s.Query<PersonIntPk>().FirstOrDefault(x => x.Id == toInsert.Id);
        retrievedPerson.Should().NotBeNull();
        retrievedPerson.Should().BeEquivalentTo(toInsert);
    }

    [SkippableFact]
    public void InsertEntityLongPkSaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonLongPk toInsert = new PersonLongPk
        {
            EmailAddress = "asd123@example.com",
            BirthYear = 1954,
            Name = "Some Person"
        };

        s.Insert(toInsert);
        s.SaveChanges();

        toInsert.Id.Should().NotBe(0);

        PersonLongPk retrievedPerson = s.Query<PersonLongPk>().FirstOrDefault(x => x.Id == toInsert.Id);
        retrievedPerson.Should().NotBeNull();
        retrievedPerson.Should().BeEquivalentTo(toInsert);
    }

    [SkippableFact]
    public void InsertEntityIntPk_PkSet_SaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonIntPk toInsert = new PersonIntPk
        {
            EmailAddress = "asd123@example.com",
            BirthYear = 1954,
            Name = "Some Person",
            Id = 34
        };

        Action a = () => s.Insert(toInsert);

        a.Should().Throw<InvalidOperationException>();
    }

    [SkippableFact]
    public void InsertEntityLongPk_PkSet_SaveChanges_GetEntity_ReturnsInsertedEntity()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonLongPk toInsert = new PersonLongPk
        {
            EmailAddress = "asd123@example.com",
            BirthYear = 1954,
            Name = "Some Person",
            Id = 34
        };

        Action a = () => s.Insert(toInsert);

        a.Should().Throw<InvalidOperationException>();
    }

    [SkippableFact]
    public void InsertNotSetStringId_SaveChanges_SetsGuidId()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonStringPk person = new PersonStringPk
        {
            Name = Guid.NewGuid().ToString(),
            EmailAddress = "asd",
            BirthYear = 5
        };

        s.Insert(person);
        person.Id.Should().NotBeEmpty();
        s.SaveChanges();

        List<PersonStringPk> persons = s.Query<PersonStringPk>().ToList();
        persons.Should().NotBeEmpty();
        PersonStringPk insertedPerson = persons[0];
        insertedPerson.Id.Should().NotBeEmpty();
        insertedPerson.Id.Should().Be(person.Id);
        insertedPerson.Name.Should().Be(person.Name);
    }

    [SkippableFact]
    public void InsertNotSetGuidId_SaveChanges_SetsGuidId()
    {
        using IDocumentSession s = GetNewDocumentSession();

        PersonGuidPk person = new PersonGuidPk
        {
            Name = Guid.NewGuid().ToString(),
            EmailAddress = "asd",
            BirthYear = 5
        };

        s.Insert(person);
        person.Id.Should().NotBeEmpty();
        s.SaveChanges();

        List<PersonGuidPk> persons = s.Query<PersonGuidPk>().ToList();
        persons.Should().NotBeEmpty();
        PersonGuidPk insertedPerson = persons[0];
        insertedPerson.Id.Should().NotBeEmpty();
        insertedPerson.Id.Should().Be(person.Id);
        insertedPerson.Name.Should().Be(person.Name);
    }

    [SkippableFact]
    public void InsertSetStringId_SaveChanges_DoesNotChangeId()
    {
        using IDocumentSession s = GetNewDocumentSession();

        string id = "this-is-valid-id-" + Guid.NewGuid();

        PersonStringPk person = new PersonStringPk
        {
            Id = id,
            Name = Guid.NewGuid().ToString(),
            EmailAddress = "asd",
            BirthYear = 5
        };

        s.Insert(person);
        person.Id.Should().Be(id);
        s.SaveChanges();

        List<PersonStringPk> persons = s.Query<PersonStringPk>().ToList();
        persons.Should().NotBeEmpty();
        PersonStringPk insertedPerson = persons[0];
        insertedPerson.Id.Should().Be(id);
        insertedPerson.Name.Should().Be(person.Name);
    }

    [SkippableFact]
    public void InsertSetGuidId_SaveChanges_DoesNotChangeId()
    {
        using IDocumentSession s = GetNewDocumentSession();

        Guid id = Guid.NewGuid();

        PersonGuidPk person = new PersonGuidPk
        {
            Id = id,
            Name = Guid.NewGuid().ToString(),
            EmailAddress = "asd",
            BirthYear = 5
        };

        s.Insert(person);
        person.Id.Should().Be(id);
        s.SaveChanges();

        List<PersonGuidPk> persons = s.Query<PersonGuidPk>().ToList();
        persons.Should().NotBeEmpty();
        PersonGuidPk insertedPerson = persons[0];
        insertedPerson.Id.Should().Be(id);
        insertedPerson.Name.Should().Be(person.Name);
    }
}