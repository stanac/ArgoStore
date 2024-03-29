﻿using ArgoStore.Helpers;
using System.Linq.Expressions;
using System.Reflection;
using ArgoStore.Statements;

namespace ArgoStore.ExpressionToStatementTranslators;

internal static class SelectLambdaTranslator
{
    public static SelectStatement Translate(LambdaExpression lambda, Type fromType, Statement from, CalledByMethods method)
    {
        if (lambda is null) throw new ArgumentNullException(nameof(lambda));
        if (fromType is null) throw new ArgumentNullException(nameof(fromType));

        if (lambda.Body.NodeType == ExpressionType.Parameter)
        {
            var selectElements = new List<SelectStatementElement>
            {
                SelectStatementElement.CreateWithStar(fromType)
            };

            return SelectStatement.Create(from, fromType, fromType, selectElements, null, method);
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

                Type toType = memAccess.Type;
                if (from is WhereStatement w) fromType = w.TargetType;
                else if (from is SelectStatement s) fromType = s.TypeFrom;

                return SelectStatement.Create(from, fromType, toType, selectElements, null, method);
            }
        }

        if (lambda.Body.NodeType == ExpressionType.New)
        {
            var ne = lambda.Body as NewExpression;

            List<SelectStatementElement> selectElements = new List<SelectStatementElement>();

            for (int i = 0; i < ne.Members.Count; i++)
            {
                Type type = ne.Members[i].GetMemberType();
                PropertyAccessStatement pa = new PropertyAccessStatement(ne.Members[i].Name, type == typeof(bool));

                var prop = ExpressionToStatementTranslatorStrategy.Translate(ne.Arguments[i]) as PropertyAccessStatement;

                var sse = new SelectStatementElement(pa, type, false, prop.Name, ne.Members[i].Name);
                selectElements.Add(sse);
            }

            Type toType = (lambda.Body as NewExpression).Type;

            return SelectStatement.Create(from, fromType, toType, selectElements, null, method);
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

                    selectElements.Add(new SelectStatementElement(s, ma.Member.GetMemberType(), false, intputProp, outputProp));
                }
                else
                {
                    throw new NotSupportedException($"Cannot use {mi.Bindings[i].GetType().FullName} in {lambda.Body} for Select translation");
                }
            }

            return SelectStatement.Create(from, fromType, mi.NewExpression.Type, selectElements, null, method);
        }

        throw new NotSupportedException($"{nameof(SelectLambdaTranslator)} cannot translate {lambda}");
    }
}