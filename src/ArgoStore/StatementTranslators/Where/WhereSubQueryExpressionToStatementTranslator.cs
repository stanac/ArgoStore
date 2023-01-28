using System.Linq.Expressions;
using ArgoStore.Command;
using ArgoStore.Implementations;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using ArgoStore.StatementTranslators.From;
using Remotion.Linq.Clauses;
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
        FromProperty fromStatement = new FromProperty(sqe.QueryModel, alias.CreateChildAlias());
        ArgoCommandBuilder cb = new ArgoCommandBuilder(fromStatement, fromStatement.Alias);

        ArgoQueryModelVisitor visitor = new ArgoQueryModelVisitor(cb);
        visitor.VisitQueryModel(sqe.QueryModel);

        return new WhereSubQueryStatement(visitor.CommandBuilder);
    }
}