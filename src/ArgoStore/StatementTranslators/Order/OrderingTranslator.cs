using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Helpers;
using ArgoStore.Statements.Order;
using Remotion.Linq.Clauses;

namespace ArgoStore.StatementTranslators.Order;

internal static class OrderTranslator
{
    public static IEnumerable<OrderByStatement> Translate(Ordering ordering)
    {
        bool asc = ordering.OrderingDirection == OrderingDirection.Asc;

        if (ordering.Expression is MemberExpression {Member: PropertyInfo pi})
        {
            yield return new OrderByStatement(pi.Name, asc);
            yield break;
        }

        if (ordering.Expression is NewExpression ne && ne.Type.IsAnonymousType())
        {
            foreach (Expression e in ne.Arguments)
            {
                if (e is MemberExpression { Member: PropertyInfo pi1 })
                {
                    yield return new OrderByStatement(pi1.Name, asc);
                }
                else
                {
                    throw new NotSupportedException("Expression not supported in ordering by anonymous type. " +
                                                    "Ordering by anonymous type can only contain properties in `new { }`. " +
                                                    "Expression: " + e.Describe());
                }
            }

            yield break;
        }

        throw new NotSupportedException($"Expression not supported for ordering: {ordering.Expression.Describe()}");
    }
}