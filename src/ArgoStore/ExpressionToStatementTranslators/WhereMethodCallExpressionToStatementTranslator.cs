using ArgoStore.Helpers;
using System;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class WhereMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression e)
            {
                return e.Method.Name == "Where";
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            MethodCallExpression e = expression as MethodCallExpression;

            Statement statement = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[1]);
            Type targetType = GetTargetType(e.Arguments[0]);

            var where = new WhereStatement(statement, targetType);

            if (e.Arguments[0].NodeType == ExpressionType.Call)
            {
                Statement targetStatement = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]);

                if (targetStatement is OrderByStatement os)
                {
                    var top = new TopStatement(where, SelectStatement.CalledByMethods.Select);
                    return top.SelectStatement.SetOrderBy(os);
                }

                // TODO: add test for q => q.Select().Where()
                // TODO: add test for q => q.Where().Where()

                throw new NotImplementedException($"Not implemented where on target {targetStatement.GetType().Name}");
            }

            return where;
        }

        private Type GetTargetType(Expression expression)
        {
            if (TypeHelpers.ImeplementsIQueryableGenericInteface(expression.Type))
            {
                return expression.Type.GetGenericArguments()[0];
            }

            throw new NotSupportedException($"Cannot get Where target type from \"{expression.NodeType}\", \"{expression.Type.Name}\", \"{expression}\"");
        }
    }
}
