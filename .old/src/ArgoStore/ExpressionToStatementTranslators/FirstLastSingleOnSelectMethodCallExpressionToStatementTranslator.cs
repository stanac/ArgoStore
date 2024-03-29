﻿using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class FirstLastSingleOnSelectMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    private static readonly string[] _supportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault" };

    public bool CanTranslate(Expression expression)
    {
        bool result = false;

        if (expression is MethodCallExpression m)
        {
            result = _supportedMethodNames.Contains(m.Method.Name)
                     && m.Arguments[0].IsSelectCall()
                     && m.Arguments[0].Type.ImplementsIQueryableGenericInterface();
        }

        return result;
    }

    public Statement Translate(Expression expression)
    {
        MethodCallExpression me = expression as MethodCallExpression;

        CalledByMethods method = (CalledByMethods)Enum.Parse(typeof(CalledByMethods), me.Method.Name);

        Type targetType = GetTargetType(me.Arguments[0]);

        SelectStatement selectStatement = (SelectStatement)ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[0]);

        selectStatement
            .SetTop(1)
            .SetCalledByMethod(method);

        return selectStatement;
    }

    private Type GetTargetType(Expression expression)
    {
        if (expression.Type.ImplementsIQueryableGenericInterface())
        {
            return expression.Type.GetGenericArguments()[0];
        }

        throw new ArgumentException($"Cannot get target type for select from \"{expression.NodeType}\", \"{expression.Type.FullName}\", \"{expression}\"");
    }
}