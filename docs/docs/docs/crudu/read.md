# Read

Reading can be done using `IArgoQueryDocumentSession` or `IArgoDocumentSession`.
`IArgoQueryDocumentSession` is read-only session, so it's recommended to be used for
fetching data.

## Get By Id

Document can be fetched by id:

```csharp
// store is instance of IArgoDocumentStore
IArgoQueryDocumentSession session = store.OpenQuerySession();
Person? person = session.GetById<Person>(id);
```

or using LINQ.

## Query

Querying data using LINQ can be done by using `Query<T>()` method on 
`IArgoQueryDocumentSession` or `IArgoDocumentSession`.

Example:

```csharp
// store is instance of IArgoDocumentStore
IArgoQueryDocumentSession session = store.OpenQuerySession();

List<Person> myFavPeeps = session.Query<Person>()
    .Where(x => x.CookiesCount > 3)
    .ToList();
```