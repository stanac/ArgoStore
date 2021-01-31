using JsonDbLite.IntegrationTests.Entities;
using System;
using System.Linq;
using Xunit;

namespace JsonDbLite.IntegrationTests
{
    public class UnitTest1 : IntegrationTestsBase
    {
        [Fact]
        public void Test1()
        {
            using (var session = GetNewDocumentSession())
            {
                var persons = session.Query<Person>()
                    .Where(x => x.Name == "1")
                    .ToList();

                int count = persons.Count;
            }
        }
    }
}
