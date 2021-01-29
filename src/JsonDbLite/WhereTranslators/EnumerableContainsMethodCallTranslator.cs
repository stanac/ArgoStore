using JsonDbLite.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class EnumerableContainsMethodCallTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.DeclaringType == typeof(Enumerable) && m.Method.Name == "Contains";
            }

            return false;
        }

        public WhereClauseExpressionData Translate(Expression expression)
        {
            var m = expression as MethodCallExpression;

            if (m.Arguments.Count != 2)
            {
                throw new NotSupportedException($"Enumerable.Contains not supported with {m.Arguments.Count} argument(s), expression: {expression}");
            }

            var arg1 = WhereTranslatorStrategy.Translate(m.Arguments[0]);
            var arg2 = WhereTranslatorStrategy.Translate(m.Arguments[1]);

            return new WhereMethodCallExpressionData
            {
                Arguments = new[] { arg2, arg1 },
                MethodName = WhereMethodCallExpressionData.SupportedMethodNames.EnumerableContains
            };
        }
    }
}
