namespace ArgoStore.TestsCommon.Entities;

public class PersonPkInt64
{
    public long Id { get; set; }
    public string Name { get; set; }

    public static PersonPkInt64 Create()
    {
        return new PersonPkInt64
        {
            Id = 11,
            Name = "Test"
        };
    }
}