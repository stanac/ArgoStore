using JsonDbLite.Expressions;
using System;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class BinaryExpressionWhereTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression) => expression is BinaryExpression;

        public WhereClauseExpressionData Translate(Expression expression)
        {
            BinaryExpression e = expression as BinaryExpression;

            if (e == null) throw new ArgumentException($"{nameof(expression)} is not BinaryExpression", nameof(expression));

            WhereBinaryClauseExpressionData ret;

            switch (e.NodeType)
            {
                case ExpressionType.And:
                    ret = new WhereBinaryLogicalExpressionData { IsOr = false };
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    ret = new WhereBinaryLogicalExpressionData { IsOr = true };
                    break;

                case ExpressionType.Equal:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.Equal };
                    break;

                case ExpressionType.NotEqual:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.NotEqual };
                    break;

                case ExpressionType.LessThan:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.LessThan };
                    break;

                case ExpressionType.LessThanOrEqual:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.LessThanOrEqual };
                    break;

                case ExpressionType.GreaterThan:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.GreaterThan };
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    ret = new WhereBinaryComparisonExpressionData { Operator = WhereBinaryComparisonExpressionData.Operators.GreaterThanOrEqual };
                    break;

                default:
                    throw new NotSupportedException($"Binary operator \"{expression.NodeType}\" isn't supported");
            }

            ret.Left = WhereTranslatorStrategy.Translate(e.Left);
            ret.Right = WhereTranslatorStrategy.Translate(e.Right);

            return ret;
        }
    }
}
