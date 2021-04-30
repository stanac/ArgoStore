using FluentAssertions;
using ArgoStore.IntegrationTests.Entities;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
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

        [SkippableFact]
        public void SelectOnQueryableNewAnonymousObject()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                var p = s.Query<Person>()
                    .Select(x => new { x.Name, x.CackeDay })
                    .ToList();

                p.Count.Should().BeGreaterThan(2);
                p.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue();
            }
        }

        [SkippableFact]
        public void SelectOnWhereNewAnonymousObject()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                var p = s.Query<Person>()
                    .Where(x => x.Name != "non existing")
                    .Select(x => new { x.Name, x.CackeDay })
                    .ToList();

                p.Count.Should().BeGreaterThan(2);
                p.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue();
            }
        }

        [SkippableFact]
        public void SelectOnQueryableNewObject()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                var p = s.Query<Person>()
                    .Select(x => new Person { Name = x.Name, CackeDay = x.CackeDay })
                    .ToList();

                p.Count.Should().BeGreaterThan(2);
                p.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue();
            }
        }

        [SkippableFact]
        public void SelectOnQueryableNewObjectReBindDifferentProperties()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                var p = s.Query<Person>()
                    .Select(x => new Person { Name = x.EmailAddress, EmailAddress = x.Name })
                    .ToList();

                p.Count.Should().BeGreaterThan(2);
                p.All(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name.Contains("@")).Should().BeTrue();
                p.All(x => !string.IsNullOrWhiteSpace(x.EmailAddress) && !x.EmailAddress.Contains("@")).Should().BeTrue();
            }
        }

        [SkippableFact]
        public void Last_ThrowsNotSupportedException()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Action a = () => s.Query<Person>().Last();

                a.Should().Throw<NotSupportedException>();
            }
        }

        [SkippableFact]
        public void LastOrDefault_ThrowsNotSupportedException()
        {
            using (IDocumentSession s = GetNewDocumentSession())
            {
                Action a = () => s.Query<Person>().LastOrDefault();

                a.Should().Throw<NotSupportedException>();
            }
        }
    }
}
