﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore
{
    internal class ArgoStoreQueryable<T> : IArgoStoreQueryable<T>
    {
        public ArgoStoreQueryable(ArgoStoreQueryProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));

            Expression = Expression.Constant(this);
        }

        public ArgoStoreQueryable(ArgoStoreQueryProvider provider, Expression expression)
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

        public IEnumerator<T> GetEnumerator()
        {
            var res = (IEnumerable)Provider.Execute(Expression);
            return res.Cast<T>().GetEnumerator();
        }

        public string ToSqlString()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}