namespace ArgoStore.Statements.Order;

internal class OrderByStatement
{
    public string PropertyName { get; }
    public bool Asc { get; }
    public string Direction { get; }

    public OrderByStatement(string propertyName, bool asc)
    {
        PropertyName = propertyName;
        Asc = asc;

        Direction = asc
            ? "ASC"
            : "DESC";
    }
}