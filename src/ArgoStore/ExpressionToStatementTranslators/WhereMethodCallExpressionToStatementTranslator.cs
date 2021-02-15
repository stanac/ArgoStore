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

            return new WhereStatement(statement, targetType);
        }

        private Type GetTargetType(Expression expression)
        {
            if (expression is ConstantExpression ce)
            {
                if (ce.Type.IsGenericType && ce.Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>))
                {
                    return ce.Type.GenericTypeArguments[0];
                }
            }

            throw new NotSupportedException($"Cannot get Where target type from \"{expression.NodeType}\", \"{expression.Type.Name}\", \"{expression}\"");
        }
    }
}
