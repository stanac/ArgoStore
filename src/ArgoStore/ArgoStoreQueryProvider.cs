using ArgoStore.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ArgoStore
{
    internal class ArgoStoreQueryProvider : IQueryProvider
    {
        private readonly Configuration _config;
        private readonly EntityTableHelper _entityTableHelper;
        private readonly IDbAccess _dbAccess;

        public ArgoStoreQueryProvider(Configuration config)
            : this (config, new DbAccess(config?.ConnectionString))
        {
        }

        public ArgoStoreQueryProvider(Configuration config, IDbAccess dbAccess)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _entityTableHelper = new EntityTableHelper(config);
            _dbAccess = dbAccess ?? throw new ArgumentNullException(nameof(dbAccess));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            Type resultType = expression.Type;

            if (TypeHelpers.IsCollectionType(resultType))
            {
                resultType = TypeHelpers.GetCollectionElementType(resultType);
            }

            Type queryType = typeof(ArgoStoreQueryable<>).MakeGenericType(resultType);

            return Activator.CreateInstance(queryType, this, expression) as IQueryable<TElement>;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotSupportedException("Generic method CreateQuery isn't supported");
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotSupportedException("Non generic method CreateQuery isn't supported");
        }

        public object Execute(Expression expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));

            var statement = ExpressionToStatementTranslators.ExpressionToStatementTranslatorStrategy.Translate(expression);

            // _entityTableHelper.EnsureEntityTableExists(visitor.ExpData.EntityType);
            // 
            // IReadOnlyList<string> result = _dbAccess.QueryJsonField(sql);
            // 
            // return result.Select(x => _config.Serializer.Deserialize(x, visitor.ExpData.EntityType));

            throw new NotImplementedException();
        }
    }
}
