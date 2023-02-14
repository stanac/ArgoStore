namespace ArgoStore.Statements.Where;

internal class WhereSubQueryContainsStatement : WhereSubQueryStatement
{
    public WhereSubQueryContainsStatement(FromAlias alias)
    {
        Alias = alias;
    }

    public override FromAlias Alias { get; }
}