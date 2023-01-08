namespace ArgoStore.Statements.Select;

internal class SelectAnonymousType : SelectStatementBase
{
    public Type AnonymousType { get; }
    public List<SelectPropertyStatement> SelectElements { get; }

    public SelectAnonymousType(Type anonymousType, List<SelectPropertyStatement> selectElements)
    {
        AnonymousType = anonymousType;
        SelectElements = selectElements;
    }
}