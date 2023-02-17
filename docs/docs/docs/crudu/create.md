# Create

Inserting documents can be done using `Insert<T>(params T[] documents)` method in `IArgoDocumentSession`. `IArgoQueryDocumentSession` doesn't have `Insert`.

Example:

```csharp
// store is instance of IArgoDocumentStore
IArgoDocumentSession session = store.OpenSession();
session.Insert(new Person
{
    Name = "Marcus Kowalski"
});

session.SaveChanges();
```

`IArgoDocumentSession` implements unit of work pattern. Changes are applied when `SaveChanges` on session object is called. All changes are applied atomicly in a transaction, so either all changes are applied or none.

To cancel changes use `session.DiscardChanges()`.

::: tip
Please see [Identity](/docs/configuration/identity.md) for information about identity.
:::
