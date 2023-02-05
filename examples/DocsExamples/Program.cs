namespace DocsExamples;
using ArgoStore;

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

        Person marco = session.Query<Person>().First(x => x.Name.EndsWith("marco", StringComparison.OrdinalIgnoreCase));
        Console.WriteLine(marco.Name);

        List<Person> sales = session.Query<Person>()
            .Where(x => x.Roles.Contains("sales"))
            .ToList();

        foreach (Person salesPerson in sales)
        {
            Console.WriteLine($"Sales: {salesPerson.Name}");
        }
    }
}
