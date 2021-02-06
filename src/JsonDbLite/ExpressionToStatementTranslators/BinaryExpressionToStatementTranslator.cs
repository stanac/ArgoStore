using System;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal class BinaryExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is BinaryExpression;

        public Statement Translate(Expression expression)
        {
            BinaryExpression e = expression as BinaryExpression;

            if (e == null) throw new ArgumentException($"{nameof(expression)} is not BinaryExpression", nameof(expression));

            BinaryStatement ret;

            switch (e.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    ret = new BinaryLogicalStatement { IsOr = false };
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    ret = new BinaryLogicalStatement { IsOr = true };
                    break;

                case ExpressionType.Equal:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.Equal };
                    break;

                case ExpressionType.NotEqual:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.NotEqual };
                    break;

                case ExpressionType.LessThan:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.LessThan };
                    break;

                case ExpressionType.LessThanOrEqual:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.LessThanOrEqual };
                    break;

                case ExpressionType.GreaterThan:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.GreaterThan };
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    ret = new BinaryComparisonStatement { Operator = BinaryComparisonStatement.Operators.GreaterThanOrEqual };
                    break;

                default:
                    throw new NotSupportedException($"Binary operator \"{expression.NodeType}\" isn't supported");
            }

            ret.Left = ExpressionToStatementTranslatorStrategy.Translate(e.Left);
            ret.Right = ExpressionToStatementTranslatorStrategy.Translate(e.Right);

            return ret;
        }
    }
}
