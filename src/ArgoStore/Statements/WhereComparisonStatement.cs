namespace ArgoStore.Statements;

internal class WhereComparisonStatement
{
    public ComparisonOperators Operator { get; }
    public ValueStatement Left { get; }
    public ValueStatement Right { get; }

    public WhereComparisonStatement(ValueStatement left, ComparisonOperators @operator, ValueStatement right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
}