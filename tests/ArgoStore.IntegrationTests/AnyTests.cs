using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ArgoStore.IntegrationTests
{
    public class AnyTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";
        private const string TestNameNonExisting = "Someone who is not in test DB";

        private readonly TestData _td;

        public AnyTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);

            using IDocumentSession session = GetNewDocumentSession();
            _td.InsertTestPersons();
        }

        [Fact]
        public void Any_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any();
            exists.Should().BeTrue();
        }

        [Fact]
        public void Any_EmptyTable_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();
            _td.DeleteTestPersons();

            bool exists = session.Query<Person>().Any();
            exists.Should().BeFalse();
        }

        [Fact]
        public void AnyWithCondition_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any(x => x.Name == TestNameImogenCampbell);
            exists.Should().BeTrue();
        }

        [Fact]
        public void AnyWithCondition_NonExistingPerson_ReturnsExpectedResult()
        {
            using IDocumentSession session = GetNewDocumentSession();

            bool exists = session.Query<Person>().Any(x => x.Name == TestNameNonExisting);
            exists.Should().BeFalse();
        }
    }
}
