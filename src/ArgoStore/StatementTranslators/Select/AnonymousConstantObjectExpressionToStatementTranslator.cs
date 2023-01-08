using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ArgoStore.Helpers;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class AnonymousConstantObjectExpressionToStatementTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is ConstantExpression ce && ce.Type.IsAnonymousType();
    }

    public SelectStatementBase Translate(Expression expression)
    {
        throw new NotSupportedException("Constant anonymous objects not supported");
    }
}