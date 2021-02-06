using ArgoStore.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ArgoStore.ExpressionToStatementTranslators
{
    internal class MemberExpressionToStatementTranslator : IExpressionToStatementTranslator
    {
        public bool CanTranslate(Expression expression) => expression is MemberExpression;
    
        public Statement Translate(Expression expression)
        {
            MemberExpression e = expression as MemberExpression;
            if (e.Member is PropertyInfo pInf)
            {
                return new PropertyAccessStatement { Name = pInf.Name, IsBoolean = pInf.PropertyType == typeof(bool) };
            }
            if (e.Member is FieldInfo fInf && e.Expression is ConstantExpression ce)
            {
                object value = fInf.GetValue(ce.Value);

                if (TypeHelpers.IsCollectionType(value.GetType()))
                {
                    List<string> values = new List<string>();

                    foreach (var v in value as IEnumerable)
                    {
                        values.Add(v.ToString());
                    }

                    return new ConstantStatement
                    {
                        IsCollection = true,
                        Values = values
                    };
                }
                else
                {
                    return new ConstantStatement { Value = value.ToString() };
                }
            }

            throw new NotSupportedException($"MemberExpression with member of type \"{e.Member.GetType().FullName}\" isn't supported");
        }
    }
}
