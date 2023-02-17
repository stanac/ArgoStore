namespace ArgoStore.Statements.Where;

internal class WhereSubQueryFromStatement : WhereStatementBase
{
    public FromAlias Alias { get; }
    public WhereStatementBase FromStatement { get; }

    public WhereSubQueryFromStatement(WhereStatementBase from, FromAlias alias)
    {
        Alias = alias;
        WhereStatementBase from1 = from;

        while (from1 is WhereSubQueryFromStatement s)
        {
            from1 = s.FromStatement;
        }

        FromStatement = from1;
    }
}