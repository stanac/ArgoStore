using System.Diagnostics;
using System.Linq.Expressions;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class ConstantExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is ConstantExpression;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        ConstantExpression ce = expression as ConstantExpression;

        Debug.Assert(ce != null, "Convert expression as ConstantExpression");

        if (ce.Value is null)
        {
            return new WhereNullValueStatement();
        }

        return new WhereParameterStatement(ce.Value);
    }
}