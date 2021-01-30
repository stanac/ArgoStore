using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDbLite.Expressions
{
    internal abstract class WhereClauseExpressionData
    {
        public bool IsBinaryExpressionData => this is WhereBinaryClauseExpressionData;

        public abstract WhereClauseExpressionData Negate();

        public abstract WhereClauseExpressionData Copy();
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

        public override WhereClauseExpressionData Copy()
        {
            return new WhereBinaryLogicalExpressionData
            {
                Left = Left?.Copy(),
                Right = Right?.Copy(),
                IsOr = IsOr
            };
        }

        public override WhereClauseExpressionData Negate()
        {
            return new WhereBinaryLogicalExpressionData
            {
                Left = Left.Negate(),
                Right = Right.Negate(),
                IsOr = !IsOr
            };
        }

        public override string ToString() => $"{Left} {(IsOr ? "||" : "&&")} {Right}";
    }

    internal class WhereBinaryComparisonExpressionData : WhereBinaryClauseExpressionData
    {
        public Operators Operator { get; set; }

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

        public override WhereClauseExpressionData Negate()
        {
            return new WhereBinaryComparisonExpressionData
            {
                Left = Left.Copy(),
                Right = Right.Copy(),
                Operator = GetNegatedOperator()
            };
        }

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

        public override WhereClauseExpressionData Copy()
        {
            return new WhereBinaryComparisonExpressionData
            {
                Left = Left?.Copy(),
                Right = Right?.Copy(),
                Operator = Operator
            };
        }

        public enum Operators
        {
            Equal, NotEqual,
            LessThan, LessThanOrEqual,
            GreaterThan, GreaterThanOrEqual
        }

    }

    internal class WhereNotExpressionData : WhereClauseExpressionData
    {
        public WhereClauseExpressionData InnerExpression { get; set; }

        public override WhereClauseExpressionData Copy()
        {
            return new WhereNotExpressionData
            {
                InnerExpression = InnerExpression?.Copy()
            };
        }

        public override WhereClauseExpressionData Negate()
        {
            return InnerExpression.Copy();
        }

        public override string ToString() => $"NOT ({InnerExpression})";   
    }

    internal class WherePropertyExpressionData : WhereClauseExpressionData
    {
        public string Name { get; set; }
        public bool IsBoolean { get; set; }

        public override WhereClauseExpressionData Copy()
        {
            return new WherePropertyExpressionData
            {
                Name = Name,
                IsBoolean = IsBoolean
            };
        }

        public override WhereClauseExpressionData Negate()
        {
            throw new NotSupportedException();
        }

        public override string ToString() => $"$.{Name}";


    }

    internal class WhereConstantExpressionData : WhereClauseExpressionData
    {
        public bool IsString { get; set; }
        public bool IsBoolean { get; set; }
        public bool IsCollection { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        public override WhereClauseExpressionData Copy()
        {
            List<string> values = null;

            if (Values != null) values = Values.AsEnumerable().ToList();

            return new WhereConstantExpressionData
            {
                IsBoolean = IsBoolean,
                IsCollection = IsCollection,
                IsString = IsString,
                Value = Value,
                Values = values
            };
        }

        public override WhereClauseExpressionData Negate()
        {
            throw new NotSupportedException();
        }

        public override string ToString() => Value ?? "";
    }

    internal class WhereMethodCallExpressionData : WhereClauseExpressionData
    {
        private static readonly SupportedMethodNames[] _booleanResultMethodNames = new[]
        {
           SupportedMethodNames.StringIsNullOrEmpty, SupportedMethodNames.StringIsNullOrWhiteSpace,
           SupportedMethodNames.StringEquals, SupportedMethodNames.StringEqualsIgnoreCase,
           SupportedMethodNames.StringContains, SupportedMethodNames.StringContainsIgnoreCase,
           SupportedMethodNames.StringStartsWith, SupportedMethodNames.StringStartsWithIgnoreCase,
           SupportedMethodNames.StringEndsWith, SupportedMethodNames.StringEndsWithIgnoreCase
        };

        public WhereClauseExpressionData[] Arguments { get; set; }
        public SupportedMethodNames? MethodName { get; set; }
        public bool Negated { get; set; }

        public bool IsResultBoolean => MethodName.HasValue && _booleanResultMethodNames.Contains(MethodName.Value);

        public override WhereClauseExpressionData Copy()
        {
            return new WhereMethodCallExpressionData
            {
                MethodName = MethodName,
                Arguments = Arguments?.Select(x => x.Copy())?.ToArray()
            };
        }

        public override WhereClauseExpressionData Negate()
        {
            if (MethodName.HasValue && !_booleanResultMethodNames.Contains(MethodName.Value))
            {
                throw new NotSupportedException($"Cannot negate non boolean method {MethodName.Value}");
            }

            return new WhereMethodCallExpressionData
            {
                Arguments = Arguments?.Select(x => x.Copy())?.ToArray(),
                MethodName = MethodName,
                Negated = !Negated
            };
        }

        public enum SupportedMethodNames
        {
            StringToUpper, StringToLower, StringTrim, StringTrimStart, StringTrimEnd, StringIsNullOrEmpty, StringIsNullOrWhiteSpace, StringEquals, StringEqualsIgnoreCase,
            StringContains, StringStartsWith, StringEndsWith, StringContainsIgnoreCase, StringStartsWithIgnoreCase, StringEndsWithIgnoreCase,
            EnumerableContains
        }
    }
}
