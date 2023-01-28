﻿using System.Linq.Expressions;
using ArgoStore.Statements;
using ArgoStore.Statements.Where;
using Remotion.Linq.Clauses.Expressions;

namespace ArgoStore.StatementTranslators.Where;

internal class SubQueryValueToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is QuerySourceReferenceExpression;
    }

    public WhereStatementBase Translate(Expression expression, FromAlias alias)
    {
        return new SubQueryValueWhereStatement(alias.CurrentAliasName);
    }
}