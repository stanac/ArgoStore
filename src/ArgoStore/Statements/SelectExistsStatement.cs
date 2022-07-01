namespace ArgoStore.Statements;

internal class SelectExistsStatement : Statement
{
    public SelectExistsStatement(Type fromType)
    {
        FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
    }

    public SelectExistsStatement(Type fromType, WhereStatement where)
    {
        FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
        Where = where ?? throw new ArgumentNullException(nameof(where));
    }

    public SelectExistsStatement(Type fromType, SelectStatement subQuery)
    {
        FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
        SubQuery = subQuery ?? throw new ArgumentNullException(nameof(subQuery));
    }

    public Type FromType { get; }
    public WhereStatement Where { get; }
    public SelectStatement SubQuery { get; }

    public override Statement Negate() => throw new NotSupportedException();

    public override Statement ReduceIfPossible() => this;

    public override string ToDebugString() => "SELECT EXISTS()";
}