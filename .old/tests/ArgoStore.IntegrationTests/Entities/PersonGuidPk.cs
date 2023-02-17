namespace ArgoStore.IntegrationTests.Entities;

public class PersonGuidPk
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int? BirthYear { get; set; }
    public string EmailAddress { get; set; }
}