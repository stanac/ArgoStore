# Multitenancy

Multitenancy is supported out of the box.
Limiting data operations to specific tenant can be done by opening session and providing tenant id as argument.

```csharp
// Store is instance of IArgoDocumentStore or ArgoDocumentStore
Store.OpenSession("myTenant");
```

or 

```csharp
// Store is instance of IArgoDocumentStore or ArgoDocumentStore
Store.OpenQuerySession("myTenant");
```

When opening session without specifying tenant, `DEFAULT` tenant is used.

Calling

```csharp
// Store is instance of IArgoDocumentStore or ArgoDocumentStore
Store.OpenSession();
```

is the same as calling

```csharp
// Store is instance of IArgoDocumentStore or ArgoDocumentStore
Store.OpenSession("DEFAULT");
```

::: tip
Tenant name is case sensitive. `Tenant` is not the same as `tenant`.
:::