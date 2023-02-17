using System.Linq.Expressions;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal interface ISelectStatementTranslator
{
    bool CanTranslate(Expression expression);
    SelectStatementBase Translate(Expression expression);
}