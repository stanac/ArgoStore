using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class MethodStringStaticTranslator : IExpressionToStatementTranslator
    {
        private readonly string[] _supportedMethodNames = new[]
        {
                "Equals", "IsNullOrEmpty", "IsNullOrWhiteSpace"
        };

        public bool CanTranslate(Expression expression)
        {
            MethodCallExpression e = expression as MethodCallExpression;

            return e != null
                && e.Method.IsStatic
                && e.Method.ReflectedType == typeof(string)
                && _supportedMethodNames.Contains(e.Method.Name);
        }

        public Statement Translate(Expression expression)
        {
            MethodCallExpression e = expression as MethodCallExpression;

            if (!CanTranslate(expression)) throw new ArgumentException($"Cannot translate {expression} with {nameof(MethodStringStaticTranslator)}");

            switch (e.Method.Name)
            {
                case "Equals": return TranslateEquals(e);
                case "IsNullOrEmpty": return TranslateIsNullOrEmpty(e);
                case "IsNullOrWhiteSpace": return TranslateIsNullOrWhiteSpace(e);
            }

            throw new ArgumentOutOfRangeException("Should not be thrown ever");
        }

        private Statement TranslateIsNullOrWhiteSpace(MethodCallExpression e)
        {
            var arg = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]);
            return new MethodCallStatement(MethodCallStatement.SupportedMethodNames.StringIsNullOrWhiteSpace, arg);
        }

        private Statement TranslateIsNullOrEmpty(MethodCallExpression e)
        {
            var arg = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]);
            return new MethodCallStatement(MethodCallStatement.SupportedMethodNames.StringIsNullOrEmpty, arg);
        }

        private Statement TranslateEquals(MethodCallExpression e)
        {
            bool ignoreCase = false;

            if (e.Arguments.Count == 3)
            {
                var value = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[2]);
                ignoreCase = (value as ConstantStatement).Value.Contains("IgnoreCase");
            }

            MethodCallStatement.SupportedMethodNames methodName = ignoreCase ? MethodCallStatement.SupportedMethodNames.StringEqualsIgnoreCase : MethodCallStatement.SupportedMethodNames.StringEquals;
            var args = new Statement[] { ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]), ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[1]) };
            return new MethodCallStatement(methodName, args);
        }
    }
}
