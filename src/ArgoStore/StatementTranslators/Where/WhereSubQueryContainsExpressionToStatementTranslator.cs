using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereSubQueryContainsExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is SubQueryExpression sqe
               && sqe.QueryModel.ResultOperators.Count == 1
               && sqe.QueryModel.ResultOperators[0] is ContainsResultOperator;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        throw new NotImplementedException();
    }
}