using System.Linq.Expressions;

namespace ArgoStore.Helpers;

internal static class ExpressionHelpers
{
    public static Expression RemoveQuotes(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            if (e is UnaryExpression ue)
            {
                e = ue.Operand;
            }
        }

        return e;
    }

    public static bool IsWhereCall(Expression e)
    {
        if (e is MethodCallExpression me && me.Method.Name == "Where")
        {
            return TypeHelpers.ImplementsIQueryableGenericInterface(e.Type);
        }

        return false;
    }

    public static bool IsSelectCall(Expression e)
    {
        if (e is MethodCallExpression me && me.Method.Name == "Select")
        {
            return TypeHelpers.ImplementsIQueryableGenericInterface(e.Type);
        }

        return false;
    }

    public static bool IsLambda(Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = (e as UnaryExpression).Operand;
        }

        return e.NodeType == ExpressionType.Lambda;
    }
}