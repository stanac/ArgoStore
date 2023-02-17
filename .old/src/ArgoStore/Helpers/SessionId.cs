namespace ArgoStore.Helpers;

internal class SessionId
{
    public string Id { get; }

    public SessionId()
    {
        string id = "";

        do
        {
            Guid g = Guid.NewGuid();
            id = Convert.ToBase64String(g.ToByteArray())
                .Replace("=", "")
                .Replace("/", "")
                .Replace("+", "");

        } while (id.Length < 12);

        Id = id.Substring(0, 12);
    }

    public override string ToString() => Id;
}