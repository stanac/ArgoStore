using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal partial class MethodStringStaticExpressionToStatementTranslator
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
                return new MethodCallStatement
                {
                    MethodName = MethodCallStatement.SupportedMethodNames.StringIsNullOrWhiteSpace,
                    Arguments = new Statement[] { ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]) }
                };
            }

            private Statement TranslateIsNullOrEmpty(MethodCallExpression e)
            {
                return new MethodCallStatement
                {
                    MethodName = MethodCallStatement.SupportedMethodNames.StringIsNullOrEmpty,
                    Arguments = new Statement[] { ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]) }
                };
            }

            private Statement TranslateEquals(MethodCallExpression e)
            {
                bool ignoreCase = false;

                if (e.Arguments.Count == 3)
                {
                    var value = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[2]);
                    ignoreCase = (value as ConstantStatement).Value.Contains("IgnoreCase");
                }

                return new MethodCallStatement
                {
                    MethodName = ignoreCase ? MethodCallStatement.SupportedMethodNames.StringEqualsIgnoreCase : MethodCallStatement.SupportedMethodNames.StringEquals,
                    Arguments = new Statement[] { ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]), ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[1]) }
                };
            }
        }
    }
}
