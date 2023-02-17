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
            CookiesCount = 1,
            Roles = new [] {"admin", "sales"}
        });

        session.Insert(
            new Person
            {
                Name = "Jane Doe",
                CookiesCount = 3,
                Roles = new[] { "sales" }
            },
            new Person
            {
                Name = "Mark Marco",
                CookiesCount = 6,
                Roles = new[] { "management" }
            }
        );

        session.SaveChanges();
    }

    private static void InsertExample()
    {
        const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
        ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
        store.RegisterDocument<Person>();

        IArgoDocumentSession session = store.OpenSession();
        session.Insert(new Person
        {
            Name = "Marcus Kowalski"
        });

        session.SaveChanges();
    }

    private static void ReadExample1()
    {
        Guid id = Guid.NewGuid();
        const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
        ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
        store.RegisterDocument<Person>();

        IArgoQueryDocumentSession session = store.OpenQuerySession();
        Person? person = session.GetById<Person>(id);
    }

    private static void ReadExample2()
    {
        const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
        ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
        store.RegisterDocument<Person>();

        IArgoQueryDocumentSession session = store.OpenQuerySession();

        List<Person> persons = session.Query<Person>()
            .Where(x => x.CookiesCount > 3)
            .ToList();
    }

    private void UpdateExample()
    {
        Guid id = Guid.NewGuid();
        const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
        ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
        store.RegisterDocument<Person>();

        IArgoDocumentSession session = store.OpenSession();
        Person? person = session.GetById<Person>(id);

        if (person != null)
        {
            person.CookiesCount--;

            session.Update(person);
            session.SaveChanges();
        }
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
