﻿using ArgoStore.Helpers;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class SelectOnWhereMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        bool result = false;
        if (expression is MethodCallExpression me)
        {
            result = me.Method.Name == "Select" && IsWhereCall(me.Arguments[0]) && IsLambda(me.Arguments[1]);
        }

        return result;
    }

    public Statement Translate(Expression expression)
    {
        var me = expression as MethodCallExpression;

        var calledOn = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[0]);

        LambdaExpression lambda = GetSelectLambda(me.Arguments[1]);

        if (calledOn is WhereStatement where)
        {
            return SelectLambdaTranslator.Translate(lambda, where.TargetType, where, CalledByMethods.Select);
        }

        if (calledOn is SelectStatement parentSelect)
        {
            return SelectLambdaTranslator.Translate(lambda, parentSelect.TypeTo, parentSelect, CalledByMethods.Select);
        }

        throw new NotSupportedException($"Cannot add select to statement of type {calledOn.GetType().Name}");
    }

    private static bool IsWhereCall(Expression e)
    {
        if (e is MethodCallExpression me && me.Method.Name == "Where")
        {
            if (me.Arguments[0] is ConstantExpression ce)
            {
                if (ce.Type.IsGenericType)
                {
                    var genTypeDef = ce.Type.GetGenericTypeDefinition();

                    return genTypeDef == typeof(ArgoStoreQueryable<>) || typeof(IQueryable<>).IsAssignableFrom(genTypeDef);
                }
            }
            else if (me.Arguments[0] is MethodCallExpression mc)
            {
                return mc.Type.ImplementsIQueryableGenericInterface();
            }
        }

        return false;
    }

    private LambdaExpression GetSelectLambda(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = (e as UnaryExpression).Operand;
        }

        if (e is LambdaExpression le)
        {
            return le;
        }

        throw new InvalidOperationException($"Expected lambda in Select \"{e.NodeType}\", \"{e.Type.FullName}\", \"{e}\"");
    }

    private static bool IsLambda(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = (e as UnaryExpression).Operand;
        }

        return e.NodeType == ExpressionType.Lambda;
    }
}