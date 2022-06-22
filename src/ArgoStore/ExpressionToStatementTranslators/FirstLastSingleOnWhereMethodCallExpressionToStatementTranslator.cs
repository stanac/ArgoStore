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
        private static readonly string[] SupportedMethodNames = new[] { "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault" };

        public bool CanTranslate(Expression expression)
        {
            bool result = false;
            if (expression is MethodCallExpression me)
            {
                result = SupportedMethodNames.Contains(me.Method.Name)
                         && ExpressionHelpers.IsWhereCall(me.Arguments[0])
                         && (me.Arguments.Count == 1 || (me.Arguments.Count == 2 && ExpressionHelpers.IsLambda(me.Arguments[1])));
            }

            return result;
        }

        public Statement Translate(Expression expression)
        {
            MethodCallExpression me = expression as MethodCallExpression;

            WhereStatement where = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[0]) as WhereStatement;

            SelectStatement.CalledByMethods methodName = (SelectStatement.CalledByMethods)Enum.Parse(typeof(SelectStatement.CalledByMethods), me.Method.Name);

            if (me.Arguments.Count == 2)
            {
                Statement lambdaCondition = ExpressionToStatementTranslatorStrategy.Translate(me.Arguments[1]);

                where.AddConjunctedCondition(lambdaCondition);
            }

            List<SelectStatementElement> selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(where.TargetType)
            };

            return new SelectStatement(where, where.TargetType, where.TargetType, selectElements, 1, methodName);
        }
    }
}
