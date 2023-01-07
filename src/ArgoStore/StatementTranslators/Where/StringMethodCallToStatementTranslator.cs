using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class StringMethodCallToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return false;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        throw new NotImplementedException();
    }
}