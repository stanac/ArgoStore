namespace ArgoStore.TestsCommon.Entities;

public class PersonPkUInt64
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public static PersonPkUInt64 Create()
    {
        return new PersonPkUInt64
        {
            Id = 11,
            Name = "Test"
        };
    }
}