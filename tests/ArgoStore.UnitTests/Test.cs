using System.Linq;
using ArgoStore.TestsCommon.Entities;
using Xunit;

namespace ArgoStore.UnitTests;

public class Test
{
    [Fact]
    public void Test1()
    {
        ArgoDocumentStore store = new ArgoDocumentStore(@"Data Source=c:\temp\test.db;");

        IArgoQueryDocumentSession session = store.CreateQuerySession();

        List<Person> res = session.Query<Person>().Where(x => x.BirthYear.HasValue).ToList();
    }
}