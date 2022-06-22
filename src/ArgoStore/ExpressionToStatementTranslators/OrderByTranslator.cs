using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class OrderByTranslator : IExpressionToStatementTranslator
    {
        private static readonly string[] _supportedMethodNames = new[] { "OrderBy", "ThenBy", "OrderByDescending", "ThenByDescending" };

        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression mc)
            {
                return _supportedMethodNames.Contains(mc.Method.Name) && mc.Method.DeclaringType == typeof(Queryable);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            var mc = expression as MethodCallExpression;

            Statement calledOn = null;

            if (mc.Arguments[0].NodeType != ExpressionType.Parameter)
            {
                calledOn = ExpressionToStatementTranslatorStrategy.Translate(mc.Arguments[0]);
            }

            bool isAsc = !mc.Method.Name.Contains("Descending");

            var statement = ExpressionToStatementTranslatorStrategy.Translate(mc.Arguments[1]);

            if (statement is PropertyAccessStatement pas)
            {
                var orderByStatement = OrderByStatement.Create(pas, isAsc);

                if (calledOn == null)
                {
                    return orderByStatement;
                }

                if (calledOn is OrderByStatement obs)
                {
                    return obs.Join(orderByStatement);
                }

                if (calledOn is SelectStatement select)
                {
                    return select.SetOrderBy(orderByStatement);
                }

                if (calledOn is WhereStatement where)
                {
                    List<SelectStatementElement> selectElements = new List<SelectStatementElement>
                    {
                        SelectStatementElement.CreateWithStar(where.TargetType)
                    };

                    SelectStatement ss = new SelectStatement(where, where.TargetType, where.TargetType, selectElements, null, SelectStatement.CalledByMethods.Select);
                    ss.SetOrderBy(orderByStatement);
                    return ss;
                }

                throw new NotImplementedException($"Not implemented merge of {mc.Method.Name} and {calledOn}");

            }

            throw new NotSupportedException($"Cannot create order by statement from {statement}");
        }
    }
}
