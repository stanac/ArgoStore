namespace ArgoStore.IntegrationTests.Entities;

public class PersonLongPk
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int? BirthYear { get; set; }
    public string EmailAddress { get; set; }
}