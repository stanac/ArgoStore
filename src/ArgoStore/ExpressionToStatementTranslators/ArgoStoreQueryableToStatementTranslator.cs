using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class ArgoStoreQueryableToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is ConstantExpression ce)
            {
                return ce.Type.IsGenericType && ce.Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var ce = expression as ConstantExpression;

            Type targetType = ce.Type.GetGenericArguments()[0];

            var selectStatements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(targetType)
            };

            return new SelectStatement(targetType, targetType, selectStatements, null, SelectStatement.CalledByMethods.Select);
        }
    }
}
