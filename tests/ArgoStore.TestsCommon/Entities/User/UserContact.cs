using System.Text.Json.Serialization;

namespace ArgoStore.TestsCommon.Entities.User;

[JsonDerivedType(typeof(UserEmailContact))]
[JsonDerivedType(typeof(UserSnailMailContact))]
public abstract class UserContact
{
    public int Weight { get; set; }
}