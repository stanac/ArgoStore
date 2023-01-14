namespace ArgoStore.TestsCommon.Entities;

public class PersonPkUInt32
{
    public uint Id { get; set; }
    public string Name { get; set; }
    
    public static PersonPkUInt32 Create()
    {
        return new PersonPkUInt32
        {
            Id = 13,
            Name = "Test"
        };
    }

    public static PersonPkUInt32 TestPerson1 =>
        new()
        {
            Id = 1,
            Name = "Test 1"
        };

    public static PersonPkUInt32 TestPerson2 =>
        new()
        {
            Id = 2,
            Name = "Test 2"
        };
}