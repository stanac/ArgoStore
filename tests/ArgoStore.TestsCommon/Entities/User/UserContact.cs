using System.Text.Json.Serialization;

namespace ArgoStore.TestsCommon.Entities.User;

[JsonDerivedType(typeof(UserEmailContact), typeDiscriminator: "email")]
[JsonDerivedType(typeof(UserSnailMailContact), typeDiscriminator: "snailMail")]
[JsonPolymorphic]
public abstract class UserContact
{
    public int Weight { get; set; }
}