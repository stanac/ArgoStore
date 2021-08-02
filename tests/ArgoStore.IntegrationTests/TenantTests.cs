using System.Linq;
using ArgoStore.IntegrationTests.Entities;
using FluentAssertions;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class TenantTests : IntegrationTestsBase
    {
        private readonly TestData _td;

        public TenantTests()
        {
            _td = new TestData(TestDbConnectionString);
        }

        [SkippableFact]
        public void InsertInTenant1_SelectFromTenant2_NotFound()
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
            }

            using (IDocumentSession s2 = GetNewDocumentSession(tenantId: "t2"))
            {
                bool exists = s2.Query<Person>().Any(x => x.EmailAddress == p.EmailAddress);
                exists.Should().BeFalse();
            }
        }
    }
}
