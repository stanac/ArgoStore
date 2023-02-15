using ArgoStore;

namespace ConsoleExamples;

class Program
{
    private static void Main()
    {
        const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
        ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
        store.RegisterDocument<Person>();
        
        Insert(store);
        Query(store);
    }

    private static void Insert(ArgoDocumentStore store)
    {
        using IArgoDocumentSession session = store.OpenSession();

        if (session.Query<Person>().Any())
        {
            return;
        }

        session.Insert(new Person
        {
            Name = "John Doe",
            CakesCount = 1,
            Roles = new [] {"admin", "sales"}
        });

        session.Insert(
            new Person
            {
                Name = "Jane Doe",
                CakesCount = 3,
                Roles = new[] { "sales" }
            },
            new Person
            {
                Name = "Mark Marco",
                CakesCount = 6,
                Roles = new[] { "management" }
            }
        );

        session.SaveChanges();
    }

    private static void Query(ArgoDocumentStore store)
    {
        using IArgoQueryDocumentSession session = store.OpenQuerySession();

        Person marco = session.Query<Person>()
            .First(x => x.Name.EndsWith("marco", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"{marco.Id}: {marco.Name}");

        List<Person> sales = session.Query<Person>()
            .Where(x => x.Roles.Contains("sales"))
            .ToList();

        Console.WriteLine("sales:");
        foreach (Person salesPerson in sales)
        {
            Console.WriteLine($"{salesPerson.Id}: {salesPerson.Name}");
        }
    }
}
