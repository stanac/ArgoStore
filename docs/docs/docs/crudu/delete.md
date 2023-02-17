# Delete

Deleting document call can be done by calling `Delete` or `DeleteById` methods on `IArgoDocumentSession`.
After that `SaveChanges` needs to be called in order for pending operation
to take effect.

Following examples are from [ASP.NET Core integration](/docs/introduction/getting-started-aspnetcore).

## Deleting by Id

```csharp
[HttpDelete, Route("{id}")]
public IActionResult DeletePerson([FromRoute] Guid id)
{
    // _session is IArgoDocumentSession
    Person? person = _session.GetById<Person>(id);

    if (person == null) return NotFound();

    _session.Delete(person);
    _session.SaveChanges();

    return NoContent();
}
```

## Deleting

```csharp
[HttpDelete, Route("{id}")]
public IActionResult DeletePersonById([FromRoute] Guid id)
{
    // _session is IArgoDocumentSession
    _session.DeleteById<Person>(id);
    _session.SaveChanges();
    return NoContent();
}
```
---

Both `Delete` and `DeleteById` methods accept `params` array as arguments so multiple
documents can be deleted with one call.