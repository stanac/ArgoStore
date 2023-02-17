# Configuration

## Store Configuration

Both `ArgoDocumentStore` constructor and `AddArgoStore` DI extension method
have same set of parameters.
One one overload accepts `string connectionString` and the other `Action<IArgoStoreConfiguration>`.

Providing only `connectionString` is simplest configuration there can be.
When we are using only `connectionString` we have to use `ArgoDocumentStore` (or `IArgoDocumentStore`)
to register document types.

If we are using configure action code may look like this:

```csharp
builder.Services.AddArgoStore(c =>
{
    c.ConnectionString(dbConnectionString);
    c.RegisterDocument<Person>();
});
```

or 

```csharp
IArgoDocumentStore store = new ArgoDocumentStore(c =>
{
    c.ConnectionString(connectionString);
    c.RegisterDocument<Person>();
});
```

Using configuration action is providing instance of `IArgoStoreConfiguration`.
This interface has four methods:
- `void ConnectionString(string)`
- `void SetLogger(Func<ILoggerFactory>)`, covered in [logging page](/docs/configuration/logging)
- `IDocumentConfiguration<TDocument> RegisterDocument<TDocument>()`
- `IDocumentConfiguration<TDocument> RegisterDocument<TDocument>(Action<IDocumentConfiguration<TDocument>> configure)`

## Document Configuration

Document configuration can be set when registering document type.

::: warning
One document type can be registered only once.
Calling register with same document type second time will throw an exception.
:::

Setting configuration for `Person` document type may look like this:

```csharp
builder.Services.AddArgoStore(c =>
{
    c.ConnectionString(dbConnectionString);
    c.RegisterDocument<Person>(p =>
    {
        p.PrimaryKey(x => x.Id);
        p.TableName("Persons");
        p.UniqueIndex(x => x.EmailAddress);
        p.UniqueIndex(x => new {x.Position, x.Region});
        p.NonUniqueIndex(x => x.OfficeNumber);
        p.NonUniqueIndex(x => new { x.ParkingFloor, x.ParkingNumber });
    });
});
```

In the somewhat superficial example above we can see all of the methods available for
configuring document types:
- `PrimaryKey` - sets which property to use for primary key, see [Identity](/docs/configuration/identity) for more information
- `TableName` - sets name of underlying table
- `UniqueIndex` - sets unique index on specified property or properties
- `NonUniqueIndex` - sets non unique index on specified property or properties