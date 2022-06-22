using System;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class ConstantExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is ConstantExpression ce)
            {
                if (ce.Type.IsGenericType && ce.Type.GetGenericTypeDefinition() == typeof(IArgoStoreQueryable<>))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var e = expression as ConstantExpression;

            string value = null;

            if (e.Value != null)
            {
                value = e.Value.ToString();
            }

            // todo: add support for other types
            
            if (expression.Type == typeof(string))
            {
                return new ConstantStatement(true, false, value);
            }
            else if (expression.Type.IsEnum)
            {
                return new ConstantStatement(true, false, value);
            }
            else if (expression.Type == typeof(bool))
            {
                return new ConstantStatement(false, true, value);
            }
            else if (expression.Type == typeof(int))
            {
                return new ConstantStatement(false, false, value);
            }
            else if (expression.Type == typeof(char))
            {
                return new ConstantStatement(true, false, value);
            }

            throw new NotSupportedException($"Constant of type {expression.Type.Name} isn't supported");
        }
    }
}
