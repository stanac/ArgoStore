using ArgoStore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class FirstLastSingleOnWhereMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        private static string[] SupportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault" };

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
                Statement lambdaContition = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[1]);

                where.AddConjunctedCondition(lambdaContition);
            }

            List<SelectStatementElement> selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            return new SelectStatement(where, where.TargetType, where.TargetType, selectElements, 1, methodName);
        }

        private static bool IsWhereCall(Expression e)
        {
            if (e is MethodCallExpression me && me.Method.Name == "Where")
            {
                return TypeHelpers.ImeplementsIQueryableGenericInteface(e.Type);
            }
            return false;
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
