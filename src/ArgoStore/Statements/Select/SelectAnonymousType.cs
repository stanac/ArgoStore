namespace ArgoStore.Statements.Select;

internal class SelectAnonymousType : SelectStatementBase
{
    public Type AnonymousType { get; }
    public List<SelectValueStatement> SelectElements { get; }

    public SelectAnonymousType(Type anonymousType, List<SelectValueStatement> selectElements)
    {
        AnonymousType = anonymousType;
        SelectElements = selectElements;
    }
}