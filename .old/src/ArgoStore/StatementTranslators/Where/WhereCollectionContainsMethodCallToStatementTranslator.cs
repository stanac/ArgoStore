﻿using System.Linq.Expressions;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class WhereCollectionContainsMethodCallToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MethodCallExpression mce && mce.Method.Name == "Contains")
        {
            return mce.Method.DeclaringType?.IsTypeCollectionOfSupportedPrimitiveType() ?? false;
        }

        return false;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias, ArgoActivity? activity)
    {
        ArgoActivity? ca = activity?.CreateChild("CollectionContains");

        MethodCallExpression mce = (MethodCallExpression)expression;

        WhereStatementBase c = WhereToStatementTranslatorStrategies.Translate(mce.Object!, alias, ca);
        WhereValueStatement collection;

        if (c is WhereValueStatement temp1)
        {
            collection = temp1;
        }
        else
        {
            throw new NotSupportedException($"Unexpected collection type object expression: {mce.Object!.Describe()}");
        }

        WhereStatementBase a = WhereToStatementTranslatorStrategies.Translate(mce.Arguments[0], alias, ca);
        WhereValueStatement argument;

        if (a is WhereValueStatement temp2)
        {
            argument = temp2;
        }
        else
        {
            throw new NotSupportedException($"Unexpected collection type argument expression: {mce.Arguments[0].Describe()}");
        }

        WhereCollectionContainsStatement result = new WhereCollectionContainsStatement(collection, argument);

        ca?.Stop();

        return result;
    }
}