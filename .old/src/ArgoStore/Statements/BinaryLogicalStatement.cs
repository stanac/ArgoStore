namespace ArgoStore.Statements;

internal class BinaryLogicalStatement : BinaryStatement
{
    public BinaryLogicalStatement(Statement left, Statement right, bool isOr)
        : base (left, right)
    {
        if (left is WhereStatement || right is WhereStatement)
        {
            throw new ArgumentException();
        }

        IsOr = isOr;
    }

    public bool IsOr { get; set; }
    public bool IsAnd => !IsOr;

    public override Statement Negate() => new BinaryLogicalStatement(Left.Negate(), Right.Negate(), !IsOr);

    public override Statement ReduceIfPossible() => new BinaryLogicalStatement(Left.ReduceIfPossible(), Right.ReduceIfPossible(), IsOr);

    public override string ToDebugString() => $"{Left?.ToDebugString()} {(IsOr ? "||" : "&&")} {Right?.ToDebugString()}";

    public override string OperatorString => IsAnd ? "AND" : "OR";
}