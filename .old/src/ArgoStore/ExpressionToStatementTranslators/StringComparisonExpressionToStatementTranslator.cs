using System.Diagnostics;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class StringComparisonExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression mce)
        {
            return mce.Method.DeclaringType == typeof(string) && mce.Method.Name == "Equals";
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression mce = expression as MethodCallExpression;

        Debug.Assert(mce != null, $"MethodCallExpression != null in {nameof(StringComparisonExpressionToStatementTranslator)}");

        Statement left = ExpressionToStatementTranslatorStrategy.Translate(mce.Object);
        Statement right = ExpressionToStatementTranslatorStrategy.Translate(mce.Arguments[0]);

        return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.Equal);
    }
}