namespace ArgoStore.Statements.Select;

internal class SelectParameterStatement : SelectValueStatement
{
    private string? _resultName;
    public object Value { get; }
    public override string? ResultName => _resultName;

    public SelectParameterStatement(object value)
    {
        Value = value;
    }

    public override void SetResultName(string name)
    {
        _resultName = name;
    }
}