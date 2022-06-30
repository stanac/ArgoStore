using ArgoStore.IntegrationTests.Entities;

namespace ArgoStore.IntegrationTests
{
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
        public void InsertEntityStringPk__SaveChanges_GetEntity_ReturnsInsertedEntity()
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
        public void InsertEntityIntPk__SaveChanges_GetEntity_ReturnsInsertedEntity()
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
        public void InsertEntityLongPk__SaveChanges_GetEntity_ReturnsInsertedEntity()
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
    }
}
