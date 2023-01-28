using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal interface IWhereToStatementTranslator
{
    bool CanTranslate(Expression expression);

    WhereStatementBase Translate(Expression expression, FromAlias alias);
}