using JsonDbLite.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.WhereTranslators
{
    internal partial class MethodStringInstanceTranslator : IWhereTranslator
    {
        private readonly string[] _supportedMethodNames = new[]
        {
            "ToUpper", "ToLower", "Trim", "TrimStart", "TrimEnd", "Contains", "StartsWith", "EndsWith"
        };

        private readonly string[] _supportedBoolMethods = new[]
        {
            "Contains", "StartsWith", "EndsWith"
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

            if (_supportedBoolMethods.Contains(e.Method.Name))
            {
                return TranslateBoolMethod(e);
            }
            else
            {
                return TranslateStringMethod(e);
            }
        }

        private static WhereClauseExpressionData TranslateStringMethod(MethodCallExpression e)
        {
            List<WhereClauseExpressionData> args = new List<WhereClauseExpressionData>();

            args.Add(WhereTranslatorStrategy.Translate(e.Object));

            if (e.Arguments.Count == 1)
            {
                WhereClauseExpressionData arg = WhereTranslatorStrategy.Translate(e.Arguments[0]);
                args.Add(arg);
            }

            return new WhereMethodCallExpressionData
            {
                MethodName = (WhereMethodCallExpressionData.SupportedMethodNames)Enum.Parse(typeof(WhereMethodCallExpressionData.SupportedMethodNames), "String" + e.Method.Name),
                Arguments = args.ToArray()
            };
        }

        private WhereClauseExpressionData TranslateBoolMethod(MethodCallExpression e)
        {
            List<WhereClauseExpressionData> arguments = new List<WhereClauseExpressionData>();

            var arg1 = WhereTranslatorStrategy.Translate(e.Object);
            var arg2 = WhereTranslatorStrategy.Translate(e.Arguments[0]);

            bool ignoreCase = false;

            if (e.Arguments.Count > 1)
            {
                var arg3 = WhereTranslatorStrategy.Translate(e.Arguments[1]);
                ignoreCase = arg3 is WhereConstantExpressionData c && c.Value.Contains("IgnoreCase");
            }

            string methodName = "String" + e.Method.Name;

            if (ignoreCase)
            {
                methodName += "IgnoreCase";
            }

            return new WhereMethodCallExpressionData
            {
                Arguments = new WhereClauseExpressionData[] { arg1, arg2 },
                MethodName = (WhereMethodCallExpressionData.SupportedMethodNames)Enum.Parse(typeof(WhereMethodCallExpressionData.SupportedMethodNames), methodName)
            };
        }
    }
}
