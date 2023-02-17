namespace ArgoStore.Statements;

internal class OrderByStatement : Statement
{
    public OrderByStatement(IReadOnlyList<OrderByElement> elements)
    {
        Elements = elements ?? throw new ArgumentNullException(nameof(elements));

        if (Elements.Count == 0) throw new ArgumentException("Collection cannot be empty", nameof(elements));
    }

    public static OrderByStatement Create(PropertyAccessStatement pas, bool asc)
    {
        return new OrderByStatement(new List<OrderByElement> { new OrderByElement(pas.Name, asc) });
    }

    public IReadOnlyList<OrderByElement> Elements { get; }

    public override Statement Negate() => throw new NotSupportedException();

    public override Statement ReduceIfPossible() => this;

    public override string ToDebugString() => $"OrderBy ";

    internal OrderByStatement Join(OrderByStatement orderByStatement)
    {
        var elements = Elements.ToList();
        elements.AddRange(orderByStatement.Elements);

        return new OrderByStatement(elements);
    }
}