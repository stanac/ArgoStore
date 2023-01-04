namespace ArgoStore.Statements.Where;

internal class WhereParameterStatement : WhereValueStatement
{
    public string ParameterName { get; }

    public WhereParameterStatement(string parameterName)
    {
        ParameterName = parameterName;
    }
}