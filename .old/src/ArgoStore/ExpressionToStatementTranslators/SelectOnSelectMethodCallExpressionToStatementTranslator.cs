﻿using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class SelectOnSelectMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression mc)
        {
            if (mc.Method.Name == "Select" && mc.Arguments.Count == 2 && mc.Arguments[0] is MethodCallExpression mc2)
            {
                return mc2.Method.Name == "Select";
            }
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        var mc = expression as MethodCallExpression;

        var subQuery = ExpressionToStatementTranslatorStrategy.Translate(mc.Arguments[0]) as SelectStatement;

        if (subQuery == null) throw new NotSupportedException($"Expected subquery on Select in {nameof(SelectOnSelectMethodCallExpressionToStatementTranslator)}");

        LambdaExpression l = mc.Arguments[1].RemoveQuotes() as LambdaExpression;

        if (l == null) throw new NotSupportedException($"Expected lambda in Select in {nameof(SelectOnSelectMethodCallExpressionToStatementTranslator)}");

        return SelectLambdaTranslator.Translate(l, l.ReturnType, subQuery, CalledByMethods.Select);
    }

}