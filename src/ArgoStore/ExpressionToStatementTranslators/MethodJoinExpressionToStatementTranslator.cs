using System;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class MethodJoinExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression mce)
            {
                return mce.Method.Name == "Join" || mce.Method.Name == "GroupJoin";
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            throw new NotSupportedException($"LINQ methods Join and GroupJoin are not supported");
        }
    }
}
