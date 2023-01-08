using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Statements.Where;

namespace ArgoStore.StatementTranslators.Where;

internal class PropertyExpressionToStatementTranslator : IWhereToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is MemberExpression;
    }

    public WhereStatementBase Translate(Expression expression)
    {
        MemberExpression me = (MemberExpression)expression;

        if (me.Member is PropertyInfo pi)
        {
            return new WherePropertyStatement(pi.Name);
        }

        throw new NotSupportedException("f1dde3f265cb");
    }
}