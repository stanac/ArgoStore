namespace ArgoStore.Statements.Where;

internal class WhereParameterStatement : WhereValueStatement
{
    public object Value { get; }

    public WhereParameterStatement(object value)
    {
        if (value is Guid) Value = value.ToString()!;
        else Value = value;
    }
}