using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.Statements;

internal abstract class ValueStatement
{
    public static ValueStatement From(Expression expression)
    {
        if (expression is MemberExpression me && me.Member is PropertyInfo pi)
        {
            string propName = pi.Name;
            return new PropertyStatement(propName);
        }
        else if (expression is ConstantExpression ce)
        {
            if (ce.Value is null)
            {
                return new NullValueStatement();
            }

            throw new NotImplementedException();
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}