# Identity

## Default Behavior

Each registered document has to have a key property.
By default ArgoStore will search for following property name:
- `Id`
- `ID`
- `Key`
- `documentType + Id`
- `documentType + ID`
- `documentType + Key`

If document type is `Person` ArgoStore will check for following properties:
- `Id`
- `ID`
- `Key`
- `PersonId`
- `PersonID`
- `PersonKey`

If multiple matched properties are found, exception will be thrown.
If no matched property is found, exception will be thrown.

::: warning
Key property must be of type `System.Guid` or `System.String`, integer types are not supported.
:::

## Overriding Default Behavior

Default behavior can be overridden when registering document type, e.g.:

```csharp
Store.RegisterDocument<Person>(c =>
{
    c.PrimaryKey(p => p.EmailAddress);
});
```

Primary key must be `String` or `Guid` in this case as well.

::: warning
Composite keys are not supported.
:::

Following code will throw exception:

```csharp
Store.RegisterDocument<Person>(c =>
{
    c.PrimaryKey(p => new { p.EmailAddress, p.CookiesCount });
});
```

## Insert Behavior

When key property is of type `String`, key property must be set, otherwise exception will be thrown.

When key property is of type `Guid`, key property can be set, but it is not required.
If `Guid` key property is not set (has `default` value), 
it will be set to random value on the document object before it's inserted.

Nullable Guid properties, i.e. `Nullable<Guid>` or `Guid?`, are not supported for key properties.

::: info
In the background ArgoStore tables are using incremental integer value for clustering.
Randomness of key value does not affect table data ordering, only ordering
of unique index used for primary key.

Please check [Underlying tables page](/docs/configuration/underlying-tables) for more information.
:::