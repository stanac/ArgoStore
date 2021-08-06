using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ArgoStore.IntegrationTests
{
    public class TenantTests : IntegrationTestsBase
    {
        private readonly TestData _td;

        public TenantTests(ITestOutputHelper output) : base(output)
        {
            _td = new TestData(TestDbConnectionString);
        }

        [SkippableFact]
        public void InsertInTenant1_SelectAnyFromTenant2_AnyShouldReturnFalse()
        {
            Person p = _td.Persons[0];

            using (IDocumentSession s1 = GetNewDocumentSession(tenantId: "t1"))
            {
                s1.Insert(p);
                s1.SaveChanges();
            }

            using (IDocumentSession s1 = GetNewDocumentSession(tenantId: "t1"))
            {
                bool exists = s1.Query<Person>().Any(x => x.EmailAddress == p.EmailAddress);
                exists.Should().BeTrue();

                exists = s1.Query<Person>().Any();
                exists.Should().BeTrue();
            }

            using (IDocumentSession s2 = GetNewDocumentSession(tenantId: "t2"))
            {
                bool exists = s2.Query<Person>().Any(x => x.EmailAddress == p.EmailAddress);
                exists.Should().BeFalse();

                exists = s2.Query<Person>().Any();
                exists.Should().BeFalse();
            }
        }

        [SkippableFact]
        public void InsertInTenant1_SelectCountFromTenant2_CountShouldReturnZero()
        {
            Person p = _td.Persons[0];
            
            using (IDocumentSession s1 = GetNewDocumentSession(tenantId: "t1"))
            {
                s1.Insert(p);
                s1.SaveChanges();
            }

            using (IDocumentSession s1 = GetNewDocumentSession(tenantId: "t1"))
            {
                int count =  s1.Query<Person>().Count(x => x.EmailAddress == p.EmailAddress);
                count.Should().Be(1);
                
                count = s1.Query<Person>().Count();
                count.Should().Be(1);
            }

            using (IDocumentSession s2 = GetNewDocumentSession(tenantId: "t2"))
            {
                int count = s2.Query<Person>().Count(x => x.EmailAddress == p.EmailAddress);
                count.Should().Be(0);
                
                count = s2.Query<Person>().Count();
                count.Should().Be(0);
            }
        }
        
    }
}
