using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class SelectConstantParameterExpressionTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is ConstantExpression ce && !ce.Type.IsAnonymousType();
    }

    public SelectStatementBase Translate(Expression expression)
    {
        ConstantExpression ce = (ConstantExpression)expression;

        return new SelectParameterStatement(ce.Value!);
    }
}