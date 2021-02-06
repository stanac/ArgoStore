using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal class StringToCharArrayExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.Name == "ToCharArray" && m.Method.DeclaringType == typeof(string);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var m = expression as MethodCallExpression;

            Statement calledOn;

            try
            {
                calledOn = ExpressionToStatementTranslatorStrategy.Translate(m.Object);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"ToCharArray cannot be called on {m.Object.NodeType}", ex);
            }

            if (!(calledOn is ConstantStatement))
            {
                throw new NotSupportedException($"ToCharArray cannot be called on {m.Object.NodeType}");
            }

            string value = (calledOn as ConstantStatement).Value;

            return new ConstantStatement
            {
                Values = new List<string>(value.ToCharArray().Select(x => x.ToString())),
                IsCollection = true
            };
        }
    }
}
