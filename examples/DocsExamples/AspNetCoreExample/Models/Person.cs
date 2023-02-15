using ArgoStore;

namespace AspNetCoreExample.Models;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int CakeCount { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}