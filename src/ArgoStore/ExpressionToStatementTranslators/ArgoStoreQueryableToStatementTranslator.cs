using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

            return new SelectStatement
            {
                TargetType = ce.Type.GetGenericArguments()[0],
                SelectElements = new List<SelectStatementElement>
                {
                    new SelectStatementElement
                    {
                        SelectsJson = true,
                        Statement = new SelectStarParameterStatement(),
                        ReturnType = ce.Type.GetGenericArguments()[0]
                    }
                }
            };
        }
    }
}
