namespace ArgoStore.Statements;

internal class FromAlias
{
    public FromAlias()
    {
        AliasCounter = 1;
        CurrentAlias = 1;
    }

    public int AliasCounter { get; set; }
    public int CurrentAlias { get; set; }
    public int? ParentAlias { get; set; }
    public string CurrentAliasName => $"t{CurrentAlias}";
}