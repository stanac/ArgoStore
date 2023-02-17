# Upsert

Upsert is combination of Update and Insert. If document in DB is matched by key it will
be updated with new value, if not new document will be inserted.

Example:

```csharp
void Upsert(Person p)
{
    // _store is instance of IArgoDocumentStore
    IArgoDocumentSession session = _store.OpenSession();
    session.Upsert(person);
    session.SaveChanges();
}
```

::: warning
When calling upsert make sure key of the document is set.
Check [Identity](/docs/configuration/identity) for information about identity.
:::