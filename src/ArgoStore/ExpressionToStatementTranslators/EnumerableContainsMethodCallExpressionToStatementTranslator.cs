using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class EnumerableContainsMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.DeclaringType == typeof(Enumerable) && m.Method.Name == "Contains";
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var m = expression as MethodCallExpression;

            if (m.Arguments.Count != 2)
            {
                throw new NotSupportedException($"Enumerable.Contains not supported with {m.Arguments.Count} argument(s), expression: {expression}");
            }

            var arg1 = ExpressionToStatementTranslatorStrategy.Translate(m.Arguments[0]);
            var arg2 = ExpressionToStatementTranslatorStrategy.Translate(m.Arguments[1]);

            return new MethodCallStatement(MethodCallStatement.SupportedMethodNames.EnumerableContains, arg1, arg2);
        }
    }
}
