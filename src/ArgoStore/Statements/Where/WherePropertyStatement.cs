namespace ArgoStore.Statements.Where;

internal class WherePropertyStatement : WhereValueStatement
{
    public string PropertyName { get; }
    public Type PropertyType { get; }
    public string? FromAlias { get; }

    public WherePropertyStatement(string propertyName, Type propertyType, string? fromAlias = null)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
        FromAlias = fromAlias;
    }

    public WherePropertyStatement AddChild(string child, Type propertyType)
    {
        return new WherePropertyStatement($"{PropertyName}.{child}", propertyType);
    }
}
