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
                if (m.Method.DeclaringType.IsGenericType && m.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Enumerable))
                {

                }
            }

            return false;
        }

        public WhereClauseExpressionData Translate(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
