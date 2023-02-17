# Logging

ArgoStore supports `Microsoft.Extensions.Logging` logging.
All Generated SQL queries are logged on `Debug` level without any sensitive data.
Parameter values and connection string are never logger.

## Adding Logging without DI

To add logging to ArgoStore without dependency injection you will need to instantiate a
logging provider, e.g. `Microsoft.Extensions.Logging.Console`.

Add reference to you project to `Microsoft.Extensions.Logging.Console`.

Create `ILoggerFactory`:

```csharp
ILoggerFactory loggerFactory = LoggerFactory.Create(c =>
{
    c.AddConsole();
    c.SetMinimumLevel(LogLevel.Debug);
});
```

In ArgoStore configuration provide `Func<ILoggerFactory>`:

```csharp
IArgoDocumentStore store = new ArgoDocumentStore(c =>
{
    // ...
    c.SetLogger(() => loggerFactory);
    // ...
});
```

## Adding Logging with DI

When using `ArgoStore.Extensions.DependencyInjection` package and calling
`builder.Services.AddArgoStore` logging is automatically set to be used by
ArgoStore from IoC container.

If you don't want ArgoStore to log anything you can either in configuration
set `NullLoggerFactory` or set minimal logging level in `appsettings.json`.

Following will disable logging:
```csharp
builder.Services.AddArgoStore(c =>
{
    // ...
    c.SetLogger(() => NullLoggerFactory.Instance);
    // ...
});
```

Also setting minimal log level to be high, e.g. `Warning`, will disable ArgoStore logging because
nothing is logged with warning, only debug level is used.

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "ArgoStore": "Warning"
  }
}
```

By default when creating new web application, minimal logging level is set to `Information`.
To enable ArgoStore logging, set log level to `Debug` in `appsettings.json`.

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "ArgoStore": "Debug"
  }
}
```

## Log Examples

Example of logs when enabled on insert (using console provider):
```
dbug: ArgoStore.Implementations.ArgoSession[0]
      Started session: TALyprDkOkLR
dbug: ArgoStore.Implementations.ArgoSession[0]
      Method Insert<T> start. SessionId: TALyprDkOkLR, type: AspNetCoreExample.Models.Person
dbug: ArgoStore.Implementations.ArgoSession[0]
      Method SaveChanges start. SessionId: TALyprDkOkLR
dbug: ArgoStore.Implementations.ArgoSession[0]
      Method Dispose start. SessionId: TALyprDkOkLR
```

Each session has it's id which is logged using structured logging. Method names are also logged.

When querying `SQL` command text is also logged.
Following is example from query logs:

```
dbug: ArgoStore.Implementations.ArgoSession[0]
      Started session: WzJk918KGkaM
dbug: ArgoStore.Implementations.ArgoSession[0]
      Method Query<T> start. SessionId: WzJk918KGkaM, type: AspNetCoreExample.Models.Person
dbug: ArgoStore.Command.ArgoCommandExecutor[0]
      Execute command type: ToList in session: WzJk918KGkaM with command text:
      SELECT t1.jsonData
      FROM Person t1
      WHERE t1.tenantId = @p_1_1
       AND  (   json_extract(t1.jsonData, '$.cookiesCount')   =   @p_1_2    )

dbug: ArgoStore.Implementations.ArgoSession[0]
      Method Dispose start. SessionId: WzJk918KGkaM
```