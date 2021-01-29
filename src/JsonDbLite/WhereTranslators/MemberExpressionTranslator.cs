using JsonDbLite.Expressions;
using JsonDbLite.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace JsonDbLite.WhereTranslators
{
    internal class MemberExpressionTranslator : IWhereTranslator
    {
        public bool CanTranslate(Expression expression) => expression is MemberExpression;
    
        public WhereClauseExpressionData Translate(Expression expression)
        {
            MemberExpression e = expression as MemberExpression;
            if (e.Member is PropertyInfo pInf)
            {
                return new WherePropertyExpressionData { Name = pInf.Name, IsBoolean = pInf.PropertyType == typeof(bool) };
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

                    return new WhereConstantExpressionData
                    {
                        IsCollection = true,
                        Values = values
                    };
                }
                else
                {
                    return new WhereConstantExpressionData { Value = value.ToString() };
                }
            }

            throw new NotSupportedException($"MemberExpression with member of type \"{e.Member.GetType().FullName}\" isn't supported");
        }
    }
}
