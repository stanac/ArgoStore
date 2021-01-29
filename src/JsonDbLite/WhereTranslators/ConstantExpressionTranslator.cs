using JsonDbLite.Expressions;
using System;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class ConstantExpressionTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression) => expression is ConstantExpression;

        public WhereClauseExpressionData Translate(Expression expression)
        {
            var e = expression as ConstantExpression;

            string value = null;

            if (e.Value != null)
            {
                value = e.Value.ToString();
            }

            // todo: add support for other types
            
            if (expression.Type == typeof(string))
            {
                return new WhereConstantExpressionData { IsString = true, Value = value };
            }
            else if (expression.Type.IsEnum)
            {
                return new WhereConstantExpressionData { IsString = false, Value = value };
            }
            else if (expression.Type == typeof(bool))
            {
                return new WhereConstantExpressionData { IsBoolean = true, Value = value };
            }
            else if (expression.Type == typeof(int))
            {
                return new WhereConstantExpressionData { Value = value };
            }

            throw new NotSupportedException($"Constant of type {expression.Type.Name} isn't supported");
        }
    }
}
