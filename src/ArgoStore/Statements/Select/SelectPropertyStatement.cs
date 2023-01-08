namespace ArgoStore.Statements.Select;

internal class SelectPropertyStatement : SelectStatementBase
{
    public string Name { get; }
    public string ResultName { get; set; }

    public SelectPropertyStatement(string name)
    {
        Name = name;
        ResultName = name;
    }
}