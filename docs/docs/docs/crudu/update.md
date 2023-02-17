# Update

::: tip
Don't forget that ArgoStore supports [Upsert](/docs/crudu/upsert).
:::

Following example shows how to get document by id,
change it, and update it in DB.

```csharp
// store is instance of IArgoDocumentStore
IArgoDocumentSession session = store.OpenSession();
Person? person = session.GetById<Person>(id);

if (person != null)
{
    person.CookiesCount--;

    session.Update(person);
    session.SaveChanges();
}
```

Calling `Update` will set document to be updated. Document id must be set.
`SaveChanges` will execute all pending operations in a transaction.
In this case the only pending operation is `Update`.

::: tip
Unlike Entity Framework Core, ArgoStore does not track changed objects.
You have to explicitly call `Update` with modified object.
:::