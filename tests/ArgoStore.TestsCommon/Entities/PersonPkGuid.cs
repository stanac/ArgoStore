using System;

namespace ArgoStore.TestsCommon.Entities;

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
}