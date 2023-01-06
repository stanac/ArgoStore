namespace ArgoStore.Statements.Select;

internal class FirstSingleMaybeDefaultStatement : SelectStatementBase
{
    public bool IsFirst { get; }
    public bool IsDefault { get; }

    public FirstSingleMaybeDefaultStatement(bool isFirst, bool isDefault)
    {
        IsFirst = isFirst;
        IsDefault = isDefault;
    }
}