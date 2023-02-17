namespace ArgoStore.Statements.Where;

internal class WhereStringLengthStatement : WhereStatementBase
{
    public WhereStatementBase Value { get; }

    public WhereStringLengthStatement(WhereStatementBase value)
    {
        Value = value;
    }
}