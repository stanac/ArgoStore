using JsonDbLite.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class StringToCharArrayTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.Name == "ToCharArray" && m.Method.DeclaringType == typeof(string);
            }

            return false;
        }

        public WhereClauseExpressionData Translate(Expression expression)
        {
            var m = expression as MethodCallExpression;

            WhereClauseExpressionData calledOn;

            try
            {
                calledOn = WhereTranslatorStrategy.Translate(m.Object);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"ToCharArray cannot be called on {m.Object.NodeType}", ex);
            }

            if (!(calledOn is WhereConstantExpressionData))
            {
                throw new NotSupportedException($"ToCharArray cannot be called on {m.Object.NodeType}");
            }

            string value = (calledOn as WhereConstantExpressionData).Value;

            return new WhereConstantExpressionData
            {
                Values = new List<string>(value.ToCharArray().Select(x => x.ToString())),
                IsCollection = true
            };
        }
    }
}
