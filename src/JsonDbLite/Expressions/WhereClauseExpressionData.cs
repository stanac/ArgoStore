using System;

namespace JsonDbLite.Expressions
{
    internal abstract class WhereClauseExpressionData
    {
        public bool IsBinaryExpressionData => this is WhereBinaryClauseExpressionData;
    }

    internal abstract class WhereBinaryClauseExpressionData : WhereClauseExpressionData
    {
        public WhereClauseExpressionData Left { get; set; }
        public WhereClauseExpressionData Right { get; set; }
    }

    internal class WhereBinaryLogicalExpressionData : WhereBinaryClauseExpressionData
    {
        public bool IsOr { get; set; }
        public bool IsAnd => !IsOr;

        public override string ToString() => $"{Left} {(IsOr ? "||" : "&&")} {Right}";
    }

    internal class WhereBinaryComparisonExpressionData : WhereBinaryClauseExpressionData
    {
        public Operators Operator { get; set; }

        public enum Operators
        {
            Equal, NotEqual,
            LessThan, LessThanOrEqual,
            GreaterThan, GreaterThanOrEqual
        }

        public string OperatorString
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

        public override string ToString() => $"{Left} {OperatorString} {Right}";
    }

    internal class WhereNotExpressionData : WhereClauseExpressionData
    {
        public WhereClauseExpressionData InnerExpression { get; set; }

        public override string ToString() => $"NOT ({InnerExpression})";
    }

    internal class WherePropertyExpressionData : WhereClauseExpressionData
    {
        public string Name { get; set; }
        public bool IsBoolean { get; set; }
        public override string ToString() => $"$.{Name}";
    }

    internal class WhereConstantExpressionData : WhereClauseExpressionData
    {
        public bool IsString { get; set; }
        public bool IsBoolean { get; set; }
        public string Value { get; set; }

        public override string ToString() => Value ?? "";
    }

    internal class WhereMethodCallExpressionData : WhereClauseExpressionData
    {
        public WhereClauseExpressionData[] Arguments { get; set; }
        public SupportedMethodNames MethodName { get; set; }

        public enum SupportedMethodNames
        {
            StringToUpper, StringToLower, StringTrim, StringIsNullOrEmpty, StringIsWhiteSpace, StringEquals, StringEqualsIgnoreCase
        }
    }
}
