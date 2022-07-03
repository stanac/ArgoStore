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

    [HttpGet(Name = "GetAllPersons")]
    public IEnumerable<Person> GetAll(string? name, int? minAge)
    {
        // if name is not null check for Person.Name.Contains(name) otherwise don't check
        // if minAge hasValue check for Person.Age >= minAge, otherwise don't check age

        return _session.Query<Person>()
            .Where(x => (name == null || x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                        && (!minAge.HasValue || x.Age >= minAge)
            )
            .ToList();
    }

    [HttpGet("{id:guid}", Name = "GetPersonById")]
    public IActionResult GetById([FromRoute]Guid id)
    {
        Person? person = _session.Query<Person>().FirstOrDefault(x => x.Id == id);

        if (person == null)
        {
            return NotFound();
        }

        return Ok(person);
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