using ArgoStore.StatementTranslators.Where;
using Remotion.Linq.Clauses;

namespace ArgoStore.Statements.Where;

internal class WhereStatement
{
    public WhereStatementBase Statement { get; }

    public WhereStatement(WhereClause clause, FromAlias alias, ArgoActivity? activity)
    {
        Statement = WhereToStatementTranslatorStrategies.Translate(clause.Predicate, alias, activity);
    }

    public WhereStatement(WhereStatementBase statement)
    {
        Statement = statement;
    }
}