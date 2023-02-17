using System.Linq.Expressions;

namespace ArgoStore.Helpers;

internal static class ExpressionExtensions
{
    public static Expression RemoveQuotes(this Expression e)
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

    public static bool IsWhereCall(this Expression e)
    {
        if (e is MethodCallExpression me && me.Method.Name == "Where")
        {
            return e.Type.ImplementsIQueryableGenericInterface();
        }

        return false;
    }

    public static bool IsSelectCall(this Expression e)
    {
        if (e is MethodCallExpression me && me.Method.Name == "Select")
        {
            return e.Type.ImplementsIQueryableGenericInterface();
        }

        return false;
    }

    public static bool IsLambda(this Expression e)
    {
        while (e.NodeType == ExpressionType.Quote)
        {
            e = (e as UnaryExpression).Operand;
        }

        return e.NodeType == ExpressionType.Lambda;
    }
}