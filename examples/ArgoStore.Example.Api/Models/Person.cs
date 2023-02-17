namespace ArgoStore.Example.Api.Models;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public int Age { get; set; }
    public int BirthYear { get; set; }
}