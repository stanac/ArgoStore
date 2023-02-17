# Multitenancy

Multitenancy is supported out of the box.
Limiting data operations to specific tenant can be done by opening session and providing tenant id as argument.

::: info
In every code snippet on this page `Store` is instance of `IArgoDocumentStore` or `ArgoDocumentStore`.
:::

## Opening Tenant Session

Use

```csharp
Store.OpenSession("myTenant");
```

or 

```csharp
Store.OpenQuerySession("myTenant");
```
to open session for given tenant.

## Default Tenant Session

When opening session without specifying tenant, `DEFAULT` tenant is used.

Calling

```csharp
Store.OpenSession();
```

is the same as calling

```csharp
Store.OpenSession("DEFAULT");
```

::: tip
Tenant name is case sensitive. Value `Tenant` is different from `tenant`.
:::

## Listing Tenants

Listing tenants can be done by calling:

```csharp
Store.ListTenants<T>();
```
