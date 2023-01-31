namespace ArgoStore.Statements.Where;

internal class WherePropertyStatement : WhereValueStatement
{
    public string PropertyName { get; }
    public Type PropertyType { get; }

    public WherePropertyStatement(string propertyName, Type propertyType)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
    }

    public WherePropertyStatement AddChild(string child, Type propertyType)
    {
        return new WherePropertyStatement($"{PropertyName}.{child}", propertyType);
    }
}
