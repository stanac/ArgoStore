namespace ArgoStore.TestsCommon.Entities;

public class PersonPkUInt64
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    public static PersonPkUInt64 Create()
    {
        return new PersonPkUInt64
        {
            Id = 12,
            Name = "Test"
        };
    }

    public static PersonPkUInt64 TestPerson1 =>
        new()
        {
            Id = 1,
            Name = "Test 1"
        };

    public static PersonPkUInt64 TestPerson2 =>
        new()
        {
            Id = 2,
            Name = "Test 2"
        };
}