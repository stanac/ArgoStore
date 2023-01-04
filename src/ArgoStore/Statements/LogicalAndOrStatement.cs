namespace ArgoStore.Statements;

internal class LogicalAndOrStatement
{
    public bool IsAnd { get; }
    public bool IsOr => !IsAnd;

    public LogicalAndOrStatement(bool isAnd, WhereComparisonStatement left, WhereComparisonStatement right)
    {
        IsAnd = isAnd;
    }
}