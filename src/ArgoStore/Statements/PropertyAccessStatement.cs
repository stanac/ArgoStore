namespace ArgoStore.Statements;

internal class PropertyAccessStatement : Statement
{
    public PropertyAccessStatement(string name, bool isBoolean)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace", nameof(name));

        Name = name;
        IsBoolean = isBoolean;
    }

    public string Name { get; }
    public bool IsBoolean { get; }
        
    public override Statement Negate()
    {
        if (IsBoolean)
        {
            Statement right = ConstantStatement.CreateBoolean(false);

            return new BinaryComparisonStatement(this, right, BinaryComparisonStatement.Operators.Equal);
        }

        throw new NotSupportedException();
    }

    public override Statement ReduceIfPossible() => this;

    public override string ToDebugString() => $"$.{Name}";
}