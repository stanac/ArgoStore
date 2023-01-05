using System;

namespace ArgoStore.TestsCommon.Entities;

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
}