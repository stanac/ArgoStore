namespace ArgoStore.TestsCommon.Entities.Person;

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

    public static PersonPkInt32 TestPerson1 =>
        new()
        {
            Id = 1,
            Name = "Test 1"
        };

    public static PersonPkInt32 TestPerson2 =>
        new()
        {
            Id = 2,
            Name = "Test 2"
        };
}