namespace ArgoStore.Statements.Select;

internal class SelectPropertyStatement : SelectValueStatement
{
    private string _resultName;
    public string Name { get; }
    public override string ResultName => _resultName;

    public SelectPropertyStatement(string name)
    {
        Name = name;
        _resultName = name;
    }

    public override void SetResultName(string name)
    {
        _resultName = name;
    }
}