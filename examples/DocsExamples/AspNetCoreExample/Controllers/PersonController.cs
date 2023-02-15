using ArgoStore;
using AspNetCoreExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreExample.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IArgoDocumentSession _session;

    public PersonController(IArgoDocumentSession session)
    {
        _session = session;
    }

    [HttpPost]
    public IActionResult CreatePerson([FromBody] Person person)
    {
        _session.Insert(person);
        _session.SaveChanges();

        return Created($"/api/person/{person.Id}", person);
    }

    [HttpGet, Route("{id}")]
    public IActionResult GetPersonById([FromRoute] Guid id)
    {
        Person? person = _session.GetById<Person>(id);
        
        if (person == null) return NotFound();

        return Ok(person);
    }

    [HttpDelete, Route("{id}")]
    public IActionResult DeletePersonById([FromRoute] Guid id)
    {
        Person? person = _session.GetById<Person>(id);

        if (person == null) return NotFound();

        _session.Delete(person);
        _session.SaveChanges();

        return NoContent();
    }

    // Alternative implementation for delete
    /*
    [HttpDelete, Route("{id}")]
    public IActionResult DeletePersonByIdAlternative([FromRoute] Guid id)
    {
        _session.DeleteById<Person>(id);
        _session.SaveChanges();
        return NoContent();
    }
    */

    [HttpGet]
    public IActionResult GetPersons(
        [FromQuery] string? name,
        [FromQuery] string? role,
        [FromQuery] int? cakesCount)
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

        if (cakesCount.HasValue)
        {
            query = query.Where(x => x.CookiesCount == cakesCount);
        }

        return Ok(query.ToList());
    }

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
        Person? dbPerson = _session.GetById<Person>(person.Id);

        if (dbPerson == null) return NotFound();

        _session.Update(person);
        _session.SaveChanges();

        return Ok(person);
    }

    private IActionResult Upsert(Person person)
    {
        _session.Upsert(person);
        _session.SaveChanges();
        return Ok(person);
    }


}