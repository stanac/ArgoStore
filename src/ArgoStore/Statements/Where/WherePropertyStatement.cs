namespace ArgoStore.Statements.Where;

internal class WherePropertyStatement : WhereValueStatement
{
    public string PropertyName { get; }

    public WherePropertyStatement(string propertyName)
    {
        PropertyName = propertyName;
    }
}
