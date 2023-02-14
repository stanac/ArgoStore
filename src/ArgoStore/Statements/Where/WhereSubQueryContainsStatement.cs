namespace ArgoStore.Statements.Where;

internal class WhereSubQueryContainsStatement : WhereSubQueryStatement
{
    public override FromAlias Alias { get; }
    public WhereSubQueryFromStatement From { get; }
    public WhereValueStatement Value { get; }

    public WhereSubQueryContainsStatement(FromAlias alias, WhereSubQueryFromStatement from, WhereValueStatement value)
    {
        Alias = alias;
        From = from;
        Value = value;
    }
}