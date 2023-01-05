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
}