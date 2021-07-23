using System;
using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class UpdateTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";
        private readonly TestData _td;

        public UpdateTests()
        {
            _td = new TestData(TestDbConnectionString);

            using IDocumentSession session = GetNewDocumentSession();
            _td.InsertTestPersons();
        }

        [SkippableFact]
        public void GuidEntity_UpdateEntity_UpdatesEntity()
        {
            using IDocumentSession s = GetNewDocumentSession();

            Person person = s.Query<Person>().Single(x => x.Name == TestNameImogenCampbell);

            person.CackeDay = new DateTime(2020, 04, 03);
            person.EmailAddress = $"a{Guid.NewGuid().ToString("N").ToLower()}@example.com";

            s.Update(person);

            s.SaveChanges();

            Person updated = s.Query<Person>().Single(x => x.Name == TestNameImogenCampbell);
            
            updated.Should().BeEquivalentTo(person);

            Person notUpdated = s.Query<Person>().FirstOrDefault(x => x.Name != TestNameImogenCampbell);
            notUpdated.Should().NotBeNull();
            notUpdated.Should().NotBeEquivalentTo(updated);
        }
    }
}
