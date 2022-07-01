using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class BinaryExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression) => expression is BinaryExpression;

    public Statement Translate(Expression expression)
    {
        BinaryExpression e = expression as BinaryExpression;

        if (e == null) throw new ArgumentException($"{nameof(expression)} is not BinaryExpression", nameof(expression));

        var left = ExpressionToStatementTranslatorStrategy.Translate(e.Left);
        var right = ExpressionToStatementTranslatorStrategy.Translate(e.Right);

        switch (e.NodeType)
        {
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                return new BinaryLogicalStatement(left, right, false);
                    
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                return new BinaryLogicalStatement(left, right, true);
                    
            case ExpressionType.Equal:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.Equal);

            case ExpressionType.NotEqual:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.NotEqual);

            case ExpressionType.LessThan:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.LessThan);

            case ExpressionType.LessThanOrEqual:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.LessThanOrEqual);

            case ExpressionType.GreaterThan:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.GreaterThan);

            case ExpressionType.GreaterThanOrEqual:
                return new BinaryComparisonStatement(left, right, BinaryComparisonStatement.Operators.GreaterThanOrEqual);

            default:
                throw new NotSupportedException($"Binary operator \"{expression.NodeType}\" isn't supported");
        }
    }
}