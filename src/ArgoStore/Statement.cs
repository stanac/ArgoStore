﻿using System;
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
    }

    internal class TopStatement
    {
        public Type TargetType { get; }
        public SelectStatement SelectStatement { get; }

        public TopStatement(WhereStatement where, SelectStatement.CalledByMethods method)
        {
            var selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            SelectStatement = new SelectStatement(where, where.TargetType, selectElements, null, method);
            TargetType = where.TargetType;
        }

        public TopStatement(SelectStatement select)
        {
            SelectStatement = select ?? throw new ArgumentNullException(nameof(select));
            TargetType = select.TargetType ?? throw new ArgumentException("TargetType not set", nameof(select));
        }

        public static TopStatement Create(Statement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            if (statement is SelectStatement ss)
            {
                return new TopStatement(ss);
            }
            
            if (statement is WhereStatement ws)
            {
                return new TopStatement(ws, SelectStatement.CalledByMethods.Select); // todo: check CalledByMethods.Select in other methods
            }

            throw new ArgumentException($"Cannot create {nameof(TopStatement)} from {statement.GetType().FullName}", nameof(statement));
        }
    }

    internal class SelectStatement : Statement
    {
        public WhereStatement WhereStatement { get; }
        public Type TargetType { get; }
        public IReadOnlyList<SelectStatementElement> SelectElements { get; } = new List<SelectStatementElement>();
        public int? Top { get; }
        public CalledByMethods CalledByMethod { get; }

        public SelectStatement(WhereStatement whereStatement, Type targetType, IReadOnlyList<SelectStatementElement> selectElements, int? top, CalledByMethods calledByMethod)
        {
            WhereStatement = whereStatement;
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            SelectElements = selectElements ?? throw new ArgumentNullException(nameof(selectElements));
            Top = top;
            CalledByMethod = calledByMethod;

            if (top.HasValue && top < 1) throw new ArgumentException($"{nameof(top)} cannot be less than 1", nameof(top));
        }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString()
        {
            return $"SELECT {string.Join(", ", SelectElements.Select(x => x.ToDebugString()))} {WhereStatement?.ToDebugString()}";
        }

        public enum CalledByMethods
        {
            Select, First, FirstOrDefault, Last, LastOrDefault, Single, SingleOrDefault, Count
        }
    }

    internal class SelectStatementElement : Statement
    {
        public SelectStatementElement(Statement statement, Type returnType, bool selectsJson)
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            SelectsJson = selectsJson;
        }

        public static SelectStatementElement CreateWithStar(Type returnType) => new SelectStatementElement(new SelectStarParameterStatement(), returnType, true);

        public Statement Statement { get; }
        public Type ReturnType { get; }
        public bool SelectsJson { get; }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible()
        {
            return this;
        }

        public override string ToDebugString() => $"{Statement?.ToDebugString()}";
    }

    internal class SelectStarParameterStatement : Statement
    {
        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString() => "*";
    }

    internal class WhereStatement : Statement
    {
        public WhereStatement(Statement statement, Type targetType)
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public Statement Statement { get; private set; }

        public Type TargetType { get; }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible() => new WhereStatement(Statement.ReduceIfPossible(), TargetType);

        public void AddConjunctedCondition(Statement statement)
        {
            if (statement is null) throw new ArgumentNullException(nameof(statement));

            Statement = new BinaryLogicalStatement(Statement, statement, false);
        }

        public override string ToDebugString()
        {
            return $"WHERE {Statement?.ToDebugString()}";
        }
    }

    internal abstract class BinaryStatement : Statement
    {
        protected BinaryStatement(Statement left, Statement right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Statement Left { get; }
        public Statement Right { get; }

        public abstract string OperatorString { get; }
    }

    internal class BinaryLogicalStatement : BinaryStatement
    {
        public BinaryLogicalStatement(Statement left, Statement right, bool isOr)
            : base (left, right)
        {
            IsOr = isOr;
        }

        public bool IsOr { get; set; }
        public bool IsAnd => !IsOr;

        public override Statement Negate() => new BinaryLogicalStatement(Left.Negate(), Right.Negate(), !IsOr);

        public override Statement ReduceIfPossible() => new BinaryLogicalStatement(Left.ReduceIfPossible(), Right.ReduceIfPossible(), IsOr);

        public override string ToDebugString() => $"{Left?.ToDebugString()} {(IsOr ? "||" : "&&")} {Right?.ToDebugString()}";

        public override string OperatorString => IsAnd ? "AND" : "OR";
    }

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

    internal class NotStatement : Statement
    {
        public Statement InnerStatement { get; set; }

        public override Statement Negate()
        {
            return InnerStatement;
        }

        public override string ToDebugString() => $"NOT ({InnerStatement})";

        public override Statement ReduceIfPossible() => InnerStatement.Negate();
    }

    internal class PropertyAccessStatement : Statement
    {
        public string Name { get; set; }
        public bool IsBoolean { get; set; }

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

    internal class ConstantStatement : Statement
    {
        public ConstantStatement(bool isString, bool isBoolean, string value)
        {
            IsString = isString;
            IsBoolean = isBoolean;
            Value = value;
        }

        public ConstantStatement(bool isString, bool isBoolean, List<string> values)
        {
            IsString = isString;
            IsBoolean = isBoolean;
            Values = values ?? throw new ArgumentNullException(nameof(values));
            IsCollection = true;
        }

        public bool IsString { get; }
        public bool IsBoolean { get; }
        public bool IsCollection { get; }
        public string Value { get; }
        public List<string> Values { get; }

        public static ConstantStatement CreateBoolean(bool value) => new ConstantStatement(false, true, value.ToString());

        public override Statement Negate()
        {
            if (IsBoolean)
            {
                Statement right = ConstantStatement.CreateBoolean(false);

                return new BinaryComparisonStatement(this, right, BinaryComparisonStatement.Operators.Equal);
            }

            throw new NotSupportedException($"Cannot negate non boolean value");
        }

        public override Statement ReduceIfPossible() => this;

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
        public SupportedMethodNames MethodName { get; set; }
        public bool Negated { get; }

        public MethodCallStatement(SupportedMethodNames methodName, params Statement[] arguments)
            : this (methodName, false, arguments)
        {
        }

        public MethodCallStatement(SupportedMethodNames methodName, bool negated, params Statement[] arguments)
        {
            MethodName = methodName;
            Negated = negated;
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public bool IsResultBoolean => _booleanResultMethodNames.Contains(MethodName);

        public override Statement Negate()
        {
            if (!_booleanResultMethodNames.Contains(MethodName))
            {
                throw new NotSupportedException($"Cannot negate non boolean method {MethodName}");
            }

            return new MethodCallStatement(MethodName, !Negated, Arguments);
        }

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString() => $"{MethodName}({string.Join(", ", Arguments?.Select(x => x.ToDebugString()))})";

        public enum SupportedMethodNames
        {
            StringToUpper, StringToLower, StringTrim, StringTrimStart, StringTrimEnd, StringIsNullOrEmpty, StringIsNullOrWhiteSpace, StringEquals, StringEqualsIgnoreCase,
            StringContains, StringStartsWith, StringEndsWith, StringContainsIgnoreCase, StringStartsWithIgnoreCase, StringEndsWithIgnoreCase,
            EnumerableContains
        }
    }
}