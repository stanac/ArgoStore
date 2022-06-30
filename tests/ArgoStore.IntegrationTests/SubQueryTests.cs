using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using ArgoStore.TestsCommon.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ArgoStore.IntegrationTests
{
    public class SubQueryTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";

        public SubQueryTests(ITestOutputHelper output) : base(output)
        {
            using var session = GetNewDocumentSession();

            TestData td = new TestData(TestDbConnectionString);
            td.InsertTestPersons();
        }
        
        [SkippableFact]
        public void SelectSinglePropertyFromSubQuery()
        {
            using IDocumentSession s = GetNewDocumentSession();

            List<string> p = s.Query<Person>()
                .Where(x => x.Name != "a")
                .Select(x => new { x.EmailAddress, x.Name, x.EmailConfirmed })
                .Select(x => x.Name)
                .ToList();

            p.Count.Should().BeGreaterThan(2);
            p.All(x => !string.IsNullOrWhiteSpace(x)).Should().BeTrue();
            p.Count(x => x == TestNameImogenCampbell).Should().Be(1);
        }

        //[SkippableFact]
        //public void Fact2()
        //{
        //    using (IDocumentSession s = GetNewDocumentSession())
        //    {
        //        List<string> p = s.Query<Person>()
        //            .Where(x => x.Name != "a")
        //            .Select(x => new { Joined = x.Name + x.EmailAddress, x.EmailConfirmed })
        //            .Select(x => x.Joined)
        //            .ToList();

        //        p.Count.Should().BeGreaterThan(2);
        //        p.All(x => !string.IsNullOrWhiteSpace(x)).Should().BeTrue();
        //    }
        //}
    }
}
