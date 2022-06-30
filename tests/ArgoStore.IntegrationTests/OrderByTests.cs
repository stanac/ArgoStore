using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.TestsCommon.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ArgoStore.IntegrationTests
{
    public class OrderByTests : IntegrationTestsBase
    {
        private readonly TestData _td;

        public OrderByTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);
            
            using IDocumentSession session = GetNewDocumentSession();

            _td.InsertTestPersons();
        }

        [Fact]
        public void OrderBy_ReturnsEntitiesInCorrectOrder()
        {
            using IDocumentSession session = GetNewDocumentSession();

            List<Person> persons = session.Query<Person>()
                .OrderBy(x => x.Name)
                .Where(x => x.EmailAddress != null)
                .ToList();

            int count = persons.Count;
            count.Should().Be(_td.Persons.Count);

            List<Person> orderedPersons = _td.Persons.OrderBy(x => x.Name).ToList();

            persons.First().Should().BeEquivalentTo(orderedPersons.First());
            persons.Last().Should().BeEquivalentTo(orderedPersons.Last());
        }

        [Fact]
        public void OrderByDesc_ReturnsEntitiesInCorrectOrder()
        {
            using IDocumentSession session = GetNewDocumentSession();

            List<Person> persons = session.Query<Person>()
                .OrderByDescending(x => x.Name)
                .Where(x => x.EmailAddress != null)
                .ToList();

            int count = persons.Count;
            count.Should().Be(_td.Persons.Count);

            List<Person> orderedPersons = _td.Persons.OrderByDescending(x => x.Name).ToList();

            persons.First().Should().BeEquivalentTo(orderedPersons.First());
            persons.Last().Should().BeEquivalentTo(orderedPersons.Last());
        }
    }
}
