using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Statements.Select;

namespace ArgoStore.StatementTranslators.Select;

internal class PropertyAccessExpressionToStatementTranslator : ISelectStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        return expression is MemberExpression me
               && me.Member.MemberType == MemberTypes.Property;
    }

    public SelectStatementBase Translate(Expression expression)
    {
        MemberExpression me = (MemberExpression)expression;
        PropertyInfo pi = (PropertyInfo)me.Member;

        return new SelectPropertyStatement(pi.Name);
    }
}