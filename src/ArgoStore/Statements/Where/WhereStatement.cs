using ArgoStore.StatementTranslators.Where;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace ArgoStore.Statements.Where;

internal class WhereStatement
{
    private WhereStatementBase Statement { get; }

    public WhereStatement(WhereClause clause, QueryModel model)
    {
        Statement = WhereToStatementTranslatorStrategies.Translate(clause.Predicate);
    }
}