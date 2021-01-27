using JsonDbLite.Expressions;
using System;
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
                string value = fInf.GetValue(ce.Value).ToString();
                return new WhereConstantExpressionData { Value = value };
            }

            throw new NotSupportedException($"MemberExpression with member of type \"{e.Member.GetType().FullName}\" isn't supported");
        }
    }
}
