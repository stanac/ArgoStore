using JsonDbLite.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal partial class MethodStringInstanceTranslator : IWhereTranslator
    {
        private readonly string[] _supportedMethodNames = new[]
        {
            "ToUpper", "ToLower", "Trim"
        };

        public bool CanTranslate(Expression expression)
        {
            MethodCallExpression e = expression as MethodCallExpression;

            return e != null
                && !e.Method.IsStatic
                && e.Method.ReflectedType == typeof(string)
                && _supportedMethodNames.Contains(e.Method.Name);
        }

        public WhereClauseExpressionData Translate(Expression expression)
        {
            MethodCallExpression e = expression as MethodCallExpression;

            var calledOn = WhereTranslatorStrategy.Translate(e.Object);

            return new WhereMethodCallExpressionData
            {
                MethodName = (WhereMethodCallExpressionData.SupportedMethodNames)Enum.Parse(typeof(WhereMethodCallExpressionData.SupportedMethodNames), "String" + e.Method.Name),
                Arguments = new WhereClauseExpressionData[] { calledOn }
            };
        }
    }
}
