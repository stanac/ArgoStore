using System;
using System.IO;
using System.Linq;

namespace ArgoStore.Example.Console;

static class Program
{
    static void Main()
    {
        string dbFilePath = GetDbFilePath();
        string connectionString = $"Data Source={dbFilePath}";

        DocumentStore store = new DocumentStore(connectionString);

        // INSERT
        using IDocumentSession session = store.CreateSession();
        session.Insert(new Person
        {
            Name = "Someone"
        });
        session.SaveChanges();

        // QUERY
        using IQueryDocumentSession querySession = store.CreateReadOnlySession();
        Person person = querySession.Query<Person>().First(x => x.Name.Contains("one"));
        System.Console.WriteLine(person.Name);
    }

    private static string GetDbFilePath()
    {
        // return "c:\\temp\\test1.sqlite";

        string tempDir = Path.GetTempPath();
        return Path.Combine(tempDir, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.sqlite");
    }
}