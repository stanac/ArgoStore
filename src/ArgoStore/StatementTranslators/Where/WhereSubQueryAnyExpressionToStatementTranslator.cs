using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereSubQueryAnyExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is SubQueryExpression sqe
               && sqe.QueryModel.ResultOperators.Count == 1
               && sqe.QueryModel.ResultOperators[0] is AnyResultOperator;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        SubQueryExpression sqe = (SubQueryExpression) expression;

        WhereStatementBase from = WhereToStatementTranslatorStrategies.Translate(sqe.QueryModel.MainFromClause.FromExpression, alias);

        WhereStatementBase? where = null;

        if (sqe.QueryModel.BodyClauses.Count > 1)
        {
            throw new NotSupportedException("Not supported subquery with multiple body clauses");
        }

        if (sqe.QueryModel.BodyClauses.Count == 1)
        {
            if (sqe.QueryModel.BodyClauses[0] is WhereClause wc)
            {
                where = WhereToStatementTranslatorStrategies.Translate(wc.Predicate, alias);
            }
            else
            {
                throw new NotSupportedException(
                    $"Not supported subquery with body clause of type {sqe.QueryModel.BodyClauses[0].GetType().FullName}"
                    );
            }
        }

        return new WhereSubQueryAnyStatement(from, where, alias.CreateChildAlias());
    }
}