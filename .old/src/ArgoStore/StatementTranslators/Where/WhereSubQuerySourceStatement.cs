using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using Remotion.Linq.Clauses.Expressions;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereSubQuerySourceStatement : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is QuerySourceReferenceExpression;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("SubQuerySource");

        WhereSubQueryValueStatement r = new WhereSubQueryValueStatement(alias.CurrentAliasName);

        ca?.Stop();

        return r;
    }
}