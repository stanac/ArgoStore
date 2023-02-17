using System.Linq.Expressions;
using ArgoStore.Statements.Select;
using Remotion.Linq.Clauses.Expressions;

namespace ArgoStore.StatementTranslators.Select;

internal class QuerySourceReferenceExpressionToStatementTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is QuerySourceReferenceExpression;
    }

    public SelectStatementBase Translate(Expression expression)
    {
        return new SelectDocumentStatement();
    }
}