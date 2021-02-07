using FluentAssertions;
using ArgoStore.IntegrationTests.Entities;
using System.Linq;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class UnitTest1 : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";

        [Fact]
        public void Test1()
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

        //[Fact]
        //public void Test2()
        //{
        //    using (var s = GetNewDocumentSession())
        //    {
        //        TestData td = new TestData(TestDbConnectionString);
        //        td.InsertTestPersons();

        //        var persons = s.Query<Person>()
        //            .Where(x => x.Name == TestNameImogenCampbell)
        //            .Select(x => x.Name)
        //            .ToList();

        //        int count = persons.Count;
        //        count.Should().Be(1);

        //        persons.First().Should().Be(TestNameImogenCampbell);
        //    }
        //}
    }
}
