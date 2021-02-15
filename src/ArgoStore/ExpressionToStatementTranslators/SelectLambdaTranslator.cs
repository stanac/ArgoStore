using ArgoStore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal static class SelectLambdaTranslator
    {
        public static SelectStatement Translate(LambdaExpression lambda, Type targetType, WhereStatement where, SelectStatement.CalledByMethods method)
        {
            if (lambda is null) throw new ArgumentNullException(nameof(lambda));
            if (targetType is null) throw new ArgumentNullException(nameof(targetType));

            if (lambda.Body.NodeType == ExpressionType.Parameter)
            {
                var selectElements = new List<SelectStatementElement>
                {
                    SelectStatementElement.CreateWithStar(targetType)
                };

                return new SelectStatement(where, targetType, targetType, selectElements, null, method);
            }

            if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memAccess = lambda.Body as MemberExpression;

                if (memAccess.Member is PropertyInfo pi)
                {
                    var statement = ExpressionToStatementTranslatorStrategy.Translate(lambda.Body);

                    var selectElements = new List<SelectStatementElement>
                    {
                        new SelectStatementElement(statement, pi.PropertyType, false)
                    };

                    return new SelectStatement(where, targetType, pi.PropertyType, selectElements, null, method);
                }
            }

            if (lambda.Body.NodeType == ExpressionType.New)
            {
                List<SelectStatementElement> selectElements = (lambda.Body as NewExpression)
                    .Members
                    .Select(x =>
                    {
                        Type type = TypeHelpers.GetMemberType(x);
                        PropertyAccessStatement pa = new PropertyAccessStatement(x.Name, type == typeof(bool));
                        return new SelectStatementElement(pa, type, false);
                    })
                    .ToList();

                Type toType = (lambda.Body as NewExpression).Type;

                return new SelectStatement(null, targetType, toType, selectElements, null, method);
            }

            throw new NotSupportedException($"{nameof(SelectLambdaTranslator)} cannot translate {lambda}");
        }
    }
}
