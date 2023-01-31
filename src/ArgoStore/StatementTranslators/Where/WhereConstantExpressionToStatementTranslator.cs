using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereConstantExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is ConstantExpression;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        ConstantExpression ce = (ConstantExpression)expression;
        
        if (ce.Value is null)
        {
            return new WhereNullValueStatement();
        }

        return new WhereParameterStatement(ce.Value, ce.Type);
    }
}