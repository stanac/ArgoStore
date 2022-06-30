using System;
using System.IO;

namespace ArgoStore.Example.Console;

static class Program
{
    static void Main(string[] args)
    {
        string dbFilePath = GetDbFilePath();

        DocumentStore store = new DocumentStore(dbFilePath, createDocumentTablesOnTheFly: false);
        store.CreateTableForEntityIfNotExists<Person>();

        using IDocumentSession session = store.CreateSession();
        
        session.SaveChanges();

        throw new NotImplementedException();
    }

    private static string GetDbFilePath()
    {
        // return "c:\\temp\\test1.sqlite";

        string tempDir = Path.GetTempPath();
        return Path.Combine(tempDir, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.sqlite");
    }
}