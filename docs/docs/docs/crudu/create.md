# Create

Inserting documents can be done using `Insert<T>(params T[] documents)` method in `IArgoDocumentSession`.

Example:

```csharp
ArgoDocumentStore store = new ArgoDocumentStore(connectionString);
store.RegisterDocument<Person>();

IArgoDocumentSession session = store.OpenSession();
session.Insert(new Person
{
    Name = "Marcus Kowalski"
});

session.SaveChanges();
```

`IArgoDocumentSession` is transactional unit of work. Changes are applied when `SaveChanges` on session object is called.

::: tip
Please see [Identity](/docs/configuration/identity.md) for information about identity.
:::