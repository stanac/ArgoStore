namespace ArgoStore.Statements.Select;

internal class SelectPropertyStatement : SelectStatementBase
{
    public string Name { get; }

    public SelectPropertyStatement(string name)
    {
        Name = name;
    }
}