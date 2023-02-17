namespace ConsoleExamples;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CookiesCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
