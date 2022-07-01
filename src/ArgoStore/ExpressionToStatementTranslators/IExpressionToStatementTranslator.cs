using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal interface IExpressionToStatementTranslator
{
    bool CanTranslate(Expression expression);
    Statement Translate(Expression expression);
}