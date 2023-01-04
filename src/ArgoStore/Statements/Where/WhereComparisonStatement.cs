namespace ArgoStore.Statements.Where;

internal class WhereComparisonStatement : WhereStatementBase
{
    public ComparisonOperators Operator { get; }
    public WhereValueStatement Left { get; }
    public WhereValueStatement Right { get; }

    public WhereComparisonStatement(WhereValueStatement left, ComparisonOperators @operator, WhereValueStatement right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
}