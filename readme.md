# ArgoStore

Embedded transactional document store for .NET and .NET Core powered by [Sqlite](https://www.sqlite.org) and [JSON1 extension](https://www.sqlite.org/json1.html).

## What is ArgoStore?
ArgoStore is a `NETSTANDARD2.0` library that is using SQLite and JSON1 to store and retrieve JSON documents.
It supports identity, indexes, nested objects, collections, LINQ queries, etc...

## Is it stable?

Not fully. Even though it's work in progress and some features are missing (optimistic concurrency and async methods)
library interfaces are mostly stable and not expected to change. Focus for future releases is on
stabilizing select expressions, testing where predicates and improving performance.

- Documentation -> [argostore.net](http://argostore.net)
- Nuget -> [ArgoStore](https://www.nuget.org/packages/ArgoStore)

---
## Getting started
---

### Console application (no dependency injection)

Create new project and add `ArgoStore` nuget reference.

```
dotnet add package ArgoStore
```

Define document type

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CookiesCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
```

Register document type

```csharp
const string connectionString = "Data Source=c:\\temp\\mydb.sqlite";
ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
store.RegisterDocument<Person>();
```

Insert data

```csharp
using IArgoDocumentSession session = store.OpenSession();

session.Insert(new Person
{
    Name = "John Doe",
    CookiesCount = 1,
    Roles = new [] {"admin", "sales"}
});

session.SaveChanges();
```

Query data:

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

---

### ASP.NET Core application with default dependency injection

Create new web API application and add nuget dependency:

```
dotnet add package ArgoStore.Extensions.DependencyInjection
``` 

Add connection string to `appsettings.json`

```json
{
  "ConnectionStrings": {
    "db": "Data Source=/path/to/db.sqlite"
  },
  "AllowedHosts": "*"
}
```

Edit Program.cs and register ArgoStore before var app = `builder.Build();` line.

```csharp
string dbConnectionString = builder.Configuration.GetConnectionString("db")
    ?? throw new InvalidOperationException("`ConnectionStrings:db` not set");

builder.Services.AddArgoStore(c =>
{
    c.ConnectionString(dbConnectionString);
    c.RegisterDocument<Person>();
});
```

This will register:
- ArgoDocumentStore as singleton
- IArgoDocumentStore as singleton
- IArgoDocumentSession as scoped
- IArgoQueryDocumentSession as transient

Use same session methods to query and modify documents as in console example.

For more examples see [documentation](http://argostore.net).

