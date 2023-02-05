# Introduction

## What is ArgoStore?

ArgoStore is `NETSTANDARD2.0` library that is using [SQLite](https://www.sqlite.org)
and [JSON1](https://www.sqlite.org/json1.html) to store and retrieve `JSON` documents.
It supports identity, indexes, nested objects, collections, LINQ queries, etc...

It is inspired by awesome [Marten](https://martendb.io/).

## Getting started

This guide will get you started with using ArgoStore in console applications.
For integration with ASP.NET Core, please see [Getting Started with ASP.NET Core]().
It is recommended to read this guide first.

### Prerequisites

Install [dotnet sdk](https://dot.net) 6, or newer.
ArgoStore works on any framework that is implementing `NETSTANDARD2.0` so you can try using older frameworks if you want. However ArgoStore is only tested with .NET 6 and 7.
ArgoStore is following [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), which means we may not fix issues occurring specifically only on older frameworks.

---

Create new directory

```cmd
mkdir MyConsoleApp
```

and navigate to it

```cmd
cd MyConsoleApp
```

Create project

```cmd
dotnet new console --use-program-main
```

Add ArgoStore nuget dependency.

```cmd
dotnet add package ArgoStore
```

### Document Type and Document Store

Create new file called `Person.cs`.
This class represents type of documents we are going to store in the DB.
(Namespaces removed for brevity.)

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CakesCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
```

`Id` will be used for identity.
It will be automatically populated when not set (when it's `Guid.Empty`).
See [Identity]() for more information.

Open `Program.cs` and add using:

```csharp
using ArgoStore;
```

In `Main` we can now create instance of `ArgoDocumentStore`:

```csharp
const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
store.RegisterDocument<Person>();
```

- It is recommended to use full path for DB file path in connection string.
- To register a document type use `store.RegisterDocument<T>();`
- ArgoStore intentionally does not support operations on non registered documents.

### Inserting data

To insert data we need an instance of `IArgoDocumentSession`.
We can get new session by calling `store.OpenSession()`.

We can insert one or more documents at the same time using `session.Insert()` method.

To apply changes at the end we have to call `session.SaveChanges()` method.

Each session is a transaction. When calling `SaveChanges` transaction
is opened, all operations are processed and transaction is committed.
Each `SaveChanges` call is a single unit of work.

```csharp
using IArgoDocumentSession session = store.OpenSession();

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
```

### Querying data

To query data we can use either `IArgoDocumentSession` or `IArgoQueryDocumentSession`.
`IArgoDocumentSession` is inheriting `IArgoQueryDocumentSession` so query operations 
are available on both, but insert, update, delete and upsert operations are available
only in `IArgoDocumentSession`.

To get new `IArgoQueryDocumentSession` we can call `store.OpenQuerySession()`.
On session we call `Query<T>` to get queryable object on which we can execute
LINQ queries.

```csharp
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
```

After adding code `Program.cs` looks like this:

```csharp
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
```

Calling `dotnet run` you should get the output (you will of course get different ids):

```
215d12d9-a307-41f5-815d-00c0f376df51: Mark Marco
sales:
e2544fde-8ee7-42d7-a7ac-525a288d660f: John Doe
b9758f3a-7f48-4e20-a79d-5249a05fd6b5: Jane Doe
```
