using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal interface IExpressionToStatementTranslator
    {
        bool CanTranslate(Expression expression);
        Statement Translate(Expression expression);
    }
}
