using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgoStore
{
    internal abstract class Statement
    {
        public abstract string ToDebugString();

        public override string ToString() => ToDebugString();

        public abstract Statement Negate();

        public abstract Statement ReduceIfPossible();

        public abstract Statement Copy();
    }

    internal class SelectStatement : Statement
    {
        public Statement Statement { get; set; }

        public override Statement Copy()
        {
            return new SelectStatement
            {
                Statement = Statement?.Copy()
            };
        }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible()
        {
            return new SelectStatement
            {
                Statement = Statement?.ReduceIfPossible()
            };
        }

        public override string ToDebugString()
        {
            return $"SELECT {Statement?.ToDebugString()}";
        }
    }

    internal class WhereStatement : Statement
    {
        public Statement Statement { get; set; }

        public override Statement Copy()
        {
            return new WhereStatement
            {
                Statement = Statement?.Copy()
            };
        }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible()
        {
            return new WhereStatement
            {
                Statement = Statement.ReduceIfPossible()
            };
        }

        public override string ToDebugString()
        {
            return $"WHERE {Statement?.ToDebugString()}";
        }
    }

    internal abstract class BinaryStatement : Statement
    {
        public Statement Left { get; set; }
        public Statement Right { get; set; }
    }

    internal class BinaryLogicalStatement : BinaryStatement
    {
        public bool IsOr { get; set; }
        public bool IsAnd => !IsOr;

        public override Statement Copy()
        {
            return new BinaryLogicalStatement
            {
                Left = Left?.Copy(),
                Right = Right?.Copy(),
                IsOr = IsOr
            };
        }

        public override Statement Negate()
        {
            return new BinaryLogicalStatement
            {
                Left = Left?.Negate(),
                Right = Right?.Negate(),
                IsOr = !IsOr
            };
        }

        public override Statement ReduceIfPossible()
        {
            return new BinaryLogicalStatement
            {
                Left = Left?.ReduceIfPossible(),
                Right = Right?.ReduceIfPossible(),
                IsOr = IsOr
            };
        }

        public override string ToDebugString() => $"{Left?.ToDebugString()} {(IsOr ? "||" : "&&")} {Right?.ToDebugString()}";
    }

    internal class BinaryComparisonStatement : BinaryStatement
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

        public override Statement Negate()
        {
            return new BinaryComparisonStatement
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

        public override Statement Copy()
        {
            return new BinaryComparisonStatement
            {
                Left = Left?.Copy(),
                Right = Right?.Copy(),
                Operator = Operator
            };
        }

        public override string ToDebugString() => $"{Left?.ToDebugString()} {OperatorString} {Right?.ToDebugString()}";

        public override Statement ReduceIfPossible()
        {
            return new BinaryComparisonStatement
            {
                Left = Left?.ReduceIfPossible(),
                Right = Right?.ReduceIfPossible(),
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

    internal class NotStatement : Statement
    {
        public Statement InnerStatement { get; set; }

        public override Statement Copy()
        {
            return new NotStatement
            {
                InnerStatement = InnerStatement?.Copy()
            };
        }

        public override Statement Negate()
        {
            return InnerStatement.Copy();
        }

        public override string ToDebugString() => $"NOT ({InnerStatement})";

        public override Statement ReduceIfPossible() => InnerStatement.Negate();
    }

    internal class PropertyAccessStatement : Statement
    {
        public string Name { get; set; }
        public bool IsBoolean { get; set; }

        public override Statement Copy()
        {
            return new PropertyAccessStatement
            {
                Name = Name,
                IsBoolean = IsBoolean
            };
        }

        public override Statement Negate()
        {
            if (IsBoolean)
            {
                return new BinaryComparisonStatement
                {
                    Operator = BinaryComparisonStatement.Operators.Equal,
                    Left = this,
                    Right = new ConstantStatement
                    {
                        IsBoolean = true,
                        Value = "False"
                    }
                };
            }

            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible() => Copy();

        public override string ToDebugString() => $"$.{Name}";
    }

    internal class ConstantStatement : Statement
    {
        public bool IsString { get; set; }
        public bool IsBoolean { get; set; }
        public bool IsCollection { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        public override Statement Copy()
        {
            List<string> values = null;

            if (Values != null) values = Values.AsEnumerable().ToList();

            return new ConstantStatement
            {
                IsBoolean = IsBoolean,
                IsCollection = IsCollection,
                IsString = IsString,
                Value = Value,
                Values = values
            };
        }

        public override Statement Negate()
        {
            if (IsBoolean)
            {
                return new BinaryComparisonStatement
                {
                    Left = this,
                    Operator = BinaryComparisonStatement.Operators.Equal,
                    Right = new ConstantStatement
                    {
                        Value = "False",
                        IsBoolean = true
                    }
                };
            }

            throw new NotSupportedException($"Cannot negate non boolean value");
        }

        public override Statement ReduceIfPossible() => Copy();

        public override string ToDebugString()
        {
            if (IsCollection && Values != null)
            {
                return $"[{string.Join(", ", Values)}]";
            }

            return Value ?? "UnknownConstant";
        }
    }

    internal class MethodCallStatement : Statement
    {
        private static readonly SupportedMethodNames[] _booleanResultMethodNames = new[]
        {
           SupportedMethodNames.StringIsNullOrEmpty, SupportedMethodNames.StringIsNullOrWhiteSpace,
           SupportedMethodNames.StringEquals, SupportedMethodNames.StringEqualsIgnoreCase,
           SupportedMethodNames.StringContains, SupportedMethodNames.StringContainsIgnoreCase,
           SupportedMethodNames.StringStartsWith, SupportedMethodNames.StringStartsWithIgnoreCase,
           SupportedMethodNames.StringEndsWith, SupportedMethodNames.StringEndsWithIgnoreCase
        };

        public Statement[] Arguments { get; set; }
        public SupportedMethodNames? MethodName { get; set; }
        public bool Negated { get; set; }

        public bool IsResultBoolean => MethodName.HasValue && _booleanResultMethodNames.Contains(MethodName.Value);

        public override Statement Copy()
        {
            return new MethodCallStatement
            {
                MethodName = MethodName,
                Arguments = Arguments?.Select(x => x.Copy())?.ToArray()
            };
        }

        public override Statement Negate()
        {
            if (MethodName.HasValue && !_booleanResultMethodNames.Contains(MethodName.Value))
            {
                throw new NotSupportedException($"Cannot negate non boolean method {MethodName.Value}");
            }

            return new MethodCallStatement
            {
                Arguments = Arguments?.Select(x => x.Copy())?.ToArray(),
                MethodName = MethodName,
                Negated = !Negated
            };
        }

        public override Statement ReduceIfPossible() => Copy();

        public override string ToDebugString() => $"{MethodName}({string.Join(", ", Arguments?.Select(x => x.ToDebugString()))})";

        public enum SupportedMethodNames
        {
            StringToUpper, StringToLower, StringTrim, StringTrimStart, StringTrimEnd, StringIsNullOrEmpty, StringIsNullOrWhiteSpace, StringEquals, StringEqualsIgnoreCase,
            StringContains, StringStartsWith, StringEndsWith, StringContainsIgnoreCase, StringStartsWithIgnoreCase, StringEndsWithIgnoreCase,
            EnumerableContains
        }
    }
}
