using System.Linq.Expressions;

namespace ArgoStore.Helpers
{
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
    }
}
