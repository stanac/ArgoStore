namespace ArgoStore.Statements.Where;

internal class WhereParameterStatement : WhereValueStatement
{
    public object Value { get; }
    public Type Type { get; }

    public WhereParameterStatement(object value, Type type)
    {
        if (value is Guid) Value = value.ToString()!;
        else Value = value;
        Type = type;
    }
}