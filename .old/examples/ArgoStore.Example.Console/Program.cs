﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ArgoStore.Example.Console;

static class Program
{
    static void Main()
    {
        string dbFilePath = GetDbFilePath();
        string connectionString = $"Data Source={dbFilePath}";

        using ILoggerFactory loggerFactory = LoggerFactory.Create(c =>
        {
            c.AddConsole();
            c.SetMinimumLevel(LogLevel.Debug);
        });

        IArgoDocumentStore store = new ArgoDocumentStore(c =>
        {
            c.ConnectionString(connectionString);
            
            c.RegisterDocument<Person>(p =>
            {
                p.PrimaryKey(x => x.Id);
                p.NonUniqueIndex(x => x.Name);
                p.UniqueIndex(x => x.EmailAddress);
                p.NonUniqueIndex(x => new {x.EmailAddress, x.Id});
            });

            // ReSharper disable once AccessToDisposedClosure
            c.SetLogger(() => loggerFactory);
        });

        // INSERT
        using IArgoDocumentSession session = store.OpenSession();

        if (!session.Query<Person>().Any())
        {
            session.Insert(new Person
            {
                Id = Guid.NewGuid(), // does not have to be set for Guid Id
                Name = "Someone",
                EmailAddress = "someone@example.com",
                PhoneNumber = "00 00 0000000"
            });
            session.SaveChanges();
        }
        
        // QUERY
        using IArgoQueryDocumentSession querySession = store.OpenQuerySession(); // read-only session
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