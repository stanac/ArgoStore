using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class SelectOnOrderByExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression m)
        {
            return m.Method.Name == "Select" && IsOrderByCall(m.Arguments[0]);
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression m = expression as MethodCallExpression;

        OrderByStatement orderBy = ExpressionToStatementTranslatorStrategy.Translate(m.Arguments[0]) as OrderByStatement;

        LambdaExpression lambda = ExpressionHelpers.RemoveQuotes(m.Arguments[1]) as LambdaExpression;

        SelectStatement ret = SelectLambdaTranslator.Translate(lambda, lambda.ReturnType, orderBy, CalledByMethods.Select);

        return ret;
    }

    private bool IsOrderByCall(Expression ex)
    {
        ex = ExpressionHelpers.RemoveQuotes(ex);

        if (ex is MethodCallExpression m)
        {
            return m.Method.Name == "OrderBy" || m.Method.Name == "OrderByDescending";
        }

        return false;
    }
}