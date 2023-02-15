namespace DocsExamples;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CakesCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}
