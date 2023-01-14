namespace ArgoStore.TestsCommon.Entities;

public class PersonPkInt64
{
    public long Id { get; set; }
    public string Name { get; set; }

    public static PersonPkInt64 Create()
    {
        return new PersonPkInt64
        {
            Id = 15,
            Name = "Test"
        };
    }

    public static PersonPkInt64 TestPerson1 =>
        new()
        {
            Id = 1,
            Name = "Test 1"
        };

    public static PersonPkInt64 TestPerson2 =>
        new()
        {
            Id = 2,
            Name = "Test 2"
        };
}