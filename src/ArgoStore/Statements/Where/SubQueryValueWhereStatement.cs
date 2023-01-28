namespace ArgoStore.Statements.Where;

internal class SubQueryValueWhereStatement : WhereStatementBase
{
    public string CurrentAlias { get; }

    public SubQueryValueWhereStatement(string currentAlias)
    {
        CurrentAlias = currentAlias;
    }
}