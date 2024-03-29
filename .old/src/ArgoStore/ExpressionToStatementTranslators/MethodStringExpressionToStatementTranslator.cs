﻿using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class MethodStringExpressionToStatementTranslator : IExpressionToStatementTranslator
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
        if (expression is MethodCallExpression e)
        {
            return !e.Method.IsStatic
                   && e.Method.ReflectedType == typeof(string)
                   && _supportedMethodNames.Contains(e.Method.Name);
        }

        return false;
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

        MethodCallStatement.SupportedMethodNames methodName = (MethodCallStatement.SupportedMethodNames)Enum.Parse(typeof(MethodCallStatement.SupportedMethodNames), "String" + e.Method.Name);

        return new MethodCallStatement(methodName, args.ToArray());
    }

    private Statement TranslateBoolMethod(MethodCallExpression e)
    {
        List<Statement> arguments = new List<Statement>
        {
            ExpressionToStatementTranslatorStrategy.Translate(e.Object),
            ExpressionToStatementTranslatorStrategy.Translate(e.Arguments[0])
        };

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

        var methodNameE = (MethodCallStatement.SupportedMethodNames)Enum.Parse(typeof(MethodCallStatement.SupportedMethodNames), methodName);

        return new MethodCallStatement(methodNameE, arguments.ToArray());
    }
}