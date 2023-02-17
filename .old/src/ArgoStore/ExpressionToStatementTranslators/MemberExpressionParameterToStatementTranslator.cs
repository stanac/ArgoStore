using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal class MemberExpressionParameterToStatementTranslator : IExpressionToStatementTranslator
{
    public bool CanTranslate(Expression expression)
    {
        if (expression is MemberExpression me)
        {
            return IsParameter(me);
        }

        return false;
    }

    public Statement Translate(Expression expression)
    {
        MemberExpression e = expression as MemberExpression;

        Debug.Assert(e != null, "MemberExpression != null in MemberExpressionParameterToStatementTranslator");

        if (e.Member is PropertyInfo pInf)
        {
            return new PropertyAccessStatement(e.Member.Name, pInf.PropertyType == typeof(bool));
        }
            
        throw new NotSupportedException($"MemberExpression with member of type \"{e.Member.GetType().FullName}\" isn't supported");
    }
        
    private bool IsParameter(MemberExpression ex)
    {
        if (ex.NodeType == ExpressionType.Parameter)
        {
            return true;
        }

        while (ex.Expression is MemberExpression me)
        {
            ex = me;
        }

        return ex.Expression.NodeType == ExpressionType.Parameter;
    }

}