using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal class NewArrayInitExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is NewArrayExpression;

        public Statement Translate(Expression expression)
        {
            var exp = expression as NewArrayExpression;

            var translated = exp.Expressions.Select(x => ExpressionToStatementTranslatorStrategy.Translate(x) as ConstantStatement).Select(x => x.Value).ToList();

            return new ConstantStatement
            {
                Values = translated,
                IsCollection = true
            };
        }
    }
}
