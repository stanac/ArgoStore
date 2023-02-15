# Getting Started with ASP .NET Core

Please, if you didn't already, read [Getting started with console application](/docs/introduction/getting-started).

This guide is for integrating ArgoStore into web APIs and general web applications using
ASP.NET Core framework.

## Create Application

Create new directory
```cmd
mkdir AspNetCoreExample
```

Navigate to new directory
```cmd
cd AspNetCoreExample
```

Create application
```cmd
dotnet new webapi --use-program-main --no-https
```

Add ArgoStore nuget package
```cmd
dotnet add package ArgoStore.Extensions.DependencyInjection
```

::: tip
In this case we are using `ArgoStore.Extensions.DependencyInjection`
instead of `ArgoStore` package. DI package is referencing `ArgoStore` package
and has support for dependency injection.
:::

## Create Document model

Add new file `Models/Person.cs`

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CakeCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
```

Property `Id` will be used for identity. To understand more about identity
see [Identity page](/docs/configuration/identity).

## Configure ArgoStore

Edit `appsettings.json` and add connection string

```json
{
  "ConnectionStrings": {
    "db": "Data Source=c:\\temp\\mywebapp.sqlite"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

::: tip
This example code is available in [GitHub repo](https://github.com/stanac/ArgoStore/tree/master/examples)
:::

Edit `Program.cs` and register ArgoStore before `var app = builder.Build();` line.

```csharp
string dbConnectionString = builder.Configuration.GetConnectionString("db")
    ?? throw new InvalidOperationException("`ConnectionStrings:db` not set");

builder.Services.AddArgoStore(c =>
{
    c.ConnectionString(dbConnectionString);
    c.RegisterDocument<Person>();
});
```

In snipped above code is getting connection string from configuration JSON file and
registering ArgoStore with it. `Person` is also registered as supported document type.

If we wanted to register another document type outside `Program.cs`
we can call `IArgoDocumentStore.RegisterDocument<T>()`.

::: warning
- ArgoStore intentionally does not support operations on non registered documents.
:::

Calling `builder.Services.AddArgoStore` will register following types within IoC container:
- `ArgoDocumentStore` as singleton
- `IArgoDocumentStore` as singleton
- `IArgoDocumentSession` as scoped
- `IArgoQueryDocumentSession` as transient

::: tip
Sessions can also be opened by calling one of the following methods or overrides:
-  `IArgoDocumentStore.OpenSession()`
- `IArgoDocumentStore.OpenQuerySession()`
:::

