using FluentAssertions;
using ArgoStore.IntegrationTests.Entities;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;

namespace ArgoStore.IntegrationTests
{
    public class LinqMethodsTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";

        public LinqMethodsTests()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();
            }
        }

        [Fact]
        public void WhereOnlyTest()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<Person> persons = session.Query<Person>()
                    .Where(x => x.Name == TestNameImogenCampbell)
                    .ToList();

                int count = persons.Count;
                count.Should().Be(1);

                persons.First().Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void ToListOnlyTest()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<Person> persons = session.Query<Person>()
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);

                persons.First().Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void SelectWholeObjectTest()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<Person> persons = session.Query<Person>()
                    .Select(x => x)
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);
            }
        }

        [Fact]
        public void SelectOnlySinglePropertyTest()
        {
            using (IDocumentSession session = GetNewDocumentSession())
            {
                List<string> persons = session.Query<Person>()
                    .Select(x => x.Name)
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);
            }
        }

        [Fact]
        public void SelectOnWhereOnArgoQueryable()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                List<string> persons = s.Query<Person>()
                    .Where(x => x.Name == TestNameImogenCampbell)
                    .Select(x => x.Name)
                    .ToList();

                int count = persons.Count;
                count.Should().Be(1);

                persons.First().Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void FirstOrDefaultOnWhere()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Person person = s.Query<Person>()
                    .Where(x => x.Name == TestNameImogenCampbell)
                    .FirstOrDefault();

                person.Should().NotBeNull();
                person.Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void NonExistingFirstOrDefaultOnWhere()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Person person = s.Query<Person>()
                    .Where(x => x.Name == "non existing")
                    .FirstOrDefault();

                person.Should().BeNull();
            }
        }

        [Fact]
        public void NonExistingFirstOnWhere_ThrowsException()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Action query = () =>
                {
                    Person person = s.Query<Person>()
                        .Where(x => x.Name == "non existing")
                        .First();
                };

                query.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void FirstOrDefaultOnQueryable()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Person person = s.Query<Person>()
                    .FirstOrDefault();

                person.Should().NotBeNull();
                person.Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void FirstOrDefaultWithLambdaOnQueryable()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Person person = s.Query<Person>()
                    .FirstOrDefault(x => x.Name == TestNameImogenCampbell);

                person.Should().NotBeNull();
                person.Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void FirstOrDefaultWithLambdaOnWhere()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Person person = s.Query<Person>()
                    .Where(x => x.Name != "a")
                    .FirstOrDefault(x => x.Name == TestNameImogenCampbell);

                person.Should().NotBeNull();
                person.Name.Should().Be(TestNameImogenCampbell);

                person = s.Query<Person>()
                    .Where(x => x.Name != TestNameImogenCampbell)
                    .FirstOrDefault(x => x.Name == TestNameImogenCampbell);

                person.Should().BeNull();
            }
        }

        [Fact]
        public void SelectOnQueryableNewAnonymousObject()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                var p = s.Query<Person>()
                    .Select(x => new { x.Name, x.CackeDay })
                    .ToList();

                p.Count.Should().BeGreaterThan(2);
            }
        }
    }
}
