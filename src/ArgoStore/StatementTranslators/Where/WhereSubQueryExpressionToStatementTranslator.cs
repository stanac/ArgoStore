using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using ArgoStore.StatementTranslators.From;
using Remotion.Linq.Clauses.Expressions;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereSubQueryExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is SubQueryExpression;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        SubQueryExpression sqe = (SubQueryExpression)expression;

        FromProperty fromStatement = new FromProperty(sqe.QueryModel, 1, 2);

        throw new NotImplementedException();
    }
}