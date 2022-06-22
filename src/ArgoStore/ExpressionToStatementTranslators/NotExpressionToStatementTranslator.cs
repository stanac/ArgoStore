using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class NotExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is UnaryExpression && expression.NodeType == ExpressionType.Not;
        
        public Statement Translate(Expression expression)
        {
            var e = expression as UnaryExpression;

            Statement innerStatement = ExpressionToStatementTranslatorStrategy.Translate(e.Operand);
            return new NotStatement(innerStatement);
        }
    }
}
