namespace ArgoStore.Statements.Where;

internal class WhereSubQueryCountStatement : WhereSubQueryStatement
{
    public WhereSubQueryFromStatement From { get; }
    public WhereStatementBase? Condition { get; }
    public override FromAlias Alias { get; }

    public WhereSubQueryCountStatement(WhereSubQueryFromStatement from, WhereStatementBase? condition, FromAlias alias)
    {
        From = from;
        Condition = condition;
        Alias = alias;
    }

}