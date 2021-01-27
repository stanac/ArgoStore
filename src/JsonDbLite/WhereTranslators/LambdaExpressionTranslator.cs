using JsonDbLite.Expressions;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class LambdaExpressionTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression) => expression is LambdaExpression;

        public WhereClauseExpressionData Translate(Expression expression)
        {
            var e = expression as LambdaExpression;
            return WhereTranslatorStrategy.Translate(e.Body);
        }
    }
}
