namespace ArgoStore.Statements.Where;

internal class WhereLogicalAndOrStatement : WhereStatementBase
{
    public bool IsAnd { get; }
    public WhereComparisonStatement Left { get; }
    public WhereComparisonStatement Right { get; }
    public bool IsOr => !IsAnd;

    public WhereLogicalAndOrStatement(bool isAnd, WhereComparisonStatement left, WhereComparisonStatement right)
    {
        IsAnd = isAnd;
        Left = left;
        Right = right;
    }
}