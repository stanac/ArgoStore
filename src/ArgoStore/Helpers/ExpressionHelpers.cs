using System.Linq.Expressions;

namespace ArgoStore.Helpers
{
    internal static class ExpressionHelpers
    {
        public static Expression RemoveQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }

            return e;
        }
    }
}
