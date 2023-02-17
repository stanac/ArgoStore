# Underlying Tables

For each document type a table is created with the name of the class.
This can be problematic when multiple classes with same name (and different namespaces)
are used for document types.

As an example if document type full name is `MyApp.Models.Person` table will
be created with name `Person`. This behavior can be overridden when registering 
document type.

Example of overriding table name:


```csharp
builder.Services.AddArgoStore(c =>
{
    c.ConnectionString(dbConnectionString);
    c.RegisterDocument<Person>(p =>
    {
        p.TableName("Users");
    });
});
```

Please see [Configuration](/docs/configuration/configuration) for more information.
