using ArgoStore.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class OrderByTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression mc)
            {
                return mc.Method.Name == "OrderBy" && mc.Method.DeclaringType == typeof(Queryable);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var mc = expression as MethodCallExpression;

            var lambda = ExpressionHelpers.RemoveQuotes(mc.Arguments[1]);

            var statement = ExpressionToStatementTranslatorStrategy.Translate(mc.Arguments[1]);

            throw new NotImplementedException();
        }
    }
}
