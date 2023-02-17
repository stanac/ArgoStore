# Indexing

When registering document types we can defined unique or non unique indexes.

Example:

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

Also see [Configuration](/docs/configuration/configuration) for more information.