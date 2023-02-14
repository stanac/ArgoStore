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
        SubQueryExpression sqe = (SubQueryExpression) expression;
        WhereStatementBase from = WhereToStatementTranslatorStrategies.Translate(sqe.QueryModel.MainFromClause.FromExpression, alias);
        WhereValueStatement value;
        FromAlias childAlias = alias.CreateChildAlias();

        if (sqe.QueryModel.ResultOperators.Count > 1)
        {
            throw new NotSupportedException("Not supported subquery with multiple result operators");
        }

        ContainsResultOperator cre = (ContainsResultOperator) sqe.QueryModel.ResultOperators[0];
        WhereStatementBase item = WhereToStatementTranslatorStrategies.Translate(cre.Item, childAlias);

        if (item is WhereValueStatement wvs)
        {
            value = wvs;
        }
        else
        {
            throw new InvalidOperationException($"Cannot convert {item.GetType().FullName} to value statement in contains subquery.");
        }

        return new WhereSubQueryContainsStatement(childAlias, new WhereSubQueryFromStatement(from, childAlias), value);
    }
}