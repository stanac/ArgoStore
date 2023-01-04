namespace ArgoStore.Statements;

internal class PropertyStatement : ValueStatement
{
    public string PropertyName { get; }

    public PropertyStatement(string propertyName)
    {
        PropertyName = propertyName;
    }
}
