using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.Statements.Where;

internal abstract class WhereValueStatement : WhereStatementBase
{
    public static WhereValueStatement From(Expression expression)
    {
        if (expression is MemberExpression me && me.Member is PropertyInfo pi)
        {
            string propName = pi.Name;
            return new WherePropertyStatement(propName);
        }
        
        if (expression is ConstantExpression ce)
        {
            if (ce.Value is null)
            {
                return new WhereNullValueStatement();
            }

            return new WhereParameterStatement(ce.Value);
        }

        throw new NotSupportedException("5f294d396056");
    }
}