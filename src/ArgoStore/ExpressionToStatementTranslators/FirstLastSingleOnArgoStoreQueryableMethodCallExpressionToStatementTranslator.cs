using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class FirstLastSingleOnArgoStoreQueryableMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        private static string[] SupportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault" };

        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return SupportedMethodNames.Contains(m.Method.Name) && m.Arguments[0].Type.IsGenericType && m.Arguments[0].Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            MethodCallExpression me = expression as MethodCallExpression;

            SelectStatement.CalledByMethods method = (SelectStatement.CalledByMethods)Enum.Parse(typeof(SelectStatement.CalledByMethods), me.Method.Name);

            Type targetType = GetTargetType(me.Arguments[0]);

            WhereStatement whereStatement = null;

            if (me.Arguments.Count == 2)
            {
                Statement whereCondition = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[1]);
                whereStatement = new WhereStatement(whereCondition, targetType);
            }

            List<SelectStatementElement> selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(targetType)
            };

            return new SelectStatement(whereStatement, targetType, targetType, selectElements, 1, method);
        }

        private Type GetTargetType(Expression expression)
        {
            if (expression is ConstantExpression ce)
            {
                if (ce.Type.IsGenericType && ce.Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>))
                {
                    return ce.Type.GetGenericArguments()[0];
                }
            }

            throw new ArgumentException($"Cannot get target type for select from \"{expression.NodeType}\", \"{expression.Type.FullName}\", \"{expression}\"");
        }
    }
}
