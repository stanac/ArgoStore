using JsonDbLite.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite
{
    public class JsonDbLiteQueryProvider : IQueryProvider
    {
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Type resultType = expression.Type;

            if (TypeHelpers.IsCollectionType(resultType))
            {
                resultType = TypeHelpers.GetCollectionElementType(resultType);
            }

            Type queryType = typeof(JsonDbLiteQueryable<>).MakeGenericType(resultType);

            return Activator.CreateInstance(queryType, this, expression) as IQueryable<TElement>;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            var visitor = new QueryVisitor();
            string sql = visitor.Translate(expression);

            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotSupportedException("Non generic method CreateQuery isn't supported");
        }

        public object Execute(Expression expression)
        {
            throw new NotSupportedException("Non generic method Execute isn't supported");
        }
    }
}
