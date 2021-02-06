using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
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
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = (expression as UnaryExpression).Operand;
            }

            return ExpressionToStatementTranslatorStrategy.Translate(expression);
        }
    }
}
