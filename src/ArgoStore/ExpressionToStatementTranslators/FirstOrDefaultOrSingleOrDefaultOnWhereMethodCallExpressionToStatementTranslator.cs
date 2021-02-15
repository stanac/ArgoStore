using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class FirstOrDefaultOrSingleOrDefaultOnWhereMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        private static string[] SupportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault" };

        public bool CanTranslate(Expression expression)
        {
            bool result = false;
            if (expression is MethodCallExpression me)
            {
                result = SupportedMethodNames.Contains(me.Method.Name) && IsWhereCall(me.Arguments[0]) && (me.Arguments.Count == 1 || (me.Arguments.Count == 2 && IsLambda(me.Arguments[1])));
            }

            return result;
        }

        public Statement Translate(Expression expression)
        {
            var me = expression as MethodCallExpression;

            WhereStatement where = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[0]) as WhereStatement;

            SelectStatement.CalledByMethods methodName = (SelectStatement.CalledByMethods)Enum.Parse(typeof(SelectStatement.CalledByMethods), me.Method.Name);

            if (me.Arguments.Count == 2)
            {
                LambdaExpression lambda = GetSelectLambda(me.Arguments[1]);

                // todo: translate lambda
                throw new NotImplementedException();
            }

            List<SelectStatementElement> selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            return new SelectStatement(where, where.TargetType, selectElements, null, methodName);
        }

        private static bool IsWhereCall(Expression e)
        {
            if (e is MethodCallExpression me && me.Method.Name == "Where")
            {
                if (me.Arguments[0] is ConstantExpression ce)
                {
                    if (ce.Type.IsGenericType)
                    {
                        var genTypeDef = ce.Type.GetGenericTypeDefinition();

                        return genTypeDef == typeof(ArgoStoreQueryable<>) || typeof(IQueryable<>).IsAssignableFrom(genTypeDef);
                    }
                }
            }
            return false;
        }

        private LambdaExpression GetSelectLambda(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }

            if (e is LambdaExpression le)
            {
                return le;
            }

            throw new InvalidOperationException($"Expected lambda in Select \"{e.NodeType}\", \"{e.Type.FullName}\", \"{e}\"");
        }

        private static bool IsLambda(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }

            return e.NodeType == ExpressionType.Lambda;
        }
    }
}
