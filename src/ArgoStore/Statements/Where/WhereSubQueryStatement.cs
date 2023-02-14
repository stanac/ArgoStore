namespace ArgoStore.Statements.Where;

internal abstract class WhereSubQueryStatement : WhereStatementBase
{
    public abstract FromAlias Alias { get; }
}