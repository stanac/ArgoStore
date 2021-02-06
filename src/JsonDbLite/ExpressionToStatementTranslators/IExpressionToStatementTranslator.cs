using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal interface IExpressionToStatementTranslator
    {
        bool CanTranslate(Expression expression);
        Statement Translate(Expression expression);
    }
}
