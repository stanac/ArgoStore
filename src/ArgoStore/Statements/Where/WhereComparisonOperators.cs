namespace ArgoStore.Statements.Where;

internal enum ComparisonOperators
{
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Equal,
    NotEqual
}

internal static class ComparisonOperatorsExtensions
{
    public static string ToSqlOperator(this ComparisonOperators op)
    {
        switch (op)
        {
            case ComparisonOperators.GreaterThan:
                return ">";
            case ComparisonOperators.GreaterThanOrEqual:
                return ">=";
            case ComparisonOperators.LessThan:
                return "<";
            case ComparisonOperators.LessThanOrEqual:
                return "<=";
            case ComparisonOperators.Equal:
                return "=";
            case ComparisonOperators.NotEqual:
                return "!=";
            default:
                throw new ArgumentOutOfRangeException(nameof(op), op, null);
        }
    }

    public static bool IsEqualOrNotEqual(this ComparisonOperators op)
    {
        return op == ComparisonOperators.Equal || op == ComparisonOperators.NotEqual;
    }
}