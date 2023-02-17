namespace ArgoStore.Statements.Where;

internal class WhereComparisonStatement : WhereStatementBase
{
    public ComparisonOperators Operator { get; }
    public WhereStatementBase Left { get; }
    public WhereStatementBase Right { get; }

    public WhereComparisonStatement(WhereStatementBase left, ComparisonOperators @operator, WhereStatementBase right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
}