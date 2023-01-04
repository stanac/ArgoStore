using System.Linq.Expressions;

namespace ArgoStore;

internal static class ExpressionExtensions
{
    public static string Describe(this Expression e)
    {
        return $"Expression name: `{e.GetType().Name}`, Expression node type: {e.NodeType}, Expression type: `{e.Type.Name}`";
    }
}