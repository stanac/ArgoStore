using System.Linq.Expressions;
using FluentAssertions;

namespace ArgoStore.Sandbox
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Person p = Person.TestData().First();

            ArgoDocumentStore store = new ArgoDocumentStore("Data Source=c:\\temp\\temp.sqlite");
            store.RegisterDocument<Person>();
            var s = store.OpenSession();
            
            List<Person> items = s.Query<Person>().Where(x => x.Id != Guid.Empty).ToList();

            Assert.NotEmpty(items);
        }

        private static IQueryable<Person> Queryable()
        {
            return new EnumerableQuery<Person>(Person.TestData());
        }
    }

    public class Person
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }

        public static IEnumerable<Person> TestData()
        {
            yield return new Person
            {
                Id = Guid.NewGuid(),
                IsDeleted = false
            };

            yield return new Person
            {
                Id = Guid.NewGuid(),
                IsDeleted = true
            };
        }
    }
}