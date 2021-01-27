using JsonDbLite.Expressions;
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
            var member = e.Member as PropertyInfo;
            return new WherePropertyExpressionData { Name = member.Name, IsBoolean = member.PropertyType == typeof(bool) };
        }
    }
}
