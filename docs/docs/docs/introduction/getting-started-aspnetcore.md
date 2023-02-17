# Getting Started with ASP .NET Core

Please, if you didn't already, read [Getting started with console applications](/docs/introduction/getting-started).
Reading it is recommended before reading this page.
It will explain types, interfaces and some methods provided by ArgoStore.

This guide is for integrating ArgoStore into web APIs and general web applications using
ASP .NET Core framework.
In this guide we are going to create new web API and a controller that will be able to create,
update, delete, get, query and upsert DB documents.

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
In this guide we are using `ArgoStore.Extensions.DependencyInjection`
instead of `ArgoStore` package. DI package is referencing `ArgoStore` package
and adds support for dependency injection.
:::

## Create Document model

Add new file `Models/Person.cs`

```csharp
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CookiesCount { get; set; }
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
Whole code for this example is available in [GitHub repo](https://github.com/stanac/ArgoStore/tree/master/examples).
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

In the snipped above, code is getting connection string from configuration JSON file and
registering ArgoStore with it. `Person` is also registered as supported document type.

If you wanted to register another document type outside `Program.cs`
you can call `IArgoDocumentStore.RegisterDocument<T>()` or `ArgoDocumentStore.RegisterDocument<T>()`.

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
- `IArgoDocumentStore.OpenSession()`
- `IArgoDocumentStore.OpenQuerySession()`
- `ArgoDocumentStore.OpenSession()`
- `ArgoDocumentStore.OpenQuerySession()`
:::

## Create Controller

For your convenience Postman collection for this API can be found [here](/downloads/ArgoStoreExample.postman_collection.json).

---

Create new controller in `Controllers/PersonController.cs` file and inject `IArgoDocumentSession`

```csharp
[ApiController]
[Route("/api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IArgoDocumentSession _session;

    public PersonController(IArgoDocumentSession session)
    {
        _session = session;
    }
}
```

## Create POST and GET Actions

Create `POST` action in the controller:

```csharp
[HttpPost]
public IActionResult CreatePerson([FromBody] Person person)
{
    _session.Insert(person);
    _session.SaveChanges();

    return Created($"/api/person/{person.Id}", person);
}
```

Create `GET` action to retrieve person by id:

```csharp
[HttpGet, Route("{id}")]
public IActionResult GetPersonById([FromRoute] Guid id)
{
    Person? person = _session.GetById<Person>(id);
    
    if (person == null) return NotFound();

    return Ok(person);
}
```

We can now test our actions with postman or similar REST tool.
But first run the application:

```cmd
dotnet run
```

Notice the URL used by application, we are going to need it to call our API.

Calling `POST http://localhost:5034/api/person` with following JSON body:

```json
{
    "name": "Tom Doe",
    "cookiesCount": 3,
    "roles": ["admin", "sales"]
}
```

Should give response:

```json
{
    "id": "76a2c0e4-641c-43f1-8cff-59e98195f03d",
    "name": "Tom Doe",
    "cookiesCount": 3,
    "roles": [
        "admin",
        "sales"
    ]
}
```

::: tip
- Port will be different in your application, please check `Properties\launchSettings.json`
- Id of your model will be different
:::

Copy the `id` so we can use it to call `GET` action.

Calling `GET http://localhost:5034/api/person/::id` should return:

```json
{
    "id": "76a2c0e4-641c-43f1-8cff-59e98195f03d",
    "name": "Tom Doe",
    "cookiesCount": 3,
    "roles": [
        "admin",
        "sales"
    ]
}
```

## Create DELETE Action

Stop the application and create following action:

```csharp
[HttpDelete, Route("{id}")]
public IActionResult DeletePersonById([FromRoute] Guid id)
{
    Person? person = _session.GetById<Person>(id);

    if (person == null) return NotFound();

    _session.Delete(person);
    _session.SaveChanges();

    return NoContent();
}
```

::: warning
Delete is permanent, ArgoStore does not support soft delete out of the box (at least for now).
:::

This action when called first time with valid Id will return `204`
and second time `404`.

Alternatively we can call `_session.DeleteById<Person>(id)`:

```csharp
[HttpDelete, Route("{id}")]
public IActionResult DeletePersonById([FromRoute] Guid id)
{
    _session.DeleteById<Person>(id);
    _session.SaveChanges();
    return NoContent();
}
```

In this case we are deleting person if exists, if not nothing will happen.
In both cases `204` is returned.

## Create PUT Update/Upsert Action

ArgoStore supports both update and upsert operations.
Upsert is combination of update and insert where insert is performed if document is not 
found and cannot be updated.

To support both update and upsert in single API endpoint we are going to use
`x-upsert` header, which if set to `true` will indicate to perform upsert.

Following code is implementing upsert or update logic: 

```csharp
[HttpPut, Route("{id}")]
public IActionResult UpdatePerson(
    [FromRoute] Guid id,
    [FromBody] Person person,
    [FromHeader(Name = "x-upsert")] bool upsert)
{
    person.Id = id;

    if (id == default) return BadRequest("Id not set");
    
    if (upsert) return Upsert(person);

    return Update(person);
}

private IActionResult Update(Person person)
{
    throw new NotImplementedException();
}

private IActionResult Upsert(Person person)
{
    throw new NotImplementedException();
}
```

Now we need to implement `Update` and `Upsert` methods.

### Update

For `Update` method we are going to get the person and if not found return `404`.
Otherwise we are going to update document in DB and return `200`.

Replace `Update` method with following:

```csharp
private IActionResult Update(Person person)
{
    Person? dbPerson = _session.GetById<Person>(person.Id);

    if (dbPerson == null) return NotFound();

    _session.Update(person);
    _session.SaveChanges();

    return Ok(person);
}
```

::: tip
- `Update` method will call update in DB using key property from provided object.
- ArgoStore does not track changed objects like Entity Framework
:::

::: warning
If we don't check if person exists and call `Update`, on non existing document, no row will be updated.
`SaveChanges()` will not throw exception it's a simple `SQL` `UPDATE` call in the background.
:::

### Upsert

For upsert we are going to call `Upsert` method and always return `200`.

Replace `Upsert` method with following:

```csharp
private IActionResult Upsert(Person person)
{
    _session.Upsert(person);
    _session.SaveChanges();
    return Ok(person);
}
```

### Testing Update and Upsert


::: info
Before we begin with testing this Action please delete db or delete created documents using API.
:::

Create new Person by calling `POST` with following JSON request body:

```json
{
  "id": "b2ffa3af-4ac5-401e-a98c-fd435e71c6c5",
  "name": "Thomas Doe",
  "cookiesCount": 3,
  "roles": [
      "admin",
      "sales"
  ]
}
```

This time we have provided the `id` and ArgoStore is going to use it as the document key.
It won't generate new random value for the key.

If we call update endpoint `PUT http://localhost:5034/api/person/b2ffa3af-4ac5-401e-a98c-fd435e71c6c5`
with following JSON request body and without any additional headers:

```json
{
  "name": "Thomas Doe",
  "cookiesCount": 3,
  "roles": [
      "admin",
      "sales"
  ]
}
```

We should get response:
```json
{
    "id": "b2ffa3af-4ac5-401e-a98c-fd435e71c6c5",
    "name": "Thomas Doe",
    "cookiesCount": 3,
    "roles": [
        "admin",
        "sales"
    ]
}
```

If we try to update non existing person with new random Guid `f542964d-5679-4e0b-9043-f90e3832d676`
by calling `PUT http://localhost:5034/api/person/f542964d-5679-4e0b-9043-f90e3832d676`
with following JSON request body and without any additional headers:

```json
{
    "name": "Marcus Kovalski",
    "cookiesCount": 7,
    "roles": []
}
```

We should get `404`. However if we provide `x-upsert` header set to `true` we should get following 
`200` response:

```json
{
    "id": "f542964d-5679-4e0b-9043-f90e3832d676",
    "name": "Marcus Kovalski",
    "cookiesCount": 7,
    "roles": []
}
```

## Create GET Query Action

::: info
Before we create Query Action stop the application if running
and delete db file.
:::

Run the application and create some sample
data by calling `POST` action with:

```json
{
    "name": "Tom Doe",
    "cookiesCount": 3,
    "roles": ["admin", "sales"]
}
```

and

```json
{
    "name": "Jane Doe",
    "cookiesCount": 4,
    "roles": ["admin", "management"]
}
```

We are going to use this documents for testing.

---

Empty action method:

```csharp
[HttpGet]
public IActionResult GetPersons(
    [FromQuery] string? name,
    [FromQuery] string? role,
    [FromQuery] int? cookiesCount)
{
    throw new NotImplementedException();
}
```

We want to be able to query persons by name, role and cookiesCount.
In case when name is provided we want to filter by name,
if role is provided we want to query by role, etc...

In order to build query dynamically we need to use `IQueryable<Person>`
which we can get by calling `_session.Query<Person>()`.

So following code will add query filter on `name` if `name` is set.

```csharp
IQueryable<Person> query = _session.Query<Person>();

if (!string.IsNullOrWhiteSpace(name))
{
    query = query.Where(x => x.Name.Contains(name,
        StringComparison.OrdinalIgnoreCase));
}
```

::: warning
`_session.Query<T>()` is returning `IArgoStoreQueryable<T>`. Do not user `var`
in this example. Use explicitly `IQueryable<T>`.
:::

In order to filter on `roles` add:

```csharp
if (!string.IsNullOrWhiteSpace(role))
{
    query = query.Where(x => x.Roles.Contains(role));
}
```

Similarly for `cookiesCount`:

```csharp
if (cookiesCount.HasValue)
{
    query = query.Where(x => x.CookiesCount == cookiesCount);
}
```


::: tip
Query is  executed at the end when needed (when writing HTTP response or calling `ToList`, `First`, ...), no data is filtered in memory.
:::

Full method looks like this:

```csharp
[HttpGet]
public IActionResult GetPersons(
    [FromQuery] string? name,
    [FromQuery] string? role,
    [FromQuery] int? cookiesCount)
{
    IQueryable<Person> query = _session.Query<Person>();

    if (!string.IsNullOrWhiteSpace(name))
    {
        query = query.Where(x => x.Name.Contains(name,
            StringComparison.OrdinalIgnoreCase));
    }

    if (!string.IsNullOrWhiteSpace(role))
    {
        query = query.Where(x => x.Roles.Contains(role));
    }

    if (cookiesCount.HasValue)
    {
        query = query.Where(x => x.CookiesCount == cookiesCount.Value);
    }

    return Ok(query.ToList());
}
```