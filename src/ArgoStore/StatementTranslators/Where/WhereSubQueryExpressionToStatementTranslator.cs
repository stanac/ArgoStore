using System.Linq.Expressions;
using ArgoStore.Implementations;
using ArgoStore.Statements.Where;
using Remotion.Linq.Clauses.Expressions;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereSubQueryExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is SubQueryExpression;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        SubQueryExpression sqe = (SubQueryExpression)expression;

        sqe.QueryModel.MainFromClause.

        throw new NotImplementedException();
    }
}