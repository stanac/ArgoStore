using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class CountTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";
        private const string TestNameNonExisting = "Someone who is not in test DB";

        private readonly TestData _td;

        public CountTests()
        {
            _td = new TestData(TestDbConnectionString);

            using IDocumentSession session = GetNewDocumentSession();
            _td.InsertTestPersons();
        }
        
        [Fact]
        public void Count_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            int count = session.Query<Person>().Count();
            count.Should().Be(_td.Persons.Count);
        }

        [Fact]
        public void LongCount_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            long count = session.Query<Person>().LongCount();
            count.Should().Be(_td.Persons.Count);
        }

        [Fact]
        public void CountWithCondition_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            int count = session.Query<Person>().Count(x => x.Name == TestNameImogenCampbell);
            count.Should().Be(1);
        }

        [Fact]
        public void LongCountWithCondition_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            long count = session.Query<Person>().LongCount(x => x.Name == TestNameImogenCampbell);
            count.Should().Be(1);
        }

        [Fact]
        public void CountWithCondition_NonExistingPerson_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            int count = session.Query<Person>().Count(x => x.Name == TestNameNonExisting);
            count.Should().Be(0);
        }

        [Fact]
        public void LongCountWithCondition_NonExistingPerson_ReturnsExpectedCount()
        {
            using IDocumentSession session = GetNewDocumentSession();

            long count = session.Query<Person>().LongCount(x => x.Name == TestNameNonExisting);
            count.Should().Be(0);
        }
    }
}
