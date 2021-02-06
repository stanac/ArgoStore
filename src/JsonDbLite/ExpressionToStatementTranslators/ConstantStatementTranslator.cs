using System;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal class ConstantStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is ConstantExpression;

        public Statement Translate(Expression expression)
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
                return new ConstantStatement { IsString = true, Value = value };
            }
            else if (expression.Type.IsEnum)
            {
                return new ConstantStatement { IsString = false, Value = value };
            }
            else if (expression.Type == typeof(bool))
            {
                return new ConstantStatement { IsBoolean = true, Value = value };
            }
            else if (expression.Type == typeof(int))
            {
                return new ConstantStatement { Value = value };
            }
            else if (expression.Type == typeof(char))
            {
                return new ConstantStatement { IsString = true, Value = value };
            }

            throw new NotSupportedException($"Constant of type {expression.Type.Name} isn't supported");
        }
    }
}
