namespace ArgoStore.Statements.Where;

internal class WhereSubQueryValueStatement : WhereStatementBase
{
    public string CurrentAlias { get; }
    
    public WhereSubQueryValueStatement(string currentAlias)
    {
        CurrentAlias = currentAlias;
    }
}