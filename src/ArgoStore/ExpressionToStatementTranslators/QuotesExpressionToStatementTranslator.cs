using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class QuotesExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is UnaryExpression ex)
        {
            return ex.NodeType == ExpressionType.Quote;
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        var ex = ExpressionHelpers.RemoveQuotes(expression);
        return ExpressionToStatementTranslatorStrategy.Translate(ex);
    }
}