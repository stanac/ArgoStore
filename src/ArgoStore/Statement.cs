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

        public string StatementTypeName => GetType().Name;
    }

    internal class TopStatement
    {
        public Type TypeFrom { get; }
        public Type TypeTo { get; }
        public SelectStatement SelectStatement { get; }
        public bool IsAnyQuery { get; private set; }
        public bool IsCountQuery { get; private set; }

        public TopStatement(WhereStatement where, SelectStatement.CalledByMethods method)
        {
            var selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            SelectStatement = new SelectStatement(where, where.TargetType, where.TargetType, selectElements, null, method);
            TypeFrom = where.TargetType;
            TypeTo = where.TargetType;
        }

        public TopStatement(SelectStatement select)
        {
            SelectStatement = select ?? throw new ArgumentNullException(nameof(select));
            TypeFrom = select.TypeFrom ?? throw new ArgumentException("TypeFrom not set", nameof(select));
            TypeTo = select.TypeTo ?? throw new ArgumentException("TypeTo not set", nameof(select));
        }

        public TopStatement(Type entityType)
        {
            TypeFrom = entityType;
            TypeTo = entityType;
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

            if (statement is SelectCountStatement cq)
            {
                if (cq.Where != null)
                {
                    return new TopStatement(cq.Where, SelectStatement.CalledByMethods.Count)
                    {
                        IsCountQuery = true
                    };
                }
                
                if (cq.SubQuery != null)
                {
                    return new TopStatement(cq.SubQuery)
                    {
                        IsCountQuery = true
                    };
                }

                if (cq.FromType != null)
                {
                    return new TopStatement(cq.FromType)
                    {
                        IsCountQuery = true
                    };
                }
            }

            if (statement is SelectExistsStatement eq)
            {
                if (eq.Where != null)
                {
                    return new TopStatement(eq.Where, SelectStatement.CalledByMethods.Count)
                    {
                        IsAnyQuery = true
                    };
                }

                if (eq.SubQuery != null)
                {
                    return new TopStatement(eq.SubQuery)
                    {
                        IsAnyQuery = true
                    };
                }

                if (eq.FromType != null)
                {
                    return new TopStatement(eq.FromType)
                    {
                        IsAnyQuery = true
                    };
                }
            }

            throw new ArgumentException($"Cannot create {nameof(TopStatement)} from {statement.GetType().FullName}", nameof(statement));
        }

        internal void SetAliases()
        {
            if (SelectStatement != null)
            {
                SelectStatement.SetAliases(0);
                SelectStatement.SetSubQueryAliases(null);
            }
        }
    }

    internal class SelectCountStatement : Statement
    {
        public SelectCountStatement(Type fromType, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            LongCount = longCount;
        }

        public SelectCountStatement(Type fromType, WhereStatement where, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            Where = where ?? throw new ArgumentNullException(nameof(where));
            LongCount = longCount;
        }

        public SelectCountStatement(Type fromType, SelectStatement subQuery, bool longCount)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            SubQuery = subQuery ?? throw new ArgumentNullException(nameof(subQuery));
            LongCount = longCount;
        }

        public Type FromType { get; }
        public WhereStatement Where { get; }
        public SelectStatement SubQuery { get; }
        public bool LongCount { get; }

        public override Statement Negate() => throw new NotSupportedException();

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString() => "SELECT COUNT()";
    }

    internal class SelectExistsStatement : Statement
    {
        public SelectExistsStatement(Type fromType)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
        }

        public SelectExistsStatement(Type fromType, WhereStatement where)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            Where = where ?? throw new ArgumentNullException(nameof(where));
        }

        public SelectExistsStatement(Type fromType, SelectStatement subQuery)
        {
            FromType = fromType ?? throw new ArgumentNullException(nameof(fromType));
            SubQuery = subQuery ?? throw new ArgumentNullException(nameof(subQuery));
        }

        public Type FromType { get; }
        public WhereStatement Where { get; }
        public SelectStatement SubQuery { get; }

        public override Statement Negate() => throw new NotSupportedException();

        public override Statement ReduceIfPossible() => this;

        public override string ToDebugString() => "SELECT EXISTS()";
    }

    internal class SelectStatement : Statement
    {
        public WhereStatement WhereStatement { get; private set; }
        public SelectStatement SubQueryStatement { get; private set; }
        public OrderByStatement OrderByStatement { get; private set; }
        public Type TypeFrom { get; }
        public Type TypeTo { get; }
        public IReadOnlyList<SelectStatementElement> SelectElements { get; } = new List<SelectStatementElement>();
        public int? Top { get; }
        public CalledByMethods CalledByMethod { get; }
        public string Alias { get; private set; }
        
        public SelectStatement(WhereStatement whereStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
            int? top, CalledByMethods calledByMethod)
            : this (typeFrom, typeTo, selectElements, top, calledByMethod)
        {
            WhereStatement = whereStatement ?? throw new ArgumentNullException(nameof(whereStatement));
        }

        public SelectStatement(SelectStatement subQueryStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
            int? top, CalledByMethods calledByMethod)
            : this (typeFrom, typeTo, selectElements, top, calledByMethod)
        {
            SubQueryStatement = subQueryStatement ?? throw new ArgumentNullException(nameof(subQueryStatement));
        }

        public SelectStatement(OrderByStatement orderByStatement, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
            int? top, CalledByMethods calledByMethod)
            : this(typeFrom, typeTo, selectElements, top, calledByMethod)
        {
            OrderByStatement = orderByStatement ?? throw new ArgumentNullException(nameof(orderByStatement));
        }

        public SelectStatement(Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
            int? top, CalledByMethods calledByMethod)
        {
            TypeFrom = typeFrom ?? throw new ArgumentNullException(nameof(typeFrom));
            TypeTo = typeTo ?? throw new ArgumentNullException(nameof(typeTo));
            SelectElements = selectElements ?? throw new ArgumentNullException(nameof(selectElements));
            Top = top;
            CalledByMethod = calledByMethod;

            if (top.HasValue && top < 1) throw new ArgumentException($"{nameof(top)} cannot be less than 1", nameof(top));
        }

        public static SelectStatement Create(Statement from, Type typeFrom, Type typeTo, IReadOnlyList<SelectStatementElement> selectElements,
            int? top, CalledByMethods calledByMethod)
        {
            if (from == null) return new SelectStatement(typeFrom, typeTo, selectElements, top, calledByMethod);

            if (from is WhereStatement w) return new SelectStatement(w, typeFrom, typeTo, selectElements, top, calledByMethod);

            if (from is SelectStatement s) return new SelectStatement(s, typeFrom, typeTo, selectElements, top, calledByMethod);

            if (from is OrderByStatement o) return new SelectStatement(o, typeFrom, typeTo, selectElements, top, calledByMethod);

            throw new ArgumentException($"{from} statement of type {from.GetType().FullName} not supported in SelectStatement.Create", nameof(from));
        }

        public override Statement Negate()
        {
            throw new NotSupportedException();
        }

        public override Statement ReduceIfPossible()
        {
            if (SubQueryStatement != null) SubQueryStatement = SubQueryStatement.ReduceIfPossible() as SelectStatement;
            if (WhereStatement != null) WhereStatement = WhereStatement.ReduceIfPossible() as WhereStatement;
            return this;
        }

        public override string ToDebugString()
        {
            return $"SELECT {string.Join(", ", SelectElements.Select(x => x.ToDebugString()))} {WhereStatement?.ToDebugString()}";
        }

        internal void SetAliases(int level)
        {
            Alias = $"\"..t{level}\"";

            if (WhereStatement != null) WhereStatement.SetAliases(level);
            if (SubQueryStatement != null) SubQueryStatement.SetAliases(level + 1);

            if (SelectElements != null)
            {
                for (int i = 0; i < SelectElements.Count; i++)
                {
                    SelectElements[i].Alias = $"\"...c{i}\"";
                }
            }
        }

        internal void SetSubQueryAliases(SelectStatement parent)
        {
            if (SubQueryStatement != null)
            {
                SubQueryStatement.SetSubQueryAliases(this);
            }

            if (parent != null)
            {
                foreach (SelectStatementElement e in parent.SelectElements)
                {
                    e.BindsToSubQueryAlias = SelectElements.First(x => x.OutputProperty == e.InputProperty).Alias;
                }
            }
        }

        internal SelectStatement SetOrderBy(OrderByStatement orderByStatement)
        {
            if (OrderByStatement == null)
            {
                OrderByStatement = orderByStatement ?? throw new ArgumentNullException(nameof(orderByStatement));
            }
            else
            {
                OrderByStatement = OrderByStatement.Join(orderByStatement);
            }
            return this;
        }

        internal SelectStatement AddWhereCondition(WhereStatement where)
        {
            if (where is null) throw new ArgumentNullException(nameof(where));

            if (WhereStatement == null)
            {
                WhereStatement = where;
            }
            else
            {
                WhereStatement.AddConjunctedCondition(where.Statement);
            }

            return this;
        }

        public enum CalledByMethods
        {
            Select, First, FirstOrDefault, Last, LastOrDefault, Single, SingleOrDefault, Count, Any
        }
    }

    internal class SelectStatementElement
    {
        public SelectStatementElement(Statement statement, Type returnType, bool selectsJson, string inputProperty, string outputProperty)
        {
            Statement = statement ?? throw new ArgumentNullException(nameof(statement));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            SelectsJson = selectsJson;

            if (!selectsJson)
            {
                if (string.IsNullOrWhiteSpace(inputProperty)) throw new ArgumentException($"Parameter {nameof(inputProperty)} needs to be set when {nameof(selectsJson)} is false", nameof(inputProperty));
                if (string.IsNullOrWhiteSpace(outputProperty)) throw new ArgumentException($"Parameter {nameof(outputProperty)} needs to be set when {nameof(selectsJson)} is false", nameof(outputProperty));
            }

            InputProperty = inputProperty;
            OutputProperty = outputProperty;
        }

        public static SelectStatementElement CreateWithStar(Type returnType) => new SelectStatementElement(new SelectStarParameterStatement(), returnType, true, null, null);

        public Statement Statement { get; }
        public Type ReturnType { get; }
        public string Alias { get; set; }
        public bool FromSubQuery { get; set; }
        public string BindsToSubQueryAlias { get; set; }
        public bool SelectsJson { get; }
        public string InputProperty { get; }
        public string OutputProperty { get; }

        public string ToDebugString() => $"{Statement?.ToDebugString()}";
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

        public string Alias { get; private set; }

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

        internal void SetAliases(int level)
        {
            Alias = $"\"..t{level}\"";
        }
    }

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
        public Statement InnerStatement { get; }

        public NotStatement(Statement innerStatement)
        {
            InnerStatement = innerStatement ?? throw new ArgumentNullException(nameof(innerStatement));
        }

        public override Statement Negate()
        {
            return InnerStatement;
        }

        public override string ToDebugString() => $"NOT ({InnerStatement})";

        public override Statement ReduceIfPossible() => InnerStatement.Negate();
    }

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
        public string SubQueryAlias { get; set; }
        
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

        public bool IsNull => Value == null && !IsCollection;

        public static ConstantStatement CreateBoolean(bool value) => new ConstantStatement(false, true, value.ToString());

        public override Statement Negate()
        {
            if (IsBoolean)
            {
                Statement right = CreateBoolean(false);

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

            if (IsNull) return "NULL";

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

        public Statement[] Arguments { get; }
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

        public override string ToDebugString() => $"{MethodName}({string.Join(", ", Arguments.Select(x => x.ToDebugString()))})";

        public enum SupportedMethodNames
        {
            StringToUpper, StringToLower, StringTrim, StringTrimStart, StringTrimEnd, StringIsNullOrEmpty, StringIsNullOrWhiteSpace, StringEquals, StringEqualsIgnoreCase,
            StringContains, StringStartsWith, StringEndsWith, StringContainsIgnoreCase, StringStartsWithIgnoreCase, StringEndsWithIgnoreCase,
            EnumerableContains
        }
    }
}
