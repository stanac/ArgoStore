using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal class NotExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is UnaryExpression && expression.NodeType == ExpressionType.Not;
        
        public Statement Translate(Expression expression)
        {
            var e = expression as UnaryExpression;

            return new NotStatement
            {
                InnerStatement = ExpressionToStatementTranslatorStrategy.Translate(e.Operand)
            };
        }
    }
}
