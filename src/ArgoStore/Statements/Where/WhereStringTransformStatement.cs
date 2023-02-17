namespace ArgoStore.Statements.Where;

internal class WhereStringTransformStatement : WhereStatementBase
{
    public WhereStatementBase Statement { get; }
    public StringTransformTypes Transform { get; }

    public WhereStringTransformStatement(WhereStatementBase statement, StringTransformTypes transform)
    {
        Transform = transform;

        if (statement is WhereValueStatement valueStatement)
        {
            Statement = statement;
        }
        else
        {
            throw new ArgumentException("Expected value statement", nameof(statement));
        }
    }
    
}