using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class LambdaExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is LambdaExpression;

        public Statement Translate(Expression expression)
        {
            var e = expression as LambdaExpression;
            return ExpressionToStatementTranslatorStrategy.Translate(e.Body);
        }
    }
}
