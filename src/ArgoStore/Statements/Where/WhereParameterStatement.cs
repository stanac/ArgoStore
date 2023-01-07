namespace ArgoStore.Statements.Where;

internal class WhereParameterStatement : WhereValueStatement
{
    public object Value { get; }

    public WhereParameterStatement(object value)
    {
        Value = value;
    }
}