using JsonDbLite.Expressions;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal interface IWhereTranslator
    {
        bool CanTranslate(Expression expression);
        WhereClauseExpressionData Translate(Expression expression);
    }
}
