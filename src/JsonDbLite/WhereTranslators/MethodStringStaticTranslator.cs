using JsonDbLite.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal partial class MethodStringInstanceTranslator
    {
        internal class MethodStringStaticTranslator : IWhereTranslator
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

            public WhereClauseExpressionData Translate(Expression expression)
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

            private WhereClauseExpressionData TranslateIsNullOrWhiteSpace(MethodCallExpression e)
            {
                return new WhereMethodCallExpressionData
                {
                    MethodName = WhereMethodCallExpressionData.SupportedMethodNames.StringIsWhiteSpace,
                    Arguments = new WhereClauseExpressionData[] { WhereTranslatorStrategy.Translate(e.Arguments[0]) }
                };
            }

            private WhereClauseExpressionData TranslateIsNullOrEmpty(MethodCallExpression e)
            {
                return new WhereMethodCallExpressionData
                {
                    MethodName = WhereMethodCallExpressionData.SupportedMethodNames.StringIsNullOrEmpty,
                    Arguments = new WhereClauseExpressionData[] { WhereTranslatorStrategy.Translate(e.Arguments[0]) }
                };
            }

            private WhereClauseExpressionData TranslateEquals(MethodCallExpression e)
            {
                bool ignoreCase = true;

                if (e.Arguments.Count == 3)
                {
                    var value = WhereTranslatorStrategy.Translate(e.Arguments[2]);
                }

                return new WhereMethodCallExpressionData
                {
                    MethodName = ignoreCase ? WhereMethodCallExpressionData.SupportedMethodNames.StringEqualsIgnoreCase : WhereMethodCallExpressionData.SupportedMethodNames.StringEquals,
                    Arguments = new WhereClauseExpressionData[] { WhereTranslatorStrategy.Translate(e.Arguments[0]), WhereTranslatorStrategy.Translate(e.Arguments[1]) }
                };
            }
        }
    }
}
