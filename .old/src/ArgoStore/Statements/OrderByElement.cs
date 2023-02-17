namespace ArgoStore.Statements;

internal class OrderByElement
{
    public OrderByElement(string propertyName, bool asc)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace", nameof(propertyName));

        PropertyName = propertyName;
        Ascending = asc;
    }

    public string PropertyName { get; }
    public bool Ascending { get; }
}