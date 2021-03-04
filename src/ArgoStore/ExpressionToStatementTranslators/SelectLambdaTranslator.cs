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
        public static SelectStatement Translate(LambdaExpression lambda, Type targetType, Statement from, SelectStatement.CalledByMethods method)
        {
            if (lambda is null) throw new ArgumentNullException(nameof(lambda));
            if (targetType is null) throw new ArgumentNullException(nameof(targetType));

            if (lambda.Body.NodeType == ExpressionType.Parameter)
            {
                var selectElements = new List<SelectStatementElement>
                {
                    SelectStatementElement.CreateWithStar(targetType)
                };

                return SelectStatement.Create(from, targetType, targetType, selectElements, null, method);
            }

            if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memAccess = lambda.Body as MemberExpression;

                if (memAccess.Member is PropertyInfo pi)
                {
                    var statement = ExpressionToStatementTranslatorStrategy.Translate(lambda.Body);

                    var selectElements = new List<SelectStatementElement>
                    {
                        new SelectStatementElement(statement, pi.PropertyType, false, pi.Name, pi.Name)
                    };

                    Type fromType = targetType;
                    if (from is WhereStatement w) fromType = w.TargetType;
                    else if (from is SelectStatement s) fromType = s.TypeFrom;

                    return SelectStatement.Create(from, fromType, targetType, selectElements, null, method);
                }
            }

            if (lambda.Body.NodeType == ExpressionType.New)
            {
                var ne = lambda.Body as NewExpression;

                List<SelectStatementElement> selectElements = new List<SelectStatementElement>();

                for (int i = 0; i < ne.Members.Count; i++)
                {
                    Type type = TypeHelpers.GetMemberType(ne.Members[i]);
                    PropertyAccessStatement pa = new PropertyAccessStatement(ne.Members[i].Name, type == typeof(bool));

                    var prop = ExpressionToStatementTranslatorStrategy.Translate(ne.Arguments[i]) as PropertyAccessStatement;

                    var sse = new SelectStatementElement(pa, type, false, prop.Name, ne.Members[i].Name);
                    selectElements.Add(sse);
                }

                Type toType = (lambda.Body as NewExpression).Type;

                return SelectStatement.Create(from, targetType, toType, selectElements, null, method);
            }

            if (lambda.Body.NodeType == ExpressionType.MemberInit)
            {
                var mi = lambda.Body as MemberInitExpression;

                List<SelectStatementElement> selectElements = new List<SelectStatementElement>();

                for (int i = 0; i < mi.Bindings.Count; i++)
                {
                    if (mi.Bindings[i] is MemberAssignment ma)
                    {
                        Statement s = ExpressionToStatementTranslatorStrategy.Translate(ma.Expression);
                        string outputProp = ma.Member.Name;
                        var input = ExpressionToStatementTranslatorStrategy.Translate(ma.Expression);
                        string intputProp = (input as PropertyAccessStatement).Name;

                        selectElements.Add(new SelectStatementElement(s, TypeHelpers.GetMemberType(ma.Member), false, intputProp, outputProp));
                    }
                    else
                    {
                        throw new NotSupportedException($"Cannot use {mi.Bindings[i].GetType().FullName} in {lambda.Body} for Select translation");
                    }
                }

                return SelectStatement.Create(from, targetType, mi.NewExpression.Type, selectElements, null, method);
            }

            throw new NotSupportedException($"{nameof(SelectLambdaTranslator)} cannot translate {lambda}");
        }
    }
}
