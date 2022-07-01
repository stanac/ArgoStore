namespace ArgoStore.Statements;

internal class BinaryComparisonStatement : BinaryStatement
{
    public BinaryComparisonStatement(Statement left, Statement right, Operators oper)
        : base(left, right)
    {
        Operator = oper;
    }

    public Operators Operator { get; }

    public override string OperatorString
    {
        get
        {
            switch (Operator)
            {
                case Operators.Equal:
                    return "=";
                case Operators.NotEqual:
                    return "<>";
                case Operators.LessThan:
                    return "<";
                case Operators.LessThanOrEqual:
                    return "<=";
                case Operators.GreaterThan:
                    return ">";
                case Operators.GreaterThanOrEqual:
                    return ">=";

                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public override Statement Negate() => new BinaryComparisonStatement(Left, Right, GetNegatedOperator());

    private Operators GetNegatedOperator()
    {
        switch (Operator)
        {
            case Operators.Equal:
                return Operators.NotEqual;
            case Operators.NotEqual:
                return Operators.Equal;
            case Operators.LessThan:
                return Operators.GreaterThanOrEqual;
            case Operators.LessThanOrEqual:
                return Operators.GreaterThan;
            case Operators.GreaterThan:
                return Operators.LessThanOrEqual;
            case Operators.GreaterThanOrEqual:
                return Operators.LessThan;
        }

        throw new IndexOutOfRangeException();
    }

    public override string ToDebugString() => $"{Left?.ToDebugString()} {OperatorString} {Right?.ToDebugString()}";

    public override Statement ReduceIfPossible() => new BinaryComparisonStatement(Left.ReduceIfPossible(), Right.ReduceIfPossible(), Operator);

    public enum Operators
    {
        Equal, NotEqual,
        LessThan, LessThanOrEqual,
        GreaterThan, GreaterThanOrEqual
    }
}