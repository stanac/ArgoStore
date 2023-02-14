namespace ArgoStore.Statements.Where;

internal abstract class WhereSubQueryStatement : WhereStatementBase
{
    public abstract FromAlias Alias { get; }
}

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

internal class WhereSubQueryContainsStatement : WhereSubQueryStatement
{
    public WhereSubQueryContainsStatement(FromAlias alias)
    {
        Alias = alias;
    }

    public override FromAlias Alias { get; }
}