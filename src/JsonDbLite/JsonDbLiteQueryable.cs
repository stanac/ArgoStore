using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JsonDbLite
{
    public class JsonDbLiteQueryable<T> : IQueryable<T>
    {
        public JsonDbLiteQueryable(JsonDbLiteQueryProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));

            Expression = Expression.Constant(this);
        }

        public JsonDbLiteQueryable(JsonDbLiteQueryProvider provider, Expression expression)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
