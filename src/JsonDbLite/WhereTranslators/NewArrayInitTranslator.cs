using JsonDbLite.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal class NewArrayInitTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression) => expression is NewArrayExpression;

        public WhereClauseExpressionData Translate(Expression expression)
        {
            var exp = expression as NewArrayExpression;

            var translated = exp.Expressions.Select(x => WhereTranslatorStrategy.Translate(x) as WhereConstantExpressionData).Select(x => x.Value).ToList();

            return new WhereConstantExpressionData
            {
                Values = translated,
                IsCollection = true
            };
        }
    }
}
