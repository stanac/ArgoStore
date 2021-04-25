using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class WhereTests : IntegrationTestsBase
    {
        public WhereTests()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();
            }
        }

        [SkippableFact]
        public void WhereWithNotEqualNull_ReturnsCorrectEntities()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<Person> persons = session.Query<Person>()
                    .Where(x => x.Name != null)
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(1);
            }

        }
    }
}
