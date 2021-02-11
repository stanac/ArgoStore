using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class SelectOnArgoStoreQueryableMethodCallExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression)
        {
            if (expression is MethodCallExpression m)
            {
                return m.Method.Name == "Select" && m.Arguments[0].Type.IsGenericType && m.Arguments[0].Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>);
            }

            return false;
        }

        public Statement Translate(Expression expression)
        {
            MethodCallExpression me = expression as MethodCallExpression;

            Type targetType = GetTargetType(me.Arguments[0]);

            LambdaExpression lambda = GetSelectLambda(me);
            
            if (lambda.Body.NodeType == ExpressionType.Parameter)
            {
                return new SelectStatement
                {
                    TargetType = targetType,
                    SelectElements = new List<SelectStatementElement>
                    {
                        new SelectStatementElement
                        {
                            SelectsJson = true,
                            Statement = new SelectStarParameterStatement()
                        }
                    }
                };
            }

            if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memAccess = lambda.Body as MemberExpression;

                if (memAccess.Member is PropertyInfo pi)
                {
                    return new SelectStatement
                    {
                        TargetType = targetType,
                        SelectElements = new List<SelectStatementElement>
                        {
                            new SelectStatementElement
                            {
                                Statement = ExpressionToStatementTranslatorStrategy.Translate(lambda.Body),
                                ReturnType = pi.PropertyType,
                                SelectsJson = false
                            }
                        }
                    };
                }
            }

            throw new NotImplementedException();
        }

        private Type GetTargetType(Expression expression)
        {
            if (expression is ConstantExpression ce)
            {
                if (ce.Type.IsGenericType && ce.Type.GetGenericTypeDefinition() == typeof(ArgoStoreQueryable<>))
                {
                    return ce.Type.GetGenericArguments()[0];
                }
            }

            throw new ArgumentException($"Cannot get target type for select from \"{expression.NodeType}\", \"{expression.Type.FullName}\", \"{expression}\"");
        }

        private LambdaExpression GetSelectLambda(MethodCallExpression ex)
        {
            var e = ex.Arguments[1];

            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }

            if (e is LambdaExpression le)
            {
                return le;
            }

            throw new InvalidOperationException($"Expected lambda in Select \"{ex.NodeType}\", \"{ex.Type.FullName}\", \"{ex}\"");
        }
    }
}
