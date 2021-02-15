using ArgoStore.IntegrationTests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArgoStore.IntegrationTests
{
    public class SubQueryTests : IntegrationTestsBase
    {
        private const string TestNameImogenCampbell = "Imogen Campbell";

        public SubQueryTests()
        {
            using (var session = GetNewDocumentSession())
            {
                TestData td = new TestData(TestDbConnectionString);
                td.InsertTestPersons();
            }
        }

        //[Fact]
        //public void Fact1()
        //{
        //    using (IDocumentSession s = GetNewDocumentSession())
        //    {

            
        //    }
        //}
    }
}
