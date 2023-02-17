namespace ArgoStore.Statements;

internal class FromAlias
{
    public FromAlias()
    {
        AliasCounter = new IntegerIncrement();
        CurrentAlias = AliasCounter.Increment();
    }

    public IntegerIncrement AliasCounter { get; set; }
    public int CurrentAlias { get; set; }
    public int? ParentAlias { get; set; }
    public string CurrentAliasName => $"t{CurrentAlias}";
    public string ParentAliasName => $"t{ParentAlias}";
    public bool IsTopAlias => CurrentAlias == 1;

    public FromAlias CreateChildAlias()
    {
        FromAlias alias = new FromAlias
        {
            CurrentAlias = AliasCounter.Increment(),
            ParentAlias = CurrentAlias,
            AliasCounter = AliasCounter
        };

        return alias;
    }
}