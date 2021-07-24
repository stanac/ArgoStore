using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class DeleteTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";
        private readonly TestData _td;

        public DeleteTests()
        {
            _td = new TestData(TestDbConnectionString);

            using IDocumentSession session = GetNewDocumentSession();
            _td.InsertTestPersons();
        }

        [SkippableFact]
        public void Delete_SaveChanges_DeletesEntity()
        {
            using IDocumentSession session = GetNewDocumentSession();

            int countBefore = session.Query<Person>().Count();

            Person p = session.Query<Person>().FirstOrDefault(x => x.Name == TestNameImogenCampbell);
            p.Should().NotBeNull();

            session.Delete(p);
            session.SaveChanges();

            p = session.Query<Person>().FirstOrDefault(x => x.Name == TestNameImogenCampbell);
            p.Should().BeNull();

            int countAfter = session.Query<Person>().Count();
            countAfter.Should().Be(countBefore - 1);
        }
    }
}
