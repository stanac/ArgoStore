using ArgoStore.Example.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArgoStore.Example.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IDocumentSession _session;

    public PersonsController(IDocumentSession session)
    {
        _session = session;
    }

    [HttpGet]
    public IEnumerable<Person> Get()
    {
        return _session.Query<Person>().ToList();
    }

    [HttpGet]
    public IEnumerable<Person> Find(string name, int minAge)
    {
        return _session.Query<Person>()
            .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase) && x.Age >= minAge)
            .ToList();
    }

    [HttpPost]
    public IActionResult Create([FromBody] Person person)
    {
        _session.Insert(person);
        _session.SaveChanges();

        return Created("/api/person/" + person.Id, person);
    }
    
    [HttpPut, Route("{id}")]
    public IActionResult Update([FromRoute]Guid id, [FromBody] Person person, [FromQuery] bool upsert)
    {
        if (!upsert)
        {
            bool exists = _session.Query<Person>().Any(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }
        }
        
        _session.InsertOrUpdate(person);
        _session.SaveChanges();

        return Ok(person);
    }

    [HttpDelete, Route("{id}")]
    public IActionResult Delete(Guid id)
    {
        bool exists = _session.Query<Person>().Any(x => x.Id == id);

        if (exists)
        {
            Person person = _session.Query<Person>().First(x => x.Id == id);
            _session.Delete(person);

            // OR
            // _session.DeleteWhere<Person>(x => x.Id == id);
            _session.SaveChanges();

            return NoContent();
        }

        return NotFound();
    }
}