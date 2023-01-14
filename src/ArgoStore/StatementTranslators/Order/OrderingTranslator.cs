using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements.Order;
using Remotion.Linq.Clauses;

namespace ArgoStore.StatementTranslators.Order;

internal static class OrderTranslator
{
    public static OrderByStatement Translate(Ordering ordering)
    {
        bool asc = ordering.OrderingDirection == OrderingDirection.Asc;

        if (ordering.Expression is MemberExpression {Member: PropertyInfo pi})
        {
            return new OrderByStatement(pi.Name, asc);
        }

        throw new NotSupportedException($"Expression not supported for ordering: {ordering.Expression.Describe()}");
    }
}