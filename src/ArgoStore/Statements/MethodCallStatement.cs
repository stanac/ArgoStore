using System;
using System.Linq;

namespace ArgoStore.Statements
{
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
            StringContains, StringContainsIgnoreCase, StringStartsWith, StringStartsWithIgnoreCase, StringEndsWith, StringEndsWithIgnoreCase,
            EnumerableContains
        }
    }
}