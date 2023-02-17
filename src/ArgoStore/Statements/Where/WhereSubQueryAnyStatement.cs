namespace ArgoStore.Statements.Where;

internal class WhereSubQueryAnyStatement : WhereSubQueryStatement
{
    public WhereSubQueryFromStatement From { get; }
    public WhereStatementBase? Condition { get; }
    public override FromAlias Alias { get; }
    
    public WhereSubQueryAnyStatement(WhereSubQueryFromStatement from, WhereStatementBase? condition, FromAlias alias)
    {
        From = from;
        Condition = condition;
        Alias = alias;
    }

}