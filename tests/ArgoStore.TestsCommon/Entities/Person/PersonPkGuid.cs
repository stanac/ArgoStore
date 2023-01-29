using System;

namespace ArgoStore.TestsCommon.Entities.Person;

public class PersonPkGuid
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public static PersonPkGuid Create()
    {
        return new PersonPkGuid
        {
            Id = Guid.NewGuid(),
            Name = "Test"
        };
    }

    public static PersonPkGuid TestPerson1 =>
        new()
        {
            Id = Guid.Parse("d9a6d4a1-085f-4fa4-935a-af3d2a78e1a6"),
            Name = "Test 1"
        };

    public static PersonPkGuid TestPerson2 =>
        new()
        {
            Id = Guid.Parse("352bfb2b-5193-4650-b6a8-c792863b24aa"),
            Name = "Test 2"
        };
}