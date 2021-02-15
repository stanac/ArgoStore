using FluentAssertions;
using ArgoStore.IntegrationTests.Entities;
using System.Linq;
using Xunit;
using System;

namespace ArgoStore.IntegrationTests
{
    public class LinqMethodsTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";

        [Fact]
        public void WhereOnlyTest()
        {
            using (var session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                var persons = session.Query<Person>()
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
            using (var session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                var persons = session.Query<Person>()
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);

                persons.First().Name.Should().Be(TestNameImogenCampbell);
            }
        }

        [Fact]
        public void SelectWholeObjectTest()
        {
            using (var session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                var persons = session.Query<Person>()
                    .Select(x => x)
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);
            }
        }

        [Fact]
        public void SelectOnlySinglePropertyTest()
        {
            using (var session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                var persons = session.Query<Person>()
                    .Select(x => x.Name)
                    .ToList();

                int count = persons.Count;
                count.Should().BeGreaterThan(2);
            }
        }

        [Fact]
        public void SelectOnWhereOnArgoQueryable()
        {
            using (var s = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                var persons = s.Query<Person>()
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
            using (var s = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

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
            using (var s = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                Person person = s.Query<Person>()
                    .Where(x => x.Name == "non existing")
                    .FirstOrDefault();

                person.Should().BeNull();
            }
        }

        [Fact]
        public void NonExistingFirstOnWhere_ThrowsException()
        {
            using (var s = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();

                Action query = () =>
                {
                    Person person = s.Query<Person>()
                        .Where(x => x.Name == "non existing")
                        .First();
                };

                query.Should().Throw<InvalidOperationException>();
            }
        }

        //[Fact]
        //public void FirstOrDefaultOnQueryable()
        //{
        //    using (var s = GetNewDocumentSession())
        //    {
        //        TestData td = new TestData(TestDbConnectionString);
        //        td.InsertTestPersons();

        //        Person person = s.Query<Person>()
        //            .FirstOrDefault();

        //        person.Should().NotBeNull();
        //        person.Name.Should().Be(TestNameImogenCampbell);
        //    }
        //}

        //[Fact]
        //public void FirstOrDefaultOnQueryableWithLambda()
        //{
        //    using (var s = GetNewDocumentSession())
        //    {
        //        TestData td = new TestData(TestDbConnectionString);
        //        td.InsertTestPersons();

        //        Person person = s.Query<Person>()
        //            .FirstOrDefault(x => x.Name == TestNameImogenCampbell);

        //        person.Should().NotBeNull();
        //        person.Name.Should().Be(TestNameImogenCampbell);
        //    }
        //}
    }
}
