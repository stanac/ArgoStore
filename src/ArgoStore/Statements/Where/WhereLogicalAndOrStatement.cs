namespace ArgoStore.Statements.Where;

internal class WhereLogicalAndOrStatement : WhereStatementBase
{
    public bool IsAnd { get; }
    public WhereStatementBase Left { get; }
    public WhereStatementBase Right { get; }
    public bool IsOr => !IsAnd;

    public WhereLogicalAndOrStatement(bool isAnd, WhereStatementBase left, WhereStatementBase right)
    {
        IsAnd = isAnd;
        Left = left;
        Right = right;
    }
}