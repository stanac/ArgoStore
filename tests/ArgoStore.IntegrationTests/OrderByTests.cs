using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class OrderByTests : IntegrationTestsBase
    {
        public OrderByTests()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();
            }
        }

        [Fact]
        public void Test()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<Person> persons = session.Query<Person>()
                    .OrderByDescending(x => x.Name)
                    .Where(x => x.BirthYear.HasValue)
                    .ToList();

                int count = persons.Count;
                count.Should().Be(1);

                persons.First().Name.Should().Be("1");
            }
        }
    }
}
