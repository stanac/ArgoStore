namespace ArgoStore.Statements.Where;

internal class WhereCollectionContainsStatement : WhereStatementBase
{
    public WhereValueStatement Collection { get; }
    public WhereValueStatement Value { get; }

    public WhereCollectionContainsStatement(WhereValueStatement collection, WhereValueStatement value)
    {
        Collection = collection;
        Value = value;
    }
}