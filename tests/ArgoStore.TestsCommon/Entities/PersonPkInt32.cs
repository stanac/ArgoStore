namespace ArgoStore.TestsCommon.Entities;

public class PersonPkInt32
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static PersonPkInt32 Create()
    {
        return new PersonPkInt32
        {
            Id = 14,
            Name = "Test"
        };
    }
}