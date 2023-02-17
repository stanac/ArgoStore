namespace ArgoStore.UnitTests;

public class TestEntityPerson
{
    public string FieldStr = "str";
    public string Key { get; set; }
    public string Name { get; set; }
    public string NameLower => Name?.ToLower();
    public string EmailAddress { get; set; }
    public string EmailProvider { get; private set; }
    public int BirthYear { get; set; }
    public bool Active { get; set; }
    public int? ActiveDuration { get; set; }

    public void SetEmailProvider(string s)
    {
        EmailProvider = s;
    }
}