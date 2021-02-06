using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite.ExpressionToStatementTranslators
{
    internal partial class MethodStringStaticExpressionToStatementTranslator : IExpressionToStatementTranslator
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

        public Statement Translate(Expression expression)
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

        private static Statement TranslateStringMethod(MethodCallExpression e)
        {
            List<Statement> args = new List<Statement>();

            args.Add(ExpressionToStatementTranslatorStrategy.Translate(e.Object));

            if (e.Arguments.Count == 1)
            {
                Statement arg = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]);
                args.Add(arg);
            }

            return new MethodCallStatement
            {
                MethodName = (MethodCallStatement.SupportedMethodNames)Enum.Parse(typeof(MethodCallStatement.SupportedMethodNames), "String" + e.Method.Name),
                Arguments = args.ToArray()
            };
        }

        private Statement TranslateBoolMethod(MethodCallExpression e)
        {
            List<Statement> arguments = new List<Statement>();

            var arg1 = ExpressionToStatementTranslatorStrategy.Translate(e.Object);
            var arg2 = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0]);

            bool ignoreCase = false;

            if (e.Arguments.Count > 1)
            {
                var arg3 = ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[1]);
                ignoreCase = arg3 is ConstantStatement c && c.Value.Contains("IgnoreCase");
            }

            string methodName = "String" + e.Method.Name;

            if (ignoreCase)
            {
                methodName += "IgnoreCase";
            }

            return new MethodCallStatement
            {
                Arguments = arguments.ToArray(),
                MethodName = (MethodCallStatement.SupportedMethodNames)Enum.Parse(typeof(MethodCallStatement.SupportedMethodNames), methodName)
            };
        }
    }
}
