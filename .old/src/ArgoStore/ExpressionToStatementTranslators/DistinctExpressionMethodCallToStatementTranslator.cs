using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class DistinctExpressionMethodCallToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression ex && ex.Method.Name == "Distinct")
        {
            return ex.Type.IsQueryable();
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression mce = (MethodCallExpression)expression;

        Expression selectExpression = mce.Arguments[0];

        Statement statement = ExpressionToStatementTranslatorStrategy.Translate(selectExpression);

        if (statement is SelectStatement ss)
        {
            ss.IsDistinct = true;
            return ss;
        }

        throw new InvalidOperationException("Expected Distinct to be called on select/where method");
    }
}