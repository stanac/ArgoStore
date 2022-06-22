using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgoStore.Statements
{
    internal class ConstantStatement : Statement
    {
        public ConstantStatement(bool isString, bool isBoolean, string value)
        {
            IsString = isString;
            IsBoolean = isBoolean;
            Value = isString ? EscapeString(value) : value;
        }

        public ConstantStatement(bool isString, bool isBoolean, List<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            IsString = isString;
            IsBoolean = isBoolean;
            IsCollection = true;
            Values = isString ? values.Select(EscapeString).ToList() : values;
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

        private static string EscapeString(string s)
        {
            if (s == null) return null;

            return s
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
        }
    }
}