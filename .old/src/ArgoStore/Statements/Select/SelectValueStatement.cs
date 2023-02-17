namespace ArgoStore.Statements.Select;

internal abstract class SelectValueStatement : SelectStatementBase
{
    public abstract string? ResultName { get; }
    public abstract void SetResultName(string name);
}