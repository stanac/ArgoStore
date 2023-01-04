namespace ArgoStore.Statements.Where;

internal class WhereLogicalAndOrStatement : WhereStatementBase
{
    public bool IsAnd { get; }
    public bool IsOr => !IsAnd;

    public WhereLogicalAndOrStatement(bool isAnd, WhereComparisonStatement left, WhereComparisonStatement right)
    {
        IsAnd = isAnd;
    }
}