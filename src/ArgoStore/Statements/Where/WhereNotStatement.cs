namespace ArgoStore.Statements.Where;

internal class WhereNotStatement : WhereStatementBase
{
    public WhereStatementBase Statement { get; }

    public WhereNotStatement(WhereStatementBase statement)
    {
        Statement = statement;
    }
}