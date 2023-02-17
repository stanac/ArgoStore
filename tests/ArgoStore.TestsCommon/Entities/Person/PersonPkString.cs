using System;

namespace ArgoStore.TestsCommon.Entities.Person;

public class PersonPkString
{
    public string Id { get; set; }
    public string Name { get; set; }

    public static PersonPkString Create()
    {
        return new PersonPkString
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test"
        };
    }

    public static PersonPkString TestPerson1 =>
        new()
        {
            Id = "30bb6dd8-b6fe-4600-a0fc-7e7e04c7ecc6",
            Name = "Test 1"
        };

    public static PersonPkString TestPerson2 =>
        new()
        {
            Id = "0ca3b5ab-3d30-40f3-bcfd-3040c7e4e3f7",
            Name = "Test 2"
        };
}